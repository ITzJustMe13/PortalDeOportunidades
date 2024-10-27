using BackEnd.Controllers.Data;
using BackEnd.Models.FrontEndModels;
using BackEnd.Models.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public ReservationController(ApplicationDbContext reservationContext) => this.dbContext = reservationContext;

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Reservation>> GetActiveReservationById(int id)
        {
            if (dbContext.Reservations == null)
                return NotFound();
            var reservation = dbContext.Reservations.SingleOrDefault(s => s.reservationID == id);

            if (reservation == null)
                return NotFound();
            var reservationDto = ReservationMapper.MapToDo(reservation); 
            return Ok(reservationDto);
        }

        [HttpPost]
        public ActionResult<Reservation> CreateNewReservation(Reservation reservation)
        {
            // Validação
            if (reservation == null ||
                reservation.oppportunity == null ||
                reservation.user == null ||
               !ModelState.IsValid)
            {
                return BadRequest("Some required fields are missing or invalid.");
            }

            // Verifica se a oportunidade e usuário existem
            if (!dbContext.Opportunities.Any(o => o.OpportunityId == reservation.oppportunity.opportunityId))
            {
                return NotFound("Opportunity not found.");
            }
            if (!dbContext.Users.Any(u => u.UserId == reservation.user.userId))
            {
                return NotFound("User not found.");
            }

            var reservationModel = ReservationMapper.MapToModel(reservation);
            // Adiciona a reserva
            dbContext.Reservations.Add(reservationModel);
            dbContext.SaveChanges();

            // Retorna a resposta com o novo ID
            return CreatedAtAction(nameof(CreateNewReservation), new { id = reservation.reservationId }, reservation);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int id, Reservation reservation)
        {
            // Validação de consistência do ID
            if (reservation.reservationId != id)
                return BadRequest("ID na rota não coincide com o ID do modelo.");

            // Busca a reserva para atualizar
            var existingReservation = dbContext.Reservations.Find(id);

            if (existingReservation == null)
                return NotFound("Reserva não encontrada.");

            // Atualiza as propriedades (exceto as que não devem ser alteradas, como IDs e datas de criação)
            existingReservation.checkInDate = reservation.checkInDate;
            existingReservation.numOfPeople = reservation.numOfPeople;
            existingReservation.isActive = reservation.isActive;

            // Validação de regras de negócio (exemplo, verifiqua se a data de check-in não é anterior à data atual)
            if (reservation.checkInDate < DateTime.Now)
                return BadRequest("Data de check-in não pode ser anterior à data atual.");

            // Atualiza o estado da entidade
            dbContext.Entry(existingReservation).State = EntityState.Modified;
            dbContext.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(int id)
        {
            // Verifica se o contexto não é nulo (embora isso seja improvável aqui, mantém a consistência com o exemplo)
            if (dbContext.Reservations == null)
                return NotFound("Nenhuma reserva encontrada.");

            var reservationToDelete = dbContext.Reservations.Find(id);

            if (reservationToDelete == null)
                return NotFound("Reserva não encontrada.");

            // Remove a reserva
            dbContext.Reservations.Remove(reservationToDelete);
            dbContext.SaveChanges();
            return NoContent();
        }
    }
}
