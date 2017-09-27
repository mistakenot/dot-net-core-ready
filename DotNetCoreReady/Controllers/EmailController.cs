﻿using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Web.Mvc;
using DotNetCoreReady.Models;
using DotNetCoreReady.Services;

namespace DotNetCoreReady.Controllers
{
    public class EmailController : Controller
    {
        private readonly EmailAlertsRepository _emailAlertsRepository;
        private readonly NugetClient _nugetClient;

        public EmailController()
        {
            var connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var tableName = "emailalerts";

            _nugetClient = new NugetClient();
            _emailAlertsRepository = new EmailAlertsRepository(connectionString, tableName);
        }

        [HttpPost]
        public async Task<JsonResult> Index(CreateEmailAlertModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState
                    .SelectMany(s => s.Value.Errors)
                    .First()
                    .ErrorMessage;

                return Json(new {Error = errorMessage});
            }

            var package = await _nugetClient.FindLatestVersions(model.PackageId);

            if (!package.Any())
            {
                return Json(new {Error = $"PackageID {model.PackageId} not found."});
            }

            var result = await _emailAlertsRepository.CreateIfNotExists(model.Email, model.PackageId, model.OptedInToMarketing);

            if (result.WasSuccessful)
            {
                return Json(new {});
            }
            else
            {
                return Json(new {Error = $"Error creating alert; {result.ErrorCode}."});
            }
        }
    }
}