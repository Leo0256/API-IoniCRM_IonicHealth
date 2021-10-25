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

namespace IoniCRM.Views.Pipelines
{
    public class PipelineController : Controller
    {
        private readonly ILogger<PipelineController> _logger;
        private readonly string view = "/Views/Pipelines/Pipeline.cshtml";

        public PipelineController(ILogger<PipelineController> logger)
        {
            _logger = logger;
        }

        public IActionResult Pipeline()
        {
            return View(view);
        }
    }
}
