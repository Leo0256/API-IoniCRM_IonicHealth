using IoniCRM.Models;
using IoniCRM.Controllers.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using System.Web;
using Newtonsoft.Json;

namespace IoniCRM.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly string view = "/Views/Login.cshtml";

        private PostgreSQLConnection pgsqlcon;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
            pgsqlcon = new();
        }

        public IActionResult Login()
        {
            if (!Session.Empty(HttpContext.Session))
                ViewBag.Usuario = Session.GetUsuario(HttpContext.Session);

            return View(view);
        }
        
        public IActionResult MakeLogin(string email, string senha)
        {
            ViewData["email"] = email;
            ViewData["senha"] = senha;

            ViewData["emailNotInformed"] = string.IsNullOrEmpty(email) ? "!! Digite um e-mail." : null;
            ViewData["passNotInformed"] = string.IsNullOrEmpty(senha) ? "!! Digite uma senha." : null;

            if (
                !string.IsNullOrEmpty(email) &&
                !string.IsNullOrEmpty(senha)
                )
            {
                string sql = string.Format(@"select login('{0}','{1}')", email, senha);
                DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

                string data = string.Empty;
                foreach (DataRow row in rows)
                    data = row["login"].ToString();

                if (bool.Parse(data.ToString()))
                {
                    sql = string.Format(@"select * from dadosUsuario('{0}')", email);
                    rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

                    foreach (DataRow row in rows)
                    {
                        Usuario usuario = new(
                            int.Parse(row["pk_usuario"].ToString()),
                            int.Parse(row["nivel"].ToString()),
                            row["nome"].ToString(),
                            row["email"].ToString(),
                            row["img"].ToString(),
                            row["cargo"].ToString(),
                            "dark");

                        Session.SetUser(HttpContext.Session, usuario);
                    }
                    
                    return RedirectToAction("Home", "Home");
                }
                else
                    ViewData["emailOrPassWrong"] = "!! E-mail ou Senha incorretos.";
            }

            return View(view);
        }
    }
}
