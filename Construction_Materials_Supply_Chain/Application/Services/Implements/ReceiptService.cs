using Application.DTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Infrastructure.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly IMapper _mapper;

        public ReceiptService(IReceiptRepository receiptRepository, IMapper mapper)
        {
            _receiptRepository = receiptRepository;
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

        public void CreateReceipt(ReceiptDTO dto, IFormFile file)
        {
            var receipt = _mapper.Map<Receipt>(dto);
            receipt.ReceiptNumber = GenerateReceiptNumber();
            receipt.AttachmentFile = SaveFile(file);
            receipt.Status = "Draft";

            _receiptRepository.Add(receipt);
        }

        public Receipt GetReceiptById(int id)
        {
            return _receiptRepository.GetById(id);
        }

        public List<Receipt> GetAllReceipts()
        {
            return _receiptRepository.GetAll();
        }

        public List<Receipt> GetReceiptsByPartnerId(int partnerId)
        {
            return _receiptRepository.GetReceiptsByPartnerId(partnerId);
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
