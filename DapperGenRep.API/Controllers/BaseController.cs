using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace DapperGenRep.API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Handles the response of methods that return the number of affected rows in the database.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns><see cref="ActionResult"/> object response.</returns>
        protected ActionResult HandleResponse(int rows)
        {
            switch (HttpContext.Request.Method)
            {
                case "POST":
                    if (rows > 0)
                    {
                        return NoContent();
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, "Could not create record.");

                case "PUT":
                    if (rows > 0)
                    {
                        return NoContent();
                    }
                    return NotFound("Error updating. Could not find record.");

                case "DELETE":
                    if (rows > 0)
                    {
                        return NoContent();
                    }
                    return NotFound("Error deleting. Could not find record.");

                default:
                    return BadRequest("Unsupported HTTP method.");
            }
        }

        /// <summary>
        /// Handles the response of methods that return the object model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="ActionResult"/> object response.</returns>
        protected ActionResult HandleResponse<TModel>(TModel model)
            where TModel : BaseModel
        {
            switch (HttpContext.Request.Method)
            {
                case "GET":
                    if (model is not null)
                    {
                        return Ok(model);
                    }

                    return NotFound("Could not find record.");

                default:
                    return BadRequest("Unsupported HTTP method.");
            }
        }

        /// <summary>
        /// Handles the response of methods that return a list of object models.
        /// </summary>
        /// <param name="models"></param>
        /// <returns><see cref="ActionResult"/> object response.</returns>
        protected ActionResult HandleResponse<TModel>(IEnumerable<TModel> models)
            where TModel : BaseModel
        {
            switch (HttpContext.Request.Method)
            {
                case "GET":
                    return Ok(models);

                default:
                    return BadRequest("Unsupported HTTP method.");
            }
        }
    }
}
