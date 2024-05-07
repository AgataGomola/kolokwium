using System.ComponentModel.DataAnnotations;

namespace kolokwium.Models;

public class Author
{
    [Required]
    [MinLength(3)]
    [MaxLength(15)]
    public string FirstName { get; set; }
    [Required]
    [MinLength(3)]
    [MaxLength(15)]
    public string LastName { get; set; }
   
}