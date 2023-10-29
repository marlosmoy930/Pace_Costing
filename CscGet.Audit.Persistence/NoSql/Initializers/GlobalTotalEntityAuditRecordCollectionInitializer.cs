using System.Collections.Generic;
using CscGet.Audit.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CscGet.Audit.Persistence.NoSql.Initializers
{
    public class GlobalTotalEntityAuditRecordCollectionInitializer : IMongoCollectionInitializer<GlobalTotalEntityAuditRecord>
    {
        public IEnumerable<CreateIndexModel<GlobalTotalEntityAuditRecord>> CreateIndexes()
        {
            var options = new CreateIndexOptions();
            var costingVersionIndexDefinition = new IndexKeysDefinitionBuilder<GlobalTotalEntityAuditRecord>().Ascending(t => t.CostingVersionId).Ascending(t => t.ModificationDate);;

            return new[]
            {
                new CreateIndexModel<GlobalTotalEntityAuditRecord>(costingVersionIndexDefinition, options)
            };
        }

        public GuidRepresentation GetGuidRepresentation() => GuidRepresentation.Standard;
    }
}
