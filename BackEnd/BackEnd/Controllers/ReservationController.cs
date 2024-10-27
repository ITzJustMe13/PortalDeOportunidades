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
    }
}
