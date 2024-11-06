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

        //GET para obter todas as reservas ativas do user
        [Authorize]
        [HttpGet("{userId}/AllActiveReservations")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetAllActiveReservationsByUserId(int userId)
        {
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

        // Método para obter todas as reservas de um usuário
        [HttpGet("{userId}/AllReservations")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetAllReservationByUserId(int userId)
        {
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

        //GET para obter uma reserva pelo ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Reservation>> GetReservationById(int id)
        {
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

        //POST para criar uma nova Reserva
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Reservation>> CreateNewReservation(Reservation reservation)
        {
            // Validação
            if (reservation == null || reservation.opportunityId == 0 || reservation.userId == 0 || !ModelState.IsValid)
            {
                return BadRequest("Some required fields are missing or invalid.");
            }

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
                return NotFound("The value must be valid");
            }

            if (reservation.numOfPeople > opportunity.Vacancies)
            {
                return NotFound("The numberOfPeople is bigger than number of vacancies");
            }


            reservation.reservationDate = DateTime.Now;
            reservation.checkInDate = opportunity.date;
            reservation.isActive = true;
            reservation.fixedPrice = ((float?)(reservation.numOfPeople * opportunity.Price));

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

        //PUT api/Opportunity/1/deactivate
        [HttpPut("{id}/deactivate")]
        [Authorize]
        public async Task<ActionResult<Reservation>> DeactivateReservationById(int id)
        {
            var reservationModel = await dbContext.Reservations.FindAsync(id);
            if (reservationModel == null)
            {
                return NotFound($"Reservation with id {id} not found.");
            }
            if (reservationModel.checkInDate > DateTime.Now && reservationModel.isActive == true)
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

        //PUT para atualizar uma reserva
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReservation(int id, Reservation reservation)
        {
            // Busca a reserva e a oportunidade associada
            var existingReservation = await dbContext.Reservations.FindAsync(id);
            var opportunity = await dbContext.Opportunities.FindAsync(reservation.opportunityId);

            if (existingReservation == null)
            {
                return NotFound("Reservation Not Found");
            }

            if (reservation.numOfPeople < 0)
            {
                return NotFound("The value must be valid");
            }

            if (reservation.numOfPeople > opportunity.Vacancies)
            {
                return NotFound("The numberOfPeople is bigger than number of vacancies");
            }


            // Atualiza as propriedades da reserva
            existingReservation.numOfPeople = reservation.numOfPeople;
            existingReservation.fixedPrice = ((float)(reservation.numOfPeople * opportunity.Price));

            dbContext.Entry(existingReservation).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        //DELETE para apagar uma reserva
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservationToDelete = await dbContext.Reservations.FindAsync(id);

            if (reservationToDelete == null)
            {
                return NotFound("Reservation Not Found!");
            }

            dbContext.Reservations.Remove(reservationToDelete);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
