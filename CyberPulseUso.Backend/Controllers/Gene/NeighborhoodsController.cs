using CyberPulseUso.Backend.Controllers.Gene;
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
public class NeighborhoodsController : GenericController<Neighborhood>
{
    private readonly INeighborhoodUnitOfWork _modelUnitOfWork;

    public NeighborhoodsController(IGenericUnitOfWork<Neighborhood> unitOfWork, INeighborhoodUnitOfWork modelUnitOfWork)
        : base(unitOfWork)
    {
        _modelUnitOfWork = modelUnitOfWork;
    }



    [AllowAnonymous]
    [HttpGet("Combo")]
    public async Task<IActionResult> GetComboAsync(int id) => Ok(await _modelUnitOfWork.GetComboAsync(id));
}
