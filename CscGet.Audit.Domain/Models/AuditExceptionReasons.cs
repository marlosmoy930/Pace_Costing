namespace CscGet.Audit.Domain.Models
{
    public static class AuditExceptionReasons
    {
        public const string QuantityTabChanged = "Quantity tab changed";

        public const string PyramidMixChanged = "Pyramid Mix changed";

        public const string EntityInvolvedInGtRenamed = "Entity involved in GT renamed";

        public const string CrossDependencyBroken = "Converted to Custom (Cross Dependency broken by User)";

        public const string ChildDependencyBroken = "Converted to Custom (Child Dependency broken by User)";

        public const string ConvertedToCustomByUser = "Converted to custom by User";

        public const string CrossDependencyRuleBrokenAndChildDependencyRuleBroken = "Converted to Custom (Cross and Child Dependency broken by User)";

        public const string ConvertedToCustomWhileShaping = "Converted to Custom (Broken dependencies during Shaping)";

        public const string ConvertedToCustomWhileCopyingOrMoving = "Converted to Custom (Broken dependencies during Copy/Move)";
    }
}
