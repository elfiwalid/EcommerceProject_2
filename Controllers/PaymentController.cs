using EcommerceProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EcommerceProject.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Total"] = 100.00; // Example total amount
            return View(new PaymentViewModel());
        }

        [HttpPost]
public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
{
    if (ModelState.IsValid)
    {
        switch (model.PaymentMethod)
        {
            case "CreditCard":
                // Process Credit Card payment
                TempData["SuccessMessage"] = "Votre paiement par carte a été traité avec succès.";
                return RedirectToAction("Confirmation");

            case "PayPal":
                // Redirect to PayPal
                string paypalUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";
                string businessEmail = "your-paypal-email@example.com";

                string returnUrl = Url.Action("Confirmation", "Payment", null, Request.Scheme);
                string cancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme);

                string queryString = $"?cmd=_xclick&business={businessEmail}&item_name=Achat sur Mekog&amount=100.00&currency_code=EUR&return={returnUrl}&cancel_return={cancelUrl}";
                return Redirect(paypalUrl + queryString);

            case "COD":
                // Handle Cash on Delivery
                TempData["SuccessMessage"] = "Votre commande a été enregistrée avec Paiement à la livraison.";
                return RedirectToAction("Confirmation");

            case "BankTransfer":
                // Process Bank Transfer
                if (model.BankTransferFile != null && model.BankTransferFile.Length > 0)
                {
                    // Generate a unique file name
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.BankTransferFile.FileName)}";

                    // Define the path to save the file (e.g., "wwwroot/uploads")
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.BankTransferFile.CopyToAsync(stream);
                    }

                    TempData["SuccessMessage"] = "Votre preuve de virement bancaire a été téléchargée avec succès.";
                }
                else
                {
                    ModelState.AddModelError("", "Veuillez télécharger une preuve de virement.");
                    return View("Index", model);
                }
                return RedirectToAction("Confirmation");

            default:
                // If no valid payment method is selected
                ModelState.AddModelError("", "Méthode de paiement invalide.");
                return View("Index", model);
        }
    }

    // If model state is invalid, return to the form
    return View("Index", model);
}


        [HttpGet]
        public IActionResult Confirmation()
        {
            ViewBag.Message = TempData["SuccessMessage"] ?? "Votre paiement a été traité.";
            return View();
        }

        [HttpGet]
        public IActionResult Cancel()
        {
            ViewBag.Message = "Le paiement a été annulé.";
            return View("Confirmation");
        }
    }
}
