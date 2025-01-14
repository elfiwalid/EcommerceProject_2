using EcommerceProject.Data;
using EcommerceProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Tableau de bord
        public IActionResult Dashboard()
{
    var totalUsers = _context.Users.Count();
    var totalProducts = _context.Produits.Count();
    var totalCategories = _context.Categories.Count();

    ViewData["TotalUsers"] = totalUsers;
    ViewData["TotalProducts"] = totalProducts;
    ViewData["TotalCategories"] = totalCategories;

    return View();
}

        // Liste des utilisateurs
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Ajouter un utilisateur
        [HttpPost]
        public async Task<IActionResult> AddUser(string userName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Tous les champs sont obligatoires.");
                return RedirectToAction("Users");
            }

            var user = new IdentityUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return RedirectToAction("Users");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction("Users");
        }

        // Modifier un utilisateur
        [HttpPost]
        public async Task<IActionResult> EditUser(string id, string userName, string email)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = userName;
            user.Email = email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Users");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction("Users");
        }

        // Supprimer un utilisateur
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Users");
        }

        // Liste des produits
        public IActionResult Produits()
        {
            var produits = _context.Produits.ToList();
            return View(produits);
        }

        // Ajouter un produit
        [HttpPost]
        public async Task<IActionResult> AjouterProduit(Produit produit)
        {
            if (ModelState.IsValid)
            {
                _context.Produits.Add(produit);
                await _context.SaveChangesAsync();
                return RedirectToAction("Produits");
            }

            return RedirectToAction("Produits");
        }

        // Modifier un produit
        [HttpPost]
        public async Task<IActionResult> ModifierProduit(Produit produit)
        {
            if (ModelState.IsValid)
            {
                _context.Produits.Update(produit);
                await _context.SaveChangesAsync();
                return RedirectToAction("Produits");
            }

            return RedirectToAction("Produits");
        }

        // Supprimer un produit
        [HttpPost]
        public async Task<IActionResult> SupprimerProduit(int id)
        {
            var produit = await _context.Produits.FindAsync(id);
            if (produit != null)
            {
                _context.Produits.Remove(produit);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Produits");
        }
        // Liste des livraisons
        public IActionResult Order()
        {
            var deliveries = _context.Deliveries.ToList(); // Remplacez 'Deliveries' par le nom correct de votre DbSet pour les livraisons
            return View(deliveries);
        }
    }
}
