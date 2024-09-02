using System.Collections;

namespace BlazorApp1.Components
{
    public class AthenaTreeItemRenderEventArgs
    {
        //
        // Summary:
        //     Gets or sets the item HTML attributes.
        public IDictionary<string, object> Attributes { get; private set; } = new Dictionary<string, object>();

        //
        // Summary:
        //     Gets tree item.
        public object? Value { get; internal set; }

        //
        // Summary:
        //     Gets child items.
        public IEnumerable? Data { get; internal set; }
    }
}
