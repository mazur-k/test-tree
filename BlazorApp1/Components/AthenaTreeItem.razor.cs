using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor.Rendering;
using System.Collections;

namespace BlazorApp1.Components
{
    public partial class AthenaTreeItem : IDisposable
    {
        /// <summary>
        /// Specifies additional custom attributes that will be rendered by the component.
        /// </summary>
        /// <value>The attributes.</value>
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object> Attributes { get; set; }

        ClassList ContentClassList => ClassList.Create("rz-treenode-content")
                                               .Add("rz-treenode-content-selected", selected)
                                               .Add("rz-state-focused", Tree.IsFocused(this));
        //ClassList IconClassList => ClassList.Create("rz-tree-toggler rzi")
        //                                       .Add("rzi-caret-down", clientExpanded)
        //                                       .Add("rzi-caret-right", !clientExpanded);

        /// <summary>
        /// Gets or sets the child content.
        /// </summary>
        /// <value>The child content.</value>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the template. Use it to customize the appearance of a tree item.
        /// </summary>
        [Parameter]
        public RenderFragment<AthenaTreeItem> Template { get; set; }

        /// <summary>
        /// Gets or sets the text displayed by the tree item.
        /// </summary>
        [Parameter]
        public string Text { get; set; }

        private bool expanded;

        /// <summary>
        /// Specifies whether this item is expanded. Set to <c>false</c> by default.
        /// </summary>
        [Parameter]
        public bool Expanded { get; set; }

        /// <summary>
        /// Gets or sets the value of the tree item.
        /// </summary>
        [Parameter]
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has children.
        /// </summary>
        [Parameter]
        public bool HasChildren { get; set; }

        private bool selected;

        /// <summary>
        /// Specifies whether this item is selected or not. Set to <c>false</c> by default.
        /// </summary>
        [Parameter]
        public bool Selected { get; set; }

        [Parameter]
        public bool Loading { get; set; }

        /// <summary>
        /// The AthenaTree which this item is part of.
        /// </summary>
        [CascadingParameter]
        public AthenaTree Tree { get; set; }

        /// <summary>
        /// The AthenaTreeItem which contains this item.
        /// </summary>
        [CascadingParameter]
        public AthenaTreeItem ParentItem { get; set; }

        /// <summary>
        /// The children data.
        /// </summary>
        [Parameter]
        public IEnumerable Data { get; set; }

        public void StartLoading(bool isLoading) { 
            Loading = isLoading;
        }

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

        /// <inheritdoc />
        public void Dispose()
        {
            if (ParentItem != null)
            {
                ParentItem.RemoveItem(this);
            }
            else if (Tree != null)
            {
                Tree.RemoveItem(this);
            }
        }

        bool clientExpanded;
        internal async Task Toggle()
        {
            if (expanded)
            {
                clientExpanded = !clientExpanded;

                if (clientExpanded)
                {
                    await Expand();
                }
                else
                {
                    if (items.Count > 0)
                    {
                        Tree.RemoveFromCurrentItems(Tree.CurrentItems.IndexOf(items[0]), items.Count);
                    }

                    if (Tree != null)
                    {
                        await Tree.Collapse.InvokeAsync(new TreeEventArgs()
                        {
                            Text = Text,
                            Value = Value
                        });
                    }
                }

                return;
            }

            expanded = !expanded;
            clientExpanded = !clientExpanded;

            if (expanded)
            {
                await Expand();
            }
        }

        internal async Task ExpandCollapse(bool value)
        {
            expanded = value;
            clientExpanded = value;

            if (expanded || clientExpanded)
            {
                await Expand();
            }
            else
            {
                if (items.Count > 0)
                {
                    Tree.RemoveFromCurrentItems(Tree.CurrentItems.IndexOf(items[0]), items.Count);
                }

                if (Tree != null)
                {
                    await Tree.Collapse.InvokeAsync(new TreeEventArgs()
                    {
                        Text = Text,
                        Value = Value
                    });
                }
            }
        }

        async Task Expand()
        {
            if (Tree != null)
            {
                await Tree.ExpandItem(this);
            }
        }

        void Select()
        {
            selected = true;
            Tree?.SelectItem(this);
        }

        internal void Unselect()
        {
            selected = false;
            StateHasChanged();
        }

        internal void RenderChildContent(RenderFragment content)
        {
            ChildContent = content;
        }

        /// <inheritdoc />
        override protected async Task OnInitializedAsync()
        {
            expanded = Expanded;
            clientExpanded = expanded;

            if (expanded)
            {
                await Tree?.ExpandItem(this);
            }

            selected = Selected;

            if (selected)
            {
                await Tree?.SelectItem(this);
            }

            if (Tree != null && ParentItem == null)
            {
                Tree.AddItem(this);
            }

            if (ParentItem != null)
            {
                ParentItem.AddItem(this);

                var currentItems = Tree.items;

                Tree.InsertInCurrentItems(currentItems.IndexOf(ParentItem) + (ParentItem != null ? ParentItem.items.Count : 0), this);
            }
        }

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var shouldExpand = false;

            if (parameters.DidParameterChange(nameof(Expanded), Expanded))
            {
                // The Expanded property has changed - update the expanded state
                var e = parameters.GetValueOrDefault<bool>(nameof(Expanded));
                if (expanded != e)
                {
                    expanded = e;
                    clientExpanded = expanded;
                    shouldExpand = expanded;
                }
            }

            if (parameters.DidParameterChange(nameof(Value), Value))
            {
                // The Value property has changed - the children may have also changed
                shouldExpand = expanded;
            }

            if (shouldExpand)
            {
                // Either the expanded state or Value changed - expand the node to render its children
                Tree?.ExpandItem(this);
            }

            if (parameters.DidParameterChange(nameof(Selected), Selected))
            {
                selected = parameters.GetValueOrDefault<bool>(nameof(Selected));

                if (selected)
                {
                    Tree?.SelectItem(this);
                }
            }

            await base.SetParametersAsync(parameters);
        }

        internal IEnumerable<object> GetAllChildValues(Func<object, bool> predicate = null)
        {
            var children = items.Concat(items.SelectManyRecursive(i => i.items)).Select(i => i.Value);

            return predicate != null ? children.Where(predicate) : children;
        }

        IEnumerable<object> GetValueAndAllChildValues()
        {
            return new object[] { Value }.Concat(GetAllChildValues());
        }

        internal bool Contains(AthenaTreeItem child)
        {
            var parent = child.ParentItem;

            while (parent != null)
            {
                if (parent == this)
                {
                    return true;
                }

                parent = parent.ParentItem;
            }

            return false;
        }
    }
}
