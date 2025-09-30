using BusinessObjects;

namespace DataAccess
{
    public class NotificationDAO
    {
        public static List<Notification> GetNotificationsByUser(int userId)
        {
            var list = new List<Notification>();
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    list = context.Notifications
                                  .Where(n => n.UserId == userId)
                                  .OrderByDescending(n => n.CreatedAt)
                                  .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }

        public static void MarkAsRead(int notificationId)
        {
            try
            {
                using (var context = new ScmVlxdContext())
                {
                    var noti = context.Notifications.SingleOrDefault(x => x.NotificationId == notificationId);
                    if (noti != null)
                    {
                        noti.IsRead = true;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
