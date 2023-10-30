using CscGet.Audit.Domain.Models;

namespace CscGet.Audit.Persistence.NoSql.Mappings
{
    public static class MongoDbClassMapRegistrator
    {
        private static readonly IMongoDbClassMap[] ClassMaps =
        {
            new AuditRecordClassMap<Quantity[]>(), 
            new AuditRecordClassMap<PyramidMixValue[]>(), 
            new AuditRecordClassMap<string>(), 
            new QuantitiesAuditRecordClassMap(),
            new PyramidMixAuditRecordClassMap(),
            new QuantityClassMap(),
            new DateClassMap(),
            new PyramidMixValueClassMap(),
            new GlobalTotalEntityAuditRecordClassMap()
        };

        public static void Register()
        {
            foreach (var mongoDbClassMap in ClassMaps)
            {
                mongoDbClassMap.Map();
            }
        }
    }
}
