using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class UserController : BaseController
    {
        private readonly IRepositoryService<User> _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEncryptionService _encryptionService;

        public UserController(IRepositoryService<User> userRepository,
            IConfiguration configuration,
            IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _encryptionService = encryptionService;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await _userRepository.FindAsync(x => true));
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userRepository.FindOneAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Email,Password,EmailConfirmed")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = _encryptionService.Encrypt(user.Password);

                await _userRepository.InsertOneAsync(user);
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userRepository.FindOneAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Surname,Email,EmailConfirmed")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var dbUser = await _userRepository.FindOneAsync(x => x.Id == id);
                    if (dbUser == null)
                    {
                        return NotFound();
                    }

                    dbUser.Name = user.Name;
                    dbUser.Surname = user.Surname;
                    dbUser.Email = user.Email;
                    dbUser.EmailConfirmed = user.EmailConfirmed;

                    await _userRepository.ReplaceOneAsync(x => x.Id == id, dbUser);
                }
                catch (Exception)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workspace = await _userRepository.FindOneAsync(x => x.Id == id);
            if (workspace == null)
            {
                return NotFound();
            }

            return View(workspace);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userRepository.FindOneAsync(x => x.Id == id);
            if (user != null)
            {
                await _userRepository.DeleteOneAsync(x => x.Id == id);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userRepository.FindOneAsync(f => f.Email == model.Email
            && f.Password == _encryptionService.Encrypt(model.Password)
            && !f.IsDeleted);
            if (user == null)
            {
                model.ErrorMessage = "Hatalı giriş denemesi.";
                return View(model);
            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    RedirectUri = "/"
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

            }
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string returnUrl = "")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        private bool UserExists(string id)
        {
            return _userRepository.FindOneAsync(x => x.Id == id) != null;
        }
    }
}
