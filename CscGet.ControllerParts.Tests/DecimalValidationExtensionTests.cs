using System.Collections.Generic;
using CscGet.RestConventions;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace CscGet.ControllerParts.Tests
{
    public class DecimalValidationExtensionTests
    {
        public static IEnumerable<object[]> GetInvalidDecimals()
        {
            yield return new object[] { 1234567890123456789m };
            yield return new object[] { 1234567890123456789.123m };
            yield return new object[] { 1234567890123456789.123456789m };
            yield return new object[] { 1234567890123456789.123456712m };
            yield return new object[] { 1234567890123456789.123456789m };
            yield return new object[] { 12.123456789m };
        }

        [Theory]
        [MemberData("GetInvalidDecimals")]
        public void ValidateDecimalPrecisionIsFalseTest(decimal value)
        {
            // Arrange
            var validator = new TestClassModelValidator();
            var result = validator.Validate(new TestClassModel { Property1 = value, Property2 = 2 });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.Contain(x => x.PropertyName == "Property1" && x.ErrorMessage == string.Format(ValidatorConstants.FieldPrecisionLowerThanOrEqualNumbers, DecimalValidationExtension.Precision));
        }

        public static IEnumerable<object[]> GetValidDecimals()
        {
            yield return new object[] { 1234567890123456m };
            yield return new object[] { 1234567890123.123m };
            yield return new object[] { 1234567.1234567m };
            yield return new object[] { 123456789.1234567m };
        }

        [Theory]
        [MemberData("GetValidDecimals")]
        public void ValidateDecimalPrecisionTrueTest(decimal value)
        {
            // Arrange
            var validator = new TestClassModelValidator();
            var result = validator.Validate(new TestClassModel { Property1 = value, Property2 = 2 });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(-3, 2)]
        public void ValidateDecimalGreaterOrEqualZeroFalseTest(decimal value1, decimal value2)
        {
            // Arrange
            var validator = new TestClassModelValidator();
            var result = validator.Validate(new TestClassModel { Property1 = value1, Property2 = value2 });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.Contain(x => x.PropertyName == "Property1" && x.ErrorMessage == ValidatorConstants.FieldGreaterThanOrEqualZero);
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(4, 2)]
        public void ValidateDecimalTrueTest(decimal value1, decimal value2)
        {
            // Arrange
            var validator = new TestClassModelValidator();
            var result = validator.Validate(new TestClassModel { Property1 = value1, Property2 = value2 });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 0)]
        public void ValidateDecimalGreaterThanZeroFalseTest(decimal value1, decimal value2)
        {
            // Arrange
            var validator = new TestClassModelValidator();
            var result = validator.Validate(new TestClassModel { Property1 = value1, Property2 = value2 });

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.Contain(x => x.PropertyName == "Property2" && x.ErrorMessage == ValidatorConstants.FieldGreaterThanZero);
        }

        public class TestClassModel
        {
            public decimal Property1 { get; set; }

            public decimal Property2 { get; set; }
        }

        public class TestClassModelValidator : AbstractValidator<TestClassModel>
        {
            public TestClassModelValidator()
            {
                RuleFor(x => x.Property1).AddDecimalGreaterThanOrEqualToZeroRule();

                RuleFor(x => x.Property2).AddDecimalGreaterThanZeroRule();
            }
        }
    }
}