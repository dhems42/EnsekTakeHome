using EnsekTakeHome.Repositories;
using EnsekTakeHome.Repositories.Accounts;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Net.Http;

namespace EnsekTakeHome.Controllers
{
    [ApiController]
    [Route("api/")]
    public class CsvIngestionController : ControllerBase
    {
        private readonly IDbConnection _connection;

        private IAccountsRepository _accountsRepository;

        public CsvIngestionController(IDbConnection connection, IAccountsRepository repository)
        {
            _connection = connection;
            _accountsRepository = repository;
        }

        [HttpPost]
        [Route("meter-reading-uploads")]
        public IActionResult UploadCsvFile(IFormFile file)
        {
            var stream = file.OpenReadStream();

            if (file == null || file.Length == 0)
            {
                return BadRequest("Uploaded file contained no data");
            }

            // This will read the CSV file
            
            var readings = _accountsRepository.HandleCsv(stream);

            var response = new
            {
                message = "OK",
                data = $"Csv file processed. {readings.valid} Valid meter reading(s). {readings.invalid} Invalid meter reading(s)."
            };
            
            return Ok(response);
        }
    }
}
