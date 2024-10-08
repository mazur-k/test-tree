﻿using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components
{
    public partial class AthenaTreeLevel : ComponentBase
    {
        /// <summary>
        /// Specifies the name of the property which provides values for the <see cref="AthenaTreeItem.Text" /> property of the child items.
        /// </summary>
        [Parameter]
        public string? TextProperty { get; set; }

        /// <summary>
        /// Specifies the name of the property which returns child data. The value returned by that property should be IEnumerable
        /// </summary>
        [Parameter]
        public string? ChildrenProperty { get; set; }

        /// <summary>
        /// Determines if a child item has children or not. Set to <c>value =&gt; true</c> by default.
        /// </summary>
        /// <example>
        /// <code>
        ///     &lt;RadzenTreeLevel HasChildren=@(e =&gt; (e as Employee).Employees1.Any()) /&gt;
        /// </code>
        /// </example>
        [Parameter]
        public Func<object, bool> HasChildren { get; set; } = value => true;

        /// <summary>
        /// Determines if a child item is expanded or not. Set to <c>value =&gt; false</c> by default.
        /// </summary>
        /// <example>
        /// <code>
        ///     &lt;RadzenTreeLevel Expanded=@(e =&gt; (e as Employee).Employees1.Any()) /&gt;
        /// </code>
        /// </example>
        [Parameter]
        public Func<object, bool> Expanded { get; set; } = value => false;

        /// <summary>
        /// Determines if a child item is selected or not. Set to <c>value =&gt; false</c> by default.
        /// </summary>
        /// <example>
        /// <code>
        ///     &lt;RadzenTreeLevel Selected=@(e =&gt; (e as Employee).LastName == "Fuller") /&gt;
        /// </code>
        /// </example>
        [Parameter]
        public Func<object, bool> Selected { get; set; } = value => false;

        /// <summary>
        /// Determines the text of a child item.
        /// </summary>
        /// <example>
        /// <code>
        ///     &lt;RadzenTreeLevel Text=@(e =&gt; (e as Employee).LastName) /&gt;
        /// </code>
        /// </example>
        [Parameter]
        public Func<object, string>? Text { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        [Parameter]
        public RenderFragment<AthenaTreeItem>? Template { get; set; }

        /// <summary>
        /// The RadzenTree which this item is part of.
        /// </summary>
        [CascadingParameter]
        public AthenaTree Tree
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                value.AddLevel(this);
            }
        }
    }
}
