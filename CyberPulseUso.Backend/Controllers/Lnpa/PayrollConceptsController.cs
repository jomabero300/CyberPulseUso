using CyberPulseUso.Backend.Controllers.Gene;
using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.Helpers.Reports.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberPulseUso.Backend.Controllers.Lnpa;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class PayrollConceptsController : GenericController<PayrollConcept>
{
    private readonly IPayrollConceptUnitOfWork _modelUnitOfWork;
    private readonly IWebHostEnvironment _env;
    public PayrollConceptsController(IGenericUnitOfWork<PayrollConcept> unitOfWork, IPayrollConceptUnitOfWork modelUnitOfWork, IWebHostEnvironment env)
        : base(unitOfWork)
    {
        _modelUnitOfWork = modelUnitOfWork;
        _env = env;
    }

    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _modelUnitOfWork.GetAsync(id);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _modelUnitOfWork.GetAsync(pagination);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpDelete("full/{id:int}")]
    public override async Task<IActionResult> DeleteAsync(int id)
    {
        var response = await _modelUnitOfWork.DeleteAsync(id);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPost("full")]
    public async Task<IActionResult> PostAsync([FromBody] PayrollConceptDTO entity)
    {
        var response = await _modelUnitOfWork.AddAsync(entity);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutAsync([FromBody] PayrollConceptDTO entity)
    {
        var response = await _modelUnitOfWork.UpdateAsync(entity);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpGet("TotalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _modelUnitOfWork.GetTotalRecordsAsync(pagination);

        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [AllowAnonymous]
    [HttpGet("Combo")]
    public async Task<IActionResult> GetComboAsync() => Ok(await _modelUnitOfWork.GetComboAsync());

    [HttpGet("Report/{type}")]
    public async Task<IActionResult> GetAsync(string type)
    {
        var response = await _modelUnitOfWork.GetAsync();

        if (!response.WasSuccess)
        {
            return BadRequest(response.Message);
        }
        if (response.Result == null || !response.Result.Any())
        {
            return BadRequest("No hay datos para generar el reporte.");
        }
        if (type.ToLower() == "pdf")
        {
            string rutaPath = _env.WebRootPath;

            var pdf = PayrollConceptReport.PayrollConceptPdf([.. response.Result], rutaPath);

            return File(pdf, "application/pdf", "PayrollConcept.pdf");
        }
        else
        {


            var excelData = response.Result.Select(x => new PayrollConceptExcelDTO
            {
                Codigo = x.Id,
                Name = x.Name,
                Estado = x.Statu?.Name
            }).ToList();

            byte[] fileBytes = GeneExcel.GenerarReporte(excelData, "Conceptos de nomina");
            string nombreArchivo = $"PayrollConcept_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(fileBytes, contentType, nombreArchivo);
        }
    }
}