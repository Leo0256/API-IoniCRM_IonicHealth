using IoniCRM.Models;
using IoniCRM.Controllers.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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


            var menuRecursion = new MenuRecursion(clientes, ViewBag.Usuario.theme);
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

        public IActionResult AddCliente(string id)
        {
            if (Session.Empty(HttpContext.Session))
                return RedirectToAction("Login", "Login");

            ViewBag.Usuario = Session.GetUsuario(HttpContext.Session);
            ViewBag.Cliente = !string.IsNullOrEmpty(id) && int.Parse(id) > 0 ? GetData("Cliente", id)[0] : new Cliente();

            ViewBag.websites = string.Join("\n", ViewBag.Cliente.websites);
            ViewBag.enderecos = string.Join("\n", ViewBag.Cliente.enderecos);

            if(ViewBag.Cliente.contatos.Count > 0)
            {
                foreach(string[] contato in ViewBag.Cliente.contatos)
                    ViewBag.contatos += contato[1] + "\n";
            }
            else
                ViewBag.contatos = string.Empty;


            ViewBag.ListClientes = GetData("Cliente", "null");

            ViewData["nomeNotInformed"] = !string.IsNullOrWhiteSpace(TempData["nomeNotInformed"] as string) ? "!! Campo vazio." : null;
            ViewData["razaoNotInformed"] = !string.IsNullOrWhiteSpace(TempData["razaoNotInformed"] as string) ? "!! Campo vazio." : null;
            ViewData["cpfcnpjNotInformed"] = !string.IsNullOrWhiteSpace(TempData["cpfcnpjNotInformed"] as string) ? "!! Campo vazio." : null;

            return View("/Views/Listagem/AddCliente.cshtml");
        }

        public IActionResult UpsertCliente(
            string id_cliente, string nome, string razao_social, string emp,
            string cpf_cnpj_antigo, string cpf_cnpj, string crm, string enderecos, 
            string cargo, string websites, string contatos, string descr)
        {
            if (!string.IsNullOrWhiteSpace(nome) && !string.IsNullOrWhiteSpace(razao_social) && !string.IsNullOrWhiteSpace(cpf_cnpj))
            {
                // antes de tudo, pega o id da empresa (se houver)
                string emp_id;
                string sql, hist_mensagem;
                if (string.IsNullOrEmpty(emp) || new string[] { "Selecionar", "Vazio" }.Any(s => emp.Contains(s)))
                {
                    emp_id = "null";
                }
                else
                {
                    sql = string.Format(@"select pk_cliente from Cliente where nome like '{0}'", emp);
                    DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();
                    emp_id = string.IsNullOrEmpty(rows[0]["pk_cliente"].ToString()) ? null : rows[0]["pk_cliente"].ToString();
                }

                if (string.IsNullOrEmpty(contatos))
                    contatos = string.Empty;


                JObject json;
                Regex regex = new("[\"']");
                nome = regex.Replace(nome, "´");
                razao_social = regex.Replace(razao_social, "´");

                if (string.IsNullOrEmpty(descr)) 
                    descr = string.Empty;
                descr = regex.Replace(descr, "´");

                // Novo Cliente
                if (int.Parse(id_cliente) == 0)
                {
                    json = JObject.Parse("{" +
                        "\"fk_emp\":" + emp_id + "," +
                        "\"cpf_cnpj\":\"" + cpf_cnpj + "\"," +
                        "\"crm\":\"" + crm + "\"," +
                        "\"img\":null," + //
                        "\"nome\":\"" + nome + "\"," +
                        "\"razao_social\":\"" + razao_social + "\"," +
                        "\"categoria\":\"" + cargo + "\"," +
                        "\"descr\":\"" + descr + "\"," +
                        "}");

                    sql = string.Format(@"select addCliente('{0}')", json);
                    _ = pgsqlcon.ExecuteCmdAsync(sql);

                    sql = string.Format(@"select pk_cliente from Cliente where razao_social like '{0}'", razao_social);
                    id_cliente = pgsqlcon.ExecuteCmdAsync(sql).Result.Select()[0]["pk_cliente"].ToString();

                    AddInfoContato(false, id_cliente, enderecos, websites, contatos);

                    hist_mensagem = string.Equals(emp_id, "null") ?
                        string.Format(@"Novo Cliente <{0}> adicionado, por <{1}>.", nome, Session.GetUsuario(HttpContext.Session).nome) :
                        string.Format(@"Novo Cliente <{0}>, funcionario de <{1}>, adicionado, por <{2}>.", nome, emp, Session.GetUsuario(HttpContext.Session).nome);
                }
                // Editar Cliente
                else
                {
                    // atualiza o cpf/cnpj
                    if (!cpf_cnpj_antigo.Equals(cpf_cnpj))
                    {
                        sql = string.Format(@"select atualizarCPF_CNPJ('{0}','{1}')", cpf_cnpj_antigo, cpf_cnpj);
                        _ = pgsqlcon.ExecuteCmdAsync(sql);
                    }

                    json = JObject.Parse("{" +
                        "\"fk_emp\":" + emp_id + "," +
                        "\"cpf_cnpj\":\"" + cpf_cnpj + "\"," +
                        "\"crm\":\"" + crm + "\"," +
                        "\"img\":null," + //
                        "\"nome\":\"" + nome + "\"," +
                        "\"razao_social\":\"" + razao_social + "\"," +
                        "\"categoria\":\"" + cargo + "\"," +
                        "\"descr\":\"" + descr + "\"," +
                        "}");

                    sql = string.Format(@"select addCliente('{0}')", json);
                    _ = pgsqlcon.ExecuteCmdAsync(sql);

                    AddInfoContato(true, id_cliente, enderecos, websites, contatos);
                    
                    hist_mensagem = string.Equals(emp_id, "null") ?
                        string.Format(@"Cliente <{0}> atualizado, por <{1}>.", nome, Session.GetUsuario(HttpContext.Session).nome) :
                        string.Format(@"Cliente <{0}>, funcionario de <{1}>, atualizado, por <{2}>.", nome, emp, Session.GetUsuario(HttpContext.Session).nome);
                }

                _ = new AddHistorico(HttpContext.Session, hist_mensagem);
            }
            else
            {
                TempData["nomeNotInformed"] = string.IsNullOrWhiteSpace(nome) ? "campo vazio." : null;
                TempData["razaoNotInformed"] = string.IsNullOrWhiteSpace(razao_social) ? "campo vazio." : null;
                TempData["cpfcnpjNotInformed"] = string.IsNullOrWhiteSpace(cpf_cnpj) ? "campo vazio" : null;
                return RedirectToAction("AddCliente", "Listagem", new { id = id_cliente });
            }

            return RedirectToAction("Clientes", "Listagem");
        }

        private void AddInfoContato(bool clienteEditado, string id_cliente, string enderecos, string websites, string contatos)
        {
            List<List<string>> info = new();
            JObject json;
            Regex regex = new("[\"']");
            string sql;

            // Endereços
            if (string.IsNullOrEmpty(enderecos))
                info.Add(new() { string.Empty, string.Empty });
            else
            {
                enderecos = regex.Replace(enderecos, "´");
                info.Add(enderecos.Split("\r\n").Where(x => !string.IsNullOrWhiteSpace(x)).ToList());
            }
            
            // Websites
            if (string.IsNullOrEmpty(websites))
                info.Add(new() { string.Empty, string.Empty });
            else
            {
                websites = regex.Replace(websites, "´");
                info.Add(websites.Split("\r\n").Where(x => !string.IsNullOrWhiteSpace(x)).ToList());
            }


            if (info[0].Count > info[1].Count)
                info[1].Add(string.Empty);
            else if (info[1].Count > info[0].Count)
                info[0].Add(string.Empty);


            if (clienteEditado)
            {
                sql = string.Format(@"select delClienteInfo({0})", id_cliente);
                _ = pgsqlcon.ExecuteCmdAsync(sql);

                sql = string.Format(@"select delClienteContato({0})", id_cliente);
                _ = pgsqlcon.ExecuteCmdAsync(sql);
            }

            for (int x = 0; x < info[0].Count; x++)
            {
                json = JObject.Parse("{" +
                    "\"fk_cliente\":" + id_cliente + "," +
                    "\"endereco\":\"" + info[0][x] + "\"," +
                    "\"website\":\"" + info[1][x] + "\"," +
                    "}");

                sql = string.Format(@"select addClienteInfo('{0}')", json);
                _ = pgsqlcon.ExecuteCmdAsync(sql);
            }

            if (!string.IsNullOrWhiteSpace(contatos))
            {
                contatos = regex.Replace(contatos, "´");
                foreach (string contato in contatos.Split("\r\n").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray())
                {
                    int tipo_contato = contato.Contains("@") ? 1 : 0;
                    /* tipo_contato:
                     * 0 - Telefone
                     * 1 - E-mail
                     */

                    json = JObject.Parse("{" +
                        "\"fk_cliente\":" + id_cliente + "," +
                        "\"tipo\":" + tipo_contato + "," +
                        "\"contato\":\"" + contato + "\"," +
                        "}");

                    sql = string.Format(@"select addClienteContato('{0}')", json);
                    _ = pgsqlcon.ExecuteCmdAsync(sql);
                }
            }
            else
            {
                json = JObject.Parse("{" +
                        "\"fk_cliente\":" + id_cliente + "," +
                        "\"tipo\":0," +
                        "\"contato\":\" \"," +
                        "}");

                sql = string.Format(@"select addClienteContato('{0}')", json);
                _ = pgsqlcon.ExecuteCmdAsync(sql);
            }
        }

        public IActionResult DelCliente(string id, string nome, string emp)
        {
            string sql = string.Format(@"select delCliente({0})", id);
            _ = pgsqlcon.ExecuteCmdAsync(sql);

            string mensagem = string.IsNullOrEmpty(emp) ?
                string.Format("Cliente <{0}> deletado, por <{1}>.", nome, Session.GetUsuario(HttpContext.Session).nome):
                string.Format("Cliente <{0}>, funcionario de <{1}>, deletado, por <{2}>.", nome, emp, Session.GetUsuario(HttpContext.Session).nome);
            _ = new AddHistorico(HttpContext.Session, mensagem);

            return RedirectToAction("Clientes", "Listagem");
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
                        row["website"].ToString().Split(";").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
                        row["endereco"].ToString().Split(";").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
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

        /* */
        public class MenuRecursion
        {
            List<Cliente> clientes;

            public string OpenItem(string nome, int id, string tema)
            {
                return "<li class=\"nav-item\">" +
                            "<a class=\"nav-link text-theme-" + tema + " align-middle p-1\" href=\"Clientes?id=" + id + "\" >" +
                                nome +
                            "</a>" +
                        "</li>";
            }

            public string OpenItemWithSubs(string nome, int id, string tema)
            {
                return "<li class=\"nav-item border-bottom border-primary\">" +
                            "<div class=\"d-flex flex-row align-middle\">" +
                                "<a class=\"nav-link text-theme-" + tema + " p-1\" href=\"Clientes?id=" + id + "\">" +
                                    nome +
                                "</a>" +

                                "<a class=\"nav-link\" href=\"#submenu-" + id +
                                    "\" data-toggle=\"collapse\" data-target=\"#submenu-" + id + "\">" +
                                    "<svg xmlns = \"http://www.w3.org/2000/svg\"" +
                                        " fill = \"currentColor\" class=\"align-top btn-info rounded-circle wh-15\" viewBox=\"0 0 16 16\">" +
                                      "<path fill-rule=\"evenodd\" " + 
                                        "d=\"M1 8a7 7 0 1 0 14 0A7 7 0 0 0 1 8zm15 0A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM8.5 4.5a.5.5 0 0 0-1 0v5.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V4.5z\"/>" +
                                    "</svg>" +
                                "</a>" +
                            "</div>" +
                            "<div class=\"collapse\" id=\"submenu-" + id + "\" aria-expanded=\"false\">" +
                                "<ul class=\"flex-column p-1 nav\">";
            }
            public const string CLOSE_SUBITEM = "</ul></div></li>";

            private StringBuilder strBuilder;
            

            public MenuRecursion(List<Cliente> clientes, string tema)
            {
                this.clientes = clientes;
                strBuilder = new(GenerateMenuUi(tema));
            }

            public string GetMenu() => strBuilder.ToString();

            public string GenerateMenuUi(string tema)
            {
                StringBuilder builder = new();
                List<Cliente> parentItems = clientes;

                foreach (var parentcat in parentItems)
                {
                    List<Cliente> childItems = parentcat.funcionarios;
                    if (childItems.Count == 0)
                        builder.Append(OpenItem(parentcat.nome, parentcat.GetPk_Cliente(), tema));
                    
                    else
                    {
                        builder.Append(OpenItemWithSubs(parentcat.nome, parentcat.GetPk_Cliente(), tema));

                        builder.Append(AddChildItem(parentcat, tema));

                        builder.Append(CLOSE_SUBITEM);
                    }
                }
                return builder.ToString();
            }

            private string AddChildItem(Cliente childItem, string tema)
            {
                StringBuilder builder = new();
                List<Cliente> childItems = childItem.funcionarios;
                foreach (Cliente cItem in childItems)
                {
                    List<Cliente> subChilds = cItem.funcionarios;
                    if (subChilds.Count == 0)
                        builder.Append(OpenItem(cItem.nome, cItem.GetPk_Cliente(), tema));

                    else
                    {
                        builder.Append(OpenItemWithSubs(cItem.nome, cItem.GetPk_Cliente(), tema));

                        builder.Append(AddChildItem(cItem, tema));

                        builder.Append(CLOSE_SUBITEM);
                    }
                }
                return builder.ToString();
            }
        }
        /* */
    }
}
