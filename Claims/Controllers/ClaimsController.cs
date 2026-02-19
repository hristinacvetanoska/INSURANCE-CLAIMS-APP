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
        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await this.claimService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<Claim> GetByIdAsync(string id)
        {
            return await this.claimService.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Claim claim)
        {
            try
            {
                var createdClaim = await this.claimService.CreateAsync(claim);

                return CreatedAtAction(
                    nameof(GetByIdAsync),
                    new { id = createdClaim.Id },
                    createdClaim
                    );
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                await this.claimService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound();
            }
        }
    }
}
