using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberPulseUso.Backend.Controllers.Lnpa;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class PayrollDashboardController : ControllerBase
{
    private readonly IPayrollSettlementUnitOfWork _modelUnitOfWork;
    private readonly IUserUnitOfWork _userUnitOfWork;

    public PayrollDashboardController(IPayrollSettlementUnitOfWork modelUnitOfWork,IUserUnitOfWork userUnitOfWork)
    {
        _modelUnitOfWork = modelUnitOfWork;
        _userUnitOfWork = userUnitOfWork;
    }

    // Dashboard del trabajador: únicamente sus propios registros.
    [Authorize(Roles = "work")]
    [HttpGet("worker")]
    public async Task<IActionResult> GetWorkerDashboardAsync()
    {
        var email = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized();

        var user = await _userUnitOfWork.GetUserAsync(email);

        if (user == null)
            return Unauthorized();

        var response = await _modelUnitOfWork
            .GetWorkerDashboardAsync(user.Id);

        return response.WasSuccess
            ? Ok(response.Result)
            : BadRequest(response.Message);
    }

    // Dashboard administrativo: información consolidada.
    [Authorize(Roles = "Admi")]
    [HttpGet("admin")]
    public async Task<IActionResult> GetAdminDashboardAsync()
    {
        var response = await _modelUnitOfWork.GetAdminDashboardAsync();

        return response.WasSuccess
            ? Ok(response.Result)
            : BadRequest(response.Message);
    }
}