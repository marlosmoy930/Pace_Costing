namespace CscGet.ControllerParts.Tests.XssSanitizer
{
    public class SanitizerTestModel
    {
        public string StringProperty { get; set; }

        public SanitizerTestModel(string stringProperty)
        {
            StringProperty = stringProperty;
        }
    }
}