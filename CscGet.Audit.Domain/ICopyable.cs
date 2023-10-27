using System;

namespace CscGet.Audit.Domain
{
    public interface ICopyable<TRecord> where TRecord : ICopyable<TRecord>
    {
        TRecord Copy(Guid newId, int newCostingVersionId);
    }
}
