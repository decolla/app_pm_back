using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.FichasServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using app_pm_back.Services.GenericServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace app_pm_back.Tests.Services;

public class ClassificationServiceTests
{
    private AppDbContext CreateDbContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new AppDbContext(options);
    }

    [Theory]
    [InlineData(null, "Txt.grp.cod.pr.", "nan")]
    [InlineData("", "Txt.code prob.", "nan")]
    [InlineData(null, "Texto grp.causa", "nan")]
    [InlineData("", "Txt.code causa", "nan")]
    [InlineData(null, "OutraColuna", "missing")]
    [InlineData("", "OutraColuna", "missing")]
    [InlineData("Manutenção Elétrica", "Qualquer", "manutencao eletrica")]
    [InlineData("AÇÇÃO Rápida", "Qualquer", "accao rapida")]
    public void NormalizeAsciiLower_ShouldNormalizeCorrectly(string? input, string columnName, string expected)
    {
        // Act
        var result = ClassificationService<NotaModel>.NormalizeAsciiLower(input, columnName);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task HumanClassification_ShouldUpdateProperties_WhenNotaExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var configService =
            new ConfiguracaoGlobalService(context, new Logger<ConfiguracaoGlobalService>(new LoggerFactory()));
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var catboostProvider = new CatboostModelProviderService(memoryCache, null!);
        var service = new ClassificationService<NotaModel>(context, configService, catboostProvider);

        var nota = new NotaModel
        {
            IdNota = 101,
            NumeroNota = 1,
            Classificacao = Classificacao.NAOCLASSIFICADA,
            ClassificadoPor = null,
            MetodoClassificacao = MetodoClassificacao.NAOCLASSIFICADA
        };
        context.PM_Notas.Add(nota);
        await context.SaveChangesAsync();

        // Act
        var result = await service.HumanClassification(101, Classificacao.CORRETIVA, "AdminUser");

        // Assert
        result.Should().NotBeNull();
        result!.Classificacao.Should().Be(Classificacao.CORRETIVA);
        result.ClassificadoPor.Should().Be("AdminUser");
        result.MetodoClassificacao.Should().Be(MetodoClassificacao.HUMANA);
        result.DataClassificacao.Should().NotBeNull();

        // Verify Database
        using var verifyContext = CreateDbContext(dbName);
        var dbNota = await verifyContext.PM_Notas.FindAsync(101L);
        dbNota.Should().NotBeNull();
        dbNota!.Classificacao.Should().Be(Classificacao.CORRETIVA);
        dbNota.ClassificadoPor.Should().Be("AdminUser");
        dbNota.MetodoClassificacao.Should().Be(MetodoClassificacao.HUMANA);
    }

    [Fact]
    public async Task HumanClassification_ShouldReturnNull_WhenNotaDoesNotExist()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var configService =
            new ConfiguracaoGlobalService(context, new Logger<ConfiguracaoGlobalService>(new LoggerFactory()));
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var catboostProvider = new CatboostModelProviderService(memoryCache, null!);
        var service = new ClassificationService<NotaModel>(context, configService, catboostProvider);

        // Act
        var result = await service.HumanClassification(999, Classificacao.CORRETIVA, "AdminUser");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task WordsClassification_ShouldProcessAndReturnInputList()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var configService =
            new ConfiguracaoGlobalService(context, new Logger<ConfiguracaoGlobalService>(new LoggerFactory()));
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var catboostProvider = new CatboostModelProviderService(memoryCache, null!);
        var service = new ClassificationService<NotaModel>(context, configService, catboostProvider);

        var config = await configService.ObterConfiguracaoAsync();
        config.ClassificacaoAutomatica = ModoClassificacaoAutomatica.ClassificarPalavrasChave;

        // Input list
        var inputList = new List<NotaModel>
        {
            new NotaModel { IdNota = 3, NumeroNota = 3, Classificacao = Classificacao.CORRETIVA }
        };

        // Act
        var result = await service.WordsClassification(inputList);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.Should().Contain(n => n.IdNota == 3);
    }

    [Fact]
    public async Task AutoClassifyWords_ShouldReturnNotesWithoutClassification()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var configService =
            new ConfiguracaoGlobalService(context, new Logger<ConfiguracaoGlobalService>(new LoggerFactory()));
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var catboostProvider = new CatboostModelProviderService(memoryCache, null!);
        var service = new ClassificationService<NotaModel>(context, configService, catboostProvider);

        var config = await configService.ObterConfiguracaoAsync();
        config.ClassificacaoAutomatica = ModoClassificacaoAutomatica.ClassificarPalavrasChave;

        // Save some notes without classification to DB
        context.PM_Notas.AddRange(
            new NotaModel { IdNota = 1, NumeroNota = 1, Classificacao = Classificacao.NAOCLASSIFICADA },
            new NotaModel { IdNota = 2, NumeroNota = 2, Classificacao = Classificacao.PREVENTIVA } // should be skipped
        );
        await context.SaveChangesAsync();

        // Act
        var result = await service.AutoClassifyWords();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.Should().Contain(n => n.IdNota == 1);
    }

    [Fact]
    public async Task WordsClassification_ShouldPersistDetachedNotes_WhenKeywordMatches()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var configService =
            new ConfiguracaoGlobalService(context, new Logger<ConfiguracaoGlobalService>(new LoggerFactory()));
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var catboostProvider = new CatboostModelProviderService(memoryCache, null!);
        var service = new ClassificationService<NotaModel>(context, configService, catboostProvider);

        context.Set<RegraClassificacaoModel>().Add(new RegraClassificacaoModel
        {
            IdRegra = 1,
            PalavraChave = "corretiva",
            ClassificacaoAplicada = Classificacao.CORRETIVA,
            Permitida = true,
            OrdemPrioridade = 1
        });

        context.PM_Notas.Add(new NotaModel
        {
            IdNota = 1001,
            NumeroNota = 1001,
            Classificacao = Classificacao.NAOCLASSIFICADA,
            MetodoClassificacao = MetodoClassificacao.NAOCLASSIFICADA,
            DescricaoBreve = "MANUTENÇÃO CORRETIVA - VALE"
        });
        await context.SaveChangesAsync();

        var detachedNotas = await context.PM_Notas.AsNoTracking().ToListAsync();

        // Act
        var result = await service.WordsClassification(detachedNotas);

        // Assert
        result.Should().ContainSingle();
        result[0].Classificacao.Should().Be(Classificacao.CORRETIVA);
        result[0].MetodoClassificacao.Should().Be(MetodoClassificacao.ALGORITMO);

        using var verifyContext = CreateDbContext(dbName);
        var dbNota = await verifyContext.PM_Notas.FindAsync(1001L);
        dbNota.Should().NotBeNull();
        dbNota!.Classificacao.Should().Be(Classificacao.CORRETIVA);
        dbNota.MetodoClassificacao.Should().Be(MetodoClassificacao.ALGORITMO);
    }
}