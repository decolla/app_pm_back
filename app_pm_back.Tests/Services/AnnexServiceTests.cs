using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.AspNetCore.Http;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services;

namespace app_pm_back.Tests.Services;

public class AnnexServiceTests
{
    private AppDbContext CreateDbContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task UploadAnexo_ShouldThrowArgumentException_WhenBothNotaAndOrdemAreProvided()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new AnnexService(context);

        var fileMock = new Mock<IFormFile>();

        // Act
        var act = () => service.UploadAnexo(fileMock.Object, 1L, 2L, "tipoRef", CategoriaAnexo.Nota);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Informe apenas NotaId ou OrdemId para o anexo.");
    }

    [Fact]
    public async Task UploadAnexo_ShouldThrowArgumentException_WhenNeitherNotaNorOrdemAreProvided()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new AnnexService(context);

        var fileMock = new Mock<IFormFile>();

        // Act
        var act = () => service.UploadAnexo(fileMock.Object, null, null, "tipoRef", CategoriaAnexo.Nota);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Informe apenas NotaId ou OrdemId para o anexo.");
    }

    [Fact]
    public async Task UploadAnexo_ShouldSaveAnexoNota_WhenNotaIdIsProvided()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new AnnexService(context);

        var fileMock = new Mock<IFormFile>();
        var content = "Hello World";
        var fileName = "test.txt";
        var contentType = "text/plain";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, token) => ms.CopyTo(stream))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.UploadAnexo(fileMock.Object, 123L, null, "tipoRef", CategoriaAnexo.Nota);

        // Assert
        result.Should().NotBeNull();
        
        // Dynamic/anonymous object checks
        var resultType = result.GetType();
        var idProp = resultType.GetProperty("IdAnexo");
        var nameProp = resultType.GetProperty("Nome");
        
        idProp.Should().NotBeNull();
        nameProp.Should().NotBeNull();

        var idVal = (long)idProp!.GetValue(result)!;
        var nameVal = (string)nameProp!.GetValue(result)!;

        nameVal.Should().Be(fileName);

        // Verify Database
        using var verifyContext = CreateDbContext(dbName);
        var dbAnexo = await verifyContext.PM_AnexosNota.FirstOrDefaultAsync(x => x.IdAnexoNota == idVal);
        dbAnexo.Should().NotBeNull();
        dbAnexo!.Nome.Should().Be(fileName);
        dbAnexo.TipoArquivo.Should().Be(contentType);
        dbAnexo.IdNota.Should().Be(123L);
        dbAnexo.Dados.Should().NotBeNull();
        System.Text.Encoding.UTF8.GetString(dbAnexo.Dados!).Should().Be(content);
    }

    [Fact]
    public async Task UploadAnexo_ShouldSaveAnexoOrdem_WhenOrdemIdIsProvided()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new AnnexService(context);

        var fileMock = new Mock<IFormFile>();
        var content = "Hello Ordem";
        var fileName = "ordem.txt";
        var contentType = "text/plain";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, token) => ms.CopyTo(stream))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.UploadAnexo(fileMock.Object, null, 456L, "tipoRef", CategoriaAnexo.Ordem);

        // Assert
        result.Should().NotBeNull();
        
        var resultType = result.GetType();
        var idProp = resultType.GetProperty("IdAnexo");
        var nameProp = resultType.GetProperty("Nome");
        
        idProp.Should().NotBeNull();
        nameProp.Should().NotBeNull();

        var idVal = (long)idProp!.GetValue(result)!;
        var nameVal = (string)nameProp!.GetValue(result)!;

        nameVal.Should().Be(fileName);

        // Verify Database
        using var verifyContext = CreateDbContext(dbName);
        var dbAnexo = await verifyContext.PM_AnexosOrdem.FirstOrDefaultAsync(x => x.IdAnexoOrdem == idVal);
        dbAnexo.Should().NotBeNull();
        dbAnexo!.Nome.Should().Be(fileName);
        dbAnexo.TipoArquivo.Should().Be(contentType);
        dbAnexo.IdOrdem.Should().Be(456L);
        dbAnexo.TipoAnexo.Should().Be(CategoriaAnexo.Ordem.ToString());
        dbAnexo.Dados.Should().NotBeNull();
        System.Text.Encoding.UTF8.GetString(dbAnexo.Dados!).Should().Be(content);
    }

    [Fact]
    public async Task DownloadAnexo_ShouldReturnAnexoNota_WhenExistsInPM_AnexosNota()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new AnnexService(context);

        var anexo = new AnexoNotaModel
        {
            IdAnexoNota = 1L,
            Nome = "test_nota.txt",
            TipoArquivo = "text/plain",
            Dados = System.Text.Encoding.UTF8.GetBytes("Data Nota"),
            IdNota = 100L
        };
        context.PM_AnexosNota.Add(anexo);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DownloadAnexo(1L);

        // Assert
        result.Should().NotBeNull();
        result!.Value.Nome.Should().Be("test_nota.txt");
        result!.Value.MimeType.Should().Be("text/plain");
        System.Text.Encoding.UTF8.GetString(result.Value.Dados).Should().Be("Data Nota");
    }

    [Fact]
    public async Task DownloadAnexo_ShouldReturnAnexoOrdem_WhenExistsInPM_AnexosOrdem()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new AnnexService(context);

        var anexo = new AnexoOrdemModel
        {
            IdAnexoOrdem = 2L,
            Nome = "test_ordem.txt",
            TipoArquivo = "text/plain",
            Dados = System.Text.Encoding.UTF8.GetBytes("Data Ordem"),
            IdOrdem = 200L
        };
        context.PM_AnexosOrdem.Add(anexo);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DownloadAnexo(2L);

        // Assert
        result.Should().NotBeNull();
        result!.Value.Nome.Should().Be("test_ordem.txt");
        result!.Value.MimeType.Should().Be("text/plain");
        System.Text.Encoding.UTF8.GetString(result.Value.Dados).Should().Be("Data Ordem");
    }

    [Fact]
    public async Task DownloadAnexo_ShouldReturnNull_WhenDoesNotExist()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new AnnexService(context);

        // Act
        var result = await service.DownloadAnexo(999L);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarAnexo_ShouldReturnNotaAnexos_WhenNotaIdIsProvided()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new AnnexService(context);

        context.PM_AnexosNota.AddRange(
            new AnexoNotaModel { IdAnexoNota = 10L, Nome = "file1.txt", TipoArquivo = "text/plain", IdNota = 11L },
            new AnexoNotaModel { IdAnexoNota = 11L, Nome = "file2.txt", TipoArquivo = "text/plain", IdNota = 11L },
            new AnexoNotaModel { IdAnexoNota = 12L, Nome = "file3.txt", TipoArquivo = "text/plain", IdNota = 22L }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await service.BuscarAnexo(11L, null, "tipoRef");

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Should().ContainSingle(a => a.IdAnexo == 10L && a.Nome == "file1.txt");
        result.Should().ContainSingle(a => a.IdAnexo == 11L && a.Nome == "file2.txt");
    }

    [Fact]
    public async Task BuscarAnexo_ShouldReturnOrdemAnexos_WhenOrdemIdIsProvided()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateDbContext(dbName);
        var service = new AnnexService(context);

        context.PM_AnexosOrdem.AddRange(
            new AnexoOrdemModel { IdAnexoOrdem = 30L, Nome = "file1.txt", TipoArquivo = "text/plain", IdOrdem = 33L },
            new AnexoOrdemModel { IdAnexoOrdem = 31L, Nome = "file2.txt", TipoArquivo = "text/plain", IdOrdem = 33L },
            new AnexoOrdemModel { IdAnexoOrdem = 32L, Nome = "file3.txt", TipoArquivo = "text/plain", IdOrdem = 44L }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await service.BuscarAnexo(null, 33L, "tipoRef");

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Should().ContainSingle(a => a.IdAnexo == 30L && a.Nome == "file1.txt");
        result.Should().ContainSingle(a => a.IdAnexo == 31L && a.Nome == "file2.txt");
    }
}
