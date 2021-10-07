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
using System.Text;
using Microsoft.AspNetCore.Html;

namespace IoniCRM.Controllers
{
    public class ListagemController : Controller
    {
        private readonly ILogger<ListagemController> _logger;
        private readonly string view = "/Views/Listagem/Clientes.cshtml";

        readonly LoginModel login = new();
        private PostgreSQLConnection pgsqlcon;
        

        public Usuario usuario;
        public List<Cliente> clientes;


        public ListagemController(ILogger<ListagemController> logger)
        {
            _logger = logger;
            pgsqlcon = new();
            clientes ??= SetList();
        }

        public IActionResult Clientes()
        {
            var menuRecursion = new MenuRecursion(clientes);
            var menuUi = menuRecursion.GetMenu();
            ViewBag.Lista = menuUi;

            ViewBag.Clientes = clientes;

            return View();
        }

        /*testes*/
        public IActionResult Cliente(string value)
        {
            ViewData["teste"] = value;
            var menuRecursion = new MenuRecursion(clientes);
            var menuUi = menuRecursion.GetMenu();
            ViewBag.Lista = menuUi;
            return View(view);
        }
        /*/testes*/

        private bool flag = false;
        public List<Cliente> SetList()
        {
            string sql = string.Format(@"select pk, emp, nome from dadosCliente(null)");
            DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

            List<Cliente> data = new();
            foreach (DataRow row in rows)
            {
                Cliente cliente = new(
                        int.Parse(row["pk"].ToString()),
                        row["emp"].ToString(),
                        row["nome"].ToString()
                        );

                if (string.IsNullOrEmpty(row["emp"].ToString()))
                    data.Add(cliente);

                else
                {
                    foreach (Cliente c in data)
                    {
                        if (c.nome == cliente.GetEmp())
                            c.AddFuncionario(cliente);
                        else if (!c.Equals(cliente))
                        {
                            flag = false;
                            AddFuncionarioInEmpresa(cliente, c);
                        }
                    }
                }

            }
            return data;
        }

        private void AddFuncionarioInEmpresa(Cliente funcionario, Cliente emp)
        {
            if (funcionario.GetEmp() == emp.nome)
            {
                emp.AddFuncionario(funcionario);
                flag = true;
            }
            else
            {
                var foo = emp.funcionarios;
                if (foo.Count > 0)
                    foreach (Cliente c in foo)
                    {
                        if (!c.Equals(funcionario))
                            AddFuncionarioInEmpresa(funcionario, c);
                        if (!flag)
                            break;
                    }
            }
        }

        public class MenuRecursion
        {
            List<Cliente> clientes;
            private PostgreSQLConnection pgsqlcon;

            /*Precisa melhorar isso depois*/
            public string OpenItem(string nome)
            {
                return "<li class=\"nav-item\">" +
                            "<a class=\"nav-link p-1\" >" +
                                nome +
                            "</a>" +
                        "</li>";
            }

            public string OpenItemWithSubs(string nome, int id)
            {
                return "<li class=\"nav-item border-bottom border-dark\">" + 
                            "<div class=\"d-flex flex-row\">" +
                                "<a class=\"nav-link p-1\" >" +
                                    nome +
                                "</a>" +

                                "<a class=\"nav-link\" href=\"#submenu-" + id +
                                    "\" data-toggle=\"collapse\" data-target=\"#submenu-" + id + "\">" +
                                    "<img src=\"/images/arrow-down-circle.svg\" class=\"wh-15\" />" +
                                "</a>" +
                            "</div>" +
                            "<div class=\"collapse\" id=\"submenu-" + id + "\" aria-expanded=\"false\">" +
                                "<ul class=\"flex-column p-1 nav\">";
            }
            public const string CLOSE_SUBITEM = "</ul></div></li>";

            private StringBuilder strBuilder;
            

            public MenuRecursion(List<Cliente> clientes)
            {
                pgsqlcon = new();
                this.clientes = clientes;
                strBuilder = new(GenerateMenuUi());
            }

            public string GetMenu() => strBuilder.ToString();

            

            
            

            public string GenerateMenuUi()
            {
                StringBuilder builder = new();
                List<Cliente> parentItems = clientes;

                foreach (var parentcat in parentItems)
                {
                    List<Cliente> childItems = parentcat.funcionarios;
                    if (childItems.Count == 0)
                        builder.Append(OpenItem(parentcat.nome));
                    
                    else
                    {
                        builder.Append(OpenItemWithSubs(parentcat.nome, parentcat.GetPk_Cliente()));

                        builder.Append(AddChildItem(parentcat));

                        builder.Append(CLOSE_SUBITEM);
                    }
                }
                return builder.ToString();
            }

            private string AddChildItem(Cliente childItem)
            {
                StringBuilder builder = new();
                List<Cliente> childItems = childItem.funcionarios;
                foreach (Cliente cItem in childItems)
                {
                    List<Cliente> subChilds = cItem.funcionarios;
                    if (subChilds.Count == 0)
                        builder.Append(OpenItem(cItem.nome));

                    else
                    {
                        builder.Append(OpenItemWithSubs(cItem.nome, cItem.GetPk_Cliente()));

                        builder.Append(AddChildItem(cItem));

                        builder.Append(CLOSE_SUBITEM);
                    }
                }
                return builder.ToString();
            }
        }
    }
}
