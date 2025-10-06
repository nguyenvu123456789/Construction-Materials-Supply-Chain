using BusinessObjects;

namespace DataAccess
{
    public class NotificationDAO : BaseDAO
    {
        public NotificationDAO(ScmVlxdContext context) : base(context) { }

        public List<Notification> GetNotificationsByUser(int userId)
        {
            return Context.Notifications
                          .Where(n => n.UserId == userId)
                          .OrderByDescending(n => n.CreatedAt)
                          .ToList();
        }

        public void MarkAsRead(int notificationId)
        {
            var noti = Context.Notifications.SingleOrDefault(x => x.NotificationId == notificationId);
            if (noti != null)
            {
                noti.IsRead = true;
                Context.SaveChanges();
            }
        }
    }
}
