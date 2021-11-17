using IoniCRM.Models;
using IoniCRM.Controllers.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IoniCRM.Controllers
{
    public class HomeController : Controller
    {

        private PostgreSQLConnection pgsqlcon;
        

        public HomeController()
        {
            pgsqlcon = new();
        }
        

        public IActionResult Home()
        {
            if (Session.Empty(HttpContext.Session))
                return RedirectToAction("Login", "Login");
            
            ViewBag.Usuario = Session.GetUsuario(HttpContext.Session);
            return View();
        }

        // teste
        public void Historico() { }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
