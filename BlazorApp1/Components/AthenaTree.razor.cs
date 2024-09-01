using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System.Collections;

namespace BlazorApp1.Components
{
    public partial class AthenaTree : RadzenComponent
    {
        [Inject]
        public required IDataService DataService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the open button aria-label attribute.
        /// </summary>
        [Parameter]
        public string SelectItemAriaLabel { get; set; } = "Select item";

        /// <inheritdoc />
        protected override string GetComponentCssClass()
        {
            return "rz-tree";
        }

        internal AthenaTreeItem SelectedItem { get; private set; }

        IList<AthenaTreeLevel> Levels { get; set; } = new List<AthenaTreeLevel>(3);

        
        [Parameter]
        public EventCallback<TreeEventArgs> Change { get; set; }

        /// <summary>
        /// A callback that will be invoked when the user expands an item.
        /// </summary>
        /// <example>
        /// <code>
        /// &lt;RadzenTree Expand=@OnExpand&gt;
        ///     &lt;RadzenTreeItem Text="BMW"&gt;
        ///         &lt;RadzenTreeItem Text="M3" /&gt;
        ///         &lt;RadzenTreeItem Text="M5" /&gt;
        ///     &lt;/RadzenTreeItem&gt;
        ///     &lt;RadzenTreeItem Text="Audi"&gt;
        ///         &lt;RadzenTreeItem Text="RS4" /&gt;
        ///         &lt;RadzenTreeItem Text="RS6" /&gt;
        ///     &lt;/RadzenTreeItem&gt;
        ///     &lt;RadzenTreeItem Text="Mercedes"&gt;
        ///         &lt;RadzenTreeItem Text="C63 AMG" /&gt;
        ///         &lt;RadzenTreeItem Text="S63 AMG" /&gt;
        ///     &lt;/RadzenTreeItem&gt;
        /// &lt;/RadzenTree&gt;
        /// @code {
        ///   void OnExpand(TreeExpandEventArgs args) 
        ///   {
        /// 
        ///   }
        /// }
        /// </code>
        /// </example>
        [Parameter]
        public EventCallback<AthenaTreeExpandEventArgs> Expand { get; set; }

        /// <summary>
        /// A callback that will be invoked when the user collapse an item.
        /// </summary>
        [Parameter]
        public EventCallback<TreeEventArgs> Collapse { get; set; }

        /// <summary>
        /// A callback that will be invoked when item is rendered.
        /// </summary>
        [Parameter]
        public Action<AthenaTreeItemRenderEventArgs> ItemRender { get; set; }

        internal Tuple<AthenaTreeItemRenderEventArgs, IReadOnlyDictionary<string, object>> ItemAttributes(AthenaTreeItem item)
        {
            var args = new AthenaTreeItemRenderEventArgs() { Data = item.GetAllChildValues(), Value = item.Value };

            if (ItemRender != null)
            {
                ItemRender(args);
            }

            return new Tuple<AthenaTreeItemRenderEventArgs, IReadOnlyDictionary<string, object>>(args, new System.Collections.ObjectModel.ReadOnlyDictionary<string, object>(args.Attributes));
        }

        /// <summary>
        /// Gets or sets the child content.
        /// </summary>
        /// <value>The child content.</value>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Specifies the collection of data items which RadzenTree will create its items from.
        /// </summary>
        [Parameter]
        public IEnumerable Data { get; set; }

        /// <summary>
        /// Specifies the selected value. Use with <c>@bind-Value</c> to sync it with a property.
        /// </summary>
        [Parameter]
        public object Value { get; set; }

        /// <summary>
        /// A callback which will be invoked when <see cref="Value" /> changes.
        /// </summary>
        [Parameter]
        public EventCallback<object> ValueChanged { get; set; }

        internal List<AthenaTreeItem> items = new List<AthenaTreeItem>();

        internal void AddItem(AthenaTreeItem item)
        {
            if (items.IndexOf(item) == -1)
            {
                items.Add(item);
            }
        }

        internal void RemoveItem(AthenaTreeItem item)
        {
            if (items.IndexOf(item) != -1)
            {
                items.Remove(item);
            }
        }

        void RenderTreeItem(RenderTreeBuilder builder, object data, RenderFragment<AthenaTreeItem> template, Func<object, string> text, Func<object, bool> hasChildren, Func<object, bool> expanded, Func<object, bool> selected, IEnumerable children = null)
        {
            builder.OpenComponent<AthenaTreeItem>(0);
            builder.AddAttribute(1, nameof(AthenaTreeItem.Text), text(data));
            builder.AddAttribute(3, nameof(AthenaTreeItem.Value), data);
            builder.AddAttribute(4, nameof(AthenaTreeItem.HasChildren), hasChildren(data));
            builder.AddAttribute(5, nameof(AthenaTreeItem.Template), template);
            builder.AddAttribute(6, nameof(AthenaTreeItem.Expanded), expanded(data));
            builder.AddAttribute(7, nameof(AthenaTreeItem.Selected), Value == data || selected(data));
            builder.SetKey(data);
        }

