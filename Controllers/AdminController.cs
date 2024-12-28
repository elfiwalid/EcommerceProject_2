using EcommerceProject.Data;
using EcommerceProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace EcommerceProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

         public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult AjouterProduit()
{
    return View(); // This looks for AjouterProduit.cshtml in Views/Admin
}



      public IActionResult ModifierProduit()
{
    return View(); // This looks for AjouterProduit.cshtml in Views/Admin
}
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List Products
        public IActionResult Produits()
{
    var produits = _context.Produits.ToList();
    return View(produits);
}

[HttpPost]
[HttpPost]
public async Task<IActionResult> AjouterProduit(Produit produit)
{
    if (!ModelState.IsValid)
    {
        // Debugging output
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine(error.ErrorMessage);
        }

        return View(produit);
    }

    _context.Produits.Add(produit);
    await _context.SaveChangesAsync();
    return RedirectToAction("Produits");
}



[HttpPost]
public async Task<IActionResult> ModifierProduit(Produit produit)
{
    if (ModelState.IsValid)
    {
        _context.Produits.Update(produit);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction("Produits");
}

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
