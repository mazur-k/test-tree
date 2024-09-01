using BlazorApp1.Helper;

namespace BlazorApp1.Components
{
    public class DataService : IDataService
    {
        public async Task<IEnumerable<Item>> GetChildrenAsync(string parameter, CancellationToken cancellationToken)
        {
            //    var item1 = new Item();
            //    item1.Children = new List<Item> {
            //    new Item("item2", "folder", new List<Item> {
            //        new Item("item3", "folder", new List<Item>
            //        {
            //            new Item("item4", "folder",new List<Item>
            //            {
            //                new Item("item5", "file", new List<Item>())
            //            })
            //        })}),
            //    new Item("item6", "file", new List<Item>()),
            //    new Item("item7", "file", new List<Item>())
            //};
            //    item1.Name = "item1";

            //    var items = new List<Item>
            //{
            //    item1,
            //    new Item("item8", "file", new List<Item>()),
            //    new Item("item9", "file", new List<Item>()),
            //    new Item("item10", "file", new List<Item>())
            //};

            var items = new List<Item>
            {
                new Item("item8", "file", new List<Item>()),
                new Item("item9", "file", new List<Item>()),
                new Item("item10", "file", new List<Item>())
            };

            await Task.Delay(2000);

            return await Task.FromResult<IEnumerable<Item>>(items);
        }
    }
}
