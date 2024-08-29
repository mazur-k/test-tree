using BlazorApp1.Helper;

namespace BlazorApp1.Components
{
    public interface IDataService
    {
        Task<IEnumerable<Item>> GetChildrenAsync(string parameter, CancellationToken cancellationToken);
    }
}
