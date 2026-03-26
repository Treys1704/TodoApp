using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApp.Api.Controllers;
using TodoApp.Api.Models.Dtos;
using TodoApp.Api.Services;

namespace TodoApp.Api.Tests.Controllers;

public class TodoControllerTests
{
    private readonly Mock<ITodoService> _serviceMock;
    private readonly TodoController _sut;

    public TodoControllerTests()
    {
        _serviceMock = new Mock<ITodoService>();
        _sut = new TodoController(_serviceMock.Object);
    }

    // --- GetAll ---

    [Fact]
    public async Task GetAll_ReturnsOkWithItems()
    {
        var items = new List<TodoItemDto>
        {
            new() { Id = 1, Title = "A" },
            new() { Id = 2, Title = "B" }
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(items);

        var result = await _sut.GetAll();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(items);
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ReturnsOkWithEmptyList()
    {
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<TodoItemDto>());

        var result = await _sut.GetAll();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = ok.Value as IEnumerable<TodoItemDto>;
        value.Should().BeEmpty();
    }

    // --- GetById ---

    [Fact]
    public async Task GetById_WhenItemExists_ReturnsOkWithItem()
    {
        var dto = new TodoItemDto { Id = 1, Title = "Test" };
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);

        var result = await _sut.GetById(1);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetById_WhenItemDoesNotExist_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((TodoItemDto?)null);

        var result = await _sut.GetById(999);

        result.Should().BeOfType<NotFoundResult>();
    }

    // --- Create ---

    [Fact]
    public async Task Create_WithValidDto_ReturnsCreatedAtAction()
    {
        var createDto = new CreateTodoDto { Title = "New" };
        var created = new TodoItemDto { Id = 1, Title = "New" };
        _serviceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(created);

        var result = await _sut.Create(createDto);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(TodoController.GetById));
        createdResult.RouteValues!["id"].Should().Be(1);
        createdResult.Value.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task Create_WithInvalidModelState_ReturnsBadRequest()
    {
        _sut.ModelState.AddModelError("Title", "The Title field is required.");

        var result = await _sut.Create(new CreateTodoDto());

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_CallsServiceCreateAsync()
    {
        var dto = new CreateTodoDto { Title = "Check call" };
        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(new TodoItemDto { Id = 1, Title = "Check call" });

        await _sut.Create(dto);

        _serviceMock.Verify(s => s.CreateAsync(dto), Times.Once);
    }

    // --- Update ---

    [Fact]
    public async Task Update_WhenItemExists_ReturnsOkWithUpdatedItem()
    {
        var updateDto = new UpdateTodoDto { Title = "Updated" };
        var updated = new TodoItemDto { Id = 1, Title = "Updated" };
        _serviceMock.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(updated);

        var result = await _sut.Update(1, updateDto);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(updated);
    }

    [Fact]
    public async Task Update_WhenItemDoesNotExist_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.UpdateAsync(999, It.IsAny<UpdateTodoDto>()))
            .ReturnsAsync((TodoItemDto?)null);

        var result = await _sut.Update(999, new UpdateTodoDto { Title = "X" });

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_WithInvalidModelState_ReturnsBadRequest()
    {
        _sut.ModelState.AddModelError("Title", "The Title field is required.");

        var result = await _sut.Update(1, new UpdateTodoDto());

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // --- Complete ---

    [Fact]
    public async Task Complete_WhenItemExists_ReturnsOkWithCompletedItem()
    {
        var completed = new TodoItemDto { Id = 1, Title = "Done", IsCompleted = true };
        _serviceMock.Setup(s => s.CompleteAsync(1)).ReturnsAsync(completed);

        var result = await _sut.Complete(1);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = ok.Value as TodoItemDto;
        value!.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task Complete_WhenItemDoesNotExist_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.CompleteAsync(999)).ReturnsAsync((TodoItemDto?)null);

        var result = await _sut.Complete(999);

        result.Should().BeOfType<NotFoundResult>();
    }

    // --- Delete ---

    [Fact]
    public async Task Delete_WhenItemExists_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _sut.Delete(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_WhenItemDoesNotExist_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);

        var result = await _sut.Delete(999);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_CallsServiceDeleteAsync()
    {
        _serviceMock.Setup(s => s.DeleteAsync(5)).ReturnsAsync(true);

        await _sut.Delete(5);

        _serviceMock.Verify(s => s.DeleteAsync(5), Times.Once);
    }
}
