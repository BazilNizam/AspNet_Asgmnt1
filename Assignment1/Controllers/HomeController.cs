using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Linq;

namespace Assignment1.Controllers
{
    public class HomeController : Controller
    {
        // path to the json file where user data is stored
        private readonly string _usersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "users.json");

        // index action
        public IActionResult Index()
        {
            return View(new UserModel());
        }

        // astra action
        public IActionResult Astra()
        {
            return View();
        }

        // post action for index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(UserModel model, string action)
        {
            // check if the model is valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //check model state again (redundant?)
            if (ModelState.IsValid)
            {
                // save the user data
                try
                {
                    SaveUserData(model);
                    TempData["SuccessMessage"] = "User created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            return View(model);
        }

        // method to save user data
        private void SaveUserData(UserModel model)
        {
            // load the existing user data  
            List<UserModel> users = LoadUserData();

            // ensure the country code is saved correctly
            model.CountryCode = model.CountryCode.Replace("\\u002B", "+");

            users.Add(model);

            // set up json serializer options
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            // serialize and save the updated user data
            string updatedJson = JsonSerializer.Serialize(users, options);
            System.IO.File.WriteAllText(_usersFilePath, updatedJson);
        }

        // login page action
        public IActionResult Login()
        {
            return View();
        }

        // login post action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var users = LoadUserData();
                Console.WriteLine($"Attempting login for email: {model.Email}");
                var user = users.FirstOrDefault(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));

                if (user != null)
                {
                    Console.WriteLine("User found. Checking password.");
                    if (user.Password == model.Password)
                    {
                        Console.WriteLine("Password correct. Redirecting to Success.");
                        return RedirectToAction("Success", new { email = user.Email });
                    }
                    else
                    {
                        Console.WriteLine("Password incorrect.");
                    }
                }
                else
                {
                    Console.WriteLine("User not found.");
                }

                ModelState.AddModelError("", "Invalid email or password");
            }

            return View(model);
        }

        // success action after successful login
        public IActionResult Success(string email)
        {
            var users = LoadUserData();
            var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // method to load user data from json file
        private List<UserModel> LoadUserData()
        {
            if (System.IO.File.Exists(_usersFilePath))
            {
                string jsonContent = System.IO.File.ReadAllText(_usersFilePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var users = JsonSerializer.Deserialize<List<UserModel>>(jsonContent, options) ?? new List<UserModel>();
                Console.WriteLine($"Loaded {users.Count} users from JSON file.");
                return users;
            }
            Console.WriteLine("Users file not found.");
            return new List<UserModel>();
        }
    }
}