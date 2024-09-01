using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components
{
    public class AthenaTreeExpandEventArgs
    {
        //new
        public RenderFragment<AthenaTreeItem> Template { get; set; }


        //
        // Summary:
        //     Gets the AthenaTreeItem.Value the expanded AthenaTreeItem.
        public object Value { get; set; }

        //
        // Summary:
        //     Gets the AthenaTreeItem.Text the expanded AthenaTreeItem.
        public string Text { get; set; }

        //
        // Summary:
        //     Gets or sets the children of the expanded AthenaTreeItem.
        //
        // Value:
        //     The children.
        public AthenaTreeItemSettings Children { get; set; }

        public AthenaTreeItem TargetNode { get; set; }
    }
}
