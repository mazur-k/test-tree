using BlazorApp1.Helper;

namespace BlazorApp1.Components
{
    public class DataService : IDataService
    {
        public Task<IEnumerable<Item>> GetChildrenAsync(string parameter, CancellationToken cancellationToken)
        {
            var item1 = new Item();
            item1.Children = new List<Item> {
            new Item("item2", new List<Item> {
                new Item("item3", new List<Item>
                {
                    new Item("item4", new List<Item>
                    {
                        new Item("item5", new List<Item>())
                    })
                })}),
            new Item("item6", new List<Item>()),
            new Item("item7", new List<Item>())
        };
            item1.Name = "item1";

            var items = new List<Item>
        {
            item1,
            new Item("item8", new List<Item>()),
            new Item("item9", new List<Item>()),
            new Item("item10", new List<Item>())
        };
            return Task.FromResult<IEnumerable<Item>>(items);
        }
    }
}
