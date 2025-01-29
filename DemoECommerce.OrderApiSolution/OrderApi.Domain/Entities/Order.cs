namespace OrderApi.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int PurchaseQuantity { get; set; }
        public DateTime OrderedDate { get; set; } = DateTime.UtcNow;
        
    }
}
