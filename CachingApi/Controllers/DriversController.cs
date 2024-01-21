using CachingApi.Database;
using CachingApi.Models;
using CachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CachingApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DriversController: ControllerBase
{
    private readonly ILogger<DriversController> _logger;
    private readonly ICacheService _cacheService;
    private readonly ApplicationDbContext _dbContext;
    
    public DriversController(ILogger<DriversController> logger, ICacheService cacheService, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    [HttpGet("drivers/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        try
        {
            //check cache
            var cachedData = _cacheService.GetData<IEnumerable<Driver>>($"driver{id}");
            if (cachedData != null && cachedData.Any())
            {
                return Ok(cachedData);
            }

            cachedData = await _dbContext.Driver.ToListAsync();
        
            //set expiry Time

            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData<IEnumerable<Driver>>($"driver{id}", cachedData, expiryTime);

            return Ok(cachedData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        

    }

    [HttpPost("AddDriver")]
    public async Task<IActionResult> Post(Driver model)
    {
        var addedObj = await _dbContext.Driver.AddAsync(model);

        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData<Driver>($"driver{model.Id}", addedObj.Entity, expiryTime);
        
        await _dbContext.SaveChangesAsync();

        return Ok(addedObj.Entity);

    }

    [HttpDelete("DeleteDriver")]
    public async Task<IActionResult> Delete(int id)
    {
        var exist =  await _dbContext.Driver
            .FirstOrDefaultAsync(a => a.Id == id);

        if (exist != null)
        {
            _dbContext.Remove(exist);
            _cacheService.RemoveData($"driver{id}");

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }
    
}