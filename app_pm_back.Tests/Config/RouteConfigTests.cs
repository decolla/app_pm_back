using System;
using System.Collections.Generic;
using System.Security;
using app_pm_back.Config;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace app_pm_back.Tests.Config;

public class RouteConfigTests : IDisposable
{
    public RouteConfigTests()
    {
        // Clear environment variables before each test
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", null);
        Environment.SetEnvironmentVariable("SCHEMA", null);
    }

    public void Dispose()
    {
        // Clean up environment variables after each test
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", null);
        Environment.SetEnvironmentVariable("SCHEMA", null);
    }

    [Fact]
    public void GetDatabaseSchema_ShouldReturnNull_WhenNoEnvIsSet()
    {
        // Act
        var result = RouteConfig.GetDatabaseSchema();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("cg/apm_back", "cg")]
    [InlineData("/cg/reliability_back", "cg")]
    [InlineData("my_schema", "my_schema")]
    public void GetDatabaseSchema_ShouldExtractSchemaPart(string rawValue, string expectedSchema)
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", rawValue);

        // Act
        var result = RouteConfig.GetDatabaseSchema();

        // Assert
        result.Should().Be(expectedSchema);
    }

    [Theory]
    [InlineData("cg/apm_back", "apm_back")]
    [InlineData("/cg/reliability_back", "reliability_back")]
    [InlineData("my_schema", null)]
    public void GetServiceName_ShouldExtractServicePart(string rawValue, string? expectedService)
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", rawValue);

        // Act
        var result = RouteConfig.GetServiceName();

        // Assert
        result.Should().Be(expectedService);
    }

    [Fact]
    public void GetApmBaseUrl_ShouldUseApmConfig_WhenNoEnvIsSet()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?> {
            {"APIs:APM", "http://fake-apm"}
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var result = RouteConfig.GetApmBaseUrl(configuration);

        // Assert
        result.Should().Be("http://fake-apm");
    }

    [Fact]
    public void GetApmBaseUrl_ShouldUseReliabilityConfig_WhenEnvSpecifiesReliability()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", "cg/reliability_back");
        var inMemorySettings = new Dictionary<string, string?> {
            {"APIs:APM", "http://fake-apm"},
            {"APIs:Reliability", "http://fake-reliability"}
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var result = RouteConfig.GetApmBaseUrl(configuration);

        // Assert
        result.Should().Be("http://fake-reliability");
    }

    [Fact]
    public void GetReliabilityBaseUrl_ShouldUseApmConfig_WhenEnvSpecifiesApm()
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", "cg/apm_back");
        var inMemorySettings = new Dictionary<string, string?> {
            {"APIs:APM", "http://fake-apm"},
            {"APIs:Reliability", "http://fake-reliability"}
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var result = RouteConfig.GetReliabilityBaseUrl(configuration);

        // Assert
        result.Should().Be("http://fake-apm");
    }

    [Theory]
    [InlineData("cg/../path")]
    [InlineData("cg;drop table pm;/apm_back")]
    [InlineData("cg/apm;drop;")]
    public void GetDatabaseSchema_Or_GetServiceName_ShouldThrowSecurityException_WhenInputIsInsecure(string insecureInput)
    {
        // Arrange
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", insecureInput);

        // Act & Assert
        Action actSchema = () => RouteConfig.GetDatabaseSchema();
        Action actService = () => RouteConfig.GetServiceName();

        Assert.ThrowsAny<SecurityException>(() => {
            var s = RouteConfig.GetDatabaseSchema();
            var serv = RouteConfig.GetServiceName();
        });
    }
}
