namespace BlazorApp1.Helper
{
    public class Item
    {
        public IList<Item> Children { get; set; } = new List<Item>();
        public string? Name { get; set; }
        public string? Type { get; set; }

        public Item()
        {
            
        }

        public Item(string name, string type, IList<Item> children)
        {
            Name = name;
            Type = type;
            Children = children;
        }
    }
}
