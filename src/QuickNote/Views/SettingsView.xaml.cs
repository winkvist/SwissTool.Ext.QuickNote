// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsView.xaml.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for SettingsView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.Views
{
    using System;
    using System.Windows.Media.Imaging;

    using SwissTool.Framework.UI.Controls;
    using SwissTool.Framework.UI.Managers;

    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : MetroDialogWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// </summary>
        public SettingsView()
        {
            InitializeComponent();

            var uiHint = WindowManager.CurrentTheme.UiHint;
            var uriString = $"/QuickNote;component/Resources/Icons/{uiHint}/48x48/services.png";
            this.Image = new BitmapImage(new Uri(uriString, UriKind.RelativeOrAbsolute));
        }
    }
}
