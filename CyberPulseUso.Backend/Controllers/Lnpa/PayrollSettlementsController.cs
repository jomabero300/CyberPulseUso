using CyberPulseUso.Backend.Controllers.Gene;
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
    public PayrollSettlementsController(IGenericUnitOfWork<PayrollSettlement> unitOfWork, IPayrollSettlementUnitOfWork modelUnitOfWork, IUserUnitOfWork userUnitOfWork)
        : base(unitOfWork)
    {
        _modelUnitOfWork = modelUnitOfWork;
        _userUnitOfWork = userUnitOfWork;
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
    public async Task<IActionResult> PostAsync([FromBody] PayrollSettlementDTO entity)
    {
        var response = await _modelUnitOfWork.AddAsync(entity);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutAsync([FromBody] PayrollSettlementDTO entity)
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


    [HttpGet("Sett")]
    public async Task<IActionResult> GetLiqAsync()
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var result = await _userUnitOfWork.GetUserAsync(email);
        if (result.ChargeId == null) return BadRequest("El usuario no tiene un cargo asignado.");

        var response = await _modelUnitOfWork.GetLiqAsync(result.ChargeId ?? 0,result.Id);

        if (response.WasSuccess)
        {

            return Ok(response.Result);
        }

        return BadRequest();
    }

}