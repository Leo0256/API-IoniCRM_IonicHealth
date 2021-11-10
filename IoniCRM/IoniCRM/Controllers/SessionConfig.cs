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
    public class SessionConfig : Controller
    {
        private PostgreSQLConnection pgsqlcon;
        public Usuario usuario;
        
        public SessionConfig(PostgreSQLConnection connection)
        {
            pgsqlcon = connection;
        }
        
        public IActionResult VerifySession(string redirectController, string redirectview)
        {
            if (HttpContext.Session.GetInt32(Session.SessionKeyName) != default)
            {
                string sql = String.Format(@"select * from Usuario where pk_usuario = {0}", HttpContext.Session.GetInt32(Session.SessionKeyName));
                DataRow user = pgsqlcon.ExecuteCmdAsync(sql).Result.Select()[0];

                usuario = new Usuario(
                    int.Parse(user["pk_usuario"].ToString()),
                    int.Parse(user["nivel"].ToString()),
                    user["nome"].ToString(),
                    user["email"].ToString(),
                    user["img"].ToString(),
                    user["cargo"].ToString()
                    );
                

                return RedirectToAction(redirectController, redirectview);
            }
            return RedirectToAction("Login", "Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
