using CyberPulseUso.Backend.Controllers.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberPulseUso.Backend.Controllers.Lnpa;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class LaborRemunerationParametersOtherValuesController : GenericController<LaborRemunerationParametersOtherValue>
{
    private readonly ILaborRemunerationParametersOtherValueUnitOfWork _modelUnitOfWork;

    public LaborRemunerationParametersOtherValuesController(IGenericUnitOfWork<LaborRemunerationParametersOtherValue> unitOfWork, ILaborRemunerationParametersOtherValueUnitOfWork modelUnitOfWork)
        : base(unitOfWork)
    {
        _modelUnitOfWork = modelUnitOfWork;
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var response = await _modelUnitOfWork.GetAsync();
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }


    [HttpPost("full")]
    public async Task<IActionResult> PostAsync([FromBody] List<LaborRemunerationParametersOtherValueCRUDDTO> entity)
    {
            var response = await _modelUnitOfWork.AddAsync(entity);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

}
