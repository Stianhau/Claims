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

        /// <summary>
        /// Get all claims
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetAsync()
        {
            return Ok(await _claimsService.GetClaimsAsync());
        }

        /// <summary>
        /// Create claim
        /// </summary>
        /// <param name="claimReq"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Claim>> CreateAsync(CreateClaimRequest claimReq)
        {
            var id = Guid.NewGuid().ToString();
            Claim claim = new Claim()
            {
                Id = id,
                CoverId = claimReq.CoverId,
                Created = DateTime.UtcNow,
                DamageCost = claimReq.DamageCost,
                Name = claimReq.Name,
                Type = claimReq.Type
            };

            _ = _auditService.AuditClaim(claim.Id, "POST");

            try
            {
                var res = await _claimsService.AddClaimAsync(claim);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete claim by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            _ = _auditService.AuditClaim(id, "DELETE");
            var deletedClaim = await _claimsService.DeleteClaimAsync(id);
            if (deletedClaim is null) return NotFound("Claim not found");
            return NoContent();
        }

        /// <summary>
        /// Get claim by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<Claim>> GetAsync(string id)
        {
            var claim = await _claimsService.GetClaimAsync(id);
            if (claim is null) return NoContent();
            return Ok(claim);
        }
    }
}