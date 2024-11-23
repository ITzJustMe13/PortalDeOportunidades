using BackEnd.Controllers.Data;
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
        private readonly ApplicationDbContext dbContext;

        public ReservationController(ApplicationDbContext reservationContext) => this.dbContext = reservationContext;

        /// <summary>
        /// Endpoint that gets all the active Reservations of a certain User by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{userId}/AllActiveReservations")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetAllActiveReservationsByUserId(int userId)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            var reservations = await dbContext.Reservations
                .Where(r => r.userID == userId && r.isActive)
                .ToListAsync();

            if (!reservations.Any())
            {
                return NotFound("No active reservations found for the specified user.");
            }

            try
            {
                var reservationDtos = reservations.Select(r => ReservationMapper.MapToDto(r));
                return Ok(reservationDtos);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint that gets all the Reservations (active and inactive) of a certain User by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/AllReservations")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetAllReservationByUserId(int userId)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            var reservations = await dbContext.Reservations
                .Where(r => r.userID == userId)
                .ToListAsync();

            if (!reservations.Any())
            {
                return NotFound("No reservations found for the specified user.");
            }

            try
            {
                var reservationDtos = reservations.Select(r => ReservationMapper.MapToDto(r));
                return Ok(reservationDtos);
            }
            catch(ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        /// <summary>
        /// Endpoint to get a specific Reservation by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Reservation>> GetReservationById(int id)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            // Busca a reserva pelo ID fornecido
            var reservation = await dbContext.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }

            // Mapeia o modelo para o DTO
            try
            {
                var reservationDto = ReservationMapper.MapToDto(reservation);
                return Ok(reservationDto);

            }
            catch(ValidationException ex) 
            {
                return BadRequest(ex.Message);
            }
         
        }

        /// <summary>
        /// Endpoint that creates a new Reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Reservation>> CreateNewReservation(Reservation reservation)
        {
            // Validação
            if (reservation == null || reservation.opportunityId == 0 || reservation.userId == 0 || !ModelState.IsValid)
            {
                return BadRequest("Some required fields are missing or invalid.");
            }

            if (dbContext == null)
                return NotFound("DB context missing");

            // Verifica se a oportunidade e o usuário existem
            var opportunity = await dbContext.Opportunities.FindAsync(reservation.opportunityId);
            var user = await dbContext.Users.FindAsync(reservation.userId);

            if (opportunity == null)
            {
                return NotFound("Opportunity not found.");
            }

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if(reservation.numOfPeople < 0)
            {
                return BadRequest("The value Number Of People must be valid");
            }

            if (reservation.numOfPeople > opportunity.Vacancies)
            {
                return BadRequest("The numberOfPeople is bigger than number of vacancies");
            }


            reservation.reservationDate = DateTime.Now;
            reservation.checkInDate = opportunity.Date;
            reservation.isActive = true;
            reservation.fixedPrice = ((float)(reservation.numOfPeople * opportunity.Price));

            try
            {
                // Mapear DTO para o modelo
                var reservationModel = ReservationMapper.MapToModel(reservation);
                dbContext.Reservations.Add(reservationModel);

                await dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateNewReservation), new { id = reservationModel.reservationID }, reservation);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint that deactivates a Reservation by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/deactivate")]
        [Authorize]
        public async Task<ActionResult<Reservation>> DeactivateReservationById(int id)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            var reservationModel = await dbContext.Reservations.FindAsync(id);
            if (reservationModel == null)
            {
                return NotFound($"Reservation with id {id} not found.");
            }

            if (reservationModel.checkInDate > DateTime.Now && reservationModel.isActive)
            {
                reservationModel.isActive = false;
                await dbContext.SaveChangesAsync();
                return Ok();

            }
            else
            {
                return BadRequest("Reservation is impossible to deactivate");
            }

        }

        /// <summary>
        /// Endpoint that updates the Reservation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Reservation>> UpdateReservation(int id, Reservation reservation)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            // Busca a reserva e a oportunidade associada
            var existingReservation = await dbContext.Reservations.FindAsync(id);
            var opportunity = await dbContext.Opportunities.FindAsync(reservation.opportunityId);

            if (existingReservation == null)
            {
                return NotFound("Reservation Not Found");
            }

            if (reservation.numOfPeople < 0)
            {
                return BadRequest("The value Number Of People must be valid");
            }

            if (reservation.numOfPeople > opportunity.Vacancies)
            {
                return BadRequest("The numberOfPeople is bigger than number of vacancies");
            }


            // Atualiza as propriedades da reserva
            existingReservation.numOfPeople = reservation.numOfPeople;
            existingReservation.fixedPrice = ((float)(reservation.numOfPeople * opportunity.Price));

            dbContext.Entry(existingReservation).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Endpoint that deletes the reservation by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteReservation(int id)
        {
            if (dbContext == null)
                return NotFound("DB context missing");

            var reservationToDelete = await dbContext.Reservations.FindAsync(id);

            if (reservationToDelete == null)
            {
                return NotFound("Reservation Not Found!");
            }

            dbContext.Reservations.Remove(reservationToDelete);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
