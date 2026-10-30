using CyberPulseUso.Shared.Entities.Gene;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using CyberPulseUso.Shared.Entities.Lnpa;
namespace CyberPulseUso.Backend.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Iva> Ivas { get; set; }
    public DbSet<Neighborhood> Neighborhoods { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<Statu> Status { get; set; }
    public DbSet<LogAuditoria> LogAuditorias { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Charge> Charges { get; set; }
    public DbSet<Convention> Conventions { get; set; }
    public DbSet<ConventionValue> ConventionValues { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<LaborRemunerationParameters> LaborRemunerationParameterses { get; set; }
    public DbSet<LaborRemunerationParametersValue> LaborRemunerationParametersValues { get; set; }
    public DbSet<PayrollConcept> PayrollConcepts { get; set; }
    public DbSet<PayrollSettlement> PayrollSettlements { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Admi");

        builder.Entity<City>()
            .HasIndex(x => new { x.StateId, x.Name })
            .IsUnique();

        builder.Entity<Country>()
            .HasIndex(x => x.Name)
            .IsUnique();

        builder.Entity<Neighborhood>()
            .HasIndex(x => new { x.CityId, x.Name })
            .IsUnique();

        builder.Entity<State>()
            .HasIndex(x => new { x.CountryId, x.Name })
            .IsUnique();

        builder.Entity<Statu>()
            .HasIndex(x => new { x.Name, x.Nivel })
            .IsUnique();

        builder.Entity<Category>()
            .HasIndex(x => x.Name)
            .IsUnique();

        builder.Entity<Convention>()
            .HasIndex(x => x.Name)
            .IsUnique();

        builder.Entity<LaborRemunerationParameters>()
            .HasIndex(x => x.Name)
            .IsUnique();

        builder.Entity<Charge>()
            .HasIndex(x => new {x.GroupId,x.Name })
            .IsUnique();

        builder.Entity<Group>()
            .HasIndex(x => new {x.CategoryId,x.Name })
            .IsUnique();

        builder.Entity<PayrollConcept>()
            .HasIndex(x => x.Name)
            .IsUnique();

        builder.Entity<User>()
            .HasOne(x=>x.Charge)
            .WithMany(x=>x.Users)
            .HasForeignKey(x=>x.ChargeId)
            .IsRequired(false);


        builder.Entity<PayrollConcept>()
            .HasOne(x => x.LRP)
            .WithMany(x => x.PayrollConcepts)
            .HasForeignKey(x => x.LRPId)
            .IsRequired(false);


        DisableCascadingDelete(builder);

    }

    private static void DisableCascadingDelete(ModelBuilder builder)
    {
        var relationships = builder.Model
            .GetEntityTypes()
            .SelectMany(x => x.GetForeignKeys());

        foreach (var relationship in relationships)
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
