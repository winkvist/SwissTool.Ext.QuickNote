// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceView.xaml.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for Workspace.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using SwissTool.Ext.QuickNote.Controls;
    using SwissTool.Ext.QuickNote.Managers;

    /// <summary>
    /// Interaction logic for Workspace.xaml
    /// </summary>
    public partial class WorkspaceView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceView"/> class.
        /// </summary>
        public WorkspaceView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the OnKeyDown event of the TextEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void TextEditor_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!ApplicationManager.Settings.EnableSyntaxHighlighting)
            {
                return;
            }

            if (((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) && e.Key == Key.Space)
            {
                this.highlightingComboBox.IsDropDownOpen = true;
            }
        }

        private void TextEditor_OnLoaded(object sender, RoutedEventArgs e)
        {
            var textBox = sender as WorkspaceTextBox;
            if (textBox == null)
            {
                return;
            }
            
            textBox.InitializeScrollViewer();
        }

        private void HighlightingComboBox_OnDropDownClosed(object sender, EventArgs e)
        {
            this.TextEditor.Focus();
        }
    }
}
