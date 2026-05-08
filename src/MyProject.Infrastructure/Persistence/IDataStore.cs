namespace MyProject.Infrastructure.Persistence;

public interface IDataStore<T>
{
    Task<IReadOnlyCollection<T>> LoadAsync();
    Task SaveAsync(IReadOnlyCollection<T> items);
}