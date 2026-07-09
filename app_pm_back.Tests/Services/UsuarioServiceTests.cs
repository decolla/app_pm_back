using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;

namespace app_pm_back.Tests.Services;

public class UsuarioServiceTests
{
    private AppDbContext CreateDbContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddUsuarioToDatabase()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new UsuarioService(context);

        var usuario = new UsuarioModel
        {
            IdUsuario = 1L,
            Nome = "João Silva",
            CPF = "123.456.789-00",
            Permissoes = PermissoesTabela.Tudo
        };

        // Act
        var result = await service.CreateAsync(usuario);

        // Assert
        result.Should().NotBeNull();
        result.IdUsuario.Should().Be(1L);
        result.Nome.Should().Be("João Silva");

        // Verify database state
        using var verifyContext = CreateDbContext(dbName);
        var dbUsuario = await verifyContext.PM_Usuarios.FindAsync(1L);
        dbUsuario.Should().NotBeNull();
        dbUsuario!.Nome.Should().Be("João Silva");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUsuario_WhenExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new UsuarioService(context);

        var usuario = new UsuarioModel { IdUsuario = 10L, Nome = "Maria Souza" };
        context.PM_Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        // Act
        // GetByIdAsync comes from BaseCrudService<T>
        var result = await service.GetByIdAsync(10L);

        // Assert
        result.Should().NotBeNull();
        result!.Nome.Should().Be("Maria Souza");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUsuarioAndReturnTrue()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new UsuarioService(context);

        var usuario = new UsuarioModel { IdUsuario = 5L, Nome = "Carlos Oliveira" };
        context.PM_Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        // Act
        var deleted = await service.DeleteAsync(5L);

        // Assert
        deleted.Should().BeTrue();

        using var verifyContext = CreateDbContext(dbName);
        var dbUsuario = await verifyContext.PM_Usuarios.FindAsync(5L);
        dbUsuario.Should().BeNull();
    }

    [Fact]
    public async Task CreateRangeAsync_ShouldAddMultipleUsuarios()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new UsuarioService(context);

        var list = new List<UsuarioModel>
        {
            new UsuarioModel { IdUsuario = 101L, Nome = "User A" },
            new UsuarioModel { IdUsuario = 102L, Nome = "User B" }
        };

        // Act
        await service.CreateRangeAsync(list);

        // Assert
        using var verifyContext = CreateDbContext(dbName);
        var users = await verifyContext.PM_Usuarios.ToListAsync();
        users.Count.Should().Be(2);
        users.Should().Contain(u => u.IdUsuario == 101L);
        users.Should().Contain(u => u.IdUsuario == 102L);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingUsuario()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new UsuarioService(context);

        var usuario = new UsuarioModel { IdUsuario = 20L, Nome = "Old Name" };
        context.PM_Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        usuario.Nome = "New Name";

        // Act
        var result = await service.UpdateAsync(usuario);

        // Assert
        result.Should().NotBeNull();
        result.Nome.Should().Be("New Name");

        using var verifyContext = CreateDbContext(dbName);
        var dbUsuario = await verifyContext.PM_Usuarios.FindAsync(20L);
        dbUsuario!.Nome.Should().Be("New Name");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsuarios()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new UsuarioService(context);

        context.PM_Usuarios.AddRange(
            new UsuarioModel { IdUsuario = 301L, Nome = "User X" },
            new UsuarioModel { IdUsuario = 302L, Nome = "User Y" }
        );
        await context.SaveChangesAsync();

        // Act
        var list = await service.GetAllAsync();

        // Assert
        list.Should().NotBeNull();
        list.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedResults()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new UsuarioService(context);

        context.PM_Usuarios.AddRange(
            new UsuarioModel { IdUsuario = 401L, Nome = "User 1" },
            new UsuarioModel { IdUsuario = 402L, Nome = "User 2" },
            new UsuarioModel { IdUsuario = 403L, Nome = "User 3" }
        );
        await context.SaveChangesAsync();

        // Act
        object result = await service.GetPagedAsync(2, 2);

        // Assert
        result.Should().NotBeNull();
        int total = (int)result.GetType().GetProperty("Total")!.GetValue(result)!;
        var list = (List<UsuarioModel>)result.GetType().GetProperty("Result")!.GetValue(result)!;

        total.Should().Be(3);
        list.Count.Should().Be(1);
        list[0].IdUsuario.Should().Be(403L);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnTotalCount()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new UsuarioService(context);

        context.PM_Usuarios.AddRange(
            new UsuarioModel { IdUsuario = 501L, Nome = "User X" },
            new UsuarioModel { IdUsuario = 502L, Nome = "User Y" }
        );
        await context.SaveChangesAsync();

        // Act
        var count = await service.CountAsync();

        // Assert
        count.Should().Be(2);
    }
}
