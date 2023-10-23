namespace Csc.Get.Reporting.ExternalIntegration.Models
{
    public class FilterData
    {
        public int Level { get; set; }
        public bool? IsSelected { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool AllowedInAllocations { get; set; }
    }
}
