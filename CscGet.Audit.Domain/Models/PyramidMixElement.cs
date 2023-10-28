using System;
using System.Linq;
using CscGet.Audit.Domain.Models.Core;

namespace CscGet.Audit.Domain.Models
{
    public class PyramidMixElement : ValueObject<PyramidMixElement>
    {
        public Guid Id { get; private set; }

        public Guid CostGroupId { get; private set; }

        public PyramidMixValue[] PyramidMixValues { get; private set; }

        public PyramidMixElement(Guid id, Guid costGroupId, PyramidMixValue[] pyramidMixValues)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id));

            if (costGroupId == Guid.Empty)
                throw new ArgumentNullException(nameof(costGroupId));

            if (pyramidMixValues == null)
                throw new ArgumentNullException(nameof(pyramidMixValues));

            if (!pyramidMixValues.Any())
                throw new ArgumentException("Should not be empty", nameof(pyramidMixValues));

            Id = id;
            CostGroupId = costGroupId;
            PyramidMixValues = pyramidMixValues;
        }

        protected override bool EqualsCore(PyramidMixElement other)
        {
            return Id.Equals(other.Id) && CostGroupId.Equals(other.CostGroupId) && PyramidMixValues.SequenceEqual(other.PyramidMixValues);
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ CostGroupId.GetHashCode();
                hashCode = (hashCode * 397) ^ PyramidMixValues.GetHashCode();
                return hashCode;
            }
        }
    }
}
