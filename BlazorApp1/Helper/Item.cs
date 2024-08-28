namespace BlazorApp1.Helper
{
    public class Item
    {
        public IList<Item> Children { get; set; } = new List<Item>();
        public string Name { get; set; }

        public Item()
        {
            
        }

        public Item(string name, IList<Item> children)
        {
            Name = name;
            Children = children;
        }
    }
}
