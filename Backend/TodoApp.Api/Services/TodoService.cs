using TodoApp.Api.Models;
using TodoApp.Api.Models.Dtos;
using TodoApp.Api.Repositories;

namespace TodoApp.Api.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoItemDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToDto);
    }

    public async Task<TodoItemDto?> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item is null ? null : MapToDto(item);
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoDto dto)
    {
        var item = new TodoItem
        {
            Title = dto.Title,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(item);

        return MapToDto(item);
    }

    public async Task<TodoItemDto?> UpdateAsync(int id, UpdateTodoDto dto)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null) return null;

        item.Title = dto.Title;
        await _repository.UpdateAsync(item);

        return MapToDto(item);
    }

    public async Task<TodoItemDto?> CompleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null) return null;

        item.IsCompleted = true;
        await _repository.UpdateAsync(item);

        return MapToDto(item);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null) return false;

        await _repository.DeleteAsync(item);

        return true;
    }

    private static TodoItemDto MapToDto(TodoItem item)
    {
        return new TodoItemDto
        {
            Id = item.Id,
            Title = item.Title,
            IsCompleted = item.IsCompleted,
            CreatedAt = item.CreatedAt
        };
    }
}
