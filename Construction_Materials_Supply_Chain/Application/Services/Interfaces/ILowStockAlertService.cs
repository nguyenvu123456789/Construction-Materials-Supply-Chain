using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ILowStockAlertService
    {
        int Create(AlertRuleCreateDto dto);
        void Update(AlertRuleUpdateDto dto);
        void Toggle(int ruleId, int partnerId, bool isActive);
        void RunOnce(int partnerId);
    }
}
