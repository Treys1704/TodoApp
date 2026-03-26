using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;

namespace TodoApp.Api.Tests.Helpers;

public static class TestDbContextFactory
{
    public static TodoContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new TodoContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}
