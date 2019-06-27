using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XolidQueryExample.Models;
using XolidQueryExample.Repositories;

namespace XolidQueryExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository _userRepository;

        public HomeController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<IActionResult> Index(User user)
        {
            List<User> users = _userRepository.GetUserList(user);
            ViewData["user"] = user;
            
            return View(users);
        }

        public async Task<IActionResult> Edit(int id)
        {
            User user = _userRepository.GetUser(id);
            
            return View(user);
        }
        
        public async Task<IActionResult> Update(User user)
        {
            int rows = _userRepository.UpdateUser(user);
            ViewData["rows"] = rows;
            
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            User user = _userRepository.GetUser(id);
            
            return View(user);
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            int rows = _userRepository.DeleteUser(id);
            ViewData["rows"] = rows;
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}