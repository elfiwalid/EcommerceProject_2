using Microsoft.AspNetCore.Mvc;
using EcommerceProject.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.Data;
using EcommerceProject.Models;

namespace EcommerceProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Simulating orders data for demonstration
        private static List<Order> orders = new List<Order>
        {
            new Order
            {
                OrderId = 1,
                UserId = "user1", // Simulate logged-in user ID
                OrderDate = DateTime.Now.AddDays(-2),
                TotalAmount = 200.50m,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProduitId = 1, Nom = "Canon 400D", Quantite = 1, Prix = 999m },
                    new OrderItem { ProduitId = 2, Nom = "Macbook Pro", Quantite = 1, Prix = 1099m }
                }
            },
            new Order
            {
                OrderId = 2,
                UserId = "user1", // Simulate logged-in user ID
                OrderDate = DateTime.Now.AddDays(-10),
                TotalAmount = 899.99m,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProduitId = 3, Nom = "Samsung S24 Ultra", Quantite = 1, Prix = 899.99m }
                }
            },
            new Order
            {
                OrderId = 3,
                UserId = "user2", // Another user
                OrderDate = DateTime.Now.AddDays(-5),
                TotalAmount = 499.99m,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProduitId = 4, Nom = "iPad Air", Quantite = 1, Prix = 499.99m }
                }
            }
        };

        // Retourner les commandes d'un utilisateur spécifique
       

        // Retourner toutes les commandes (pour administrateurs ou gestionnaires)
        public IActionResult GetAllOrders()
        {
            return View(orders);
        }
        [HttpGet]
        public IActionResult Index()
        {
            var orders = _context.Deliveries.ToList();
            return View(orders);
        }
    }
}
