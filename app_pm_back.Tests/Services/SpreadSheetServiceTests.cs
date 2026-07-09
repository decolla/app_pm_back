using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Http;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.FichasServices;
using app_pm_back.Services.Manutencao;
using app_pm_back.Services.ExternIntegationsServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace app_pm_back.Tests.Services;

public class SpreadSheetServiceTests
{
    private AppDbContext CreateDbContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new AppDbContext(options);
    }

    private (Mock<IConfiguration> mockConfig, Mock<ILogger<DadosRenovacaoService>> mockLogger, HttpClient httpClient) CreateDadosRenovacaoDeps()
    {
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["APIs:APM"]).Returns("http://fake-apm");
        var mockLogger = new Mock<ILogger<DadosRenovacaoService>>();
        var httpClient = new HttpClient();
        return (mockConfig, mockLogger, httpClient);
    }

    [Fact]
    public async Task ProcessarPlanilhaAsync_ShouldThrowArgumentException_WhenDbSchemaIsCg()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", "cg");
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        
        var mockSpreadsheetService = new Mock<SpreadsheetService>(null!, null!);
        var mockNotaService = new Mock<NotaManutencaoService>(context);
        var mockOrdemService = new Mock<OrdemService>(context);
        var mockItemNotaService = new Mock<ItemNotaService>(context);
        
        var (mockConfig, mockLogger, httpClient) = CreateDadosRenovacaoDeps();
        var mockDadosRenovacaoService = new Mock<DadosRenovacaoService>(context, mockLogger.Object, mockConfig.Object, httpClient);
        var mockSyncApmService = new Mock<SynchronizeAPMService>(context, null!, null!);

        var service = new ImportSheetService(
            context,
            mockSpreadsheetService.Object,
            mockNotaService.Object,
            mockOrdemService.Object,
            mockItemNotaService.Object,
            mockDadosRenovacaoService.Object,
            mockSyncApmService.Object
        );

        var fileMock = new Mock<IFormFile>();

        // Act
        var act = () => service.ProcessarPlanilhaAsync(fileMock.Object, false, true);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("PEDRO ADVERTE: Importação de planilhas desabilitada para o schema CG.");
            
        // Reset env
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", null);
    }

    [Fact]
    public async Task ProcessarPlanilhaAsync_ShouldThrowArgumentException_WhenTipoIsNotaAndNoNotesFound()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", "other");
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var mockSpreadsheetService = new Mock<SpreadsheetService>(null!, null!);
        mockSpreadsheetService.Setup(s => s.ReadNotasSpreadSheetAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpreadsheetService.ResultadoPlanilha
            {
                TipoIdentificado = "NOTA",
                Notas = new List<NotaModel>()
            });

        var mockNotaService = new Mock<NotaManutencaoService>(context);
        var mockOrdemService = new Mock<OrdemService>(context);
        var mockItemNotaService = new Mock<ItemNotaService>(context);

        var (mockConfig, mockLogger, httpClient) = CreateDadosRenovacaoDeps();
        var mockDadosRenovacaoService = new Mock<DadosRenovacaoService>(context, mockLogger.Object, mockConfig.Object, httpClient);
        var mockSyncApmService = new Mock<SynchronizeAPMService>(context, null!, null!);

        var service = new ImportSheetService(
            context,
            mockSpreadsheetService.Object,
            mockNotaService.Object,
            mockOrdemService.Object,
            mockItemNotaService.Object,
            mockDadosRenovacaoService.Object,
            mockSyncApmService.Object
        );

        var fileMock = new Mock<IFormFile>();

        // Act
        var act = () => service.ProcessarPlanilhaAsync(fileMock.Object, false, true);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Planilha reconhecida, mas sem dados válidos.");
            
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", null);
    }

    [Fact]
    public async Task ProcessarPlanilhaAsync_ShouldImportNotes_WhenTipoIsNotaAndNotesFound()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", "other");
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var notas = new List<NotaModel>
        {
            new NotaModel { IdNota = 1, NumeroNota = 1001, DescricaoBreve = "Nota 1" },
            new NotaModel { IdNota = 2, NumeroNota = 1001, DescricaoBreve = "Nota 1 Dup" }, // Duplicate NumeroNota
            new NotaModel { IdNota = 3, NumeroNota = 1002, DescricaoBreve = "Nota 2" }
        };

        var mockSpreadsheetService = new Mock<SpreadsheetService>(null!, null!);
        mockSpreadsheetService.Setup(s => s.ReadNotasSpreadSheetAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpreadsheetService.ResultadoPlanilha
            {
                TipoIdentificado = "NOTA",
                Notas = notas
            });

        var mockNotaService = new Mock<NotaManutencaoService>(context);
        mockNotaService.Setup(s => s.ImportarNotasEmLoteSeguroAsync(It.IsAny<List<NotaModel>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mockOrdemService = new Mock<OrdemService>(context);
        var mockItemNotaService = new Mock<ItemNotaService>(context);
        
        var (mockConfig, mockLogger, httpClient) = CreateDadosRenovacaoDeps();
        var mockDadosRenovacaoService = new Mock<DadosRenovacaoService>(context, mockLogger.Object, mockConfig.Object, httpClient);
        var mockSyncApmService = new Mock<SynchronizeAPMService>(context, null!, null!);

        var service = new ImportSheetService(
            context,
            mockSpreadsheetService.Object,
            mockNotaService.Object,
            mockOrdemService.Object,
            mockItemNotaService.Object,
            mockDadosRenovacaoService.Object,
            mockSyncApmService.Object
        );

        var fileMock = new Mock<IFormFile>();

        // Act
        var result = await service.ProcessarPlanilhaAsync(fileMock.Object, true, false);

        // Assert
        result.Should().Be(2); // Deduplicated count
        mockNotaService.Verify(s => s.ImportarNotasEmLoteSeguroAsync(
            It.Is<List<NotaModel>>(list => list.Count == 2 && list.Any(n => n.IdNota == 1) && list.Any(n => n.IdNota == 3)),
            true,
            false,
            It.IsAny<CancellationToken>()
        ), Times.Once);

        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", null);
    }

    [Fact]
    public async Task ProcessarPlanilhaAsync_ShouldThrowArgumentException_WhenTipoIsOrdemAndNoOrdersFound()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", "other");
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var mockSpreadsheetService = new Mock<SpreadsheetService>(null!, null!);
        mockSpreadsheetService.Setup(s => s.ReadNotasSpreadSheetAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpreadsheetService.ResultadoPlanilha
            {
                TipoIdentificado = "ORDEM",
                Ordens = new List<OrdemModel>()
            });

        var mockNotaService = new Mock<NotaManutencaoService>(context);
        var mockOrdemService = new Mock<OrdemService>(context);
        var mockItemNotaService = new Mock<ItemNotaService>(context);
        
        var (mockConfig, mockLogger, httpClient) = CreateDadosRenovacaoDeps();
        var mockDadosRenovacaoService = new Mock<DadosRenovacaoService>(context, mockLogger.Object, mockConfig.Object, httpClient);
        var mockSyncApmService = new Mock<SynchronizeAPMService>(context, null!, null!);

        var service = new ImportSheetService(
            context,
            mockSpreadsheetService.Object,
            mockNotaService.Object,
            mockOrdemService.Object,
            mockItemNotaService.Object,
            mockDadosRenovacaoService.Object,
            mockSyncApmService.Object
        );

        var fileMock = new Mock<IFormFile>();

        // Act
        var act = () => service.ProcessarPlanilhaAsync(fileMock.Object, false, true);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Planilha de ordens reconhecida, mas sem dados válidos.");

        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", null);
    }

    [Fact]
    public async Task ProcessarPlanilhaAsync_ShouldImportOrders_WhenTipoIsOrdemAndOrdersFound()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", "other");
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var ordens = new List<OrdemModel>
        {
            new OrdemModel { IdOrdem = 1, NumeroOrdem = 5001, DescricaoBreve = "Ordem 1", DataCriacao = DateTime.Now },
            new OrdemModel { IdOrdem = 2, NumeroOrdem = 5001, DescricaoBreve = "Ordem 1 Dup", DataCriacao = DateTime.Now },
            new OrdemModel { IdOrdem = 3, NumeroOrdem = 5002, DescricaoBreve = "Ordem 2", DataCriacao = DateTime.Now }
        };

        var mockSpreadsheetService = new Mock<SpreadsheetService>(null!, null!);
        mockSpreadsheetService.Setup(s => s.ReadNotasSpreadSheetAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpreadsheetService.ResultadoPlanilha
            {
                TipoIdentificado = "ORDEM",
                Ordens = ordens
            });

        var mockNotaService = new Mock<NotaManutencaoService>(context);
        var mockOrdemService = new Mock<OrdemService>(context);
        mockOrdemService.Setup(s => s.ImportarOrdensEmLoteSeguroAsync(It.IsAny<List<OrdemModel>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mockItemNotaService = new Mock<ItemNotaService>(context);
        
        var (mockConfig, mockLogger, httpClient) = CreateDadosRenovacaoDeps();
        var mockDadosRenovacaoService = new Mock<DadosRenovacaoService>(context, mockLogger.Object, mockConfig.Object, httpClient);
        mockDadosRenovacaoService.Setup(s => s.ImportarDadosOrdemAsync(It.IsAny<List<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var mockSyncApmService = new Mock<SynchronizeAPMService>(context, null!, null!);
        mockSyncApmService.Setup(s => s.SynchronizeAPMAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new ImportSheetService(
            context,
            mockSpreadsheetService.Object,
            mockNotaService.Object,
            mockOrdemService.Object,
            mockItemNotaService.Object,
            mockDadosRenovacaoService.Object,
            mockSyncApmService.Object
        );

        var fileMock = new Mock<IFormFile>();

        // Act
        var result = await service.ProcessarPlanilhaAsync(fileMock.Object, true, false);

        // Assert
        result.Should().Be(2); // Deduplicated count
        mockOrdemService.Verify(s => s.ImportarOrdensEmLoteSeguroAsync(
            It.Is<List<OrdemModel>>(list => list.Count == 2),
            true,
            false,
            It.IsAny<CancellationToken>()
        ), Times.Once);
        mockSyncApmService.Verify(s => s.SynchronizeAPMAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockDadosRenovacaoService.Verify(s => s.ImportarDadosOrdemAsync(It.Is<List<long>>(l => l.Count == 2), It.IsAny<CancellationToken>()), Times.Once);

        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", null);
    }

    [Fact]
    public async Task ProcessarPlanilhaAsync_ShouldThrowArgumentException_WhenTipoIsItemAndNoItemsFound()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", "other");
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var mockSpreadsheetService = new Mock<SpreadsheetService>(null!, null!);
        mockSpreadsheetService.Setup(s => s.ReadNotasSpreadSheetAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpreadsheetService.ResultadoPlanilha
            {
                TipoIdentificado = "ITEM",
                Itens = new List<ItemNotaModel>()
            });

        var mockNotaService = new Mock<NotaManutencaoService>(context);
        var mockOrdemService = new Mock<OrdemService>(context);
        var mockItemNotaService = new Mock<ItemNotaService>(context);
        
        var (mockConfig, mockLogger, httpClient) = CreateDadosRenovacaoDeps();
        var mockDadosRenovacaoService = new Mock<DadosRenovacaoService>(context, mockLogger.Object, mockConfig.Object, httpClient);
        var mockSyncApmService = new Mock<SynchronizeAPMService>(context, null!, null!);

        var service = new ImportSheetService(
            context,
            mockSpreadsheetService.Object,
            mockNotaService.Object,
            mockOrdemService.Object,
            mockItemNotaService.Object,
            mockDadosRenovacaoService.Object,
            mockSyncApmService.Object
        );

        var fileMock = new Mock<IFormFile>();

        // Act
        var act = () => service.ProcessarPlanilhaAsync(fileMock.Object, false, true);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Planilha de itens reconhecida, mas sem dados válidos.");

        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", null);
    }

    [Fact]
    public async Task ProcessarPlanilhaAsync_ShouldImportItems_WhenTipoIsItemAndItemsFoundAndNotaExists()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", "other");
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        // Seed a nota in database with NumeroNota = 1001 (SAP Number)
        var existingNota = new NotaModel { IdNota = 50, NumeroNota = 1001, DescricaoBreve = "Existing Nota" };
        context.PM_Notas.Add(existingNota);
        await context.SaveChangesAsync();

        var itens = new List<ItemNotaModel>
        {
            new ItemNotaModel { IdItemNota = 1, IdNota = 1001, Causa = "Item 1" }
        };

        var mockSpreadsheetService = new Mock<SpreadsheetService>(null!, null!);
        mockSpreadsheetService.Setup(s => s.ReadNotasSpreadSheetAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpreadsheetService.ResultadoPlanilha
            {
                TipoIdentificado = "ITEM",
                Itens = itens
            });

        var mockNotaService = new Mock<NotaManutencaoService>(context);
        var mockOrdemService = new Mock<OrdemService>(context);
        
        var mockItemNotaService = new Mock<ItemNotaService>(context);
        mockItemNotaService.Setup(s => s.ImportarItensEmLoteSeguroAsync(It.IsAny<List<ItemNotaModel>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var (mockConfig, mockLogger, httpClient) = CreateDadosRenovacaoDeps();
        var mockDadosRenovacaoService = new Mock<DadosRenovacaoService>(context, mockLogger.Object, mockConfig.Object, httpClient);
        var mockSyncApmService = new Mock<SynchronizeAPMService>(context, null!, null!);

        var service = new ImportSheetService(
            context,
            mockSpreadsheetService.Object,
            mockNotaService.Object,
            mockOrdemService.Object,
            mockItemNotaService.Object,
            mockDadosRenovacaoService.Object,
            mockSyncApmService.Object
        );

        var fileMock = new Mock<IFormFile>();

        // Act
        var result = await service.ProcessarPlanilhaAsync(fileMock.Object, false, true);

        // Assert
        result.Should().Be(1);
        
        // The service should map the item's IdNota to the database internal Id (50)
        itens.First().IdNota.Should().Be(50);
        mockItemNotaService.Verify(s => s.ImportarItensEmLoteSeguroAsync(
            It.Is<List<ItemNotaModel>>(list => list.Count == 1 && list.First().IdNota == 50),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", null);
    }

    [Fact]
    public async Task ProcessarPlanilhaAsync_ShouldThrowArgumentException_WhenTipoIsDesconhecido()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", "other");
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);

        var mockSpreadsheetService = new Mock<SpreadsheetService>(null!, null!);
        mockSpreadsheetService.Setup(s => s.ReadNotasSpreadSheetAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpreadsheetService.ResultadoPlanilha
            {
                TipoIdentificado = "DESCONHECIDO"
            });

        var mockNotaService = new Mock<NotaManutencaoService>(context);
        var mockOrdemService = new Mock<OrdemService>(context);
        var mockItemNotaService = new Mock<ItemNotaService>(context);
        
        var (mockConfig, mockLogger, httpClient) = CreateDadosRenovacaoDeps();
        var mockDadosRenovacaoService = new Mock<DadosRenovacaoService>(context, mockLogger.Object, mockConfig.Object, httpClient);
        var mockSyncApmService = new Mock<SynchronizeAPMService>(context, null!, null!);

        var service = new ImportSheetService(
            context,
            mockSpreadsheetService.Object,
            mockNotaService.Object,
            mockOrdemService.Object,
            mockItemNotaService.Object,
            mockDadosRenovacaoService.Object,
            mockSyncApmService.Object
        );

        var fileMock = new Mock<IFormFile>();

        // Act
        var act = () => service.ProcessarPlanilhaAsync(fileMock.Object, false, true);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Formato de planilha não reconhecido. Verifique os cabeçalhos.");

        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", null);
    }
}
