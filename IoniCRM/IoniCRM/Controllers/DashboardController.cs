using IoniCRM.Models;
using IoniCRM.Controllers.Objects;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

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
    }
}
