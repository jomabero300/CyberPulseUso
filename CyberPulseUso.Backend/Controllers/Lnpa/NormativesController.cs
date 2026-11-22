using CyberPulseUso.Backend.Controllers.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberPulseUso.Backend.Controllers.Lnpa;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class NormativesController : GenericController<Normative>
{
    private readonly INormativeUnitOfWork _unitOfWork;

    public NormativesController(INormativeUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("tree/{id:int}")]
    public async Task<IActionResult> GetTreeAsync(int id)
    {
        var response = await _unitOfWork.GetTreeAsync(id);

        return response.WasSuccess
            ? Ok(response.Result)
            : BadRequest(response.Message);
    }
}