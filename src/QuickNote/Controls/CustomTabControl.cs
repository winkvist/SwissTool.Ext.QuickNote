// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTabControl.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The Custom Tab Control class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The Custom Tab Control class.
    /// </summary>
    public class CustomTabControl : TabControl
    {
        /// <summary>
        /// Creates or identifies the element used to display the specified item.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.TabItem"/>.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTabItem();
        }
    }
}