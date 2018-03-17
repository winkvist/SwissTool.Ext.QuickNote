// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTabItem.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The custom tab item class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    using SwissTool.Framework.UI.Utilities.Visual;

    /// <summary>
    /// The custom tab item class.
    /// </summary>
    public class CustomTabItem : TabItem
    {
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var content = DependencyObjectUtils.GetFirstDescendantOfType<ContentPresenter>(this);
            if (content != null)
            {
                content.HorizontalAlignment = HorizontalAlignment.Stretch;
            }
        }
    }
}