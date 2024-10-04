namespace UserService.EventsBus
{
    public interface IMessagePublisher: IDisposable
    {
        Task PublishAsync<T>(T message, string exchange, string routingKey);
    }
}