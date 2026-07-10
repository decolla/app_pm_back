using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace app_pm_back.Tests.Services;

public class GenericSearchServiceTests
{
    private AppDbContext CreateDbContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task BuscarPorCampoAsync_NonStringField_ShouldReturnCorrectResults()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        
        var nota1 = new NotaModel { IdNota = 12, NumeroNota = 12, Classificacao = Classificacao.NAOCLASSIFICADA };
        var nota2 = new NotaModel { IdNota = 13, NumeroNota = 13, Classificacao = Classificacao.NAOCLASSIFICADA };
        
        context.PM_Notas.AddRange(nota1, nota2);
        await context.SaveChangesAsync();

        var configService = new ConfiguracaoGlobalService(context);
        var service = new GenericSearchService(context, configService);

        // Act
        var result = await service.BuscarPorCampoAsync("PM_Notas", "IdNota", "12", parcial: true, pagina: 1, tamanho: 1);

        // Assert
        result.Should().NotBeNull();
        
        // Use reflection to inspect the anonymous object returned
        var totalProp = result.GetType().GetProperty("Total");
        var resultProp = result.GetType().GetProperty("Result");

        totalProp.Should().NotBeNull();
        resultProp.Should().NotBeNull();

        var total = (int)totalProp!.GetValue(result)!;
        var resultList = (System.Collections.IEnumerable)resultProp!.GetValue(result)!;

        total.Should().Be(1);
        
        var list = new List<object>();
        foreach (var item in resultList)
        {
            list.Add(item);
        }
        
        list.Count.Should().Be(1);
        var returnedNota = list[0] as NotaModel;
        returnedNota.Should().NotBeNull();
        returnedNota!.IdNota.Should().Be(12);
    }

    [Fact]
    public async Task BuscarPorCampoAsync_StringFieldExactMatch_ShouldReturnCorrectResults()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        
        var nota1 = new NotaModel { IdNota = 1, NumeroNota = 1, DescricaoBreve = "TestNota1", Classificacao = Classificacao.NAOCLASSIFICADA };
        var nota2 = new NotaModel { IdNota = 2, NumeroNota = 2, DescricaoBreve = "OtherNota", Classificacao = Classificacao.NAOCLASSIFICADA };
        
        context.PM_Notas.AddRange(nota1, nota2);
        await context.SaveChangesAsync();

        var configService = new ConfiguracaoGlobalService(context);
        var service = new GenericSearchService(context, configService);

        // Act - Exact match (parcial: false) which avoids Npgsql ILike in memory
        var result = await service.BuscarPorCampoAsync("PM_Notas", "DescricaoBreve", "TestNota1", parcial: false, pagina: 1, tamanho: 10);

        // Assert
        result.Should().NotBeNull();
        
        var totalProp = result.GetType().GetProperty("Total");
        var resultProp = result.GetType().GetProperty("Result");

        var total = (int)totalProp!.GetValue(result)!;
        var resultList = (System.Collections.IEnumerable)resultProp!.GetValue(result)!;

        total.Should().Be(1);
        
        var list = new List<object>();
        foreach (var item in resultList)
        {
            list.Add(item);
        }
        
        list.Count.Should().Be(1);
        var returnedNota = list[0] as NotaModel;
        returnedNota.Should().NotBeNull();
        returnedNota!.DescricaoBreve.Should().Be("TestNota1");
    }

    [Fact]
    public async Task BuscarPorCampoAsync_InvalidTable_ShouldThrowArgumentException()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var configService = new ConfiguracaoGlobalService(context);
        var service = new GenericSearchService(context, configService);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            service.BuscarPorCampoAsync("InvalidTableName", "IdNota", "12", parcial: true, pagina: 1, tamanho: 1)
        );
    }

    [Fact]
    public async Task BuscarPorCampoAsync_InvalidField_ShouldThrowArgumentException()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var configService = new ConfiguracaoGlobalService(context);
        var service = new GenericSearchService(context, configService);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            service.BuscarPorCampoAsync("PM_Notas", "InvalidFieldName", "12", parcial: true, pagina: 1, tamanho: 1)
        );
    }
}
