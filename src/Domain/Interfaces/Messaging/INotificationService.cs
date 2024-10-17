namespace Domain.Interfaces.Messaging
{
    public interface INotificationService
    {
        Task NotifyAsync(string message);
    }

}
