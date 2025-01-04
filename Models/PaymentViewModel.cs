using Microsoft.AspNetCore.Http;

namespace EcommerceProject.Models
{
    public class PaymentViewModel
    {
        public string PaymentMethod { get; set; }
        public IFormFile BankTransferFile { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
    }
}
