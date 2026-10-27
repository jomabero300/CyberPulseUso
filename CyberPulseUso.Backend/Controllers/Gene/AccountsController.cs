using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Enums;
using CyberPulseUso.Shared.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace CyberPulseUso.Backend.Controllers.Gene;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IUserUnitOfWork _userUnitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IMailHelper _mailHelper;
    private readonly ICountryUnitOfWork _countryUnitOfWork;
    private readonly ApplicationDbContext _context;

    public AccountsController(
        ApplicationDbContext context,
        ICountryUnitOfWork countryUnitOfWork,
        IMailHelper mailHelper,
        IConfiguration configuration,
        IUserUnitOfWork userUnitOfWork)
    {
        _context = context;
        _countryUnitOfWork = countryUnitOfWork;
        _mailHelper = mailHelper;
        _configuration = configuration;
        _userUnitOfWork = userUnitOfWork;
    }

    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _userUnitOfWork.GetAsync(pagination);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPost("RecoverPassword")]
    public async Task<IActionResult> RecoverPasswordAsync([FromBody] EmailDTO model)
    {
        var user = await _userUnitOfWork.GetUserAsync(model.Email);
        if (user == null) return NotFound();
        var response = await SendRecoverEmailAsync(user, model.Language);
        return response.WasSuccess ? NoContent() : BadRequest(response.Message);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDTO model)
    {
        var user = await _userUnitOfWork.GetUserAsync(model.Email);
        if (user == null) return NotFound();
        var result = await _userUnitOfWork.ResetPasswordAsync(user, model.Token, model.NewPassword);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors.FirstOrDefault()?.Description);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> PutAsync([FromBody] User user)
    {
        try
        {
            var result = await _userUnitOfWork.UpdateUserAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
            var updatedUser = await _userUnitOfWork.GetUserAsync(user.Email!);
            return updatedUser == null ? NotFound() : Ok(BuildToken(updatedUser));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("UserType")]
    public async Task<IActionResult> UpdateUserTypeAsync([FromQuery] string userId, [FromQuery] UserType userType)
    {
        await _userUnitOfWork.UpdateUserAsync(userId, userType);
        return NoContent();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("UserType")]
    public async Task<IActionResult> GetUserTypeAsync([FromQuery] string id)
    {
        if (!Guid.TryParse(id, out var userId)) return BadRequest("Identificador de usuario inválido.");
        var result = await _userUnitOfWork.GetUserAsync(userId);
        return result != null ? Ok(result) : NotFound();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();
        var result = await _userUnitOfWork.GetUserAsync(email);
        return result != null ? Ok(result) : NotFound();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDTO model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var email = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();
        var user = await _userUnitOfWork.GetUserAsync(email);
        if (user == null) return NotFound();
        var result = await _userUnitOfWork.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
        return NoContent();
    }

    [HttpPost("ResendToken")]
    public async Task<IActionResult> ResendTokenAsync([FromBody] EmailDTO model)
    {
        var user = await _userUnitOfWork.GetUserAsync(model.Email);
        if (user == null) return NotFound();
        var response = await SendConfirmationEmailAsync(user, model.Language);
        return response.WasSuccess ? NoContent() : BadRequest(response.Message);
    }

    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserDTO model)
    {
        var countryResponse = await _countryUnitOfWork.GetAsync(model.CountryId, true);
        if (countryResponse == null || !countryResponse.WasSuccess || countryResponse.Result == null)
            return BadRequest("ERR004");

        User user = model;
        user.Country = countryResponse.Result;
        var result = await _userUnitOfWork.AddUserASync(user, model.Password);
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);

        await _userUnitOfWork.AddUserToRoleAsync(user, user.UserType.ToString());
        var response = await SendConfirmationEmailAsync(user, model.Language);
        return response.WasSuccess ? NoContent() : BadRequest(response.Message);
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string token)
    {
        if (!Guid.TryParse(userId, out var parsedUserId)) return BadRequest("Identificador de usuario inválido.");
        token = token.Replace(" ", "+");
        var user = await _userUnitOfWork.GetUserAsync(parsedUserId);

        if (user == null) return NotFound();
        if (user.EmailConfirmed) return BadRequest("ERR009");

        user.Photo = await photoLastIndexOf(user.Photo);

        var result = await _userUnitOfWork.ConfirmEmailAsync(user, token);

        return result.Succeeded ? NoContent() : BadRequest(result.Errors.FirstOrDefault()?.Description);
    }

    private async Task<string?> photoLastIndexOf(string? photo)
    {
        char caracterBuscar = '\\';

        // 1. Buscar el índice de la última aparición del carácter
        int ultimoIndice = photo!.LastIndexOf(caracterBuscar);

        if (ultimoIndice != -1)
        {
            // 2. Cortar desde el índice + 1 para eliminar el carácter y todo lo que está a su izquierda
            photo = photo.Substring(ultimoIndice + 1);
        }
        return photo;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDTO model)
    {
        var result = await _userUnitOfWork.LoginAsync(model);
        if (result.Succeeded)
        {
            var user = await _userUnitOfWork.GetUserAsync(model.Email);
            if (user == null) return Unauthorized();
            await _userUnitOfWork.ResetAccessFailedCountAsync(user);
            return Ok(BuildToken(user));
        }
        if (result.IsLockedOut) return BadRequest("ERR007");
        if (result.IsNotAllowed) return BadRequest("ERR008");
        return BadRequest("ERR006");
    }

    [HttpGet("Combo")]
    public async Task<IActionResult> GetComboAsync([FromQuery] UserType userType) => Ok(await _userUnitOfWork.GetAsync(userType));

    [HttpGet("UserTypeUp")]
    public async Task<IActionResult> UpdateUserTypeGetAsync([FromQuery] string id, [FromQuery] UserType userType)
    {
        await _userUnitOfWork.UpdateUserAsync(id, userType);
        return NoContent();
    }


    [HttpGet("TotalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        if (!string.IsNullOrWhiteSpace(pagination.Email))
        {
            pagination.Email = User.Identity!.Name;
        }

        var response = await _userUnitOfWork.GetTotalRecordsAsync(pagination);

        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }



    private async Task<ActionResponse<string>> SendConfirmationEmailAsync(User user, string language)
    {
        var myToken = await _userUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
        var tokenLink = Url.Action("ConfirmEmail", "accounts", new
        {
            userid = user.Id,
            token = HttpUtility.UrlEncode(myToken)
        }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

        var subjectKey = language == "es" ? "Mail:SubjectConfirmationEs" : "Mail:SubjectConfirmationEn";
        var bodyKey = language == "es" ? "Mail:BodyConfirmationEs" : "Mail:BodyConfirmationEn";
        return await _mailHelper.SendMail(user.FullName, user.Email!, _configuration[subjectKey]!, string.Format(_configuration[bodyKey]!, tokenLink), language);
    }

    private async Task<ActionResponse<string>> SendRecoverEmailAsync(User user, string language)
    {
        var myToken = await _userUnitOfWork.GeneratePasswordResetTokenAsync(user);
        var tokenLink = Url.Action("ResetPassword", "accounts", new
        {
            userid = user.Id,
            token = HttpUtility.UrlEncode(myToken)
        }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

        var subjectKey = language == "es" ? "Mail:SubjectRecoveryEs" : "Mail:SubjectRecoveryEn";
        var bodyKey = language == "es" ? "Mail:BodyRecoveryEs" : "Mail:BodyRecoveryEn";
        return await _mailHelper.SendMail(user.FullName, user.Email!, _configuration[subjectKey]!, string.Format(_configuration[bodyKey]!, tokenLink), language);
    }

    private TokenDTO BuildToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email!),
            new(ClaimTypes.Role, user.UserType.ToString()),
            new("FrstName", user.FirstName),
            new("LastName", user.LastName),
            new("Photo", user.Photo ?? string.Empty),
            new("CountryId", user.Country.Id.ToString())
        };

        var secret = _configuration["jwMBELtKey:Secret"];
        if (string.IsNullOrWhiteSpace(secret)) throw new InvalidOperationException("Falta configurar jwMBELtKey.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(5);
        var token = new JwtSecurityToken(claims: claims, expires: expiration, signingCredentials: credentials);
        return new TokenDTO { Token = new JwtSecurityTokenHandler().WriteToken(token), Expiration = expiration };
    }
}
