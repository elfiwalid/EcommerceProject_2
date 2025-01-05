using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using EcommerceProject.Models;

namespace EcommerceProject.Controllers
{
    public class ProduitsController : Controller
    {
        private static List<Produit> produits = new List<Produit>
        {
            new Produit { Id = 1, Nom = "Produit 1", Description = "Description 1", Prix = 10.99m, ImageUrl = "/images/camera-icon.png", Categorie = "Électronique" },
            new Produit { Id = 2, Nom = "Produit 2", Description = "Description 2", Prix = 20.99m, ImageUrl = "/images/product2.jpg", Categorie = "Maison" },
            new Produit { Id = 3, Nom = "Produit 3", Description = "Description 3", Prix = 30.99m, ImageUrl = "/images/product3.jpg", Categorie = "Vêtements" }
        };

        // Affiche les produits
        public IActionResult Index()
        {
            var panier = GetPanier();
            ViewData["CartCount"] = panier.Sum(item => item.Quantite);
            return View(produits);
        }

        // Ajouter au panier
        public IActionResult AjouterAuPanier(int id)
        {
            var produit = produits.FirstOrDefault(p => p.Id == id);
            if (produit == null) return NotFound();

            var panier = GetPanier();
            var item = panier.FirstOrDefault(p => p.ProduitId == id);

            if (item != null)
            {
                item.Quantite++;
            }
            else
            {
                panier.Add(new CartItem
                {
                    ProduitId = produit.Id,
                    Nom = produit.Nom,
                    Prix = produit.Prix,
                    Quantite = 1
                });
            }

            SavePanier(panier);
            return RedirectToAction("Panier");
        }

        [HttpPost]
public IActionResult ModifierQuantite(int id, int quantite)
{
    var panier = GetPanier();
    var item = panier.FirstOrDefault(i => i.ProduitId == id);

    if (item != null)
    {
        if (quantite <= 0)
        {
            panier.Remove(item); // Supprime l'article si la quantité est 0
        }
        else
        {
            item.Quantite = quantite; // Met à jour la quantité
        }

        SavePanier(panier); // Sauvegarde les modifications dans la session
    }

    return RedirectToAction("Panier");
}


        // Affiche le panier
        public IActionResult Panier()
        {
            var panier = GetPanier();
            ViewData["CartCount"] = panier.Sum(item => item.Quantite);
            return View(panier);
        }

        // Supprime un article du panier
       [HttpPost]
public IActionResult SupprimerQuantite(int id, int quantite)
{
    var panier = GetPanier();
    var item = panier.FirstOrDefault(i => i.ProduitId == id);

    if (item != null)
    {
        if (quantite >= item.Quantite)
        {
            // Supprime complètement l'article si la quantité demandée est égale ou supérieure à la quantité dans le panier
            panier.Remove(item);
        }
        else
        {
            // Réduit la quantité de l'article
            item.Quantite -= quantite;
        }

        SavePanier(panier); // Sauvegarde les modifications dans la session
    }

    return RedirectToAction("Panier");
}

        // Page de paiement
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

        // Processus de paiement
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
                SavePanier(new List<CartItem>()); // Vide le panier après paiement
                TempData["Message"] = "Paiement validé ! Merci pour votre commande.";
                return RedirectToAction("Index");
            }

            TempData["Message"] = "Erreur : Panier vide.";
            return RedirectToAction("Panier");
        }

        // Méthodes auxiliaires
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
