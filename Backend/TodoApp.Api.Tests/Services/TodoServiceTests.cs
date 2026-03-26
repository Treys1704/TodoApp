using FluentAssertions;
using Moq;
using TodoApp.Api.Models;
using TodoApp.Api.Models.Dtos;
using TodoApp.Api.Repositories;
using TodoApp.Api.Services;

namespace TodoApp.Api.Tests.Services;

public class TodoServiceTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly TodoService _sut;

    public TodoServiceTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _sut = new TodoService(_repositoryMock.Object);
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_WhenNoItems_ReturnsEmptyList()
    {
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TodoItem>());

        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenItemsExist_ReturnsAllMappedDtos()
    {
        var items = new List<TodoItem>
        {
            new() { Id = 1, Title = "Task 1", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Title = "Task 2", CreatedAt = DateTime.UtcNow },
            new() { Id = 3, Title = "Task 3", CreatedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_CallsRepositoryOnce()
    {
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TodoItem>());

        await _sut.GetAllAsync();

        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsMappedDto()
    {
        var item = new TodoItem { Id = 1, Title = "Test", IsCompleted = false, CreatedAt = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Test");
        result.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TodoItem?)null);

        var result = await _sut.GetByIdAsync(999);

        result.Should().BeNull();
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_CallsRepositoryAddAndReturnsMappedDto()
    {
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync((TodoItem item) => { item.Id = 1; return item; });

        var result = await _sut.CreateAsync(new CreateTodoDto { Title = "New Task" });

        result.Should().NotBeNull();
        result.Title.Should().Be("New Task");
        result.IsCompleted.Should().BeFalse();
        result.Id.Should().Be(1);

        _repositoryMock.Verify(r => r.AddAsync(It.Is<TodoItem>(i => i.Title == "New Task")), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAtToUtcNow()
    {
        var before = DateTime.UtcNow;

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync((TodoItem item) => item);

        var result = await _sut.CreateAsync(new CreateTodoDto { Title = "Timed" });

        var after = DateTime.UtcNow;
        result.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public async Task CreateAsync_SetsIsCompletedToFalse()
    {
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync((TodoItem item) => item);

        var result = await _sut.CreateAsync(new CreateTodoDto { Title = "Fresh" });

        result.IsCompleted.Should().BeFalse();
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_WhenItemExists_UpdatesTitleAndReturnsDto()
    {
        var item = new TodoItem { Id = 1, Title = "Old Title", CreatedAt = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _sut.UpdateAsync(1, new UpdateTodoDto { Title = "New Title" });

        result.Should().NotBeNull();
        result!.Title.Should().Be("New Title");
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<TodoItem>(i => i.Title == "New Title")), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenItemDoesNotExist_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TodoItem?)null);

        var result = await _sut.UpdateAsync(999, new UpdateTodoDto { Title = "Whatever" });

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_DoesNotChangeOtherFields()
    {
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var item = new TodoItem { Id = 1, Title = "Original", IsCompleted = true, CreatedAt = createdAt };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _sut.UpdateAsync(1, new UpdateTodoDto { Title = "Updated" });

        result!.IsCompleted.Should().BeTrue();
        result.CreatedAt.Should().Be(createdAt);
    }

    // --- CompleteAsync ---

    [Fact]
    public async Task CompleteAsync_WhenItemExists_SetsIsCompletedToTrue()
    {
        var item = new TodoItem { Id = 1, Title = "To complete", IsCompleted = false, CreatedAt = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _sut.CompleteAsync(1);

        result.Should().NotBeNull();
        result!.IsCompleted.Should().BeTrue();
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<TodoItem>(i => i.IsCompleted)), Times.Once);
    }

    [Fact]
    public async Task CompleteAsync_WhenItemDoesNotExist_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TodoItem?)null);

        var result = await _sut.CompleteAsync(999);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task CompleteAsync_WhenAlreadyCompleted_RemainsCompleted()
    {
        var item = new TodoItem { Id = 1, Title = "Already done", IsCompleted = true, CreatedAt = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _sut.CompleteAsync(1);

        result!.IsCompleted.Should().BeTrue();
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_WhenItemExists_CallsRepositoryDeleteAndReturnsTrue()
    {
        var item = new TodoItem { Id = 1, Title = "To delete", CreatedAt = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

        var result = await _sut.DeleteAsync(1);

        result.Should().BeTrue();
        _repositoryMock.Verify(r => r.DeleteAsync(item), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemDoesNotExist_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TodoItem?)null);

        var result = await _sut.DeleteAsync(999);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    // --- Mapping ---

    [Fact]
    public async Task MapToDto_MapsAllFieldsCorrectly()
    {
        var createdAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var item = new TodoItem { Id = 42, Title = "Mapped", IsCompleted = true, CreatedAt = createdAt };
        _repositoryMock.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(item);

        var result = await _sut.GetByIdAsync(42);

        result.Should().NotBeNull();
        result!.Id.Should().Be(42);
        result.Title.Should().Be("Mapped");
        result.IsCompleted.Should().BeTrue();
        result.CreatedAt.Should().Be(createdAt);
    }
}
