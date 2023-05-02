using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        [Required/*(ErrorMessage = "Szükséges egy megjelenési név!")*/]
        public string LoginName { get; set; }

        [Required/*(ErrorMessage = "Szükséges egy bejelentkezési név!")*/]
        public string UserName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateOnly? DateOfBirth { get; set; } //optional to make it work
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        [StringLength(8,MinimumLength =4)]
        public string Password { get; set; }


    }
}
