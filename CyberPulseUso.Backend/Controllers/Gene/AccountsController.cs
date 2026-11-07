using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.UnitsOfWork.Implementations.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Enums;
using CyberPulseUso.Shared.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

    public AccountsController(
        ICountryUnitOfWork countryUnitOfWork,
        IMailHelper mailHelper,
        IConfiguration configuration,
        IUserUnitOfWork userUnitOfWork)
    {
        _countryUnitOfWork = countryUnitOfWork;
        _mailHelper = mailHelper;
        _configuration = configuration;
        _userUnitOfWork = userUnitOfWork;
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginatedAsync([FromQuery] PaginationDTO pagination)
    {
        if (User.IsInRole("SAdm") == true)
        {
            pagination.otro = UserType.SAdm.ToString();
        }

        var response = await _userUnitOfWork.GetAsync(pagination);
        return response.WasSuccess ? Ok(response.Result) : BadRequest(response.Message);
    }

    [HttpPost("RecoverPassword")]
    public async Task<IActionResult> RecoverPasswordAsync([FromBody] EmailDTO model)
    {
        var user = await _userUnitOfWork.GetUserAsync(model.Email);

        if (user == null) 
            return NotFound();
        
        var response = await SendRecoverEmailAsync(user, model.Language);

        return response.WasSuccess ? NoContent() : BadRequest(response.Message);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDTO model)
    {
        var user = await _userUnitOfWork.GetUserAsync(model.Email);

        if (user == null)
        {
            return NotFound();
        }

        //var result = await _usersUnitOfWork.ResetPasswordAsync(user, HttpUtility.UrlDecode(model.Token), model.NewPassword);
     

        var result = await _userUnitOfWork.ResetPasswordAsNoTrackingAsync(user.Id, model.Token, model.NewPassword);

        if (result.Succeeded)
        {
            return NoContent();
        }

        return BadRequest(result.Errors.FirstOrDefault()!.Description);
    }

    /// <summary>
    /// Actualizar el prefir del usuario
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> PutAsync([FromBody] User user, [FromQuery] bool rememberMe)
    {
        try
        {
            var result = await _userUnitOfWork.UpdateUserAsync(user);
            
            if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
            
            var updatedUser = await _userUnitOfWork.GetUserAsync(user.Email!);

            return updatedUser == null
                ? NotFound()
                : Ok(BuildToken(updatedUser, rememberMe));
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

            // Revocar la sesión anterior asociada a la cookie actual.
            var currentRefreshToken = Request.Cookies["CYBERPULSE_REFRESH"];

            if (!string.IsNullOrWhiteSpace(currentRefreshToken))
            {
                var currentTokenHash = HashRefreshToken(currentRefreshToken);

                var storedRefreshToken =
                    await _userUnitOfWork.GetRefreshTokenByHashAsync(currentTokenHash);

                if (storedRefreshToken != null && storedRefreshToken.RevokedAt == null)
                {
                    await _userUnitOfWork.RevokeRefreshTokenFamilyAsync(
                        storedRefreshToken.FamilyId,
                        DateTime.UtcNow);
                }
            }

            // Generar Access Token.
            var tokenDTO = BuildToken(user, model.RememberMe);
            // Si el usuario desea mantener la sesión abierta,
            // generar y almacenar un Refresh Token.
            if (model.RememberMe)
            {
                var now = DateTime.UtcNow;

                var refreshToken = GenerateRefreshToken();
                var refreshTokenHash = HashRefreshToken(refreshToken);

                var sessionExpiration = now.AddDays(7);

                var refreshTokenEntity = new RefreshToken
                {
                    UserId = user.Id,
                    TokenHash = refreshTokenHash,
                    FamilyId = Guid.NewGuid(),
                    CreatedAt = now,
                    ExpiresAt = sessionExpiration,
                    SessionExpiresAt = sessionExpiration
                };

                await _userUnitOfWork.CreateRefreshTokenAsync(
                    refreshTokenEntity);

                SetRefreshTokenCookie(refreshToken, refreshTokenEntity.SessionExpiresAt);
                // Entregar el token original al Frontend.
                //tokenDTO.RefreshToken = refreshToken;
            }
            else
            {
                // Si no se desea mantener la sesión abierta,
                // eliminar cualquier Refresh Token existente.
                DeleteRefreshTokenCookie();
            }

            tokenDTO.RefreshToken = null; // No se envía el Refresh Token en la respuesta.
            return Ok(tokenDTO);
            //return Ok(BuildToken(user));
        }
        if (result.IsLockedOut) return BadRequest("ERR007");
        if (result.IsNotAllowed) return BadRequest("ERR008");
        return BadRequest("ERR006");
    }

    [HttpPost("Logout")]
    [AllowAnonymous]
    public async Task<IActionResult> LogoutAsync()
    {
        var refreshToken = Request.Cookies["CYBERPULSE_REFRESH"];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var tokenHash = HashRefreshToken(refreshToken);

            var storedToken =
                await _userUnitOfWork.GetRefreshTokenByHashAsync(tokenHash);

            if (storedToken != null && storedToken.RevokedAt == null)
            {
                await _userUnitOfWork.RevokeRefreshTokenFamilyAsync(
                    storedToken.FamilyId,
                    DateTime.UtcNow);
            }
        }

        // Eliminar la cookie del navegador.
        DeleteRefreshTokenCookie();

        return NoContent();
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
        if (User.IsInRole("SAdm") == true)
        {
            pagination.otro = UserType.SAdm.ToString();
        }

        var response = await _userUnitOfWork.GetTotalRecordsAsync(pagination);

        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }


    [AllowAnonymous]
    [HttpPost("Refresh")]
    public async Task<IActionResult> RefreshAsync()
    {
        var refreshToken = Request.Cookies["CYBERPULSE_REFRESH"];

        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized();

        var now = DateTime.UtcNow;

        var tokenHash = HashRefreshToken(refreshToken);

        var storedToken =
            await _userUnitOfWork.GetRefreshTokenByHashAsync(tokenHash);

        if (storedToken == null)
            return Unauthorized();

        if (storedToken.RevokedAt != null)
        {
            await _userUnitOfWork.RevokeRefreshTokenFamilyAsync(
                storedToken.FamilyId, now);

            DeleteRefreshTokenCookie();

            return Unauthorized();
        }

        if (storedToken.ExpiresAt <= now ||
            storedToken.SessionExpiresAt <= now)
        {
            await _userUnitOfWork.RevokeRefreshTokenFamilyAsync(
                storedToken.FamilyId, now);

            DeleteRefreshTokenCookie();

            return Unauthorized();
        }

        var user = await _userUnitOfWork.GetUserAsync(
            storedToken.User.Email!);

        if (user == null)
        {
            DeleteRefreshTokenCookie();
            return Unauthorized();
        }

        var newRefreshToken = GenerateRefreshToken();

        var replacement = new RefreshToken
        {
            UserId = storedToken.UserId,
            TokenHash = HashRefreshToken(newRefreshToken),
            FamilyId = storedToken.FamilyId,
            CreatedAt = now,
            ExpiresAt = storedToken.SessionExpiresAt,
            SessionExpiresAt = storedToken.SessionExpiresAt
        };

        var rotated = await _userUnitOfWork.RotateRefreshTokenAsync(
            tokenHash,
            replacement,
            now);

        if (!rotated)
        {
            // Otra solicitud pudo haber renovado el token
            // simultáneamente. No revocar toda la familia.
            return Conflict(new
            {
                Message = "El Refresh Token ya fue procesado. Intente renovar nuevamente."
            });
        }

        var tokenDTO = BuildToken(user, true);

        SetRefreshTokenCookie(
            newRefreshToken,
            replacement.SessionExpiresAt);

        return Ok(tokenDTO);
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
            token = myToken
        }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

        if (language == "es")
        {
            return await _mailHelper.SendMail(user.FullName, user.Email!, _configuration["Mail:SubjectRecoveryEs"]!, string.Format(_configuration["Mail:BodyRecoveryEs"]!, tokenLink), language);
        }

        return await _mailHelper.SendMail(user.FullName, user.Email!, _configuration["Mail:SubjectRecoveryEn"]!, string.Format(_configuration["Mail:BodyRecoveryEn"]!, tokenLink), language);
    }



    private TokenDTO BuildToken(User user, bool rememberMe)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email!),
            new(ClaimTypes.Role, user.UserType.ToString()),
            new("FirstName", user.FirstName),
            new("LastName", user.LastName),
            new("Photo", user.Photo ?? string.Empty),
            new("CountryId", user.Country.Id.ToString()),
            new Claim("ChargeName", user.Charge?.Name ?? string.Empty)
        };

        var secret = _configuration["jwMBELtKey"];

        if (string.IsNullOrWhiteSpace(secret)) throw new InvalidOperationException("Falta configurar jwMBELtKey.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Duración según la preferencia del usuario.
        var expiration = rememberMe
                ? DateTime.UtcNow.AddMinutes(15)
                : DateTime.UtcNow.AddHours(5);

        //var expiration = DateTime.UtcNow.AddHours(5);
        var token = new JwtSecurityToken(claims: claims, expires: expiration, signingCredentials: credentials);
        return new TokenDTO 
        { 
            Token = new JwtSecurityTokenHandler().WriteToken(token), 
            Expiration = expiration,
            RememberMe = rememberMe
        };
    }
    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Base64UrlEncoder.Encode(randomBytes);
    }
    private static string HashRefreshToken(string token)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }
    private void SetRefreshTokenCookie(string refreshToken,DateTime expiration)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = new DateTimeOffset(expiration),
            Path = "/api"
        };

        Response.Cookies.Append(
            "CYBERPULSE_REFRESH",
            refreshToken,
            cookieOptions);
    }
    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            "CYBERPULSE_REFRESH",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api"
            });
    }
}
