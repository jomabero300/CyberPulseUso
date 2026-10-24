using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CyberPulseUso.Backend.Controllers.Gene;

[ApiController]
[Route("api/[controller]")]
public class GenericController<T> : ControllerBase where T : class
{
    private readonly IGenericUnitOfWork<T> _unitOfWork;

    public GenericController(IGenericUnitOfWork<T> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetAsync()
    {
        var response = await _unitOfWork.GetAsync();
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpGet("{id:int}")]
    public virtual async Task<IActionResult> GetAsync(int id)
    {
        var response = await _unitOfWork.GetAsync(id);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPost]
    public virtual async Task<IActionResult> PostAsync([FromBody] T model)
    {
        var response = await _unitOfWork.AddAsync(model);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPut]
    public virtual async Task<IActionResult> PutAsync([FromBody] T model)
    {
        var response = await _unitOfWork.UpdateAsync(model);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpDelete("{id:int}")]
    public virtual async Task<IActionResult> DeleteAsync(int id)
    {
        var response = await _unitOfWork.DeleteAsync(id);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpGet("paginated")]
    public virtual async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _unitOfWork.GetAsync(pagination);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }
}
