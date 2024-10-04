namespace OrderService.EventBus.Models
{
    public class OrderMessage
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public string  ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
