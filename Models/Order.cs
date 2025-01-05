namespace EcommerceProject.Models
{
    public class Order
    {
        public int OrderId { get; set; } // Unique ID for the order
        public string UserId { get; set; } // Link to the user
        public DateTime OrderDate { get; set; } = DateTime.Now; // Date of the order
        public decimal TotalAmount { get; set; } // Total amount of the order
        public List<OrderItem> Items { get; set; } = new List<OrderItem>(); // Items in the order
    }

    public class OrderItem
    {
        public int ProduitId { get; set; }
        public string Nom { get; set; }
        public int Quantite { get; set; }
        public decimal Prix { get; set; }
    }
}
