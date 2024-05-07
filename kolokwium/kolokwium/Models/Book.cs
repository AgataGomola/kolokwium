using System.ComponentModel.DataAnnotations;

namespace kolokwium.Models;

public class Book
{
    [Required]
    public int ID { get; set; }
    [Required]
    [MinLength(3)]
    [MaxLength(25)]
    public string Title { get; set; }
    [Required]
    public Author Author { get; set; }
}
public class NewBook
{
    [Required]
    [MinLength(3)]
    [MaxLength(25)]
    public string Title { get; set; }
    [Required]
    public Author Author { get; set; }
}