        RenderFragment RenderChildren(IEnumerable children, int depth)
        {
            var maxDepth = 2;

            var level = depth < Levels.Count() ? Levels.ElementAt(depth) : Levels.Last();

            return new RenderFragment(builder =>
            {
                if (depth <= maxDepth)
                {
                    Func<object, string> text = null;

                    foreach (var data in children)
                    {
                        if (text == null)
                        {
                            text = level.Text ??
                                (!string.IsNullOrEmpty(level.TextProperty) ? Getter<string>(data, level.TextProperty) : null) ??
                                (o => "");
                        }

                        RenderTreeItem(builder, data, level.Template, text, level.HasChildren, level.Expanded, level.Selected);

                        var hasChildren = level.HasChildren(data);

                        if (!string.IsNullOrEmpty(level.ChildrenProperty))
                        {
                            var grandChildren = PropertyAccess.GetValue(data, level.ChildrenProperty) as IEnumerable;

                            if (grandChildren != null && hasChildren)
                            {
                                builder.AddAttribute(7, "ChildContent", RenderChildren(grandChildren, depth + 1));
                                builder.AddAttribute(8, nameof(AthenaTreeItem.Data), grandChildren);
                            }
                            else
                            {
                                builder.AddAttribute(7, "ChildContent", (RenderFragment)null);
                            }
                        }

                        builder.CloseComponent();
                    }
                }
            });
        }

        internal async Task SelectItem(AthenaTreeItem item)
        {
            var selectedItem = SelectedItem;

            if (selectedItem != item)
            {
                SelectedItem = item;

                selectedItem?.Unselect();

                if (Value != item.Value)
                {
                    await ValueChanged.InvokeAsync(item.Value);
                }

                await Change.InvokeAsync(new TreeEventArgs()
                {
                    Text = item?.Text,
                    Value = item?.Value
                });
            }
        }
        /// <summary>
        /// Clear the current selection to allow re-selection by mouse click
        /// </summary>
        public void ClearSelection()
        {
            SelectedItem?.Unselect();
            SelectedItem = null;
        }
        internal async Task ExpandItem(AthenaTreeItem item)
        {
            var args = new AthenaTreeExpandEventArgs()
            {
                Text = item?.Text,
                Value = item?.Value,
                Children = new AthenaTreeItemSettings(),
                TargetNode = item,
            };

            await Expand.InvokeAsync(args);

            if (args.Children.Data != null)
            {
                var childContent = new RenderFragment(builder =>
                {
                    Func<object, string> text = null;

                    var children = args.Children;

                    foreach (var data in children.Data)
                    {
                        if (text == null)
                        {
                            text = children.Text ?? Getter<string>(data, children.TextProperty);
                        }

                        RenderTreeItem(builder, data, children.Template, text, children.HasChildren, children.Expanded, children.Selected);
                        builder.CloseComponent();
                    }
                });

                item.RenderChildContent(childContent);
            }
        }

        Func<object, T> Getter<T>(object data, string property)
        {
            if (string.IsNullOrEmpty(property))
            {
                return (value) => (T)value;
            }

            return PropertyAccess.Getter<T>(data, property);
        }

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.DidParameterChange(nameof(Value), Value))
            {
                var value = parameters.GetValueOrDefault<object>(nameof(Value));

                if (value == null)
                {
                    SelectedItem = null;
                }
            }

            await base.SetParametersAsync(parameters);
        }

        internal void AddLevel(AthenaTreeLevel level)
        {
            if (!Levels.Contains(level))
            {
                Levels.Add(level);
                StateHasChanged();
            }
        }

        internal int focusedIndex = -1;

        bool preventKeyPress = true;

        async Task OnKeyPress(KeyboardEventArgs args)
        {
            var key = args.Code != null ? args.Code : args.Key;

            if (key == "ArrowUp" || key == "ArrowDown")
            {
                preventKeyPress = true;

                focusedIndex = Math.Clamp(focusedIndex + (key == "ArrowUp" ? -1 : 1), 0, CurrentItems.Count - 1);
            }
            else if (key == "ArrowLeft" || key == "ArrowRight")
            {
                preventKeyPress = true;

                if (focusedIndex >= 0 && focusedIndex < CurrentItems.Count)
                {
                    var item = CurrentItems[focusedIndex];

                    if (item.ChildContent != null || item.HasChildren)
                    {
                        await item.ExpandCollapse(key == "ArrowRight");
                    }
                }
            }
            else if (key == "Enter" || key == "Space")
            {
                preventKeyPress = true;

                if (focusedIndex >= 0 && focusedIndex < CurrentItems.Count)
                {
                    await SelectItem(CurrentItems[focusedIndex]);
                }
            }
            else
            {
                preventKeyPress = false;
            }
        }

        internal bool IsFocused(AthenaTreeItem item)
        {
            return CurrentItems.IndexOf(item) == focusedIndex && focusedIndex != -1;
        }

        internal void InsertInCurrentItems(int index, AthenaTreeItem item)
        {
            if (index <= CurrentItems.Count)
            {
                CurrentItems.Insert(index, item);
            }
        }

        internal void RemoveFromCurrentItems(int index, int count)
        {
            if (index >= 0)
            {
                CurrentItems.RemoveRange(index, count);
            }

            if (focusedIndex > index)
            {
                focusedIndex = index;
            }
        }

        List<AthenaTreeItem> _currentItems;
        internal List<AthenaTreeItem> CurrentItems
        {
            get
            {
                if (_currentItems == null)
                {
                    _currentItems = items;
                }

                return _currentItems;
            }
        }

    }
}
