using Application.Common;
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

namespace Application.Services.Implements
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<PaymentCreateDto> _validator;

        public PaymentService(IPaymentRepository paymentRepository, IMapper mapper, IValidator<PaymentCreateDto> validator)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<PaymentDto> CreatePaymentAsync(PaymentCreateDto paymentCreateDto, int partnerId, string createdBy)
        {
            var validationResult = await _validator.ValidateAsync(paymentCreateDto);
            if (!validationResult.IsValid)
                throw new ArgumentException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var payment = _mapper.Map<Payment>(paymentCreateDto);
            payment.PartnerId = partnerId;
            payment.CreatedBy = createdBy;
            payment.Status = "Draft";
            payment.DateCreated = DateTime.Now;
            payment.AmountInWords = NumberToWords.Convert(paymentCreateDto.Amount);

            _paymentRepository.Add(payment);
            return _mapper.Map<PaymentDto>(payment);
        }
    }
}
