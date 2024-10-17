namespace Domain.Interfaces.Messaging
{
    public interface IProducer<T>
    {
        void Publish(T message);
    }
}
