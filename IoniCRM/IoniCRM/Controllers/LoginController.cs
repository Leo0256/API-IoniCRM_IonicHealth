using IoniCRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IoniCRM.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly string view = "/Views/Login.cshtml";
        private readonly string home = "/Views/Home/Home.cshtml";

        readonly LoginModel login = new();
        private PostgreSQLConnection pgsqlcon;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
            pgsqlcon = new();
        }

        public IActionResult Login()
        {
            return View(view);
        }

        [HttpPost]
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
                //string sql = @"select login('leo.ribeiro0256@gmail.com', '123456');";
                DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

                string data = String.Empty;
                foreach (DataRow row in rows)
                    data = row["login"].ToString();

                if (Boolean.Parse(data.ToString()))
                    return View(home);
                else
                    ViewData["emailOrPassWrong"] = "!! E-mail ou Senha incorretos.";
            }

            return View(view);
        }
    }
}
