using FluentAssertions;
using TodoApp.Api.Models;
using TodoApp.Api.Models.Dtos;
using TodoApp.Api.Services;
using TodoApp.Api.Tests.Helpers;

namespace TodoApp.Api.Tests.Services;

public class TodoServiceTests : IDisposable
{
    private readonly Data.TodoContext _context;
    private readonly TodoService _sut;

    public TodoServiceTests()
    {
        _context = TestDbContextFactory.Create();
        _sut = new TodoService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_WhenNoItems_ReturnsEmptyList()
    {
        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenItemsExist_ReturnsAllItems()
    {
        _context.TodoItems.AddRange(
            new TodoItem { Title = "Task 1", CreatedAt = DateTime.UtcNow.AddMinutes(-2) },
            new TodoItem { Title = "Task 2", CreatedAt = DateTime.UtcNow.AddMinutes(-1) },
            new TodoItem { Title = "Task 3", CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsItemsOrderedByCreatedAtDescending()
    {
        _context.TodoItems.AddRange(
            new TodoItem { Title = "Oldest", CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new TodoItem { Title = "Newest", CreatedAt = DateTime.UtcNow },
            new TodoItem { Title = "Middle", CreatedAt = DateTime.UtcNow.AddHours(-1) }
        );
        await _context.SaveChangesAsync();

        var result = (await _sut.GetAllAsync()).ToList();

        result[0].Title.Should().Be("Newest");
        result[1].Title.Should().Be("Middle");
        result[2].Title.Should().Be("Oldest");
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsItem()
    {
        var item = new TodoItem { Title = "Test", CreatedAt = DateTime.UtcNow };
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(item.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(item.Id);
        result.Title.Should().Be("Test");
        result.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_AddsItemToDatabase()
    {
        var dto = new CreateTodoDto { Title = "New Task" };

        var result = await _sut.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Title.Should().Be("New Task");
        result.IsCompleted.Should().BeFalse();
        result.Id.Should().BeGreaterThan(0);

        _context.TodoItems.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAtToUtcNow()
    {
        var before = DateTime.UtcNow;

        var result = await _sut.CreateAsync(new CreateTodoDto { Title = "Timed" });

        var after = DateTime.UtcNow;
        result.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public async Task CreateAsync_MultipleCalls_CreatesDistinctItems()
    {
        await _sut.CreateAsync(new CreateTodoDto { Title = "A" });
        await _sut.CreateAsync(new CreateTodoDto { Title = "B" });

        var all = (await _sut.GetAllAsync()).ToList();
        all.Should().HaveCount(2);
        all.Select(x => x.Title).Should().Contain(new[] { "A", "B" });
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_WhenItemExists_UpdatesTitleAndReturnsDto()
    {
        var item = new TodoItem { Title = "Old Title", CreatedAt = DateTime.UtcNow };
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();

        var result = await _sut.UpdateAsync(item.Id, new UpdateTodoDto { Title = "New Title" });

        result.Should().NotBeNull();
        result!.Title.Should().Be("New Title");

        var dbItem = await _context.TodoItems.FindAsync(item.Id);
        dbItem!.Title.Should().Be("New Title");
    }

    [Fact]
    public async Task UpdateAsync_WhenItemDoesNotExist_ReturnsNull()
    {
        var result = await _sut.UpdateAsync(999, new UpdateTodoDto { Title = "Whatever" });

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_DoesNotChangeOtherFields()
    {
        var item = new TodoItem { Title = "Original", IsCompleted = true, CreatedAt = DateTime.UtcNow.AddDays(-1) };
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();

        var result = await _sut.UpdateAsync(item.Id, new UpdateTodoDto { Title = "Updated" });

        result!.IsCompleted.Should().BeTrue();
        result.CreatedAt.Should().Be(item.CreatedAt);
    }

    // --- CompleteAsync ---

    [Fact]
    public async Task CompleteAsync_WhenItemExists_SetsIsCompletedToTrue()
    {
        var item = new TodoItem { Title = "To complete", CreatedAt = DateTime.UtcNow };
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();

        var result = await _sut.CompleteAsync(item.Id);

        result.Should().NotBeNull();
        result!.IsCompleted.Should().BeTrue();

        var dbItem = await _context.TodoItems.FindAsync(item.Id);
        dbItem!.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteAsync_WhenItemDoesNotExist_ReturnsNull()
    {
        var result = await _sut.CompleteAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CompleteAsync_WhenAlreadyCompleted_RemainsCompleted()
    {
        var item = new TodoItem { Title = "Already done", IsCompleted = true, CreatedAt = DateTime.UtcNow };
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();

        var result = await _sut.CompleteAsync(item.Id);

        result!.IsCompleted.Should().BeTrue();
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_WhenItemExists_RemovesItemAndReturnsTrue()
    {
        var item = new TodoItem { Title = "To delete", CreatedAt = DateTime.UtcNow };
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();

        var result = await _sut.DeleteAsync(item.Id);

        result.Should().BeTrue();
        _context.TodoItems.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenItemDoesNotExist_ReturnsFalse()
    {
        var result = await _sut.DeleteAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_OnlyRemovesTargetItem()
    {
        var item1 = new TodoItem { Title = "Keep", CreatedAt = DateTime.UtcNow };
        var item2 = new TodoItem { Title = "Delete", CreatedAt = DateTime.UtcNow };
        _context.TodoItems.AddRange(item1, item2);
        await _context.SaveChangesAsync();

        await _sut.DeleteAsync(item2.Id);

        _context.TodoItems.Should().HaveCount(1);
        _context.TodoItems.First().Title.Should().Be("Keep");
    }

    // --- Mapping ---

    [Fact]
    public async Task MapToDto_MapsAllFieldsCorrectly()
    {
        var item = new TodoItem
        {
            Title = "Mapped",
            IsCompleted = true,
            CreatedAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc)
        };
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(item.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(item.Id);
        result.Title.Should().Be("Mapped");
        result.IsCompleted.Should().BeTrue();
        result.CreatedAt.Should().Be(new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc));
    }
}
