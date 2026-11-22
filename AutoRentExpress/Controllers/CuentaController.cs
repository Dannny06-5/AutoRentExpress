
using AutoRentExpress.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AutoRentExpress.Controllers
{
    public class CuentaController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public CuentaController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginDto());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = await _userManager.FindByNameAsync(
                modelo.Usuario.Trim());

            if (usuario == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Usuario o contraseña incorrectos.");

                return View(modelo);
            }

            var resultado = await _signInManager.PasswordSignInAsync(
                usuario,
                modelo.Contrasena,
                modelo.Recordarme,
                lockoutOnFailure: true);

            if (resultado.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "La cuenta está bloqueada temporalmente.");

                return View(modelo);
            }

            if (resultado.IsNotAllowed)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Esta cuenta no puede iniciar sesión.");

                return View(modelo);
            }

            if (!resultado.Succeeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Usuario o contraseña incorrectos.");

                return View(modelo);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
