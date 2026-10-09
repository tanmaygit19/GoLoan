using GoLoan.Application.Interfaces;
using GoLoan.Domain.Entities;
using GoLoan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Infrastructure.Repositories
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext db;
        public NotificationService(AppDbContext db)
        {
            this.db = db;
        }
        public async Task CreateEmiReminders()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            var dayAfterTomorrow = tomorrow.AddDays(1);

            var upcomingEmis = await db.EmiSchedules.Include(e => e.LoanAccount).Where(e =>
                    e.DueDate >= tomorrow &&
                    e.DueDate < dayAfterTomorrow &&
                    e.PaymentStatus == "Pending")
                    .ToListAsync();

            foreach (var emi in upcomingEmis)
            {
                var customerId = emi.LoanAccount.CustomerId;

                var message = $"Reminder: Your EMI of {emi.Emi:C} " +
                              $"is due on {emi.DueDate:dd MMM yyyy}.";

                var alreadyExists = await db.Notifications.AnyAsync(n =>n.CustomerId == customerId & n.Message == message);

                if (!alreadyExists)
                {
                    var notification =
                        new GoLoan.Domain.Entities.Notification
                        {
                            CustomerId = customerId,
                            Message = message,
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        };

                    db.Notifications.Add(notification);
                }
            }

            await db.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetNotifications(int customerId)
        {
            return await db.Notifications.Where(n => n.CustomerId == customerId && !n.IsRead).OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task MarkAsRead(int notificationId, int customerId)
        {
            var notification = await db.Notifications.FirstOrDefaultAsync(n =>n.NotificationId == notificationId && n.CustomerId == customerId);

            if (notification != null)
            {
                notification.IsRead = true;
                await db.SaveChangesAsync();
            }
        }
    
    }
}
