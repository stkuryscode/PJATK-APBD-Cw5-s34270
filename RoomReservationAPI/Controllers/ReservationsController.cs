using Microsoft.AspNetCore.Mvc;
using RoomReservationAPI.Data;
using RoomReservationAPI.Models;

namespace RoomReservationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetReservations([FromQuery] string? date, [FromQuery] string? status, [FromQuery] int? roomId)
    {
        var query = InMemoryDataStore.Reservations.AsQueryable();

        if (!string.IsNullOrEmpty(date))
            query = query.Where(r => r.Date == date);
            
        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            
        if (roomId.HasValue)
            query = query.Where(r => r.RoomId == roomId.Value);

        return Ok(query.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetReservation(int id)
    {
        var reservation = InMemoryDataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null) return NotFound($"Nie znaleziono rezerwacji o id: {id}");
        
        return Ok(reservation);
    }

    [HttpPost]
    public IActionResult CreateReservation([FromBody] Reservation newReservation)
    {
        
        var room = InMemoryDataStore.Rooms.FirstOrDefault(r => r.Id == newReservation.RoomId);
        if (room == null) return NotFound("Wskazana sala nie istnieje.");
        if (!room.IsActive) return BadRequest("Nie można zarezerwować nieaktywnej sali.");

        
        if (IsTimeConflict(newReservation))
        {
            return Conflict("Rezerwacja nakłada się czasowo z inną rezerwacją w tej sali.");
        }

        newReservation.Id = InMemoryDataStore.Reservations.Any() ? InMemoryDataStore.Reservations.Max(r => r.Id) + 1 : 1;
        InMemoryDataStore.Reservations.Add(newReservation);

        return CreatedAtAction(nameof(GetReservation), new { id = newReservation.Id }, newReservation);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateReservation(int id, [FromBody] Reservation updatedReservation)
    {
        var reservation = InMemoryDataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null) return NotFound($"Nie znaleziono rezerwacji o id: {id}");

        var room = InMemoryDataStore.Rooms.FirstOrDefault(r => r.Id == updatedReservation.RoomId);
        if (room == null) return NotFound("Wskazana sala nie istnieje.");
        if (!room.IsActive) return BadRequest("Nie można zarezerwować nieaktywnej sali.");
        
        if (IsTimeConflict(updatedReservation, id))
        {
            return Conflict("Aktualizacja powoduje nakładanie się czasowe z inną rezerwacją.");
        }

        reservation.RoomId = updatedReservation.RoomId;
        reservation.OrganizerName = updatedReservation.OrganizerName;
        reservation.Topic = updatedReservation.Topic;
        reservation.Date = updatedReservation.Date;
        reservation.StartTime = updatedReservation.StartTime;
        reservation.EndTime = updatedReservation.EndTime;
        reservation.Status = updatedReservation.Status;

        return Ok(reservation);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteReservation(int id)
    {
        var reservation = InMemoryDataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null) return NotFound($"Nie znaleziono rezerwacji o id: {id}");

        InMemoryDataStore.Reservations.Remove(reservation);
        return NoContent();
    }
    
    private bool IsTimeConflict(Reservation newRes, int? excludeReservationId = null)
    {
        var startNew = TimeSpan.Parse(newRes.StartTime);
        var endNew = TimeSpan.Parse(newRes.EndTime);

        return InMemoryDataStore.Reservations
            .Where(r => r.RoomId == newRes.RoomId && r.Date == newRes.Date && r.Id != excludeReservationId)
            .Any(r =>
            {
                var startExisting = TimeSpan.Parse(r.StartTime);
                var endExisting = TimeSpan.Parse(r.EndTime);
                
                return startNew < endExisting && endNew > startExisting;
            });
    }
}