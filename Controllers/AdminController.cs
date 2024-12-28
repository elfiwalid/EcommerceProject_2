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

        // Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // View all users
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Add User via Modal
        [HttpPost]
public async Task<IActionResult> AddUser(string userName, string email, string password)
{
    if (ModelState.IsValid)
    {
        var user = new IdentityUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true // Automatically confirm the email
        };

        // Create the user
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Assign a default role
            await _userManager.AddToRoleAsync(user, "User");

            return RedirectToAction("Users"); // Redirect to the Users view
        }

        // Handle errors during user creation
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
    }

    return RedirectToAction("Users"); // Redirect in case of failure
}

        // Edit User via Modal
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

        // Delete User via Modal
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

        // List Products
        public IActionResult Produits()
        {
            var produits = _context.Produits.ToList();
            return View(produits);
        }

        // Add Product
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

        // Edit Product
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

        // Delete Product
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
    }
}
