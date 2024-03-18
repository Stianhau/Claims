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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var covers = await _coverService.GetCoversAsync();
        return Ok(covers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        var cover = await _coverService.GetCoverAsync(id);
        if (cover is null) return NoContent();
        return Ok(cover);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(CreateCoverRequest coverReq)
    {
        string id = Guid.NewGuid().ToString();
        Cover cover = new Cover()
        {
            Id = id,
            EndDate = coverReq.EndDate,
            StartDate = coverReq.StartDate,
            Type = coverReq.Type
        };
        _auditService.AuditCover(cover.Id, "POST");

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

    [HttpDelete("{id}")]
    public Task DeleteAsync(string id)
    {
        _auditService.AuditCover(id, "DELETE");
        return _coverService.DeleteCoverAsync(id);
    }
}