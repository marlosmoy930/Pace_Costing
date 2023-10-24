using CscGet.Audit.Application.Factories;
using CscGet.Audit.Application.Handlers;
using CscGet.Audit.Application.Jwt;
using CscGet.Audit.Application.Services;
using CscGet.Audit.Application.Services.External;
using CscGet.Audit.Domain.Models;
using CscGet.Audit.Persistence.NoSql;
using CscGet.Audit.Persistence.NoSql.Initializers;
using CscGet.Audit.Persistence.NoSql.Mappings;
using CscGet.Audit.Persistence.NoSql.Repositories;
using CscGet.Costing.Domain.Dispatcher.Events.BaselineMetric;
using CscGet.Costing.Domain.Dispatcher.Events.CostGroup;
using CscGet.Costing.Domain.Dispatcher.Events.Element;
using CscGet.Costing.Domain.Dispatcher.Events.Element.Copy;
using CscGet.Costing.Domain.Dispatcher.Events.GlobalTotals;
using CscGet.Costing.Domain.Dispatcher.Events.LaborRates;
using CscGet.Costing.Domain.Dispatcher.Events.Wbs.BaselineMetrics;
using CscGet.Costing.Domain.Dispatcher.Events.Wbs.CostGroups;
using CscGet.Costing.Domain.Dispatcher.Handlers;
using Dxc.Captn.Costing.Contracts.Operations.ConvertToCustom;
using Microsoft.Extensions.DependencyInjection;

namespace CscGet.Audit.Application
{
    public static class AuditPackage
    {
        public static IServiceCollection RegisterAuditServices(this IServiceCollection services)
        {
            MongoDbClassMapRegistrator.Register();

            services.RegisterContextEventHandlers();

            services
                .AddTransient<IMongoCollectionInitializer<GlobalTotalEntityAuditRecord>,
                    GlobalTotalEntityAuditRecordCollectionInitializer>();
            services
                .AddTransient<IMongoCollectionInitializer<PyramidMixAuditRecord>,
                    PyramidMixAuditRecordCollectionInitializer>();
            services
                .AddTransient<IMongoCollectionInitializer<QuantitiesAuditRecord>,
                    QuantitiesAuditRecordCollectionInitializer>();
            services
                .AddTransient<IMongoCollectionInitializer<ConvertToCustomAuditRecord>, 
                    ConvertToCustomAuditRecordCollectionInitializer>();

            services.AddTransient<IDataSourceTypeService, DataSourceTypeService>();

            services.AddTransient<IQuantityAuditService, QuantityAuditService>();
            services.AddTransient<IConvertToCustomAuditService, ConvertToCustomAuditService>();
            services.AddTransient<IPyramidMixAuditService, PyramidMixAuditService>();

            services.AddSingleton<IMongoDatabaseProvider, MongoDatabaseProvider>();
            services.AddSingleton<IMongoConnectionStringProvider, MongoConnectionStringProvider>();

            services.AddTransient<IAuditRecordRepository<QuantitiesAuditRecord, Quantity[]>, QuantityAuditRecordRepository>();
            services.AddTransient<IPyramidMixAuditRecordRepository, PyramidMixAuditRecordRepository>();
            services.AddTransient<IAuditRecordRepository<GlobalTotalEntityAuditRecord, string>, GlobalTotalEntityAuditRepository>();
            services.AddTransient<IGlobalTotalEntityAuditService, GlobalTotalEntityAuditService>();
            services.AddTransient<IAuditRecordRepository<ConvertToCustomAuditRecord, string>, ConvertToCustomRepository>();

            services.AddSingleton<IPyramidMixAuditRecordFactory, PyramidMixAuditRecordFactory>();
            services.AddScoped<IUserProvider, JwtTokenUserProvider>();

            services.AddTransient<ICommonAuditRecordService<QuantitiesAuditRecord, Quantity[]>, CommonAuditRecordService<QuantitiesAuditRecord, Quantity[]>>();
            services.AddTransient<ICommonAuditRecordService<GlobalTotalEntityAuditRecord, string>, CommonAuditRecordService<GlobalTotalEntityAuditRecord, string>>();
            services.AddTransient<ICommonAuditRecordService<ConvertToCustomAuditRecord, string>, CommonAuditRecordService<ConvertToCustomAuditRecord, string>>();

            return services;
        }

        private static IServiceCollection RegisterContextEventHandlers(this IServiceCollection services)
        {
            services.AddTransient<BaselineMetricEventHandler>();
            services.AddTransient<CostGroupEventHandler>();
            services.AddTransient<ElementEventHandler>();
            services.AddTransient<AuditGlobalTotalEventHandler>();
            services.AddTransient<QuantitiesEventHandler>();
            services.AddTransient<ConvertToCustomEventHandler>();
            services.AddTransient<BidEventHandler>();

            services.AddTransient<IContextEventHandlerAsync<BaselineMetricDeleted>, BaselineMetricEventHandler>();
            services.AddTransient<IContextEventHandlerAsync<BaselineMetricsCopied>, BaselineMetricEventHandler>();

            services.AddTransient<IContextEventHandlerAsync<CostGroupsCopied>, CostGroupEventHandler>();
            services.AddTransient<IContextEventHandlerAsync<CostGroupsDeleted>, CostGroupEventHandler>();

            services.AddTransient<IContextEventHandlerAsync<ElementsDeleted>, ElementEventHandler>();
            services.AddTransient<IContextEventHandlerAsync<LaborRatePyramidMixValuesChangedEvent>, ElementEventHandler>();
            services.AddTransient<IContextEventHandlerAsync<ElementsCopied>, ElementEventHandler>();

            services.AddTransient<IContextEventHandlerAsync<GlobalTotalUpdated>, AuditGlobalTotalEventHandler>();

            services.AddTransient<IContextEventHandlerAsync<CreateConversionAuditRecordCommand>, ConvertToCustomEventHandler>();

            return services;
        }
    }
}
