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
using Newtonsoft.Json.Linq;

namespace IoniCRM.Controllers
{
    public class DashboardController : Controller
    {

        private PostgreSQLConnection pgsqlcon;
        

        public DashboardController()
        {
            pgsqlcon = new();
        }
        

        public IActionResult Dashboard()
        {
            /*
            if (Session.Empty(HttpContext.Session))
                return RedirectToAction("Login", "Login");
            
            ViewBag.Usuario = Session.GetUsuario(HttpContext.Session);
            */
            return View();
        }

    }
}
