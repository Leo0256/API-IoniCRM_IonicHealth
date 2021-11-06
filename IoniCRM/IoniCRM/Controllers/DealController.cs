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
        private string view = "/Views/Pipelines/Deal.cshtml";
        private PostgreSQLConnection pgsqlcon;

        private List<Pipeline> pipelines;

        //teste
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

        public DealController()
        {
            pgsqlcon = new();
        }

        public IActionResult Deal(string id, string act)
        {
            ViewBag.teste = id + " - " + act;

            ViewBag.cor = color;

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
                }

                pipeline.deals = deals;

                data.Add(pipeline);

            }
            return data;
        }
    }
}
