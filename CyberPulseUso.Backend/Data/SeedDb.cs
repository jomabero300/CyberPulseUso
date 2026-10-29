using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace CyberPulseUso.Backend.Data;

public class SeedDb
{
    private readonly ApplicationDbContext _context;
    private readonly IUserUnitOfWork _usersUnitOf;

    public SeedDb(
        ApplicationDbContext context,
        IUserUnitOfWork usersUnitOf)
    {
        _context = context;
        _usersUnitOf = usersUnitOf;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();

        await CheckStatusAsync();
        await CheckCountriesAsync();
        await CheckStatesAsync();
        await CheckCitiesAsync();
        await CheckNeighborhoodsAsync();
        await CheckRolesAsync();
        await CheckSpPayrollAsync();
        await CheckUserAsync(
            "Manuel",
            "Bello",
            "jbellor@sena.edu.co",
            "3133670740",
            UserType.Admi,
            "17588236");
    }

    private async Task CheckSpPayrollAsync()
    {
        var sqlPath = Path.Combine(
            "Data", "Scripts", "Sp_PayrollCencepts.sql");
        if (File.Exists(sqlPath))
        {
            var sql = await File.ReadAllTextAsync(sqlPath);

            await _context.Database.ExecuteSqlRawAsync(sql);
        }
    }

    private async Task CheckStatusAsync()
    {
        if (!await _context.Status.AnyAsync())
        {
            _context.Status.AddRange(
                new Statu { Name = "Activo", Nivel = 0 },
                new Statu { Name = "Inactivo", Nivel = 0 },
                new Statu { Name = "Suspendido", Nivel = 0 },
                new Statu { Name = "Cancelado", Nivel = 0 },
                new Statu { Name = "Eliminado", Nivel = 0 },

                new Statu { Name = "Creada", Nivel = 1 },
                new Statu { Name = "Enviada", Nivel = 1 },
                new Statu { Name = "Programando", Nivel = 1 },
                new Statu { Name = "RevisiÃ³n", Nivel = 1 },
                new Statu { Name = "Rechazada", Nivel = 1 },
                new Statu { Name = "EjecuciÃ³n", Nivel = 1 },
                new Statu { Name = "Terminada", Nivel = 1 },

                new Statu { Name = "Bajo", Nivel = 2 },
                new Statu { Name = "Alto", Nivel = 2 }
            );

            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckRolesAsync()
    {
        await _usersUnitOf.CheckRoleAsync(UserType.SAdm.ToString());
        await _usersUnitOf.CheckRoleAsync(UserType.Admi.ToString());
        await _usersUnitOf.CheckRoleAsync(UserType.work.ToString());
        await _usersUnitOf.CheckRoleAsync(UserType.User.ToString());
    }

    private async Task<User> CheckUserAsync(
        string firstName,
        string lastName,
        string email,
        string phone,
        UserType userType,
        string documentId)
    {
        var user = await _usersUnitOf.GetUserAsync(email);

        if (user == null)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(x => x.Name == "Colombia");

            if (country == null)
            {
                throw new InvalidOperationException(
                    "No se encontro el pais Colombia para crear el usuario inicial.");
            }

            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                Country = country,
                UserType = userType,
                Photo = "",
                DocumentId = documentId
            };

            await _usersUnitOf.AddUserASync(user, "Mbel123*");

            await _usersUnitOf.AddUserToRoleAsync(
                user,
                userType.ToString());

            var token = await _usersUnitOf
                .GenerateEmailConfirmationTokenAsync(user);

            await _usersUnitOf.ConfirmEmailAsync(user, token);
        }

        return user;
    }

    private async Task CheckCountriesAsync()
    {
        if (!await _context.Countries.AnyAsync())
        {
            var sqlPath = Path.Combine(
                "Data", "Scripts", "Countries.sql");

            var sql = await File.ReadAllTextAsync(sqlPath);

            await _context.Database.ExecuteSqlRawAsync(sql);
        }
    }

    private async Task CheckStatesAsync()
    {
        if (!await _context.States.AnyAsync())
        {
            var sqlPath = Path.Combine(
                "Data", "Scripts", "States.sql");

            var sql = await File.ReadAllTextAsync(sqlPath);

            await _context.Database.ExecuteSqlRawAsync(sql);
        }
    }

    private async Task CheckCitiesAsync()
    {
        if (!await _context.Cities.AnyAsync())
        {
            var sqlPath = Path.Combine(
                "Data", "Scripts", "Cities.sql");

            var sql = await File.ReadAllTextAsync(sqlPath);

            await _context.Database.ExecuteSqlRawAsync(sql);
        }
    }

    private async Task CheckNeighborhoodsAsync()
    {
        if (!await _context.Neighborhoods.AnyAsync())
        {
            var sqlPath = Path.Combine(
                "Data", "Scripts", "Neighborhoods.sql");

            var sql = await File.ReadAllTextAsync(sqlPath);

            await _context.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
