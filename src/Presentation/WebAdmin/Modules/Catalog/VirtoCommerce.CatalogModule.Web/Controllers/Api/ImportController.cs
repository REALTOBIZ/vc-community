﻿using Omu.ValueInjecter;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.CatalogModule.Web.Converters;
using VirtoCommerce.Foundation.Frameworks.Extensions;
using VirtoCommerce.Foundation.Importing.Model;
using VirtoCommerce.Foundation.Importing.Repositories;
using VirtoCommerce.Foundation.Importing.Services;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [RoutePrefix("api/import")]
    public class ImportController : ApiController
    {
        // private readonly string _relativeDir = "Content/Uploads/ImportData/";
        // private readonly string _relativeDir = "Content/Uploads/";

        private readonly Func<IImportRepository> _importRepositoryFactory;
        private readonly Func<IImportService> _importServiceFactory;
        //private readonly IDataManagementService _dataManagementService;

        public ImportController(Func<IImportRepository> importRepositoryFactory, Func<IImportService> importServiceFactory /*, IDataManagementService dataManagementService*/)
        {
            _importRepositoryFactory = importRepositoryFactory;
            _importServiceFactory = importServiceFactory;
            //_dataManagementService = dataManagementService;
        }

        [HttpGet]
        [ResponseType(typeof(webModel.ImportJob))]
        [Route("new/{catalogId?}")]
        public IHttpActionResult New(string catalogId = null)
        {
            var retVal = new webModel.ImportJob
            {
                Id = Guid.NewGuid().ToString(),
                Name = "New import job",
                CatalogId = catalogId,
                ColumnDelimiter = "?",
                ImportStep = 1
            };

            return Ok(retVal);
        }

        [HttpPut]
        [ResponseType(typeof(void))]
        [Route("create")]
        public IHttpActionResult Put(webModel.ImportJob entry)
        {
            var coreEntry = entry.ToFoundation();
            using (var repository = _importRepositoryFactory())
            {
                repository.Add(coreEntry);
                repository.UnitOfWork.Commit();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("update")]
        public IHttpActionResult Post(webModel.ImportJob entry)
        {
            var coreEntry = entry.ToFoundation();
            using (var repository = _importRepositoryFactory())
            {
                var dbEntry = repository.ImportJobs.ExpandAll().SingleOrDefault(x => x.ImportJobId.Equals(entry.Id));
                dbEntry.InjectFrom(coreEntry);
                repository.UnitOfWork.Commit();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(webModel.ImportJob[]))]
        [HttpGet]
        [Route("list/{catalogId?}")]
        public IHttpActionResult List(string catalogId = null)
        {
            ImportJob[] dbEntries;
            using (var repository = _importRepositoryFactory())
            {
                dbEntries = repository.ImportJobs
                    .Where(x => catalogId == null || x.CatalogId.Equals(catalogId))
                    .OrderBy(x => x.Name).ToArray();
            }
            var retVal = dbEntries.Select(x => x.ToWebModel());
            return Ok(retVal);
        }

        [ResponseType(typeof(webModel.ImportJob))]
        [HttpGet]
        [Route("get/{id}")]
        public IHttpActionResult Get(string id)
        {
            ImportJob retVal;
            using (var repository = _importRepositoryFactory())
            {
                retVal = repository.ImportJobs.ExpandAll().SingleOrDefault(x => x.ImportJobId.Equals(id));
            }

            return Ok(retVal.ToWebModel());
        }

        [HttpDelete]
        [ResponseType(typeof(void))]
        public IHttpActionResult Delete([FromUri]string[] ids)
        {
            using (var repository = _importRepositoryFactory())
            {
                var entries = repository.ImportJobs.Where(x => ids.Contains(x.ImportJobId)).ToList();

                entries.ForEach(repository.Remove);
                repository.UnitOfWork.Commit();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(webModel.ImportJob))]
        [HttpPost]
        public IHttpActionResult UpdateMappingItems(webModel.ImportJob job)
        {
            // parameter validation
            if (string.IsNullOrEmpty(job.EntityImporter))
            {
                return BadRequest("EntityImporter is required.");
            }

            var AvailableImporters = new EntityImporterBase[]
            {
                new ItemImporter(ImportEntityType.Product.ToString()),
                new CategoryImporter()
            };

            var tmp = AvailableImporters.FirstOrDefault(x => x.Name == job.EntityImporter);
            if (tmp == null)
            {
                return BadRequest("Invalid EntityImporter.");
            }
            if (string.IsNullOrEmpty(job.CatalogId))
            {
                return BadRequest("CatalogId is required.");
            }
            if (string.IsNullOrEmpty(job.TemplatePath))
            {
                return BadRequest("TemplatePath is required.");
            }

            if (string.IsNullOrEmpty(job.ColumnDelimiter))
            {
                return BadRequest("Column Delimiter is required.");
            }

            var importService = _importServiceFactory();
            var csvColumns = importService.GetCsvColumns(job.TemplatePath, job.ColumnDelimiter);
            job.AvailableCsvColumns = csvColumns;


            // TODO:
            // GetCsvColumns from csv file;
            // SetMappingItems for job;

            return Ok(job);
        }

        [HttpPost]
        public async Task<string> Run(string id, string sourceAssetId)
        {
            var retVal = Guid.NewGuid().ToString();

            // _importService.RunImportJob(jobEntity.ImportJob.ImportJobId, sourceAssetId));

            return await Task.FromResult(retVal);
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult Cancel(string id)
        {
            // TODO

            return StatusCode(HttpStatusCode.NoContent);
        }

        #region private


        #endregion
    }
}