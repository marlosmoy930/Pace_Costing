namespace CscGet.ConfigManager.ServiceSettings.Interfaces
{
    public interface ICostingDbSettings
    {
        string EntitiesConnectionString { get; }
        string MongoDbConnectionString { get; }
        string AuditMongoDbConnectionString { get; }
        string LaborRatesMongoDbConnectionString { get; }
        string ArchivedDabsMongoDbConnectionString { get; }
        string ZipkinConnectionString { get; }
    }
}
