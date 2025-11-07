using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface INotificationService
    {
        NotificationResponseDto CreateConversation(CreateConversationRequestDto dto);
        NotificationResponseDto CreateAlert(CreateAlertRequestDto dto);
        void AddRecipientsByUsers(int notificationId, int partnerId, IEnumerable<int> userIds);
        void AddRecipientsByRoles(int notificationId, int partnerId, IEnumerable<int> roleIds);
        void Reply(ReplyRequestDto dto);
        void MarkRead(AckReadCloseRequestDto dto);
        void Acknowledge(AckReadCloseRequestDto dto);
        void Close(AckReadCloseRequestDto dto);
        NotificationResponseDto GetById(int id, int partnerId);
        List<NotificationResponseDto> GetByPartner(int partnerId);
        void SendCrossPartnerAlert(CrossPartnerAlertRequestDto dto);
        List<NotificationResponseDto> GetForUser(int partnerId, int userId);
        int CountUnreadForUser(int partnerId, int userId);
    }
}
