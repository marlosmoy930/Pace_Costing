using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CscGet.Audit.Domain.Models.Enums;
using CscGet.Costing.Domain.Dispatcher;
using CscGet.Costing.Domain.Dispatcher.Models.Request;

namespace CscGet.Audit.Application.Services.External
{
    public class DataSourceTypeService : IDataSourceTypeService
    {
        private readonly IDispatcher _dispatcher;

        public DataSourceTypeService(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task<IReadOnlyCollection<Guid>> FilterOutEntityIdsWithSctDataSource(int bidId, IReadOnlyCollection<Guid> entityIds)
        {
            var requestModel = new RequestDataSourceForEntities(bidId, entityIds);

            var response = await _dispatcher.CallAsync(requestModel).ConfigureAwait(false);

            var nonSctDataSources = response.DataSourceTypeModels
                .Where(x => x.DataSourceTypeId == (int)DataSourceType.SCT || x.DataSourceTypeId == (int)DataSourceType.SCTNSD);

            return nonSctDataSources.Select(x => x.EntityId).ToList();
        }
    }
}
