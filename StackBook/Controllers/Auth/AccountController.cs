using Microsoft.AspNetCore.Mvc;
using StackBook.Dto;
using StackBook.Models;
using StackBook.Services;

namespace StackBook.Controllers.Auth
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        public AccountController(UserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            Console.WriteLine("Vao controller");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            bool isRegistered = await _userService.RegisterUser(model);
            if (!isRegistered)
            {
                ModelState.AddModelError("", "Email is used.");
                return View(model);
            }
            return RedirectToAction("Signin");
        }
        [HttpGet]
        public IActionResult Signin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signin(LoginDto model)
        {
            System.Console.WriteLine("Model received: " + model?.Email + " - " + model?.Password);
            if (!ModelState.IsValid)
            {
                System.Console.WriteLine("ModelState is not valid");
                return View(model);
            }
            bool isLogin = false;
            if(model!=null)
            {
                isLogin = await _userService.SiginUser(model);
            }
            System.Console.WriteLine("Login result: " + isLogin);
            if (!isLogin)
            {
                ModelState.AddModelError("", "Email or password is incorrect.");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
