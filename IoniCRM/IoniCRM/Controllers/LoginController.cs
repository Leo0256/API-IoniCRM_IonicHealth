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


namespace IoniCRM.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly string view = "/Views/Login.cshtml";

        readonly LoginModel login = new();
        private PostgreSQLConnection pgsqlcon;
        

        public Usuario usuario;


        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
            pgsqlcon = new();
        }

        public IActionResult Login()
        {
            if (ViewBag.Usuario != null)
            {
                string sql = String.Format(@"select * from Usuario where pk_usuario = {0}", HttpContext.Session.GetInt32(Session.SessionKeyName));
                DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

                foreach (DataRow row in rows)
                {
                    ViewBag.Usuario = new Usuario(
                        int.Parse(row["pk_usuario"].ToString()),
                        int.Parse(row["nivel"].ToString())
                        );
                }

                return RedirectToAction("Home", "Home");
            }
            return View(view);
        }

        [HttpPost]
        public IActionResult Home(string email, string senha) => MakeLogin(email, senha);
        
        public IActionResult MakeLogin(string email, string senha)
        {
            login.Email = email;
            login.Senha = senha;

            ViewData["email"] = login.Email;
            ViewData["senha"] = login.Senha;

            ViewData["emailNotInformed"] = string.IsNullOrEmpty(login.Email) ? "!! Digite um e-mail." : null;
            ViewData["passNotInformed"] = string.IsNullOrEmpty(login.Senha) ? "!! Digite uma senha." : null;

            if (
                !string.IsNullOrEmpty(login.Email) &&
                !string.IsNullOrEmpty(login.Senha)
                )
            {
                string sql = String.Format(@"select login('{0}','{1}')", email, senha);
                DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

                string data = String.Empty;
                foreach (DataRow row in rows)
                    data = row["login"].ToString();

                if (Boolean.Parse(data.ToString()))
                {
                    sql = string.Format(@"select * from dadosUsuario('{0}')", email);
                    rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();
                    foreach(DataRow row in rows)
                    {
                        if (HttpContext.Session.GetInt32(Session.SessionKeyName) == default)
                        {
                            HttpContext.Session.SetInt32(Session.SessionKeyName, int.Parse(row["pk_usuario"].ToString()));
                            HttpContext.Session.SetInt32(Session.SessionKeyPermission, int.Parse(row["nivel"].ToString()));
                        }
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
