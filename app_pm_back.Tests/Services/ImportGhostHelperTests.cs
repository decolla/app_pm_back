using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services;

namespace app_pm_back.Tests.Services;

public class ImportGhostHelperTests
{
    private AppDbContext CreateDbContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task EnsureUsuariosSapAsync_ShouldCreateMissingUsers()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        // Pre-populate one user
        context.PM_Usuarios.Add(new UsuarioModel { IdUsuario = 10, Nome = "Existing User", CPF = "123.456.789-00", Permissoes = PermissoesTabela.Tudo });
        await context.SaveChangesAsync();

        var idsToEnsure = new List<long?> { 10L, 20L, 30L, null };

        // Act
        await ImportGhostHelper.EnsureUsuariosSapAsync(context, idsToEnsure);

        // Assert
        using var verifyContext = CreateDbContext(dbName);
        var users = await verifyContext.PM_Usuarios.ToListAsync();

        users.Count.Should().Be(3); // 10 (existing), 20 (new), 30 (new)
        users.Should().Contain(u => u.IdUsuario == 10 && u.Nome == "Existing User");
        users.Should().Contain(u => u.IdUsuario == 20 && u.Nome == "IMPORTADO_20");
        users.Should().Contain(u => u.IdUsuario == 30 && u.Nome == "IMPORTADO_30");
    }

    [Fact]
    public async Task EnsurePlanosAsync_ShouldCreateMissingPlanos_AndCreateDefaultEstrategiaIfMissing()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var planosToEnsure = new List<long?> { 100L, 200L, null };

        // Act
        await ImportGhostHelper.EnsurePlanosAsync(context, planosToEnsure);

        // Assert
        using var verifyContext = CreateDbContext(dbName);
        var planos = await verifyContext.PM_Planos.ToListAsync();
        var estrategias = await verifyContext.PM_Estrategias.ToListAsync();

        estrategias.Count.Should().Be(1);
        estrategias[0].Nome.Should().Be("DEFAULT");

        planos.Count.Should().Be(2);
        planos.Should().Contain(p => p.IdPlano == 100L && p.Nome == "PLANO_100" && p.IdEstrategia == estrategias[0].IdEstrategia);
        planos.Should().Contain(p => p.IdPlano == 200L && p.Nome == "PLANO_200" && p.IdEstrategia == estrategias[0].IdEstrategia);
    }

    [Fact]
    public async Task EnsureNotasAsync_ShouldCreateMissingNotas()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        // Pre-populate one nota
        context.PM_Notas.Add(new NotaModel { IdNota = 500L, NumeroNota = 500L, DataNota = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var notasToEnsure = new List<long?> { 500L, 600L, null };

        // Act
        await ImportGhostHelper.EnsureNotasAsync(context, notasToEnsure);

        // Assert
        using var verifyContext = CreateDbContext(dbName);
        var notas = await verifyContext.PM_Notas.ToListAsync();

        notas.Count.Should().Be(2);
        notas.Should().Contain(n => n.IdNota == 500L);
        notas.Should().Contain(n => n.IdNota == 600L && n.NumeroNota == 600L);
    }

    [Fact]
    public async Task EnsureGpmAsync_ShouldCreateMissingGpms_AndCreateDefaultCpmIfMissing()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var gpmsToEnsure = new List<long?> { 5L, 10L, null };

        // Act
        await ImportGhostHelper.EnsureGpmAsync(context, gpmsToEnsure);

        // Assert
        using var verifyContext = CreateDbContext(dbName);
        var gpms = await verifyContext.PM_Gpms.ToListAsync();
        var cpms = await verifyContext.PM_Cpms.ToListAsync();

        cpms.Count.Should().Be(1);
        cpms[0].NomeCentro.Should().Be("DEFAULT");

        gpms.Count.Should().Be(2);
        gpms.Should().Contain(g => g.IdGPM == 5 && g.Nome == "GPM_5" && g.IdCPM == cpms[0].IdCPM);
        gpms.Should().Contain(g => g.IdGPM == 10 && g.Nome == "GPM_10" && g.IdCPM == cpms[0].IdCPM);
    }

    [Fact]
    public async Task EnsureImobilizadosAsync_ShouldCreateMissingImobilizados()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var imobToEnsure = new List<long?> { 77L, 88L, null };

        // Act
        await ImportGhostHelper.EnsureImobilizadosAsync(context, imobToEnsure);

        // Assert
        using var verifyContext = CreateDbContext(dbName);
        var imobs = await verifyContext.PM_Imobilizados.ToListAsync();

        imobs.Count.Should().Be(2);
        imobs.Should().Contain(i => i.IdImobilizado == 77 && i.Nome == "IMOB_77" && i.Denominacao == "IMPORTADO");
        imobs.Should().Contain(i => i.IdImobilizado == 88 && i.Nome == "IMOB_88" && i.Denominacao == "IMPORTADO");
    }
}
