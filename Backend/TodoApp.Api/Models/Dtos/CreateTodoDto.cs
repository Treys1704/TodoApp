using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Models.Dtos;

public class CreateTodoDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = default!;
}
