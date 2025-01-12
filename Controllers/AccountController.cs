using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EcommerceProject.Models;
using EcommerceProject.Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<IdentityUser> userManager, 
                                 SignInManager<IdentityUser> signInManager, 
                                 RoleManager<IdentityRole> roleManager, 
                                 ILogger<AccountController> logger, 
                                 ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _context = context;
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        // POST: Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilisateur inscrit : {Email}", model.Email);

                    // Assigner directement le rôle "User"
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    await _userManager.AddToRoleAsync(user, "User");

                    // Connexion automatique après l'inscription
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }

                // Gérer les erreurs d'inscription
                foreach (var error in result.Errors)
                {
                    _logger.LogWarning("Erreur lors de l'inscription : {Error}", error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }


        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Utilisateur connecté : {Email}", model.Email);
                    return RedirectToAction("Index", "Home");
                }

                _logger.LogWarning("Connexion échouée pour : {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            }

            return View(model);
        }

        // POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Utilisateur déconnecté.");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Shop
        [Route("account/shop")]
        public IActionResult Shop()
        {
            _logger.LogInformation("La méthode Shop a été appelée.");

            var produits = _context.Produits.ToList();
            return View(produits);
        }

        // GET: Details
        [Route("account/details/{id}")]
        public IActionResult Details(int id)
        {
            var produit = _context.Produits.FirstOrDefault(p => p.Id == id);
            if (produit == null)
            {
                _logger.LogWarning("Produit non trouvé avec l'ID : {Id}", id);
                return NotFound("Produit non trouvé.");
            }

            return View(produit);
        }

        // GET: Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Aucun utilisateur connecté.");
                return RedirectToAction("Login");
            }

            var userProfile = new ProfileViewModel
            {
                Email = user.Email,
                UserName = user.UserName
            };

            return View(userProfile);
        }

        // POST: Update Profile
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("Aucun utilisateur connecté.");
                    return RedirectToAction("Login");
                }

                user.UserName = model.UserName;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Profil mis à jour pour : {Email}", user.Email);
                    TempData["SuccessMessage"] = "Profil mis à jour avec succès.";
                    return RedirectToAction("Profile");
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogWarning("Erreur lors de la mise à jour du profil : {Error}", error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
