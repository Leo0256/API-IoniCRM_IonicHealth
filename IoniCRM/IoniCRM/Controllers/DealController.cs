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
using IoniCRM.Controllers;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace IoniCRM.Controllers
{
    public class DealController : Controller
    {
        private readonly string view = "/Views/Pipelines/Deal.cshtml";
        private PostgreSQLConnection pgsqlcon;

        private Pipeline pipeline;
        private List<Cliente> clientes;

        private readonly string[] estagio =
        {
            "Qualificação",
            "Proposta",
            "Negociação",
            "Fechado - Ganho",
            "Fechado - Perda"
        };

        private readonly string[] status =
        {
            "Aberto",
            "Ganho",
            "Perda"
        };


        public DealController()
        {
            pgsqlcon = new();
            clientes = ViewBag.Clientes ?? GetClientes();
        }

        public IActionResult Deal(string id, string pipe)
        {
            if (Session.Empty(HttpContext.Session))
                return RedirectToAction("Login", "Login");

            ViewBag.Usuario = Session.GetUsuario(HttpContext.Session);
            ViewBag.Deal = int.Parse(id) != 0 ? GetDeal(id) : new Deal();

            if (((DateTime?)ViewBag.Deal.abertura).HasValue)
            {
                DateTime abertura = (DateTime) ViewBag.Deal.abertura;

                ViewBag.aberturaDia = abertura.Date.ToString();
                ViewBag.aberturaHora = abertura.TimeOfDay.ToString();
            }
            else
            {
                ViewBag.aberturaDia = null;
                ViewBag.aberturaHora = null;
            }

            if (((DateTime?)ViewBag.Deal.fechamento).HasValue)
            {
                DateTime fechamento = (DateTime) ViewBag.Deal.fechamento;

                ViewBag.fechamentoDia = fechamento.Date.ToString();
                ViewBag.fechamentoHora = fechamento.TimeOfDay.ToString();
            }
            else
            {
                ViewBag.fechamentoDia = null;
                ViewBag.fechamentoHora = null;
            }


            pipeline ??= GetPipe(pipe);
            ViewBag.Pipeline = pipeline;

            ViewBag.Clientes = clientes;
            ViewBag.Estagio = estagio;
            ViewBag.Status = status;

            return View(view);
        }

        public IActionResult UpsertDeal(
            string id_deal, string id_pipe, string nome, double valor, 
            string status, string estagio, double prob, string nome_cli, //<-- razao_social
            string aberturaDia, string aberturaHora,
            string fechamentoDia, string fechamentoHora, string descr)
        {
            string dados, sql;
            JObject json;
            pipeline = GetPipe(id_pipe);

            DateTime? abertura = DiaHora(aberturaDia, aberturaHora);
            DateTime? fechamento = DiaHora(fechamentoDia, fechamentoHora);

            string mensagem;
            if (int.Parse(id_deal) == 0)
            {
                dados = "{" +
                    "\"pipeline\":\"" + pipeline.nome + "\"," +
                    "\"nome\":\"" + nome + "\"," +
                    "\"cliente\":\"" + nome_cli + "\"," +
                    "\"estagio\":" + this.estagio.ToList().IndexOf(estagio) + "," +
                    "\"d_status\":" + this.status.ToList().IndexOf(status) + "," +
                    "\"valor\":" + valor + "," +
                    "\"probabilidade\":" + prob + ",";

                dados += abertura.HasValue ?
                    "\"abertura\":\"" + abertura.ToString() + "\"," :
                    "\"abertura\":null,";

                dados += fechamento.HasValue ?
                    "\"fechamento\":\"" + fechamento.ToString() + "\"," :
                    "\"fechamento\":null,";

                dados +=
                    "\"descr\":\"" + descr + "\"" +
                    "}";

                json = JObject.Parse(dados);

                sql = string.Format(@"select addDeal('{0}')", json);
                mensagem = string.Format("Nova Deal <{0}> da Pipeline <{1}> adicionada, por {2}.", nome, pipeline.nome, Session.GetUsuario(HttpContext.Session).nome);
            }
            else
            {
                dados = "{" +
                    "\"id_deal\":" + id_deal + "," +
                    "\"pipeline\":\"" + pipeline.nome + "\"," +
                    "\"nome\":\"" + nome + "\"," +
                    "\"cliente\":\"" + nome_cli + "\"," +
                    "\"estagio\":" + this.estagio.ToList().IndexOf(estagio) + "," +
                    "\"d_status\":" + this.status.ToList().IndexOf(status) + "," +
                    "\"valor\":" + valor + "," +
                    "\"probabilidade\":" + prob + ",";

                dados += abertura.HasValue ?
                    "\"abertura\":\"" + abertura.ToString() + "\"," :
                    "\"abertura\":null,";

                dados += fechamento.HasValue ?
                    "\"fechamento\":\"" + fechamento.ToString() + "\"," :
                    "\"fechamento\":null,";

                dados +=
                    "\"descr\":\"" + descr + "\"" +
                    "}";

                json = JObject.Parse(dados);

                sql = string.Format(@"select updateDeal('{0}')", json);
                mensagem = string.Format("Deal <{0}> da Pipeline <{1}> atualizada, por {2}.", nome, pipeline.nome, Session.GetUsuario(HttpContext.Session).nome);
            }

            _ = pgsqlcon.ExecuteCmdAsync(sql);

            _ = new AddHistorico(HttpContext.Session, mensagem);

            return RedirectToAction("Pipeline", "Pipeline");
        }

        private static DateTime? DiaHora(string dia, string hora)
        {
            DateTime? foo = null;
            if (!string.IsNullOrEmpty(dia))
            {
                if (!string.IsNullOrEmpty(hora))
                    foo = DateTime.Parse(string.Concat(dia, ' ', hora));

                else
                    foo = DiaHora(dia, " 00:00:00");
            }
            else if (!string.IsNullOrEmpty(hora) && string.IsNullOrEmpty(dia))
                foo = DiaHora("01/01/2000", hora);
            
            return foo;
        }

        private Deal GetDeal(string id)
        {
            string sql = string.Format(@"select * from dadosDeal({0})", id);
            DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

            Deal data = new();
            DateTime? abertura;
            DateTime? fechamento;
            foreach (DataRow row in rows)
            {
                abertura = string.IsNullOrEmpty(row["abertura"].ToString()) ?
                        null : DateTime.Parse(row["abertura"].ToString());

                fechamento = string.IsNullOrEmpty(row["fechamento"].ToString()) ?
                    null : DateTime.Parse(row["fechamento"].ToString());

                data = new(
                    int.Parse(row["id_deal"].ToString()),
                    row["nome"].ToString(),
                    int.Parse(row["estagio"].ToString()),
                    int.Parse(row["d_status"].ToString()),
                    double.Parse(row["valor"].ToString()),
                    abertura,
                    fechamento,
                    int.Parse(row["probabilidade"].ToString()),
                    row["descr"].ToString(),
                    new Cliente(
                        int.Parse(row["id_cli"].ToString()),
                        row["empresa"].ToString(),
                        row["cliente"].ToString(),
                        row["img"].ToString()
                        )
                    );

                sql = string.Format(@"select * from Pipeline where nome like '{0}' limit 1", 
                    row["pipeline"].ToString());
                DataRow row2 = pgsqlcon.ExecuteCmdAsync(sql).Result.Select()[0];
                
                pipeline = new(
                        int.Parse(row2["pk_pipeline"].ToString()),
                        row2["nome"].ToString(),
                        int.Parse(row2["prioridade"].ToString()),
                        row2["descr"].ToString()
                        );
            }


            return data;
        }

        private List<Cliente> GetClientes()
        {
            string sql = string.Format(@"select pk, emp, razao_social, img from dadosCliente(null)");
            DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

            List<Cliente> data = new();
            foreach (DataRow row in rows)
            {
                Cliente cliente = new(
                    int.Parse(row["pk"].ToString()),
                    row["emp"].ToString(),
                    row["razao_social"].ToString(),
                    row["img"].ToString()
                    );

                data.Add(cliente);
            }

            return data;
        }

        private Pipeline GetPipe(string id)
        {
            string sql = string.Format(@"select * from Pipeline where pk_pipeline = {0}", id);
            DataRow row = pgsqlcon.ExecuteCmdAsync(sql).Result.Select()[0];

            Pipeline data = new(
                    int.Parse(row["pk_pipeline"].ToString()),
                    row["nome"].ToString(),
                    int.Parse(row["prioridade"].ToString()),
                    row["descr"].ToString()
                    );
            
            return data;
        }
    }
}
