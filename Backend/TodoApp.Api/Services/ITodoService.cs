using TodoApp.Api.Models.Dtos;

namespace TodoApp.Api.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoItemDto>> GetAllAsync();
    Task<TodoItemDto?> GetByIdAsync(int id);
    Task<TodoItemDto> CreateAsync(CreateTodoDto dto);
    Task<TodoItemDto?> UpdateAsync(int id, UpdateTodoDto dto);
    Task<TodoItemDto?> CompleteAsync(int id);
    Task<bool> DeleteAsync(int id);
}
