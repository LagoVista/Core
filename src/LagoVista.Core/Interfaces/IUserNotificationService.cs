using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public enum UserNotificationTypes
    {
        Email = 0b00000001,
        TextMessage = 0b00000010,
        TeamsNotification = 0b00000100,
        MobileApp = 0b00001000,
    }

    public interface IUserNotificationService
    {
        Task QueueDiscussionNotificationAsync(string userId, EntityBase entity, Discussion discussion);
        Task QueueDiscussionNotificationAsync(string userId, IDiscussableEntity entity, Discussion discussion);
        Task QueueDiscussionNotificationAsync(string userId, IDiscussableEntity entity, Discussion discussion, DiscussionResponse response);
        Task QueueTeamsNotification(string webHookUrl, string cardContent);  
        Task SendAdaptiveCardAsync(string webHookUrl, AdaptiveCard card);
    }

    public static class UserNotificationServiceProvider
    {
       public static IUserNotificationService Instance { get; set; }
    }
}
