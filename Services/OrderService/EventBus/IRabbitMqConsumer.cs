namespace OrderService.EventBus
{
    public interface IRabbitMqConsumer
    {
        void StartListening();
    }
}
