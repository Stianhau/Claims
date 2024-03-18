using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {

        private readonly ILogger<ClaimsController> _logger;
        private readonly IAuditService _auditService;
        private readonly IClaimsService _claimsService;

        public ClaimsController(ILogger<ClaimsController> logger, IClaimsService claimsService, AuditContext auditContext, IAuditService auditService)
        {
            _logger = logger;
            _claimsService = claimsService;
            _auditService = auditService;
        }

        [HttpGet]
        public Task<IEnumerable<Claim>> GetAsync()
        {
            return _claimsService.GetClaimsAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateClaimRequest claimReq)
        {
            var id = Guid.NewGuid().ToString();
            Claim claim = new Claim()
            {
                Id = id,
                CoverId = claimReq.CoverId,
                Created = DateTime.Now,
                DamageCost = claimReq.DamageCost,
                Name = claimReq.Name,
                Type = claimReq.Type
            };

            await _auditService.AuditClaim(claim.Id, "POST");

            try
            {
                await _claimsService.AddClaimAsync(claim);
                return Ok(claim);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            await _auditService.AuditClaim(id, "DELETE");
            await _claimsService.DeleteClaimAsync(id);
        }

        [HttpGet("{id}")]
        public Task<Claim> GetAsync(string id)
        {
            return _claimsService.GetClaimAsync(id);
        }
    }
}