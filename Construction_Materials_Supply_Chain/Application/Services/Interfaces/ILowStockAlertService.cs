using Application.DTOs;

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
