using SnagIt.API.Core.Domain.Aggregates;


namespace SnagIt.API.Core.Domain.SeedWork
{
    public interface IRepository<T> where T : IAggregateRoot
    {
    }
}
