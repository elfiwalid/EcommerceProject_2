using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using EcommerceProject.Models;
using EcommerceProject.Data;

namespace EcommerceProject.Controllers
{
    public class ProduitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProduitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static List<Produit> produits = new List<Produit>
        {
            new Produit { Id = 1, Nom = "Canon 400D", Description = "Camera", Prix = 999m, ImageUrl = "/images/camera-icon.png", Categorie = "Électronique" },
            new Produit { Id = 2, Nom = "Macbook Pro", Description = "Laptop", Prix = 1099m, ImageUrl = "/images/laptop-icon.png", Categorie = "Électronique" },
            new Produit { Id = 3, Nom = "Samsung S24 Ultra", Description = "Smartphone", Prix = 899.99m, ImageUrl = "/images/phone-icon.png", Categorie = "Électronique" }
        };

        // Display all products
        public IActionResult Index()
        {
            var panier = GetPanier();
            ViewData["CartCount"] = panier.Sum(item => item.Quantite);
            return View(produits);
        }

        // Add product to cart
        // public IActionResult AjouterAuPanier(int id)
        // {
        //     var produit = produits.FirstOrDefault(p => p.Id == id);
        //     if (produit == null) return NotFound();

        //     var panier = GetPanier();
        //     var item = panier.FirstOrDefault(p => p.ProduitId == id);

        //     if (item != null)
        //     {
        //         item.Quantite++;
        //     }
        //     else
        //     {
        //         panier.Add(new CartItem
        //         {
        //             ProduitId = produit.Id,
        //             Nom = produit.Nom,
        //             Prix = produit.Prix,
        //             Quantite = 1,
        //         });
        //     }

        //     SavePanier(panier);
        //     return RedirectToAction("Panier");
        // }


        public IActionResult AjouterAuPanier(int id)
{
    // Vérifier si le produit existe
    var produit = _context.Produits.FirstOrDefault(p => p.Id == id); // Remplacez produits par _context.Produits si vous utilisez une base de données.
    if (produit == null) return NotFound("Repture de stock");

    // Récupérer le panier actuel
    var panier = GetPanier();

    // Vérifier si le produit est déjà dans le panier
    var item = panier.FirstOrDefault(p => p.ProduitId == id);

    if (item != null)
    {
        // Incrémenter la quantité si le produit est déjà dans le panier
        item.Quantite++;
    }
    else
    {
        // Ajouter un nouveau produit au panier
        panier.Add(new CartItem
        {
            ProduitId = produit.Id,
            Nom = produit.Nom,
            Prix = produit.Prix,
            Quantite = 1
        });
    }

    // Sauvegarder le panier mis à jour
    SavePanier(panier);

    // Rediriger vers la vue Panier
    return RedirectToAction("Panier");
}


        // Modify item quantity in the cart
        [HttpPost]
        public IActionResult ModifierQuantite(int id, int quantite)
        {
            var panier = GetPanier();
            var item = panier.FirstOrDefault(i => i.ProduitId == id);

            if (item != null)
            {
                if (quantite <= 0)
                {
                    panier.Remove(item);
                }
                else
                {
                    item.Quantite = quantite;
                }

                SavePanier(panier);
            }

            return RedirectToAction("Panier");
        }

        // Display the cart
        public IActionResult Panier()
        {
            var panier = GetPanier();
            ViewData["CartCount"] = panier.Sum(item => item.Quantite);
            return View(panier);
        }

        // Remove item or reduce quantity from the cart
        [HttpPost]
        public IActionResult SupprimerQuantite(int id, int quantite)
        {
            var panier = GetPanier();
            var item = panier.FirstOrDefault(i => i.ProduitId == id);

            if (item != null)
            {
                if (quantite >= item.Quantite)
                {
                    panier.Remove(item);
                }
                else
                {
                    item.Quantite -= quantite;
                }

                SavePanier(panier);
            }

            return RedirectToAction("Panier");
        }

        // Payment page
        public IActionResult Payment()
        {
            var panier = GetPanier();
            if (!panier.Any())
            {
                TempData["Message"] = "Votre panier est vide.";
                return RedirectToAction("Panier");
            }

            ViewData["Total"] = panier.Sum(item => item.Prix * item.Quantite);
            return View();
        }

        // Process payment
        [HttpPost]
        public IActionResult ProcessPayment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Veuillez vérifier vos informations de paiement.";
                return View("Payment", model);
            }

            var panier = GetPanier();
            if (panier.Any())
            {
                SavePanier(new List<CartItem>());
                TempData["Message"] = "Paiement validé ! Merci pour votre commande.";
                return RedirectToAction("Index");
            }

            TempData["Message"] = "Erreur : Panier vide.";
            return RedirectToAction("Panier");
        }

        // Search for products
        [HttpGet]
        public IActionResult Recherche(string terme)
        {
            if (string.IsNullOrWhiteSpace(terme))
            {
                return View("SearchResults", Enumerable.Empty<Produit>());
            }

            // Perform filtering in memory
            var produits = _context.Produits
                .AsEnumerable()
                .Where(p => p.Nom.Contains(terme, StringComparison.OrdinalIgnoreCase)
                         || p.Description.Contains(terme, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return View("SearchResults", produits);
        }

        // Product details
        public IActionResult Details(int id)
        {
            var produit = produits.FirstOrDefault(p => p.Id == id);
            if (produit == null) return NotFound();
            return View(produit);
        }

        // Helper methods
        private List<CartItem> GetPanier()
        {
            var panierJson = HttpContext.Session.GetString("Panier");
            return string.IsNullOrEmpty(panierJson) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(panierJson);
        }

        private void SavePanier(List<CartItem> panier)
        {
            HttpContext.Session.SetString("Panier", JsonConvert.SerializeObject(panier));
        }
    }
}
