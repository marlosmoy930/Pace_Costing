using System.Collections.Generic;
using CscGet.Audit.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CscGet.Audit.Persistence.NoSql.Initializers
{
    public class ConvertToCustomAuditRecordCollectionInitializer : IMongoCollectionInitializer<ConvertToCustomAuditRecord>
    {
        public IEnumerable<CreateIndexModel<ConvertToCustomAuditRecord>> CreateIndexes()
        {
            var options = new CreateIndexOptions();
            var costingVersionIndexDefinition = new IndexKeysDefinitionBuilder<ConvertToCustomAuditRecord>().Ascending(t => t.CostingVersionId).Ascending(t => t.ModificationDate);

            return new[]
            {
                new CreateIndexModel<ConvertToCustomAuditRecord>(costingVersionIndexDefinition, options)
            };
        }

        public GuidRepresentation GetGuidRepresentation() => GuidRepresentation.Standard;
    }
}
