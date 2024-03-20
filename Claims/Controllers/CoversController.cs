using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;

    private readonly IAuditService _auditService;

    private readonly ICoverService _coverService;

    public CoversController(IAuditService auditService, ILogger<CoversController> logger, ICoverService coverService)
    {
        _logger = logger;
        _auditService = auditService;
        _coverService = coverService;
    }

    /// <summary>
    /// Get all covers
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var covers = await _coverService.GetCoversAsync();
        return Ok(covers);
    }

    /// <summary>
    /// Get cover by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        var cover = await _coverService.GetCoverAsync(id);
        if (cover is null) return NoContent();
        return Ok(cover);
    }

    /// <summary>
    /// Create cover
    /// </summary>
    /// <param name="coverReq"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Cover>> CreateAsync(CreateCoverRequest coverReq)
    {
        string id = Guid.NewGuid().ToString();
        Cover cover = new Cover()
        {
            Id = id,
            EndDate = coverReq.EndDate,
            StartDate = coverReq.StartDate,
            Type = coverReq.Type
        };
        _ = _auditService.AuditCover(cover.Id, "POST");

        try
        {
            await _coverService.AddCoverAsync(cover);
            return Ok(cover);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    /// <summary>
    /// Delete cover by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        _ = _auditService.AuditCover(id, "DELETE");
        var coverWasDeleted = await _coverService.DeleteCoverAsync(id);
        if (!coverWasDeleted) return NotFound("Cover not found");
        return NoContent();
    }


    /// <summary>
    /// Calculate premium
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="coverType"></param>
    /// <returns></returns>
    // For now i keep it in the CoversController but might be neccessary to move to a separate controller in the future if 
    // premium calculator needs more endpoints
    // Might need validation of inputs
    [HttpPost("calculate-premium")]
    // For now i keep it in the CoversController but might be neccessary to move to a separate controller in the future if 
    // premium calculator needs more endpoints
    // Might need validation of inputs
    public ActionResult<decimal> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Ok(PremiumCalculator.ComputePremium(startDate, endDate, coverType));
    }
}