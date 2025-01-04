using System.ComponentModel.DataAnnotations;

public class PaymentViewModel
{
    [Required]
    public string PaymentMethod { get; set; }

    public IFormFile BankTransferFile { get; set; }

    [RequiredIf("PaymentMethod", "CreditCard", ErrorMessage = "Le nom sur la carte est requis.")]
    public string CardHolderName { get; set; }

    [RequiredIf("PaymentMethod", "CreditCard", ErrorMessage = "Le numéro de carte est requis.")]
    public string CardNumber { get; set; }

    [RequiredIf("PaymentMethod", "CreditCard", ErrorMessage = "La date d'expiration est requise.")]
    public string ExpiryDate { get; set; }

    [RequiredIf("PaymentMethod", "CreditCard", ErrorMessage = "Le CVV est requis.")]
    public string CVV { get; set; }
}
