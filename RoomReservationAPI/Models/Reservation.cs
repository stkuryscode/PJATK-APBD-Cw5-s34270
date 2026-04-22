using System.ComponentModel.DataAnnotations;

namespace RoomReservationAPI.Models;

public class Reservation : IValidatableObject
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    [Required(ErrorMessage = "Nazwa organizatora jest wymagana.")]
    public string OrganizerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Temat jest wymagany.")]
    public string Topic { get; set; } = string.Empty;

    [Required]
    public string Date { get; set; } = string.Empty; 

    [Required]
    public string StartTime { get; set; } = string.Empty; 

    [Required]
    public string EndTime { get; set; } = string.Empty;

    public string Status { get; set; } = "planned"; 

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (TimeSpan.TryParse(StartTime, out var start) && TimeSpan.TryParse(EndTime, out var end))
        {
            if (end <= start)
            {
                yield return new ValidationResult("Czas zakończenia musi być późniejszy niż czas rozpoczęcia.", new[] { nameof(EndTime) });
            }
        }
        else
        {
            yield return new ValidationResult("Niepoprawny format czasu.");
        }
    }
}