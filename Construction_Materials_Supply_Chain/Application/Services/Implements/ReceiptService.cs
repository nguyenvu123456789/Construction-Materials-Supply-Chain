using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interface;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;

namespace Infrastructure.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public ReceiptService(IReceiptRepository receiptRepository, IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _receiptRepository = receiptRepository;
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public List<ReceiptDTO> GetAllReceipts()
        {
            var receipts = _receiptRepository.GetAll();
            return _mapper.Map<List<ReceiptDTO>>(receipts);
        }

        public List<ReceiptDTO> GetReceiptsByPartnerId(int partnerId)
        {
            var receipts = _receiptRepository.GetReceiptsByPartnerId(partnerId);
            return _mapper.Map<List<ReceiptDTO>>(receipts);
        }

        public ReceiptDTO GetReceiptById(int id)
        {
            var receipt = _receiptRepository.GetById(id);
            return _mapper.Map<ReceiptDTO>(receipt);
        }

        public void AddReceipt(ReceiptDTO receiptDTO)
        {
            var receipt = _mapper.Map<Receipt>(receiptDTO);

            string receiptNumber = GenerateReceiptNumber();
            receipt.ReceiptNumber = receiptNumber;
            receipt.DateCreated = DateTime.Now;
            receipt.AccountingDate = DateTime.Now;
            receipt.Status = "Nháp";

            receipt.AmountInWords = NumberToWords.ConvertAmountToWords(receipt.Amount);

            receipt.DebitAccount = "1111";
            receipt.CreditAccount = "131";

            if (string.IsNullOrEmpty(receipt.Address))
            {
                receipt.Address = "Không có thông tin địa chỉ";
            }

            _receiptRepository.Add(receipt);

            if (!string.IsNullOrEmpty(receiptDTO.LinkedInvoiceIds))
            {
                foreach (var invoiceCode in receiptDTO.LinkedInvoiceIds.Split(','))
                {
                    var invoice = _invoiceRepository.GetByCode(invoiceCode.Trim());
                    if (invoice != null)
                    {
                        var receiptInvoice = new ReceiptInvoice
                        {
                            ReceiptId = receipt.Id,
                            InvoiceId = invoice.InvoiceId
                        };
                        _receiptRepository.AddReceiptInvoice(receiptInvoice);
                    }
                }
            }
        }

        public void UpdateReceipt(ReceiptDTO receiptDTO)
        {
            var receipt = _mapper.Map<Receipt>(receiptDTO);
            _receiptRepository.Update(receipt);
        }

        public void DeleteReceipt(int id)
        {
            var receipt = _receiptRepository.GetById(id);
            _receiptRepository.Delete(receipt);
        }

        private string GenerateReceiptNumber()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            int count = _receiptRepository.GetAll().Where(r => r.ReceiptNumber.StartsWith($"PT-{datePart}")).Count();
            string serialNumber = (count + 1).ToString("D3");
            return $"PT-{datePart}-{serialNumber}";
        }
    }
}
