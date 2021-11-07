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
using System.Drawing;
using Newtonsoft.Json.Linq;

namespace IoniCRM.Controllers
{
    public class PipelineController : Controller
    {
        private readonly string view = "/Views/Pipelines/Pipeline.cshtml";
        private PostgreSQLConnection pgsqlcon;

        private List<Pipeline> pipelines = new();


        private readonly string[] color = {
            "#00ffff", // 1
            "#26d9d9", // 2
            "#4fbfbf", // 3
            "#59a6a6", // 4
            "#738c8c", // 5
            "#808080", // 6
            "#8c7373", // 7
            "#a65959", // 8
            "#bf4040", // 9
            "#ff0000"  // 10
        };

        public PipelineController()
        {
            pgsqlcon = new();
            pipelines = ViewBag.TotasPipelines ?? SetList(null);
        }

        public IActionResult Pipeline(string id)
        {
            ViewBag.TodasPipelines = pipelines;

            ViewBag.Pipelines = SetList(id);

            string[] estagio = {
                "Qualificação",     // 0
                "Proposta",         // 1
                "Negociação",       // 2
                "Fechado - Ganho",  // 3
                "Fechado - Perda"   // 4
            };
            ViewBag.estagio = estagio;

            ViewBag.CorPrioridade = color;

            return View(view);
        }

        

        private List<Pipeline> SetList(string id_pipe)
        {
            string sql;
            if (string.IsNullOrEmpty(id_pipe))
                sql = string.Format(@"select * from Pipeline");
            else
                sql = string.Format(@"select * from Pipeline where pk_pipeline = {0}",id_pipe);

            DataRow[] rows = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

            List<Pipeline> data = new();
            foreach (DataRow row in rows)
            {
                Pipeline pipeline = new(
                        int.Parse(row["pk_pipeline"].ToString()),
                        row["nome"].ToString(),
                        int.Parse(row["prioridade"].ToString()),
                        row["descr"].ToString()
                        );

                sql = string.Format(@"select * from dadosPipeline({0})",pipeline.GetId());
                DataRow[] foo = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();
                
                List<Deal> deals = new();
                DateTime? abertura;
                DateTime? fechamento;
                foreach(DataRow dataRow in foo)
                {
                    abertura = string.IsNullOrEmpty(dataRow["abertura"].ToString()) ?
                        null : DateTime.Parse(dataRow["abertura"].ToString());

                    fechamento = string.IsNullOrEmpty(dataRow["fechamento"].ToString()) ?
                        null : DateTime.Parse(dataRow["fechamento"].ToString());

                    Deal deal = new(
                        int.Parse(dataRow["id_deal"].ToString()),
                        dataRow["nome"].ToString(),
                        int.Parse(dataRow["estagio"].ToString()),
                        int.Parse(dataRow["d_status"].ToString()),
                        double.Parse(dataRow["valor"].ToString()),
                        abertura,
                        fechamento,
                        int.Parse(dataRow["probabilidade"].ToString()),
                        dataRow["descr"].ToString(),
                        new Cliente(
                            int.Parse(dataRow["id_cli"].ToString()),
                            dataRow["empresa"].ToString(),
                            dataRow["cliente"].ToString(),
                            dataRow["img"].ToString()
                            )
                        );

                    deals.Add(deal);
                }

                pipeline.deals = deals;

                data.Add(pipeline);

            }
            return data;
        }

        // Ações com as Deals
        public IActionResult DelDeal(string id)
        {
            string sql = string.Format(@"select delDeal({0})", id);
            _ = pgsqlcon.ExecuteCmdAsync(sql);

            return RedirectToAction("Pipeline", "Pipeline");
        }
        //

        // Ações na Pipeline
        public IActionResult AddPipeline(string id)
        {
            ViewBag.Pipeline = int.Parse(id) != 0 ? SetList(id).First() : new Pipeline();
            ViewBag.CorPrioridade = color;

            return View("/Views/Pipelines/AddPipeline.cshtml");
        }

        public IActionResult DelPipeline(string id)
        {
            string sql = string.Format(@"select delPipeline({0})", id);
            _ = pgsqlcon.ExecuteCmdAsync(sql);

            return RedirectToAction("Pipeline", "Pipeline");
        }

        public IActionResult UpsertPipe(string id_pipe, string nomeOriginal, string nome, string prioridade, string descr)
        {
            string sql;
            JObject dados = JObject.Parse("{" +
                    "\"nome\":\"" + nome + "\"," +
                    "\"prioridade\":" + prioridade + "," +
                    "\"descr\":\"" + descr + "\"" +
                    "}");

            if (!string.Equals(nomeOriginal, nome) && int.Parse(id_pipe) > 0)
            {
                sql = string.Format(@"select renamePipeline('{0}','{1}')", nomeOriginal, nome);
                _ = pgsqlcon.ExecuteCmdAsync(sql);
            }

            sql = string.Format(@"select upsertPipeline('{0}')", dados);
            _ = pgsqlcon.ExecuteCmdAsync(sql);

            return RedirectToAction("Pipeline","Pipeline");
        }
        //
    }
}
