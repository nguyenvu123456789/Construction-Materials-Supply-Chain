using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAccountingEntryRepository _entryRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IAccountingEntryRepository entryRepository,
            IPartnerRepository partnerRepository,
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _entryRepository = entryRepository;
            _partnerRepository = partnerRepository;
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public string GeneratePaymentNumber()
        {
            var day = DateTime.Now.ToString("yyyyMMdd");
            var prefix = $"PC-{day}-";
            var lastPayment = _paymentRepository.GetLastPaymentByPrefix(prefix);
            var number = 1;

            if (lastPayment != null)
            {
                var numberPart = lastPayment.PaymentNumber.Substring(prefix.Length);
                if (int.TryParse(numberPart, out int parsed))
                    number = parsed + 1;
            }

            return $"{prefix}{number:D3}";
        }

        public void CreatePayment(PaymentDTO dto)
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

            var payment = _mapper.Map<Payment>(dto);
            payment.PaymentNumber = GeneratePaymentNumber();

            if (string.IsNullOrEmpty(payment.Account))
            {
                payment.Account = !string.IsNullOrEmpty(payment.BankAccountFrom)
                                  ? payment.BankAccountFrom
                                  : "TIEN_MAT";
            }

            payment.Status = "Approved";
            payment.PartnerName = partner.PartnerName;

            if (string.IsNullOrEmpty(payment.ApprovedBy)) payment.ApprovedBy = "System Admin";
            if (payment.ApprovalDate == null) payment.ApprovalDate = DateTime.Now;
            if (string.IsNullOrEmpty(payment.PaidBy)) payment.PaidBy = "Thu Quy";
            if (string.IsNullOrEmpty(payment.CreatedBy)) payment.CreatedBy = "System";

            _paymentRepository.Add(payment);

            var ledgerEntry = new AccountingEntry
            {
                EntryDate = payment.DateCreated,
                DocumentType = "PhieuChi",
                DocumentNumber = payment.PaymentNumber,
                Description = payment.Reason ?? $"Chi tiền {payment.PartnerName}",
                DebitAccount = payment.DebitAccount,
                CreditAccount = payment.CreditAccount,
                DebitAmount = 0,
                CreditAmount = payment.Amount,
                PartnerId = payment.PartnerId,
                PartnerName = payment.PartnerName
            };

            _entryRepository.Add(ledgerEntry);
        }

        public Payment GetPaymentById(int id) => _paymentRepository.GetById(id);
        public List<Payment> GetAllPayments() => _paymentRepository.GetAll();
        public List<Payment> GetPaymentsByPartnerId(int partnerId) => _paymentRepository.GetPaymentsByPartnerId(partnerId);

        public async Task<byte[]> ExportPaymentsToExcelAsync()
        {
            var payments = _paymentRepository.GetAll();
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Payments");

            string[] headers = {
                "Ngày Lập", "Tên Đối Tượng", "ID", "MST", "Lý Do", "Số Tiền",
                "HTTT", "NH Đi", "NH Đến", "Hóa Đơn", "Phòng Ban", "Người Yêu Cầu",
                "Người Nhận", "TK Nợ", "TK Có", "Ghi Chú", "Loại", "Mã Phiếu", "Trạng Thái"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            int row = 2;
            foreach (var item in payments)
            {
                worksheet.Cells[row, 1].Value = item.DateCreated.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 2].Value = item.PartnerName;
                worksheet.Cells[row, 3].Value = item.PartnerId;
                worksheet.Cells[row, 4].Value = item.TaxCode;
                worksheet.Cells[row, 5].Value = item.Reason;
                worksheet.Cells[row, 6].Value = item.Amount;
                worksheet.Cells[row, 7].Value = item.PaymentMethod;
                worksheet.Cells[row, 8].Value = item.BankAccountFrom;
                worksheet.Cells[row, 9].Value = item.BankAccountTo;
                worksheet.Cells[row, 10].Value = item.Invoices;
                worksheet.Cells[row, 11].Value = item.Department;
                worksheet.Cells[row, 12].Value = item.RequestedBy;
                worksheet.Cells[row, 13].Value = item.Recipient;
                worksheet.Cells[row, 14].Value = item.DebitAccount;
                worksheet.Cells[row, 15].Value = item.CreditAccount;
                worksheet.Cells[row, 16].Value = item.Notes;
                worksheet.Cells[row, 17].Value = item.PaymentType;
                worksheet.Cells[row, 18].Value = item.PaymentNumber;
                worksheet.Cells[row, 19].Value = item.Status;
                row++;
            }
            worksheet.Cells.AutoFitColumns();
            return await package.GetAsByteArrayAsync();
        }

        public async Task ImportPaymentsFromExcelAsync(IFormFile file)
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
                var invoicesText = worksheet.Cells[row, 10].Text;

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

                var bankAccountFrom = worksheet.Cells[row, 8].Text;

                var dto = new PaymentDTO
                {
                    DateCreated = createdDate,
                    AccountingDate = createdDate,
                    PartnerName = partner.PartnerName,
                    PartnerId = partnerId,
                    TaxCode = worksheet.Cells[row, 4].Text,
                    Reason = worksheet.Cells[row, 5].Text,
                    Amount = amount,
                    PaymentMethod = worksheet.Cells[row, 7].Text,
                    BankAccountFrom = bankAccountFrom,
                    BankAccountTo = worksheet.Cells[row, 9].Text,
                    Account = bankAccountFrom,
                    Invoices = invoicesText,
                    Department = worksheet.Cells[row, 11].Text,
                    RequestedBy = worksheet.Cells[row, 12].Text,
                    Recipient = worksheet.Cells[row, 13].Text,
                    DebitAccount = worksheet.Cells[row, 14].Text,
                    CreditAccount = worksheet.Cells[row, 15].Text,
                    Notes = worksheet.Cells[row, 16].Text,
                    PaymentType = "PC",
                    CreatedBy = "System Import",
                    Status = "Approved",
                    ApprovedBy = "System Admin",
                    PaidBy = "Thu Quy"
                };

                CreatePayment(dto);
            }
        }
    }
}