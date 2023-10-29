using System.Collections.Generic;
using CscGet.Audit.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CscGet.Audit.Persistence.NoSql.Initializers
{
    public class QuantitiesAuditRecordCollectionInitializer : IMongoCollectionInitializer<QuantitiesAuditRecord>
    {
        public IEnumerable<CreateIndexModel<QuantitiesAuditRecord>> CreateIndexes()
        {
            var options = new CreateIndexOptions();
            var indexDefinition = new IndexKeysDefinitionBuilder<QuantitiesAuditRecord>().Ascending(t => t.CostingVersionId).Ascending(t => t.ModificationDate);

            return new[]
            {
                new CreateIndexModel<QuantitiesAuditRecord>(indexDefinition, options)
            };
        }

        public GuidRepresentation GetGuidRepresentation()
        {
            return GuidRepresentation.Standard;
        }
    }
}
