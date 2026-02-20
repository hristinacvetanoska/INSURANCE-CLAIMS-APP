namespace Claims.Controllers
{
    using Claims.Domain.Enums;
    using Claims.Domain.Models;
    using Claims.DTOs;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.ComponentModel.DataAnnotations;


    /// <summary>
    /// REST API controller for managing Covers.
    /// Provides endpoints to compute premium, list, read, create and delete covers.
    /// </summary>
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

        /// <summary>
        /// Compute the premium for a cover period and cover type.
        /// </summary>
        /// <param name="startDate">Start date of the cover (ISO 8601).</param>
        /// <param name="endDate">End date of the cover (ISO 8601).</param>
        /// <param name="coverType">Type of cover.</param>
        /// <returns>Computed premium as decimal.</returns>
        /// <response code="200">Premium computed successfully.</response>
        /// <response code="400">Invalid input (e.g., end date before start date).</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("compute")]
        public ActionResult<decimal> ComputePremium([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] CoverType coverType)
        {
            try
            {
                this.logger.LogInformation(
                       "ComputePremium called. StartDate: {StartDate}, EndDate: {EndDate}, CoverType: {CoverType}",
                       startDate, endDate, coverType);
                return Ok(this.coverService.ComputePremium(startDate, endDate, coverType));

            }
            catch (ValidationException ex) 
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

        /// <summary>
        /// Retrieve all covers.
        /// </summary>
        /// <returns>Collection of <see cref="Cover"/>.</returns>
        /// <response code="200">List of covers returned.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoverDto>>> GetAllAsync()
        {
            try
            {

                this.logger.LogInformation("GetAllAsync called to retrieve all covers");
                var covers = await this.coverService.GetAllAsync() ?? new List<CoverDto>();
                return Ok(covers);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error retrieving covers");
                return StatusCode(500, "An error occurred while retrieving covers.");
            }
        }

        /// <summary>
        /// Retrieve a cover by id.
        /// </summary>
        /// <param name="id">Identifier of the cover.</param>
        /// <returns>The requested <see cref="Cover"/>.</returns>
        /// <response code="200">Cover found and returned.</response>
        /// <response code="404">Cover not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<CoverDto>> GetByIdAsync(string id)
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

        /// <summary>
        /// Create a new cover.
        /// </summary>
        /// <param name="cover">Cover model to create.</param>
        /// <returns>Created <see cref="Cover"/> with assigned id and computed premium.</returns>
        /// <response code="201">Cover created successfully.</response>
        /// <response code="400">Invalid input or validation failed.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CoverDto coverDto)
        {
            this.logger.LogInformation("CreateAsync called for cover: {@Cover}", coverDto);

            try
            {
                var createdCover = await this.coverService.CreateAsync(coverDto);
                this.logger.LogInformation("Cover created: {@Cover}", createdCover);
                return Created("", createdCover);
            }
            catch(ArgumentNullException ae)
            {
                this.logger.LogWarning(ae,
                            "Invalid input for ComputePremium. StartDate: {StartDate}, EndDate: {EndDate}, CoverType: {CoverType}",
                            coverDto.StartDate, coverDto.EndDate, coverDto.Type);

                return BadRequest(ae.Message);
            }
            catch(ValidationException ex)
            {
                this.logger.LogWarning("Validation failed for cover: {@Cover}. Error: {Error}", coverDto, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error creating cover: {@Cover}", coverDto);
                return StatusCode(500, "An error occurred while creating the cover.");
            }
        }

        /// <summary>
        /// Delete a cover by id.
        /// </summary>
        /// <param name="id">Identifier of the cover to delete.</param>
        /// <response code="204">Cover deleted successfully.</response>
        /// <response code="404">Cover not found.</response>
        /// <response code="500">Unexpected server error.</response>
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
