using Xunit;
using FluentAssertions;
using app_pm_back.Services;

namespace app_pm_back.Tests.Services;

public class ImportMergeHelperTests
{
    private class TestModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Email { get; set; }
    }

    [Fact]
    public void MergeNonEmpty_ShouldMergeValidProperties_WhenSourceHasValues()
    {
        // Arrange
        var target = new TestModel { Id = 1, Name = "Original Name", Age = 30, Email = "original@example.com" };
        var source = new TestModel { Id = 2, Name = "New Name", Age = 35, Email = null }; // Email is null, should not overwrite target. Id should be ignored because it is not checked or is key? Wait, KeyAttribute is checked, but TestModel.Id does not have KeyAttribute. However, it is a value type. Default value for int is 0. Here Id = 2 is not 0, so it will merge if not ignored or key. Let's test basic behavior.

        // Act
        ImportMergeHelper.MergeNonEmpty(target, source, nameof(TestModel.Id));

        // Assert
        target.Name.Should().Be("New Name");
        target.Age.Should().Be(35);
        target.Email.Should().Be("original@example.com"); // Kept original because source was null
        target.Id.Should().Be(1); // Ignored property
    }

    [Fact]
    public void MergeNonEmpty_ShouldNotOverwrite_WhenSourceHasDefaultOrEmptyValues()
    {
        // Arrange
        var target = new TestModel { Id = 1, Name = "Original Name", Age = 30, Email = "original@example.com" };
        var source = new TestModel { Id = 0, Name = " ", Age = null, Email = "" };

        // Act
        ImportMergeHelper.MergeNonEmpty(target, source);

        // Assert
        target.Name.Should().Be("Original Name"); // Ignored whitespace string
        target.Age.Should().Be(30); // Ignored null
        target.Email.Should().Be("original@example.com"); // Ignored empty string
        target.Id.Should().Be(1); // Ignored default value (0)
    }
}
