namespace CscGet.Audit.Domain.Models.Core
{
    /// <summary>
    /// Defines base entity type.
    /// </summary>
    /// <typeparam name="T">is identity type. Guid, long, int etc.</typeparam>
    public abstract class Entity<T>
    {
        public T Id { get; protected set; }

        protected Entity(T id)
        {
            Id = id;
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as Entity<T>;

            if (ReferenceEquals(compareTo, null))
                return false;

            if (ReferenceEquals(this, compareTo))
                return true;

            if (GetType() != compareTo.GetType())
                return false;

            if (Id.Equals(compareTo.Id))
                return true;

            return false;
        }

        public static bool operator ==(Entity<T> a, Entity<T> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity<T> a, Entity<T> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().ToString() + Id).GetHashCode();
        }
    }
}
