using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FriendFace.Models; // Assuming this is where your User model is located

namespace FriendFace.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Login/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Login/Register
        [HttpPost]
        public async Task<IActionResult> Register(string fname, string lname,string uname, string email, string psw)
        {
            var user = new User {FirstName = fname, LastName = lname, UserName = uname, Email = email };
            try
            {
                var result = await _userManager.CreateAsync(user, psw);
                // Handle result...
                if (result.Succeeded)
                {
                    // Optionally, sign the user in after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home"); // Modify as needed
                }

                // Handle errors
                foreach (var error in result.Errors)
                {
                    throw new Exception(error.Description);
                }
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException($"Unexpected error occurred in {nameof(Register)}", ex);
            }

            

            return View();
        }

        // GET: Login/Login
        public IActionResult Login()
        {
            // return to home page if user is already logged in
            return View("Index");
        }

        // POST: Login/Login
        [HttpPost]
        public async Task<IActionResult> Login(string uname, string psw)
        {
            var result = await _signInManager.PasswordSignInAsync(uname, psw, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home"); // Modify as needed
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View("Index");
        }

        // POST: Login/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); // Modify as needed
        }
    }
}
