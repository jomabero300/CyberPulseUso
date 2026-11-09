using Azure;
using CyberPulseUso.Backend.Controllers.Gene;
using CyberPulseUso.Backend.Helpers.Reports.Lnpa;
using CyberPulseUso.Backend.Helpers.Reports.Lnpa.Liquidacion;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
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
public class PayrollSettlementsController : GenericController<PayrollSettlement>
{
    private readonly IPayrollSettlementUnitOfWork _modelUnitOfWork;
    private readonly IUserUnitOfWork _userUnitOfWork;
    private readonly IWebHostEnvironment _env;
    public PayrollSettlementsController(IGenericUnitOfWork<PayrollSettlement> unitOfWork, IPayrollSettlementUnitOfWork modelUnitOfWork, IUserUnitOfWork userUnitOfWork, IWebHostEnvironment env)
        : base(unitOfWork)
    {
        _modelUnitOfWork = modelUnitOfWork;
        _userUnitOfWork = userUnitOfWork;
        _env = env;
    }

    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _modelUnitOfWork.GetAsync(id);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpGet("full/{fecha}/{id}/{UserId}")]
    public async Task<IActionResult> GetAsync(DateTime fecha, int id, string UserId)
    {
        var response = await _modelUnitOfWork.GetAsync(fecha, id, UserId);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        if (User.IsInRole("work"))
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();
            var result = await _userUnitOfWork.GetUserAsync(email);
            if (result.ChargeId == null) return BadRequest("El usuario no tiene un cargo asignado.");
            pagination.Email = result.Id;
        }

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
    public async Task<IActionResult> PostAsync([FromBody] List<PayrollSettlementDTO> entity)
    {
        var response = await _modelUnitOfWork.AddAsync(entity);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutAsync([FromBody] List<PayrollSettlementDTO> entity)
    {
        var response = await _modelUnitOfWork.UpdateAsync(entity);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpGet("TotalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        if (User.IsInRole("work"))
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();
            var result = await _userUnitOfWork.GetUserAsync(email);
            if (result.ChargeId == null) return BadRequest("El usuario no tiene un cargo asignado.");
            pagination.Email = result.Id;
        }

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

    [HttpGet("Sett")]
    public async Task<IActionResult> GetLiqAsync()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized();

            var result = await _userUnitOfWork.GetUserAsync(email);

            Console.WriteLine(
                $"[LIQ] Consulta usuario: {stopwatch.ElapsedMilliseconds} ms");

            if (result == null)
                return Unauthorized();

            if (result.ChargeId == null)
                return BadRequest("El usuario no tiene un cargo asignado.");

            int chargeId = result.ChargeId.Value;
            string userId = result.Id;

            stopwatch.Restart();

            var response = await _modelUnitOfWork.GetLiqAsync(
                chargeId,
                userId);

            Console.WriteLine(
                $"[LIQ] Consulta liquidación: {stopwatch.ElapsedMilliseconds} ms");

            if (response.WasSuccess)
                return Ok(response.Result);

            return BadRequest();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LIQ] ERROR: {ex}");

            return BadRequest(ex.Message);
        }
    }

    [HttpGet("generarpdf/{fecha:datetime}/{id}/{userid}")]
    public async Task<IActionResult> GenerarPdf(DateTime fecha, int id, string userid)
    {
        var response = await _modelUnitOfWork.GetAsync(fecha, id, userid, true);

        var pdf = LiquidacionDocument.GenerarReportePdf(response.Result!);

        return File(pdf, "application/pdf", "PayrollPdf.pdf");
    }
}