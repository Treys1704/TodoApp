using FluentAssertions;
using TodoApp.Api.Models;
using TodoApp.Api.Repositories;
using TodoApp.Api.Tests.Helpers;

namespace TodoApp.Api.Tests.Repositories;

public class TodoRepositoryTests : IDisposable
{
    private readonly Data.TodoContext _context;
    private readonly TodoRepository _sut;

    public TodoRepositoryTests()
    {
        _context = TestDbContextFactory.Create();
        _sut = new TodoRepository(_context);
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
            new TodoItem { Title = "Task 1", CreatedAt = DateTime.UtcNow },
            new TodoItem { Title = "Task 2", CreatedAt = DateTime.UtcNow },
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
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // --- AddAsync ---

    [Fact]
    public async Task AddAsync_PersistsItemInDatabase()
    {
        var item = new TodoItem { Title = "New Item", CreatedAt = DateTime.UtcNow };

        var result = await _sut.AddAsync(item);

        result.Id.Should().BeGreaterThan(0);
        _context.TodoItems.Should().HaveCount(1);
        _context.TodoItems.First().Title.Should().Be("New Item");
    }

    [Fact]
    public async Task AddAsync_ReturnsItemWithGeneratedId()
    {
        var item = new TodoItem { Title = "Check ID", CreatedAt = DateTime.UtcNow };

        var result = await _sut.AddAsync(item);

        result.Should().BeSameAs(item);
        result.Id.Should().BeGreaterThan(0);
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_PersistsChangesInDatabase()
    {
        var item = new TodoItem { Title = "Before", CreatedAt = DateTime.UtcNow };
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();

        item.Title = "After";
        await _sut.UpdateAsync(item);

        var dbItem = await _context.TodoItems.FindAsync(item.Id);
        dbItem!.Title.Should().Be("After");
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_RemovesItemFromDatabase()
    {
        var item = new TodoItem { Title = "To delete", CreatedAt = DateTime.UtcNow };
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();

        await _sut.DeleteAsync(item);

        _context.TodoItems.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_OnlyRemovesTargetItem()
    {
        var item1 = new TodoItem { Title = "Keep", CreatedAt = DateTime.UtcNow };
        var item2 = new TodoItem { Title = "Delete", CreatedAt = DateTime.UtcNow };
        _context.TodoItems.AddRange(item1, item2);
        await _context.SaveChangesAsync();

        await _sut.DeleteAsync(item2);

        _context.TodoItems.Should().HaveCount(1);
        _context.TodoItems.First().Title.Should().Be("Keep");
    }
}
