using System.Collections.Generic;
using CscGet.Audit.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CscGet.Audit.Persistence.NoSql.Initializers
{
    public class PyramidMixAuditRecordCollectionInitializer : IMongoCollectionInitializer<PyramidMixAuditRecord>
    {
        public IEnumerable<CreateIndexModel<PyramidMixAuditRecord>> CreateIndexes()
        {
            var options = new CreateIndexOptions();
            var costingVersionIndexDefinition = new IndexKeysDefinitionBuilder<PyramidMixAuditRecord>().Ascending(t => t.CostingVersionId).Ascending(t => t.ModificationDate);
            var costGroupIdIndexDefinition = new IndexKeysDefinitionBuilder<PyramidMixAuditRecord>().Ascending(t => t.CostGroupId);

            return new[]
            {
                new CreateIndexModel<PyramidMixAuditRecord>(costingVersionIndexDefinition, options),
                new CreateIndexModel<PyramidMixAuditRecord>(costGroupIdIndexDefinition, options) 
            };
        }

        public GuidRepresentation GetGuidRepresentation()
        {
            return GuidRepresentation.Standard;
        }
    }
}
