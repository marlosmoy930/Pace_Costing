using FluentAssertions;
using Xunit;

namespace CscGet.ControllerParts.Tests.XssSanitizer
{
    public class XssSanitizerTests
    {
        [Theory]
        [InlineData("<script>alert('hello');</script>", "")]
        [InlineData("left text<script>alert('hello');</script>", "left text")]
        [InlineData("<script>alert('hello');</script>right text", "right text")]
        [InlineData("text left<script>alert('hello');</script>text right", "text lefttext right")]
        public void ExploitSanitizerTest(string input, string result)
        {
            // Arrange
            var model = new SanitizerTestModel(input);

            //Act
            ControllerParts.XssSanitizer.Sanitize(model);

            // Assert
            model.StringProperty.Should().Be(result);
        }

        [Theory]
        [InlineData("left<a>Hello</a>", "left")]
        [InlineData("<b>Hello</b>right", "right")]
        [InlineData("nonclosed<b>Hello", "nonclosed")]
        [InlineData("<i>inside</i>", "")]
        public void HtmlSanitizerTest(string input, string result)
        {
            // Arrange
            var model = new SanitizerTestModel(input);

            //Act
            ControllerParts.XssSanitizer.Sanitize(model);

            // Assert
            model.StringProperty.Should().Be(result);
        }
    }
}
