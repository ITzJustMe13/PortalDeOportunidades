using BackEnd.Controllers.Data;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
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
            return Ok(reservation);
        }

        [HttpPost]
        public ActionResult<ReservationModel> CreateNewReservation(ReservationModel reservation)
        {
            // Validação
            if (reservation == null ||
                reservation.opportunityID == 0 ||
                reservation.userID == 0 ||
               !ModelState.IsValid)
            {
                return BadRequest("Some required fields are missing or invalid.");
            }

            // Verifica se a oportunidade e usuário existem
            if (!dbContext.Opportunities.Any(o => o.OpportunityId == reservation.opportunityID))
            {
                return NotFound("Opportunity not found.");
            }
            if (!dbContext.Users.Any(u => u.UserId == reservation.userID))
            {
                return NotFound("User not found.");
            }

            // Adiciona a reserva
            dbContext.Reservations.Add(reservation);
            dbContext.SaveChanges();

            // Retorna a resposta com o novo ID
            return CreatedAtAction(nameof(CreateNewReservation), new { id = reservation.reservationID }, reservation);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int id, ReservationModel reservation)
        {
            // Validação de consistência do ID
            if (reservation.reservationID != id)
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
