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
        private readonly ILogger<CoversController> _logger;

        public CoversController(ICoverService coverService, ILogger<CoversController> logger)
        {
            this.coverService = coverService;
            _logger = logger;
        }

        [HttpPost("compute")]
        public ActionResult<decimal> ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            return Ok(this.coverService.ComputePremium(startDate, endDate, coverType));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cover>>> GetAllAsync()
        {
            return Ok(await this.coverService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cover>> GetByIdAsync(string id)
        {
            try
            {
                return Ok(await this.coverService.GetByIdAsync(id));
            }
            catch(NotFoundException ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Cover cover)
        {
            try
            {
                var createdCover = await this.coverService.CreateAsync(cover);
                return CreatedAtAction(nameof(GetByIdAsync),
                                       new { id = createdCover.Id },
                                       createdCover);
            }
            catch(ValidationException ex)
            {
                return BadRequest(ex.Message);
            }         
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                await this.coverService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound();
            }
        }
    }
}
