using EcommerceProject.Data;
using EcommerceProject.Models;
using Microsoft.AspNetCore.Mvc;
using EcommerceProject.Models; // Ensure this matches the namespace of DeliveryViewModel


namespace EcommerceProject.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeliveryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new DeliveryViewModel());
        }

        [HttpPost]
        public IActionResult Index(DeliveryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save delivery information to the database
                var delivery = new DeliveryViewModel
                {
                    FullName = model.FullName,
                    Address = model.Address,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    Country = model.Country,
                    PhoneNumber = model.PhoneNumber
                };

                _context.Deliveries.Add(delivery);
                _context.SaveChanges();

                TempData["Message"] = "Informations de livraison enregistrées avec succès.";
                return RedirectToAction("Confirmation");
            }

            return View(model);
        }

        public IActionResult Confirmation()
        {
            ViewBag.Message = TempData["Message"] ?? "Informations de livraison soumises.";
            return View();
        }
        [HttpGet]
        public IActionResult List()
        {
            var deliveries = _context.Deliveries.ToList();
            return View(deliveries);
        }
    }
}
