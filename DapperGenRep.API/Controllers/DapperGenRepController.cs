using Application.Services.Intf;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace DapperGenRep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DapperGenRepController : BaseController
    {
        private readonly IService _service;

        public DapperGenRepController(IService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lists all records.
        /// </summary>
        /// <returns>List of all records.</returns>
        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Model>>> GetAll()
        {
            IEnumerable<Model> response = await _service.GetAllAsync();

            return HandleResponse(response);
        }

        /// <summary>
        /// Lists all records with filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>List of all filtered records.</returns>
        [HttpGet("list/{filter}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Model>>> GetAll(string filter)
        {
            IEnumerable<Model> response = await _service.GetAllAsync(e => e.DATE == DateTime.Parse(filter));

            return HandleResponse(response);
        }

        /// <summary>
        /// Gets first record.
        /// </summary>
        /// <returns>First record object.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Model>> Get()
        {
            Model response = await _service.GetAsync();

            return HandleResponse(response);
        }

        /// <summary>
        /// Gets first record with filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>First filtered record object.</returns>
        [HttpGet("{filter}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Model>> Get(int filter)
        {
            Model response = await _service.GetAsync(e => e.ID == filter);

            return HandleResponse(response);
        }

        /// <summary>
        /// Creates a record.
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Create(Model model)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            int response = await _service.CreateAsync(model);

            _service.Commit();

            return HandleResponse(response);
        }

        /// <summary>
        /// Updates the filtered record.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filter"></param>
        [HttpPut("{filter}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(Model model, int filter)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            int response = await _service.UpdateAsync(model, e => e.ID == filter);

            _service.Commit();

            return HandleResponse(response);
        }

        /// <summary>
        /// Deletes the filtered record.
        /// </summary>
        /// <param name="filter"></param>
        [HttpDelete("{filter}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int filter)
        {
            int response = await _service.DeleteAsync(e => e.ID == filter);

            _service.Commit();

            return HandleResponse(response);
        }
    }
}
