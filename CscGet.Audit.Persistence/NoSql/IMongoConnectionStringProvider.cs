namespace CscGet.Audit.Persistence.NoSql
{
    public interface IMongoConnectionStringProvider
    {
        string GetConnectionString();
    }
}