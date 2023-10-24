using System;
using System.Collections.Generic;
using System.Linq;
using CscGet.Audit.Application.Services;
using CscGet.Audit.Application.Services.External;
using CscGet.Audit.Domain.Models;
using CscGet.Audit.Domain.Models.Enums;
using CscGet.Audit.Persistence.NoSql.Repositories;
using CscGet.Costing.Domain.Core;
using Dxc.Captn.Costing.Contracts.Quantity;
using NSubstitute;
using Xunit;

namespace CscGet.Audit.Application.Tests.Services
{
    public class QuantityAuditServiceTests
    {
        [Fact]
        public async void ShouldCreateNotEmptyCollectionOfNewRecords_WhenQuantitiesChangedEventComes()
        {
            // Arrange

            var quantitiesBeforeChange = Enumerable.Range(1, 12).Select(i =>
                    new QuantityModel
                    {
                        Value = 10,
                        Date = new QuantityDate { Year = 2019, Month = (short)i },
                        Formula = ""
                    })
                .ToList();

            var quantitiesAfterChange = Enumerable.Range(1, 12).Select(i =>
                    new QuantityModel
                    {
                        Value = 5,
                        Date = new QuantityDate { Year = 2019, Month = (short)i },
                        Formula = ""
                    })
                .ToList();

            var changedEntity = new QuantityContainerModel
            {
                EntityId = Guid.NewGuid(),
                GroupTypeCode = GroupType.SWI.ToString(),
                QuantitiesBeforeChange = quantitiesBeforeChange,
                QuantitiesAfterChange = quantitiesAfterChange

            };

            var quantitiesChangedEvent = new QuantitiesChangedEvent
            {
                CostingVersionId = 1,
                UserId = Guid.NewGuid(),
                Username = "Test user",
                EventDate = DateTime.Now,
                ChangedEntities = new List<QuantityContainerModel> { changedEntity }

            };

            var auditRecordRepository = Substitute.For<IAuditRecordRepository<QuantitiesAuditRecord, Quantity[]>>();
            auditRecordRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>()).Returns(new List<QuantitiesAuditRecord>());

            var auditRecordService = Substitute.For<ICommonAuditRecordService<QuantitiesAuditRecord, Quantity[]>>();

            var eligibleIds = quantitiesChangedEvent.ChangedEntities.Select(e => e.EntityId).ToList();
            var dataSourceTypeService = Substitute.For<IDataSourceTypeService>();
            dataSourceTypeService.FilterOutEntityIdsWithSctDataSource(Arg.Any<int>(), Arg.Any<IReadOnlyCollection<Guid>>()).Returns(eligibleIds);

            var quantityAuditService = new QuantityAuditService(auditRecordRepository, auditRecordService, dataSourceTypeService);

            // Act

            await quantityAuditService.CreateOrUpdateExistingRecord(quantitiesChangedEvent);

            // Assert

            auditRecordRepository.Received().AddAsync(Arg.Is<IReadOnlyCollection<QuantitiesAuditRecord>>(x => x.Count > 0));
        }
    }
}