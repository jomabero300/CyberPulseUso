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
public class NormativeChaptersController : GenericController<NormativeChapter>
{
    private readonly INormativeChapterUnitOfWork _unitOfWork;

    public NormativeChaptersController(INormativeChapterUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("bynormative/{normativeId:int}")]
    public async Task<IActionResult> GetByNormativeAsync(int normativeId)
    {
        var response = await _unitOfWork.GetByNormativeAsync(normativeId);

        return response.WasSuccess
            ? Ok(response.Result)
            : BadRequest(response.Message);
    }
}