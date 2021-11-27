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

        private string[] nivel =
        {
            "Usuário",
            "Administrador",
            "Diretor"
        };

        public IActionResult Perfil(string id)
        {
            if (Session.Empty(HttpContext.Session))
                return RedirectToAction("Login", "Login");

            ViewBag.Usuario = Session.GetUsuario(HttpContext.Session);
            ViewBag.nivel = nivel;

            
            if (string.IsNullOrEmpty(id))
            {
                Usuario usuario = Session.GetUsuario(HttpContext.Session);
                ViewBag.Perfil = GetUsuarios(usuario.GetPk_Usuario().ToString()).First();
            }

            else if (string.Equals(id, "0"))
                ViewBag.Perfil = new Usuario();

            else
                ViewBag.Perfil = GetUsuarios(id).First();

            ViewBag.TodosUsuarios = GetUsuarios("null");
            return View();
        }

        public List<Usuario> GetUsuarios(string id)
        {
            List<Usuario> usuarios = new();
            string sql;

            sql = string.Format(@"select * from dadosUsuario({0})", id);
            DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

            foreach(DataRow row in rows)
            {
                usuarios.Add(new Usuario(
                    int.Parse(row["pk_usuario"].ToString()),
                    int.Parse(row["nivel"].ToString()),
                    row["nome"].ToString(),
                    row["email"].ToString(),
                    row["img"].ToString(),
                    row["cargo"].ToString(),
                    string.Empty));
            }

            return usuarios;
        }

        public IActionResult UpsertUsuario
            (
            string pk_usuario,
            string nome,string emailAntigo, string email,
            string senha, string cargo, string nivel
            )
        {
            string sql, dados;

            if (string.Equals(emailAntigo, email))
            {
                sql = string.Format(@"select updateEmailUsuario('{0}','{1}')", emailAntigo, email);
                _ = pgsqlcon.ExecuteCmdAsync(sql);
            }

            dados = "{" +
                "\"nivel\":" + Array.IndexOf(this.nivel, nivel) + "," +
                "\"img\":null," + //<-- mudar depois
                "\"nome\":\"" + nome + "\"," +
                "\"email\":\"" + email + "\",";

            if (!string.IsNullOrEmpty(senha))
                dados += "\"hash_senha\":\"" + LoginController.GetHashCode(senha) + "\",";

            dados += "\"cargo\":\"" + cargo + "\"" +
                "}";

            JObject usuario = JObject.Parse(dados);

            sql = string.Format(@"select addUsuario('{0}')", usuario);
            _ = pgsqlcon.ExecuteCmdAsync(sql);

            string mensagem = string.Equals(pk_usuario, "0") ?
                string.Format(@"Novo Usuario <{0}> adicionado, por {1}.", nome, Session.GetUsuario(HttpContext.Session).nome) :
                string.Format(@"Usuario <{0}> editado, por {1}.", nome, Session.GetUsuario(HttpContext.Session).nome);
            
            _ = new AddHistorico(HttpContext.Session, mensagem);
            return RedirectToAction("Home", "Home");
        }

        public IActionResult DelUsuario(string id, string nome)
        {
            _ = pgsqlcon.ExecuteCmdAsync(string.Format(@"select delUsuario({0})", id));

            string mensagem = string.Format(@"Usuario <{0}> deletado, por {1}.", nome, Session.GetUsuario(HttpContext.Session).nome);
            _ = new AddHistorico(HttpContext.Session, mensagem);
            return RedirectToAction("Perfil", "Home");
        }

        public IActionResult ChangeTheme(string theme)
        {
            Usuario usuario = Session.GetUsuario(HttpContext.Session);
            usuario.theme = theme;
            Session.SetUser(HttpContext.Session, usuario);

            return RedirectToAction("Home", "Home");
        }
    }
}
