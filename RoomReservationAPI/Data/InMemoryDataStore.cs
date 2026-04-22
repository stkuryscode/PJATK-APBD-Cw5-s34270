using RoomReservationAPI.Models;

namespace RoomReservationAPI.Data;

public static class InMemoryDataStore
{
    public static List<Room> Rooms { get; set; } = new()
    {
        new Room { Id = 1, Name = "Skybox Center", BuildingCode = "C", Floor = 10, Capacity = 15, HasProjector = true, IsActive = true },
        new Room { Id = 2, Name = "The Garden", BuildingCode = "MSG", Floor = 1, Capacity = 150, HasProjector = true, IsActive = true },
        new Room { Id = 3, Name = "Training Room B", BuildingCode = "TC", Floor = 0, Capacity = 8, HasProjector = false, IsActive = true },
        new Room { Id = 4, Name = "Media Suite", BuildingCode = "A", Floor = 4, Capacity = 45, HasProjector = true, IsActive = true },
        new Room { Id = 5, Name = "Underground Gym", BuildingCode = "B", Floor = -2, Capacity = 20, HasProjector = false, IsActive = false }
    };

    public static List<Reservation> Reservations { get; set; } = new()
    {
        new Reservation 
        { 
            Id = 1, RoomId = 2, OrganizerName = "LeBron James", Topic = "Analiza taktyczna finałów", 
            Date = "2026-05-10", StartTime = "08:00:00", EndTime = "10:30:00", Status = "confirmed" 
        },
        new Reservation 
        { 
            Id = 2, RoomId = 1, OrganizerName = "Stephen Curry", Topic = "Trening rzutowy - teoria", 
            Date = "2026-05-10", StartTime = "11:00:00", EndTime = "13:00:00", Status = "planned" 
        },
        new Reservation 
        { 
            Id = 3, RoomId = 4, OrganizerName = "Nikola Jokic", Topic = "Warsztaty z przeglądu pola", 
            Date = "2026-05-11", StartTime = "15:00:00", EndTime = "17:00:00", Status = "confirmed" 
        },
        new Reservation 
        { 
            Id = 4, RoomId = 2, OrganizerName = "Luka Doncic", Topic = "Step-back masterclass", 
            Date = "2026-05-12", StartTime = "12:00:00", EndTime = "14:00:00", Status = "planned" 
        },
        new Reservation 
        { 
            Id = 5, RoomId = 3, OrganizerName = "Giannis Antetokounmpo", Topic = "Przygotowanie siłowe", 
            Date = "2026-05-10", StartTime = "09:00:00", EndTime = "11:00:00", Status = "confirmed" 
        }
    };
}