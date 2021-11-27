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
    public class DashboardController : Controller
    {

        private PostgreSQLConnection pgsqlcon;
        private List<Pipeline> pipelines;


        public DashboardController()
        {
            pgsqlcon = new();
            pipelines = ViewBag.TotasPipelines ?? PipelineController.SetList(pgsqlcon, null);
        }
        

        public IActionResult Dashboard(string id)
        {
            if (Session.Empty(HttpContext.Session))
                return RedirectToAction("Login", "Login");
            
            ViewBag.Usuario = Session.GetUsuario(HttpContext.Session);
            ViewBag.Pipeline = PipelineController.SetList(pgsqlcon, id);
            ViewBag.TotasPipelines = pipelines;
            return View();
        }
        /*
        private List<Pipeline> SetList(string id_pipe)
        {
            string sql;
            if (string.IsNullOrEmpty(id_pipe))
                sql = string.Format(@"select * from Pipeline");
            else
                sql = string.Format(@"select * from Pipeline where pk_pipeline = {0}", id_pipe);

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

                sql = string.Format(@"select * from dadosPipeline({0})", pipeline.GetId());
                DataRow[] foo = pgsqlcon.ExecuteCmdAsync(sql).Result.Select();

                List<Deal> deals = new();
                DateTime? abertura;
                DateTime? fechamento;
                foreach (DataRow dataRow in foo)
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
        */
    }
}
