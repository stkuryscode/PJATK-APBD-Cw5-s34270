using Microsoft.AspNetCore.Mvc;
using RoomReservationAPI.Data;
using RoomReservationAPI.Models;

namespace RoomReservationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRooms([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector, [FromQuery] bool? activeOnly)
    {
        var query = InMemoryDataStore.Rooms.AsQueryable();

        if (minCapacity.HasValue)
            query = query.Where(r => r.Capacity >= minCapacity.Value);
        
        if (hasProjector.HasValue)
            query = query.Where(r => r.HasProjector == hasProjector.Value);
        
        if (activeOnly.HasValue && activeOnly.Value)
            query = query.Where(r => r.IsActive);

        return Ok(query.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetRoom(int id)
    {
        var room = InMemoryDataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room == null) return NotFound($"Nie znaleziono sali o id: {id}");
        
        return Ok(room);
    }

    [HttpGet("building/{buildingCode}")]
    public IActionResult GetRoomsByBuilding(string buildingCode)
    {
        var rooms = InMemoryDataStore.Rooms
            .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase))
            .ToList();
            
        return Ok(rooms);
    }

    [HttpPost]
    public IActionResult CreateRoom([FromBody] Room newRoom)
    {
        newRoom.Id = InMemoryDataStore.Rooms.Any() ? InMemoryDataStore.Rooms.Max(r => r.Id) + 1 : 1;
        InMemoryDataStore.Rooms.Add(newRoom);
        
        return CreatedAtAction(nameof(GetRoom), new { id = newRoom.Id }, newRoom);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRoom(int id, [FromBody] Room updatedRoom)
    {
        var room = InMemoryDataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room == null) return NotFound($"Nie znaleziono sali o id: {id}");

        room.Name = updatedRoom.Name;
        room.BuildingCode = updatedRoom.BuildingCode;
        room.Floor = updatedRoom.Floor;
        room.Capacity = updatedRoom.Capacity;
        room.HasProjector = updatedRoom.HasProjector;
        room.IsActive = updatedRoom.IsActive;

        return Ok(room);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRoom(int id)
    {
        var room = InMemoryDataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room == null) return NotFound($"Nie znaleziono sali o id: {id}");
        
        var hasReservations = InMemoryDataStore.Reservations.Any(r => r.RoomId == id);
        if (hasReservations)
        {
            return Conflict("Nie można usunąć sali, ponieważ przypisane są do niej rezerwacje.");
        }

        InMemoryDataStore.Rooms.Remove(room);
        return NoContent();
    }
}