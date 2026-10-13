using GoLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Application.Interfaces
{
  public interface INotificationService
    {
        Task CreateEmiReminders();
        Task<List<Notification>>GetNotifications(int customerId);
        Task MarkAsRead(int notificationId, int customerId);
    }
}
