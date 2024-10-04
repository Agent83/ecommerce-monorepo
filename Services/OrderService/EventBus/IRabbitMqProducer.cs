namespace OrderService.EventBus
{
    public interface IRabbitMqProducer
    {
        void SendMessage<T>(T message);
    }
}
