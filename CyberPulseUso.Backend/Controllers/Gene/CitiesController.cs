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
public class CitiesController : GenericController<City>
{
    private readonly ICityUnitOfWork _cityUnitOfWork;

    public CitiesController(IGenericUnitOfWork<City> unitOfWork, ICityUnitOfWork cityUnitOfWork) : base(unitOfWork)
    {
        _cityUnitOfWork = cityUnitOfWork;
    }

    [HttpGet("paginated")]
    public override Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination) => base.GetAsync(pagination);

    [HttpDelete("full/{id:int}")]
    public override Task<IActionResult> DeleteAsync(int id) => base.DeleteAsync(id);

    [HttpPost("full")]
    public async Task<IActionResult> PostFullAsync([FromBody] CityDTO model)
    {
        // Requiere que ICityUnitOfWork exponga AddAsync(CityDTO).
        var response = await _cityUnitOfWork.AddAsync(model);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutFullAsync([FromBody] CityDTO model)
    {
        // Requiere que ICityUnitOfWork exponga UpdateAsync(CityDTO).
        var response = await _cityUnitOfWork.UpdateAsync(model);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpGet("Combo/{id:int}")]
    public async Task<IActionResult> GetComboAsync(int id) => Ok(await _cityUnitOfWork.GetComboAsync(id));
}
