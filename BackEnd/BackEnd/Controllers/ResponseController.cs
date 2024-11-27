using BackEnd.Interfaces;
using BackEnd.ServiceResponses;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    public class ResponseController : ControllerBase
    {
        protected IActionResult HandleResponse<T>(ServiceResponse<T> serviceResponse)
        {
            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    "Conflict" => Conflict(serviceResponse.Message),
                    "Unauthorized" => Unauthorized(serviceResponse.Message),
                    _ => StatusCode(500, serviceResponse.Message) // InternalServerError
                };
            }

            return serviceResponse.Type switch
            {
                "Ok" => Ok(serviceResponse.Data),
                "NoContent" => NoContent(),
                _ => StatusCode(500, serviceResponse.Message)
            };
        }

        protected IActionResult HandleCreatedAtAction<T>(
            ServiceResponse<T> serviceResponse,
            string actionName,
            object routeValues
        )
        {
            return CreatedAtAction(actionName, routeValues,  serviceResponse.Data);
        }

    }
}
