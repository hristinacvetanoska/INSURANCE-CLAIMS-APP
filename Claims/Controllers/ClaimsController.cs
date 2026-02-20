namespace Claims.Controllers
{
    using Claims.Domain.Models;
    using Claims.DTOs;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// REST API controller for managing Claims.
    /// Provides endpoints to list, read, create and delete claims.
    /// </summary>
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

        /// <summary>
        /// Retrieve all claims.
        /// </summary>
        /// <returns>Collection of <see cref="Claim"/>.</returns>
        /// <response code="200">List of claims returned.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClaimDto>>> GetAllAsync()
        {
            try
            {
                this.logger.LogInformation("GetAllAsync called to retrieve all claims");
                var claims = await this.claimService.GetAllAsync() ?? new List<ClaimDto>();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error retrieving claims");
                return StatusCode(500, "An error occurred while retrieving claims.");
            }
        }

        /// <summary>
        /// Retrieve a claim by id.
        /// </summary>
        /// <param name="id">Identifier of the claim.</param>
        /// <returns>The requested <see cref="Claim"/>.</returns>
        /// <response code="200">Claim found and returned.</response>
        /// <response code="404">Claim not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClaimDto>> GetByIdAsync(string id)
        {

            try
            {
                return Ok(await this.claimService.GetByIdAsync(id));
            }
            catch (NotFoundException)
            {
                return NotFound("Claim not found.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Unexpected error retrieving claim {ClaimId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Create a new claim.
        /// </summary>
        /// <param name="claim">Claim model to create.</param>
        /// <returns>Created <see cref="Claim"/> with assigned id.</returns>
        /// <response code="201">Claim created successfully.</response>
        /// <response code="400">Validation failed for the provided claim.</response>
        /// <response code="404">Related cover not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(ClaimDto claimDto)
        {
            this.logger.LogInformation("CreateAsync called for Claim: {@Claim}", claimDto);

            try
            {
                var createdClaim = await this.claimService.CreateAsync(claimDto);
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
                this.logger.LogError(ex, "Error creating claim: {@Claim}", claimDto);
                return StatusCode(500, "An error occurred while creating the claim.");
            }
        }

        /// <summary>
        /// Delete a claim by id.
        /// </summary>
        /// <param name="id">Identifier of the claim to delete.</param>
        /// <response code="204">Claim deleted successfully.</response>
        /// <response code="404">Claim not found.</response>
        /// <response code="500">Unexpected server error.</response>
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
