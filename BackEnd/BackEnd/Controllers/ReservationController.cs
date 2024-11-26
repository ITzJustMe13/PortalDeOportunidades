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
    public class ReservationController : BaseController
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

            return HandleResponse(serviceResponse);
        }

        // Método para obter todas as reservas de um usuário
        [HttpGet("{userId}/AllReservations")]
        [Authorize]
        public async Task<IActionResult> GetAllReservationByUserId(int userId)
        {
            var serviceResponse = await _reservationService.GetAllReservationsByUserIdAsync(userId);

            return HandleResponse(serviceResponse);
        }

        //GET para obter uma reserva pelo ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var serviceResponse = await _reservationService.GetReservationByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        //POST para criar uma nova Reserva
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNewReservation(Reservation reservation)
        {
            var serviceResponse = await _reservationService.CreateNewReservationAsync(reservation);

            if (!serviceResponse.Success)
            {
                return HandleResponse(serviceResponse);
            }

            return HandleCreatedAtAction(serviceResponse, nameof(CreateNewReservation), new { id = serviceResponse.Data.reservationId });
        }

        //PUT api/Opportunity/1/deactivate
        [HttpPut("{id}/deactivate")]
        [Authorize]
        public async Task<IActionResult> DeactivateReservationById(int id)
        {
            var serviceResponse = await _reservationService.DeactivateReservationByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        //PUT para atualizar uma reserva
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReservation(int id, Reservation reservation)
        {
            var serviceResponse = await _reservationService.UpdateReservationAsync(id, reservation);

            return HandleResponse(serviceResponse);
        }

        //DELETE para apagar uma reserva
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var serviceResponse = await _reservationService.DeleteReservationAsync(id);

            return HandleResponse(serviceResponse);
        }
    }
}
