using Microsoft.AspNetCore.Components;
using System.Collections;

namespace BlazorApp1.Components
{
    public class AthenaTreeItemSettings
    {
        public IEnumerable? Data { get; set; }

        //
        // Summary:
        //     Gets or sets the function which returns a value for the RadzenTreeItem.Text
        //     of a child item.
        public Func<object, string>? Text { get; set; }

        //
        // Summary:
        //     Gets or sets the name of the property which provides the value for the RadzenTreeItem.Text
        //     of a child item.
        public string? TextProperty { get; set; }

        //
        // Summary:
        //     Gets or sets a function which returns whether a child item has children of its
        //     own. Called with an item from TreeItemSettings.Data. By default all items
        //     are considered to have children.
        public Func<object, bool> HasChildren { get; set; } = (object value) => true;


        //
        // Summary:
        //     Gets or sets a function which determines whether a child item is expanded. Called
        //     with an item from TreeItemSettings.Data. By default all items are collapsed.
        public Func<object, bool> Expanded { get; set; } = (object value) => false;


        //
        // Summary:
        //     Gets or sets a function which determines whether a child item is selected. Called
        //     with an item from TreeItemSettings.Data. By default all items are not
        //     selected.
        public Func<object, bool> Selected { get; set; } = (object value) => false;


        //
        // Summary:
        //     Gets or sets the AthenaTreeItem.Template of a child item.
        //
        // Value:
        //     The template.
        public RenderFragment<AthenaTreeItem> Template { get; set; }
    }
}
