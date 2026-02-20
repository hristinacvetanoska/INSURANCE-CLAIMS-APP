namespace Claims.Controllers
{
    using Claims.Domain.Enums;
    using Claims.Domain.Models;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.ComponentModel.DataAnnotations;

    [ApiController]
    [Route("[controller]")]
    public class CoversController : ControllerBase
    {
        private readonly ICoverService coverService;
        private readonly ILogger<CoversController> logger;

        public CoversController(ICoverService coverService, ILogger<CoversController> logger)
        {
            this.coverService = coverService;
            this.logger = logger;
        }

        [HttpPost("compute")]
        public ActionResult<decimal> ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            this.logger.LogInformation(
                   "ComputePremium called. StartDate: {StartDate}, EndDate: {EndDate}, CoverType: {CoverType}",
                   startDate, endDate, coverType);
            try
            {
                return Ok(this.coverService.ComputePremium(startDate, endDate, coverType));

            }
            catch (ArgumentException ex) 
            {
                this.logger.LogWarning(ex,
                            "Invalid input for ComputePremium. StartDate: {StartDate}, EndDate: {EndDate}, CoverType: {CoverType}",
                            startDate, endDate, coverType);

                return BadRequest(ex.Message);
            }
            catch(Exception e)
            {
                this.logger.LogError(e,
                            "Unexpected error while computing premium. StartDate: {StartDate}, EndDate: {EndDate}, CoverType: {CoverType}",
                            startDate, endDate, coverType);

                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cover>>> GetAllAsync()
        {
            try
            {
                this.logger.LogInformation("GetAllAsync called to retrieve all covers");
                return Ok(await this.coverService.GetAllAsync());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error retrieving covers");
                return StatusCode(500, "An error occurred while retrieving covers.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cover>> GetByIdAsync(string id)
        {
            this.logger.LogInformation("GetByIdAsync called with ID: {CoverId}", id);

            try
            {
                var cover = await this.coverService.GetByIdAsync(id);
                this.logger.LogInformation("Cover found with ID: {CoverId}", id);
                return Ok(cover);
            }
            catch (NotFoundException ex)
            {
                this.logger.LogWarning("Cover not found with ID: {CoverId}", id);
                return NotFound("Cover not found.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Unexpected error retrieving cover {CoverId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Cover cover)
        {
            this.logger.LogInformation("CreateAsync called for cover: {@Cover}", cover);

            try
            {
                var createdCover = await this.coverService.CreateAsync(cover);
                this.logger.LogInformation("Cover created with ID: {CoverId}", createdCover.Id);
                return Created("", createdCover);
            }
            catch(ArgumentNullException ae)
            {
                this.logger.LogWarning(ae,
                            "Invalid input for ComputePremium. StartDate: {StartDate}, EndDate: {EndDate}, CoverType: {CoverType}",
                            cover.StartDate, cover.EndDate, cover.Type);

                return BadRequest(ae.Message);
            }
            catch(ValidationException ex)
            {
                this.logger.LogWarning("Validation failed for cover: {@Cover}. Error: {Error}", cover, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error creating cover: {@Cover}", cover);
                return StatusCode(500, "An error occurred while creating the cover.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            this.logger.LogInformation("DeleteAsync called for ID: {CoverId}", id);

            try
            {
                await this.coverService.DeleteAsync(id);
                this.logger.LogInformation("Cover deleted with ID: {CoverId}", id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                this.logger.LogWarning("Attempted to delete non-existing cover with ID: {CoverId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error deleting cover with id: {CoverId}", id);
                return StatusCode(500, "An error occurred while deleting the cover.");
            }
        }
    }
}
