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
        private PostgreSQLConnection pgsqlcon;
        

        public Usuario usuario;
        public List<Cliente> clientes;


        public ListagemController()
        {
            pgsqlcon = new();
            clientes ??= SetList();
        }

        public IActionResult Clientes(string id)
        {
            if (Session.Empty(HttpContext.Session))
                return RedirectToAction("Login", "Login");

            ViewBag.Usuario = Session.GetUsuario(HttpContext.Session);


            var menuRecursion = new MenuRecursion(clientes);
            var menuUi = menuRecursion.GetMenu();
            ViewBag.Lista = menuUi;

            if (string.IsNullOrEmpty(id))
                ViewBag.Clientes = GetData("Cliente", "null");

            else
            {
                ViewBag.Clientes = GetData("Cliente", id);
                ViewBag.Funcionarios = GetData("Funcionarios", id);
            }
            
            return View();
        }


        private bool flag = false;
        public List<Cliente> SetList()
        {
            string sql = string.Format(@"select pk, emp, nome, img from dadosCliente(null)");
            DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

            List<Cliente> data = new();
            foreach (DataRow row in rows)
            {
                Cliente cliente = new(
                        int.Parse(row["pk"].ToString()),
                        row["emp"].ToString(),
                        row["nome"].ToString(),
                        row["img"].ToString()
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

        public List<Cliente> GetData(string dataFrom,string pk_cliente)
        {
            string sql = string.Format(@"select * from dados{0}({1})", dataFrom, pk_cliente);
            DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

            List<Cliente> data = new();
            foreach (DataRow row in rows)
            {
                var tipo_contato = row["tipo_contato"].ToString().Split(";");
                var contato = row["contato"].ToString().Split(";");
                List<string[]> contatos = new();

                for (int x = 0; x < tipo_contato.Length; x++)
                    contatos.Add(new[] { tipo_contato[x], contato[x] });


                Cliente cliente = new(
                        int.Parse(row["pk"].ToString()),
                        row["emp"].ToString(),
                        row["nome"].ToString(),
                        row["cpf_cnpj"].ToString(),
                        row["crm"].ToString(),
                        row["img"].ToString(),
                        row["razao_social"].ToString(),
                        row["categoria"].ToString(),
                        row["descr"].ToString(),
                        row["website"].ToString().Split(";"),
                        row["endereco"].ToString().Split(";"),
                        contatos
                        );

                data.Add(cliente);
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

        public List<Cliente> GetFuncionarios(int pk_cliente, List<Cliente> clientes)
        {
            List<Cliente> cliente = null;
            if (clientes.Exists(x => x.GetPk_Cliente() == pk_cliente))
            {
                cliente = clientes.Find(x => x.GetPk_Cliente() == pk_cliente).funcionarios;
                if (cliente[0] == null || cliente.Count == 0)
                    cliente = null;
                
            }
            else    
                foreach (Cliente c in clientes)
                {
                    cliente = GetFuncionarios(pk_cliente, c.funcionarios);
                    if (cliente != null)
                        break;
                    
                }
            
            return cliente;
        }

        public class MenuRecursion
        {
            List<Cliente> clientes;

            /*Precisa melhorar isso depois*/
            public string OpenItem(string nome, int id)
            {
                return "<li class=\"nav-item\">" +
                            "<a class=\"nav-link text-theme-dark align-middle p-1\" href=\"Clientes?id=" + id + "\" >" +
                                nome +
                            "</a>" +
                        "</li>";
            }

            public string OpenItemWithSubs(string nome, int id)
            {
                return "<li class=\"nav-item border-bottom border-primary\">" +
                            "<div class=\"d-flex flex-row align-middle\">" +
                                "<a class=\"nav-link text-theme-dark p-1\" href=\"Clientes?id=" + id + "\">" +
                                    nome +
                                "</a>" +

                                "<a class=\"nav-link\" href=\"#submenu-" + id +
                                    "\" data-toggle=\"collapse\" data-target=\"#submenu-" + id + "\">" +
                                    "<img src=\"/images/arrow-down-circle.svg\" class=\"align-top btn-secondary rounded-circle wh-15\" />" +
                                "</a>" +
                            "</div>" +
                            "<div class=\"collapse\" id=\"submenu-" + id + "\" aria-expanded=\"false\">" +
                                "<ul class=\"flex-column p-1 nav\">";
            }
            public const string CLOSE_SUBITEM = "</ul></div></li>";

            private StringBuilder strBuilder;
            

            public MenuRecursion(List<Cliente> clientes)
            {
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
                        builder.Append(OpenItem(parentcat.nome, parentcat.GetPk_Cliente()));
                    
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
                        builder.Append(OpenItem(cItem.nome, cItem.GetPk_Cliente()));

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
