using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
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

        public void CreatePayment(PaymentDTO dto, IFormFile file)
        {
            var payment = _mapper.Map<Payment>(dto);
            payment.PaymentNumber = GeneratePaymentNumber();
            payment.AttachmentFile = SaveFile(file);
            payment.Status = "Draft";

            _paymentRepository.Add(payment);
        }

        public Payment GetPaymentById(int id)
        {
            return _paymentRepository.GetById(id);
        }

        public List<Payment> GetAllPayments()
        {
            return _paymentRepository.GetAll();
        }

        public List<Payment> GetPaymentsByPartnerId(int partnerId)
        {
            return _paymentRepository.GetPaymentsByPartnerId(partnerId);
        }

        private string SaveFile(IFormFile file)
        {
            if (file == null) return null;

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, file.FileName);

            using var stream = new FileStream(path, FileMode.Create);
            file.CopyTo(stream);

            return Path.Combine("UploadedFiles", file.FileName);
        }
    }
}
