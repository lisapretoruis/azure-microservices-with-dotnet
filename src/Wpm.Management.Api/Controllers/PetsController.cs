using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

using Wpm.Management.Api.DataAccess;

namespace Wpm.Management.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController(ManagementDbContext dbContext, ILogger<BreedsController> logs) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var all = await dbContext.Pets.Include(p => p.Breed).ToListAsync();
        if (all == null)
        {
            logs.LogInformation("No pets found");
            return NotFound();
        }
        else
        {
            return Ok(all);
        }
        return Ok(all);
    }

    [HttpGet("{id}", Name =nameof(GetById))]
    public async Task<IActionResult> GetById(int id)
    {
        var pet = await dbContext.Pets.Include(p => p.Breed)
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        if (pet == null)
        {
            logs.LogInformation("No pets found for requested ID: " + id);
            return NotFound();
        }
        return Ok(pet);
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewPet newPet)
    {
        try
        {
            var pet = newPet.ToPet();
            await dbContext.Pets.AddAsync(pet);
            await dbContext.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetById), new { id = pet.Id }, newPet);
        }
        catch (Exception createPetException)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            logs.LogError(createPetException, "Error creating pet: " + createPetException.ToString());
            return StatusCode((int)statusCode);
        }
    }
}

public record NewPet(string Name, int Age, int BreedId)
{
    public Pet ToPet() => new Pet
    {
        Name = Name,
        Age = Age,
        BreedId = BreedId
    };
}
