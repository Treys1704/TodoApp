using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Models.Dtos;

public class UpdateTodoDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = default!;
}
