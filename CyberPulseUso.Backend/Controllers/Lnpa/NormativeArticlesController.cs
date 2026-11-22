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
public class NormativeArticlesController : GenericController<NormativeArticle>
{
    private readonly INormativeArticleUnitOfWork _unitOfWork;

    public NormativeArticlesController(INormativeArticleUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("bychapter/{chapterId:int}")]
    public async Task<IActionResult> GetByChapterAsync(int chapterId)
    {
        var response = await _unitOfWork.GetByChapterAsync(chapterId);

        return response.WasSuccess
            ? Ok(response.Result)
            : BadRequest(response.Message);
    }
}