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
            [Route("Payment/ProcessPayment")]
            public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
{
    _logger.LogInformation("ProcessPayment method called");

    if (ModelState.IsValid)
    {
        switch (model.PaymentMethod)
        {
            case "CreditCard":
                _logger.LogInformation("Credit Card payment selected");
                TempData["SuccessMessage"] = "Votre paiement par carte a été traité avec succès.";
                return RedirectToAction("Confirmation");

            case "PayPal":
                _logger.LogInformation("PayPal payment selected");
                string paypalUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";
                string businessEmail = "your-paypal-email@example.com";
                string returnUrl = Url.Action("Confirmation", "Payment", null, Request.Scheme);
                string cancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme);

                string queryString = $"?cmd=_xclick&business={businessEmail}&item_name=Achat sur Mekog&amount=100.00&currency_code=EUR&return={returnUrl}&cancel_return={cancelUrl}";
                return Redirect(paypalUrl + queryString);

            case "COD":
                _logger.LogInformation("Cash on Delivery payment selected");
                TempData["SuccessMessage"] = "Votre commande a été enregistrée avec Paiement à la livraison.";
                return RedirectToAction("Confirmation");

            case "BankTransfer":
                _logger.LogInformation("Bank Transfer payment selected");
                if (model.BankTransferFile != null && model.BankTransferFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.BankTransferFile.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

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
                _logger.LogWarning("Invalid payment method selected");
                ModelState.AddModelError("", "Méthode de paiement invalide.");
                return View("Index", model);
        }
    }

    _logger.LogWarning("ModelState is invalid");
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
