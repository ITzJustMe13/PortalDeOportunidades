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
    public class ReservationController : BaseCrudController<Reservation>
    {
        private readonly IReservationService _reservationService;
        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
        }

        /// <summary>
        /// Endpoint that gets all the active Reservations of a certain User by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{userId}/AllActiveReservations")]
        public async Task<IActionResult> GetAllActiveReservationsByUserId(int userId)
        {
            var serviceResponse = await _reservationService.GetAllActiveReservationsByUserIdAsync(userId);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that gets all the Reservations (active and inactive) of a certain User by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/AllReservations")]
        [Authorize]
        public async Task<IActionResult> GetAllReservationByUserId(int userId)
        {
            var serviceResponse = await _reservationService.GetAllReservationsByUserIdAsync(userId);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint to get a specific Reservation by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public override async Task<IActionResult> GetEntityById(int id)
        {
            var serviceResponse = await _reservationService.GetReservationByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that creates a new Reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public override async Task<IActionResult> CreateEntity(Reservation reservation)
        {
            var serviceResponse = await _reservationService.CreateNewReservationAsync(reservation);

            if (!serviceResponse.Success)
            {
                return HandleResponse(serviceResponse);
            }

            return HandleCreatedAtAction(serviceResponse, nameof(GetEntityById), new { id = serviceResponse.Data.reservationId });
        }

        /// <summary>
        /// Endpoint that deactivates a Reservation by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/deactivate")]
        [Authorize]
        public async Task<IActionResult> DeactivateReservationById(int id)
        {
            var serviceResponse = await _reservationService.DeactivateReservationByIdAsync(id);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that updates the Reservation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public override async Task<IActionResult> UpdateEntity(int id, Reservation reservation)
        {
            var serviceResponse = await _reservationService.UpdateReservationAsync(id, reservation);

            return HandleResponse(serviceResponse);
        }

        /// <summary>
        /// Endpoint that deletes the reservation by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public override async Task<IActionResult> DeleteEntity(int id)
        {
            var serviceResponse = await _reservationService.DeleteReservationAsync(id);

            return HandleResponse(serviceResponse);
        }
    }
}
