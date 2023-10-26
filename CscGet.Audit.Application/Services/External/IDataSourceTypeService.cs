using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CscGet.Audit.Domain.Models.Enums;

namespace CscGet.Audit.Application.Services.External
{
    public interface IDataSourceTypeService
    {
        Task<IReadOnlyCollection<Guid>> FilterOutEntityIdsWithSctDataSource(int bidId, IReadOnlyCollection<Guid> entityIds);
    }
}
