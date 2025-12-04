using Application.DTOs;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;

namespace Application.Services.Implements
{
    public class ReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ReceiptCreateDto> _validator;

        public ReceiptService(IReceiptRepository receiptRepository, IMapper mapper, IValidator<ReceiptCreateDto> validator)
        {
            _receiptRepository = receiptRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ReceiptDto> CreateReceiptAsync(ReceiptCreateDto receiptCreateDto, int partnerId, string createdBy)
        {
            var validationResult = await _validator.ValidateAsync(receiptCreateDto);
            if (!validationResult.IsValid)
                throw new ArgumentException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var receipt = _mapper.Map<Receipt>(receiptCreateDto);
            receipt.PartnerId = partnerId;
            receipt.CreatedBy = createdBy;
            receipt.Status = "Draft";
            receipt.DateCreated = DateTime.Now;
            receipt.AmountInWords = NumberToWords.Convert(receiptCreateDto.Amount);

            _receiptRepository.Add(receipt);
            return _mapper.Map<ReceiptDto>(receipt);
        }
    }
}
