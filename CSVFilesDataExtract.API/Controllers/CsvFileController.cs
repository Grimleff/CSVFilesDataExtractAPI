using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CSVFilesDataExtract.API.Model;
using CSVFilesDataExtract.API.Services.Interfaces;
using FluentCsv;
using FluentCsv.FluentReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CSVFilesDataExtract.API.Controllers;

/// <summary>
///     Endpoints to downloads the file and saves the selected fields in DB
/// </summary>
[ApiController]
[Route("files")]
public class CsvFileController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ILogger<CsvFileController> _logger;
    public CsvFileController(IFileService fileService, ILogger<CsvFileController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }
    /// <summary>
    ///     Download specified file from url and save all field requested in database
    /// </summary>
    /// <param name="fileUri">File URI</param>
    /// <param name="fieldsToSave">List of field</param>
    /// <returns>Execution result</returns>
    [HttpPost]
    [Route(@"file_download", Name = "file_download")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status102Processing)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SaveFileFieldAsync(string fileUri, List<string> fieldsToSave)
    {
        try
        {
            //https://static.data.gouv.fr/resources/base-sirene-des-entreprises-et-de-leurs-etablissements-siren-siret-page-en-cours-de-construction/20180920-102932/dessinstockunitelegalehistorique.csv
            var content = await _fileService.DownloadFile(fileUri);

            var csv = Read.Csv.FromString(content)
                .With.ColumnsDelimiter(",")
                .ThatReturns.ArrayOf<CsvInfos>()
                .Put.Column("Nom").Into(x => x.Name)
                .Put.Column("Libellé").Into(x => x.Description)
                .Put.Column("Longueur").As<int>().Into(x => x.Length)
                .Put.Column("Type").Into(x => x.Type)
                .Put.Column("Ordre").As<int>().Into(x => x.Order)
                .GetAll();
            
            
            
            
            Console.WriteLine("CSV DATA");
            csv.ResultSet.ForEach(r=> Console.WriteLine($"Name : {r.Name} - Description : {r.Description}"));

            Console.WriteLine("ERRORS");
            csv.Errors.ForEach(e => Console.WriteLine($"Error at line {e.LineNumber} column index {e.ColumnZeroBasedIndex} : {e.ErrorMessage}"));
            
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Internal error during get status change {e.Message}");
            return StatusCode(500, e.Message);
        }
        
    }
}