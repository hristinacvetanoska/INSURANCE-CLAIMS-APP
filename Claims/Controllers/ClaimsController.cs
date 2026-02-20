namespace Claims.Controllers
{
    using Claims.Domain.Models;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.ComponentModel.DataAnnotations;

    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IClaimService claimService;
        private readonly ILogger<ClaimsController> logger;

        public ClaimsController(IClaimService claimService, ILogger<ClaimsController> logger)
        {
            this.claimService = claimService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetAllAsync()
        {
            try
            {
                this.logger.LogInformation("GetAllAsync called to retrieve all claims");
                return Ok(await this.claimService.GetAllAsync());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error retrieving claims");
                return StatusCode(500, "An error occurred while retrieving claims.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Claim>> GetByIdAsync(string id)
        {

            try
            {
                return Ok(await this.claimService.GetByIdAsync(id));
            }
            catch (NotFoundException ex)
            {
                return NotFound("Claim not found.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Unexpected error retrieving claim {ClaimId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Claim claim)
        {
            this.logger.LogInformation("CreateAsync called for Claim: {@Claim}", claim);

            try
            {
                var createdClaim = await this.claimService.CreateAsync(claim);
                this.logger.LogInformation("Claim created: {@CreatedClaim}", createdClaim);
                return Created("", createdClaim);
            }
            catch(NotFoundException e)
            {
                this.logger.LogWarning(e.Message);
                return NotFound(e.Message);
            }
            catch (ValidationException ex)
            {
                this.logger.LogWarning("Validation failed: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error creating claim: {@Claim}", claim);
                return StatusCode(500, "An error occurred while creating the claim.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            this.logger.LogInformation("DeleteAsync called for ClaimId: {ClaimId}", id);

            try
            {
                await this.claimService.DeleteAsync(id);
                this.logger.LogInformation("Claim deleted: {ClaimId}", id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                this.logger.LogWarning("Claim not found for deletion: {ClaimId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error deleting claim: {ClaimId}", id);
                return StatusCode(500, "An error occurred while deleting the claim.");
            }
        }
    }
}
