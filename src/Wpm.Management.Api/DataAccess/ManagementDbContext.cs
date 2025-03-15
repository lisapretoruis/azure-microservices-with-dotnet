using Microsoft.EntityFrameworkCore;
namespace Wpm.Management.Api.DataAccess;

public class ManagementDbContext(DbContextOptions<ManagementDbContext>options) :DbContext(options)
{
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Breed> Breeds { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Breed>().HasData(
            [
                new Breed { Id = 1, Name = "Husky" },
                new Breed { Id = 2, Name = "Labrador" },
                new Breed { Id = 3, Name = "Great Dane" }
            ]
        );

        modelBuilder.Entity<Pet>().HasData(
            [
                new Pet { Id = 1, Name = "Buddy", Age = 5, BreedId = 1 },
                new Pet { Id = 2, Name = "Daisy", Age = 3, BreedId = 2 },
                new Pet { Id = 3, Name = "Max", Age = 2, BreedId = 3 }
            ]
            );
    }
}

public static class ManagementDbContextExtentions
{
    public static void EnsureDbIsCreated(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetService<ManagementDbContext>();
        context!.Database.EnsureCreated();
    }
}

    public class Pet
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public int BreedId { get; set; }
    public Breed Breed { get; set; }
}

public class Breed
{
    public int Id { get; set; }
    public string Name { get; set; }
}

