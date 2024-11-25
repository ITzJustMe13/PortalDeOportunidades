using BackEnd.Controllers.Data;
using BackEnd.Interfaces;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
        }

        //GET para obter todas as reservas ativas do user
        [Authorize]
        [HttpGet("{userId}/AllActiveReservations")]
        public async Task<IActionResult> GetAllActiveReservationsByUserId(int userId)
        {
            var serviceResponse = await _reservationService.GetAllActiveReservationsByUserIdAsync(userId);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    "InternalServerError" => StatusCode(500, serviceResponse.Message),
                    _ => StatusCode(500, serviceResponse.Message)
                };
            }

            return Ok(serviceResponse.Data);
        }

        // Método para obter todas as reservas de um usuário
        [HttpGet("{userId}/AllReservations")]
        [Authorize]
        public async Task<IActionResult> GetAllReservationByUserId(int userId)
        {
            var serviceResponse = await _reservationService.GetAllReservationsByUserIdAsync(userId);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    "InternalServerError" => StatusCode(500,  serviceResponse.Message),
                    _ => StatusCode(500, "Unknown error.")
                };
            }

            return Ok(serviceResponse.Data);
        }

        //GET para obter uma reserva pelo ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var serviceResponse = await _reservationService.GetReservationByIdAsync(id);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    "InternalServerError" => StatusCode(500,  serviceResponse.Message),
                    _ => StatusCode(500, serviceResponse.Message)
                };
            }

            return Ok(serviceResponse.Data);
        }

        //POST para criar uma nova Reserva
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNewReservation(Reservation reservation)
        {
            var serviceResponse = await _reservationService.CreateNewReservationAsync(reservation);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    "InternalServerError" => StatusCode(500, serviceResponse.Message),
                    _ => StatusCode(500, serviceResponse.Message)
                };
            }

            return CreatedAtAction(nameof(CreateNewReservation), new { id = serviceResponse.Data.reservationId }, serviceResponse.Data);
        }

        //PUT api/Opportunity/1/deactivate
        [HttpPut("{id}/deactivate")]
        [Authorize]
        public async Task<IActionResult> DeactivateReservationById(int id)
        {
            var serviceResponse = await _reservationService.DeactivateReservationByIdAsync(id);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    "InternalServerError" => StatusCode(500, serviceResponse.Message),
                    _ => StatusCode(500, serviceResponse.Message)
                };
            }

            return Ok(serviceResponse.Message);
        }

        //PUT para atualizar uma reserva
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReservation(int id, Reservation reservation)
        {
            var serviceResponse = await _reservationService.UpdateReservationAsync(id, reservation);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "BadRequest" => BadRequest(serviceResponse.Message),
                    "InternalServerError" => StatusCode(500, serviceResponse.Message),
                    _ => StatusCode(500, serviceResponse.Message)
                };
            }

            return Ok(serviceResponse.Message);
        }

        //DELETE para apagar uma reserva
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var serviceResponse = await _reservationService.DeleteReservationAsync(id);

            if (!serviceResponse.Success)
            {
                return serviceResponse.Type switch
                {
                    "NotFound" => NotFound(serviceResponse.Message),
                    "InternalServerError" => StatusCode(500, serviceResponse.Message),
                    _ => StatusCode(500, serviceResponse.Message)
                };
            }

            return Ok(serviceResponse.Message);
        }
    }
}
