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
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public List<PaymentDTO> GetAllPayments()
        {
            var payments = _paymentRepository.GetAll();
            return _mapper.Map<List<PaymentDTO>>(payments);
        }

        public PaymentDTO GetPaymentById(int id)
        {
            var payment = _paymentRepository.GetById(id);
            return _mapper.Map<PaymentDTO>(payment);
        }

        public void AddPayment(PaymentDTO paymentDTO)
        {
            var payment = _mapper.Map<Payment>(paymentDTO);
            string paymentNumber = GeneratePaymentNumber();
            payment.PaymentNumber = paymentNumber;
            payment.DateCreated = DateTime.Now;
            payment.AccountingDate = DateTime.Now;
            payment.Status = "Nháp";
            payment.AmountInWords = NumberToWords.ConvertAmountToWords(payment.Amount);
            if (payment.PaymentMethod == "Chuyển khoản")
            {
                payment.BankAccountFrom = "Ngân hàng xuất tiền";
                payment.BankAccountTo = "STK người nhận";
            }
            payment.DebitAccount = "331";
            payment.CreditAccount = "1111";
            _paymentRepository.Add(payment);
        }

        public void UpdatePayment(PaymentDTO paymentDTO)
        {
            var payment = _mapper.Map<Payment>(paymentDTO);
            _paymentRepository.Update(payment);
        }

        public void DeletePayment(int id)
        {
            var payment = _paymentRepository.GetById(id);
            _paymentRepository.Delete(payment);
        }

        private string GeneratePaymentNumber()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            int count = _paymentRepository.GetAll().Where(p => p.PaymentNumber.StartsWith($"PC-{datePart}")).Count();
            string serialNumber = (count + 1).ToString("D3");
            return $"PC-{datePart}-{serialNumber}";
        }
    }
}
