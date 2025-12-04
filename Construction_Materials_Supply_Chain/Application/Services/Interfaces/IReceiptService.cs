using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IReceiptService
    {
        Task<ReceiptDto> CreateReceiptAsync(ReceiptCreateDto receiptCreateDto, int partnerId, string createdBy);
    }
}
