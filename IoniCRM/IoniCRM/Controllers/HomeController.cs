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
        public Usuario usuario;
        

        public HomeController()
        {
            pgsqlcon = new();
        }
        

        public IActionResult Home()
        {
            //session here
            if (HttpContext.Session.GetInt32(Session.SessionKeyName) != default)
            {
                string sql = String.Format(@"select * from Usuario where pk_usuario = {0}", HttpContext.Session.GetInt32(Session.SessionKeyName));
                DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

                foreach (DataRow row in rows)
                {
                    ViewBag.Usuario = new Usuario(
                        int.Parse(row["pk_usuario"].ToString()),
                        int.Parse(row["nivel"].ToString()),
                        row["nome"].ToString(),
                        row["email"].ToString(),
                        row["cargo"].ToString()
                        );
                }


                return View();
            }
            return RedirectToAction("Login", "Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
