//Challenge: Implementing BreedsController and Validations

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Net;

using Wpm.Management.Api.DataAccess;

namespace Wpm.Management.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BreedsController(ManagementDbContext dbContext, ILogger<BreedsController> logs) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var all = await dbContext.Breeds.ToListAsync();
        if (all ==null)
        {
            logs.LogInformation("No breeds found");
            return NotFound();
        }
        else
        {
            return Ok(all);
        }
    }

    [HttpGet("{id}", Name = nameof(GetBreedById))]
    public async Task<IActionResult> GetBreedById(int id)
    {
        var breed = await dbContext.Breeds
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();
        if (breed == null)
        {
            logs.LogInformation("No breeds found for requested ID: "+id);
            return NotFound();
        }
        return Ok(breed);
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewBreed newBreed)
    {
        try
        {
            var breed = newBreed.ToBreed();
            await dbContext.Breeds.AddAsync(breed);
            await dbContext.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetBreedById), new { id = breed.Id }, newBreed);
        }
        catch (Exception createBreedException)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            logs.LogError(createBreedException, "Error creating breed: "+createBreedException.ToString());
            return StatusCode((int)statusCode);
        }
    }

    public record NewBreed(string Name)
    {
        public Breed ToBreed() => new Breed
        {
            Id = 0,
            Name = Name
        };
    }
}
