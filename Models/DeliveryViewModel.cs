
namespace EcommerceProject.Models
{
    public class DeliveryViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
