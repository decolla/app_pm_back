using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using app_pm_back.Controllers;
using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace app_pm_back.Tests.Controllers;

public class AnexosControllerTests
{
    [Fact]
    public async Task UploadArquivo_ShouldReturnBadRequest_WhenFileIsNull()
    {
        // Arrange
        var mockAnnexService = new Mock<AnnexService>(null!);
        var controller = new AnexosController(mockAnnexService.Object);
        var request = new UploadAnnexRequestDto
        {
            Arquivo = null!,
            NotaId = 1
        };

        // Act
        var result = await controller.UploadArquivo(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().NotBeNull();
        
        var valType = badRequestResult.Value!.GetType();
        var msgProp = valType.GetProperty("message");
        msgProp.Should().NotBeNull();
        msgProp!.GetValue(badRequestResult.Value).Should().Be("Arquivo invalido para upload.");
    }

    [Fact]
    public async Task UploadArquivo_ShouldReturnBadRequest_WhenBothNotaAndOrdemAreProvided()
    {
        // Arrange
        var mockAnnexService = new Mock<AnnexService>(null!);
        var controller = new AnexosController(mockAnnexService.Object);
        
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);

        var request = new UploadAnnexRequestDto
        {
            Arquivo = fileMock.Object,
            NotaId = 1,
            OrdemId = 2
        };

        // Act
        var result = await controller.UploadArquivo(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var valType = badRequestResult.Value!.GetType();
        var msgProp = valType.GetProperty("message");
        msgProp!.GetValue(badRequestResult.Value).Should().Be("Informe apenas NotaId ou OrdemId.");
    }

    [Fact]
    public async Task UploadArquivo_ShouldReturnOk_WhenValidUpload()
    {
        // Arrange
        var mockAnnexService = new Mock<AnnexService>(null!);
        
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        fileMock.Setup(f => f.FileName).Returns("test.png");

        dynamic expandoResult = new System.Dynamic.ExpandoObject();
        expandoResult.IdAnexo = 123L;
        expandoResult.Nome = "test.png";

        mockAnnexService.Setup(s => s.UploadAnexo(
            It.IsAny<IFormFile>(),
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<string>(),
            It.IsAny<CategoriaAnexo>()
        )).ReturnsAsync((object)expandoResult);

        var controller = new AnexosController(mockAnnexService.Object);
        var request = new UploadAnnexRequestDto
        {
            Arquivo = fileMock.Object,
            NotaId = 1,
            TipoReferencia = "Nota",
            Categoria = CategoriaAnexo.Nota
        };

        // Act
        var result = await controller.UploadArquivo(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var valType = okResult.Value!.GetType();
        var msgProp = valType.GetProperty("message");
        msgProp!.GetValue(okResult.Value).Should().Be("Arquivo enviado com sucesso 123");
    }

    [Fact]
    public async Task DownloadArquivo_ShouldReturnBadRequest_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var mockAnnexService = new Mock<AnnexService>(null!);
        var controller = new AnexosController(mockAnnexService.Object);

        // Act
        var result = await controller.DownloadArquivo(0);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var valType = badRequestResult.Value!.GetType();
        var msgProp = valType.GetProperty("message");
        msgProp!.GetValue(badRequestResult.Value).Should().Be("Id do anexo invalido.");
    }

    [Fact]
    public async Task DownloadArquivo_ShouldReturnNotFound_WhenAnexoDoesNotExist()
    {
        // Arrange
        var mockAnnexService = new Mock<AnnexService>(null!);
        mockAnnexService.Setup(s => s.DownloadAnexo(It.IsAny<long>()))
            .ReturnsAsync(((byte[] Dados, string MimeType, string Nome)?)null);

        var controller = new AnexosController(mockAnnexService.Object);

        // Act
        var result = await controller.DownloadArquivo(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DownloadArquivo_ShouldReturnFileResult_WhenAnexoExists()
    {
        // Arrange
        var mockAnnexService = new Mock<AnnexService>(null!);
        byte[] fileBytes = new byte[] { 1, 2, 3 };
        mockAnnexService.Setup(s => s.DownloadAnexo(123))
            .ReturnsAsync((fileBytes, "image/png", "test.png"));

        var controller = new AnexosController(mockAnnexService.Object);

        // Act
        var result = await controller.DownloadArquivo(123);

        // Assert
        var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
        fileResult.FileContents.Should().BeEquivalentTo(fileBytes);
        fileResult.ContentType.Should().Be("image/png");
        fileResult.FileDownloadName.Should().Be("test.png");
    }

    [Fact]
    public async Task BuscarArquivo_ShouldReturnBadRequest_WhenReferenciaIdIsInvalid()
    {
        // Arrange
        var mockAnnexService = new Mock<AnnexService>(null!);
        var controller = new AnexosController(mockAnnexService.Object);

        // Act
        var result = await controller.BuscarArquivo(0, "Nota");

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var valType = badRequestResult.Value!.GetType();
        var msgProp = valType.GetProperty("message");
        msgProp!.GetValue(badRequestResult.Value).Should().Be("ReferenciaId e obrigatorio.");
    }

    [Fact]
    public async Task BuscarArquivo_ShouldReturnBadRequest_WhenTipoReferenciaIsInvalid()
    {
        // Arrange
        var mockAnnexService = new Mock<AnnexService>(null!);
        var controller = new AnexosController(mockAnnexService.Object);

        // Act
        var result = await controller.BuscarArquivo(10, "InvalidRefType");

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var valType = badRequestResult.Value!.GetType();
        var msgProp = valType.GetProperty("message");
        msgProp!.GetValue(badRequestResult.Value).Should().Be("TipoReferencia invalido.");
    }

    [Fact]
    public async Task BuscarArquivo_ShouldReturnOk_WhenParametersAreValid()
    {
        // Arrange
        var mockAnnexService = new Mock<AnnexService>(null!);
        var list = new List<(long, string)> { (1L, "file1.txt"), (2L, "file2.txt") };
        mockAnnexService.Setup(s => s.BuscarAnexo(It.IsAny<long?>(), It.IsAny<long?>(), It.IsAny<string>()))
            .ReturnsAsync(list);

        var controller = new AnexosController(mockAnnexService.Object);

        // Act
        var result = await controller.BuscarArquivo(10, "Nota");

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(list);
    }
}
