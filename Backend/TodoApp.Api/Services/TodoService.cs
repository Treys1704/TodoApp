using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Models;
using TodoApp.Api.Models.Dtos;

namespace TodoApp.Api.Services;

public class TodoService : ITodoService
{
    private readonly TodoContext _context;

    public TodoService(TodoContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TodoItemDto>> GetAllAsync()
    {
        var items = await _context.TodoItems
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return items.Select(MapToDto);
    }

    public async Task<TodoItemDto?> GetByIdAsync(int id)
    {
        var item = await _context.TodoItems.FindAsync(id);
        return item is null ? null : MapToDto(item);
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoDto dto)
    {
        var item = new TodoItem
        {
            Title = dto.Title,
            CreatedAt = DateTime.UtcNow
        };

        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();

        return MapToDto(item);
    }

    public async Task<TodoItemDto?> UpdateAsync(int id, UpdateTodoDto dto)
    {
        var item = await _context.TodoItems.FindAsync(id);
        if (item is null) return null;

        item.Title = dto.Title;
        await _context.SaveChangesAsync();

        return MapToDto(item);
    }

    public async Task<TodoItemDto?> CompleteAsync(int id)
    {
        var item = await _context.TodoItems.FindAsync(id);
        if (item is null) return null;

        item.IsCompleted = true;
        await _context.SaveChangesAsync();

        return MapToDto(item);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.TodoItems.FindAsync(id);
        if (item is null) return false;

        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync();

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
