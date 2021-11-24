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
    public class HomeController : Controller
    {

        private PostgreSQLConnection pgsqlcon;
        

        public HomeController()
        {
            pgsqlcon = new();
        }
        

        public IActionResult Home(string act)
        {
            if (Session.Empty(HttpContext.Session))
                return RedirectToAction("Login", "Login");

            ViewBag.Usuario = Session.GetUsuario(HttpContext.Session);
            if (!string.IsNullOrEmpty(act))
            {
                LimparHistorico();
            }
            GetHistorico();
            return View();
        }

        // teste
        public void GetHistorico() 
        {
            string sql = string.Format(@"select * from dadosHistorico(null)");
            DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

            List<Historico> historicos = new();
            DateTime? data;
            foreach (DataRow row in rows)
            {
                data = string.IsNullOrEmpty(row["data"].ToString()) ?
                        null : DateTime.Parse(row["data"].ToString());

                historicos.Add(new(
                    int.Parse(row["pk_h"].ToString()),
                    int.Parse(row["fk_usuario"].ToString()),
                    data,
                    row["descr"].ToString()
                    ));
            }

            ViewBag.Historico = historicos;
        }

        public void LimparHistorico()
        {
            string sql = string.Format(@"select * from delHistorico(null)");
            _ = pgsqlcon.ExecuteCmdAsync(sql);
        }

        public IActionResult teste()
        {
            //AddHistorico(HttpContext.Session, "Eu sou um item do histórico.");
            return RedirectToAction("Home", "Home");
        }
        
        //

        public IActionResult Perfil()
        {
            if (Session.Empty(HttpContext.Session))
                return RedirectToAction("Login", "Login");

            ViewBag.Usuario = Session.GetUsuario(HttpContext.Session);
            return View();
        }

        public IActionResult ChangeTheme(string theme)
        {
            Usuario usuario = Session.GetUsuario(HttpContext.Session);
            usuario.theme = theme;
            Session.SetUser(HttpContext.Session, usuario);

            return RedirectToAction("Home", "Home");
        }

        public IActionResult Sair()
        {
            return RedirectToAction("Home", "Home");
        }
    }
}
