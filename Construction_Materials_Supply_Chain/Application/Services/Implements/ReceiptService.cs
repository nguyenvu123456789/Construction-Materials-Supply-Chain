using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly IAccountingEntryRepository _entryRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public ReceiptService(
            IReceiptRepository receiptRepository,
            IAccountingEntryRepository entryRepository,
            IPartnerRepository partnerRepository,
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _receiptRepository = receiptRepository;
            _entryRepository = entryRepository;
            _partnerRepository = partnerRepository;
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public string GenerateReceiptNumber()
        {
            var day = DateTime.Now.ToString("yyyyMMdd");
            var prefix = $"PT-{day}-";
            var lastReceipt = _receiptRepository.GetLastReceiptByPrefix(prefix);
            var number = 1;

            if (lastReceipt != null)
            {
                var numberPart = lastReceipt.ReceiptNumber.Substring(prefix.Length);
                if (int.TryParse(numberPart, out int parsed))
                    number = parsed + 1;
            }

            return $"{prefix}{number:D3}";
        }

        public void CreateReceipt(ReceiptDTO dto)
        {
            var partner = _partnerRepository.GetByIdNotDeleted(dto.PartnerId);
            if (partner == null)
                throw new Exception($"Partner ID {dto.PartnerId} không tồn tại.");

            if (!string.IsNullOrEmpty(dto.Invoices))
            {
                var invoice = _invoiceRepository.GetByCode(dto.Invoices);
                if (invoice == null)
                    throw new Exception($"Invoice Code {dto.Invoices} không tồn tại.");
            }

            var receipt = _mapper.Map<Receipt>(dto);
            receipt.ReceiptNumber = GenerateReceiptNumber();
            receipt.Status = "Approved";
            receipt.PartnerName = partner.PartnerName;

            _receiptRepository.Add(receipt);

            var ledgerEntry = new AccountingEntry
            {
                EntryDate = receipt.DateCreated,
                DocumentType = "PhieuThu",
                DocumentNumber = receipt.ReceiptNumber,
                Description = receipt.Reason ?? $"Thu tiền {receipt.PartnerName}",
                DebitAccount = receipt.DebitAccount,
                CreditAccount = receipt.CreditAccount,
                DebitAmount = receipt.Amount,
                CreditAmount = 0,
                PartnerId = receipt.PartnerId,
                PartnerName = receipt.PartnerName
            };

            _entryRepository.Add(ledgerEntry);
        }

        public Receipt GetReceiptById(int id) => _receiptRepository.GetById(id);
        public List<Receipt> GetAllReceipts() => _receiptRepository.GetAll();
        public List<Receipt> GetReceiptsByPartnerId(int partnerId) => _receiptRepository.GetReceiptsByPartnerId(partnerId);

        public async Task<byte[]> ExportReceiptsToExcelAsync()
        {
            var receipts = _receiptRepository.GetAll();
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Receipts");

            string[] headers = {
                "Ngày Lập", "Tên Đối Tượng", "ID", "Địa Chỉ", "Lý Do", "Số Tiền",
                "HTTT", "Tài Khoản NH", "Hóa Đơn", "TK Nợ", "TK Có", "Người Nộp", "Ghi Chú", "Loại", "Mã Phiếu", "Trạng Thái"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            int row = 2;
            foreach (var item in receipts)
            {
                worksheet.Cells[row, 1].Value = item.DateCreated.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 2].Value = item.PartnerName;
                worksheet.Cells[row, 3].Value = item.PartnerId;
                worksheet.Cells[row, 4].Value = item.Address;
                worksheet.Cells[row, 5].Value = item.Reason;
                worksheet.Cells[row, 6].Value = item.Amount;
                worksheet.Cells[row, 7].Value = item.PaymentMethod;
                worksheet.Cells[row, 8].Value = item.BankAccount;
                worksheet.Cells[row, 9].Value = item.Invoices;
                worksheet.Cells[row, 10].Value = item.DebitAccount;
                worksheet.Cells[row, 11].Value = item.CreditAccount;
                worksheet.Cells[row, 12].Value = item.Payee;
                worksheet.Cells[row, 13].Value = item.Notes;
                worksheet.Cells[row, 14].Value = item.ReceiptType;
                worksheet.Cells[row, 15].Value = item.ReceiptNumber;
                worksheet.Cells[row, 16].Value = item.Status;
                row++;
            }
            worksheet.Cells.AutoFitColumns();
            return await package.GetAsByteArrayAsync();
        }

        public async Task ImportReceiptsFromExcelAsync(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var dateText = worksheet.Cells[row, 1].Text;
                var partnerIdText = worksheet.Cells[row, 3].Text;
                var amountText = worksheet.Cells[row, 6].Text;
                var invoicesText = worksheet.Cells[row, 9].Text;

                if (string.IsNullOrWhiteSpace(dateText) ||
                    !int.TryParse(partnerIdText, out int partnerId) ||
                    !decimal.TryParse(amountText, out decimal amount)) continue;

                var partner = _partnerRepository.GetByIdNotDeleted(partnerId);
                if (partner == null) continue;

                if (!string.IsNullOrEmpty(invoicesText))
                {
                    var invoice = _invoiceRepository.GetByCode(invoicesText);
                    if (invoice == null) continue;
                }

                if (!DateTime.TryParse(dateText, out DateTime createdDate)) createdDate = DateTime.Now;

                var dto = new ReceiptDTO
                {
                    DateCreated = createdDate,
                    AccountingDate = createdDate,
                    PartnerName = partner.PartnerName,
                    PartnerId = partnerId,
                    Address = worksheet.Cells[row, 4].Text ?? "",
                    Reason = worksheet.Cells[row, 5].Text,
                    Amount = amount,
                    PaymentMethod = worksheet.Cells[row, 7].Text,
                    BankAccount = worksheet.Cells[row, 8].Text,
                    Invoices = invoicesText,
                    DebitAccount = worksheet.Cells[row, 10].Text,
                    CreditAccount = worksheet.Cells[row, 11].Text,
                    Payee = worksheet.Cells[row, 12].Text,
                    Notes = worksheet.Cells[row, 13].Text,
                    ReceiptType = "PT",
                    CreatedBy = "System Import",
                    Status = "Approved"
                };

                CreateReceipt(dto);
            }
        }
    }
}