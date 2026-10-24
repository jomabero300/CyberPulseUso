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
public class CountriesController : GenericController<Country>
{
    private readonly ICountryUnitOfWork _countryUnitOfWork;

    public CountriesController(IGenericUnitOfWork<Country> unitOfWork, ICountryUnitOfWork countryUnitOfWork) : base(unitOfWork)
    {
        _countryUnitOfWork = countryUnitOfWork;
    }

    [HttpGet("full")]
    public async Task<IActionResult> GetFullAsync()
    {
        var response = await _countryUnitOfWork.GetAsync();
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _countryUnitOfWork.GetAsync(id);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _countryUnitOfWork.GetAsync(pagination);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpDelete("full/{id:int}")]
    public override async Task<IActionResult> DeleteAsync(int id)
    {
        var response = await _countryUnitOfWork.DeleteAsync(id);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPost("full")]
    public async Task<IActionResult> PostFullAsync([FromBody] CountryDTO entity)
    {
        var response = await _countryUnitOfWork.AddAsync(entity);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutFullAsync([FromBody] CountryDTO entity)
    {
        var response = await _countryUnitOfWork.UpdateAsync(entity);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [AllowAnonymous]
    [HttpGet("Combo")]
    public async Task<IActionResult> GetComboAsync() => Ok(await _countryUnitOfWork.GetComboAsync());
}
