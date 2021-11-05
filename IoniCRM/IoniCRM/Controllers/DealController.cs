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

namespace IoniCRM.Controllers
{
    public class DealController : Controller
    {
        private string view = "/Views/Pipelines/Pipeline.cshtml";
        private PostgreSQLConnection pgsqlcon;

        private List<Pipeline> pipelines;

        //teste
        private int[] totalDeals = new int[5];
        private double[] totalValor = new double[5];

        public DealController()
        {
            pgsqlcon = new();
        }

        public IActionResult Pipeline(string act, string id)
        {
            ViewBag.Pipelines = pipelines;
            
            string[] estagio = {
                "Qualificação",     // 0
                "Proposta",         // 1
                "Negociação",       // 2
                "Fechado - Ganho",  // 3
                "Fechado - Perda"   // 4
            };
            ViewBag.estagio = estagio;
            ViewBag.totalDeals = totalDeals;
            ViewBag.totalValor = totalValor;

            switch (act)
            {
                case "selecionar":
                    ToDeal(id);
                    break;

                case "adicionar":
                    AddDeal();
                    break;

                case "deletar":
                    DelDeal(id);
                    break;

                default:
                    break;
            }

            return View(view);
        }

        private List<Pipeline> SetList()
        {
            string sql = string.Format(@"select * from Pipeline");
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
                foreach(DataRow dataRow in foo)
                {
                    Deal deal = new(
                        int.Parse(dataRow["id_deal"].ToString()),
                        dataRow["nome"].ToString(),
                        int.Parse(dataRow["estagio"].ToString()),
                        int.Parse(dataRow["d_status"].ToString()),
                        double.Parse(dataRow["valor"].ToString()),
                        dataRow["abertura"].ToString(),
                        dataRow["fechamento"].ToString(),
                        int.Parse(dataRow["probabilidade"].ToString()),
                        dataRow["descr"].ToString(),
                        new Cliente(
                            int.Parse(dataRow["id_cli"].ToString()),
                            dataRow["empresa"].ToString(),
                            dataRow["cliente"].ToString()
                            )
                        );

                    deals.Add(deal);
                    totalDeals[deal.estagio]++;
                    totalValor[deal.estagio] += deal.valor;
                }

                pipeline.deals = deals;

                data.Add(pipeline);

            }
            return data;
        }

        private void ToDeal(string id_deal)
        {
            view = "/View/Pipeline/Deal.cshtml?id=" + id_deal;
        }

        private void AddDeal()
        {
            view = "/View/Pipeline/Deal.cshtml";
        }

        private void DelDeal(string id_deal)
        {
            string sql = string.Format(@"select delDeal({0})",id_deal);
            pgsqlcon.ExecuteCmdAsync(sql).Result.Select();
        }
    }
}
