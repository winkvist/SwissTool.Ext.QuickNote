// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatternReplaceView.xaml.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for PatternReplaceView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using MahApps.Metro.Controls.Dialogs;

namespace SwissTool.Ext.QuickNote.Views
{
    using System;
    using System.Windows.Media.Imaging;

    using SwissTool.Framework.UI.Controls;
    using SwissTool.Framework.UI.Managers;

    /// <summary>
    /// Interaction logic for PatternReplaceView.xaml
    /// </summary>
    public partial class PatternReplaceView : MetroDialogWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternReplaceView"/> class.
        /// </summary>
        public PatternReplaceView()
        {
            InitializeComponent();

            var uiHint = WindowManager.CurrentTheme.UiHint;
            var uriString = $"/QuickNote;component/Resources/Icons/{uiHint}/48x48/replace.png";
            this.Image = new BitmapImage(new Uri(uriString, UriKind.RelativeOrAbsolute));
        }
    }
}
