using CyberPulseUso.Backend.UnitsOfWork.Interfaces;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberPulseUso.Backend.Controllers.Gene;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class StatusController : GenericController<Statu>
{
    private readonly IStatuUnitOfWork _statuUnitOfWork;

    public StatusController(IGenericUnitOfWork<Statu> unitOfWork, IStatuUnitOfWork statuUnitOfWork) : base(unitOfWork)
    {
        _statuUnitOfWork = statuUnitOfWork;
    }

    [HttpGet("full")]
    public async Task<IActionResult> GetFullAsync()
    {
        var response = await _statuUnitOfWork.GetAsync();
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _statuUnitOfWork.GetAsync(id);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _statuUnitOfWork.GetAsync(pagination);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpDelete("full/{id:int}")]
    public override async Task<IActionResult> DeleteAsync(int id)
    {
        var response = await _statuUnitOfWork.DeleteAsync(id);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPost("full")]
    public async Task<IActionResult> PostFullAsync([FromBody] StatuDTO entity)
    {
        var response = await _statuUnitOfWork.AddAsync(entity);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutFullAsync([FromBody] StatuDTO entity)
    {
        var response = await _statuUnitOfWork.UpdateAsync(entity);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpGet("Combo/{id:int}")]
    public async Task<IActionResult> GetComboAsync(int id) => Ok(await _statuUnitOfWork.GetComboAsync(id));
}
