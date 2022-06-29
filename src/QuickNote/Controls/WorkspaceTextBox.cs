// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceTextBox.cs" company="Fredrik Winkvist">
//   Copyright © Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Defines the WorkspaceTextBox type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Editing;

    using SwissTool.Ext.QuickNote.Managers;
    using SwissTool.Framework.UI.Utilities.Visual;

    /// <summary>
    /// The workspace text box.
    /// </summary>
    /// <seealso cref="ICSharpCode.AvalonEdit.TextEditor" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class WorkspaceTextBox : TextEditor, INotifyPropertyChanged
    {
        /// <summary>
        /// The text box scroll viewer property
        /// </summary>
        public static DependencyProperty TextBoxScrollViewerProperty =
            DependencyProperty.Register(
            "TextBoxScrollViewer", 
            typeof(ScrollViewer), 
            typeof(WorkspaceTextBox), 
            new PropertyMetadata());

        /// <summary>
        /// The bindable selection start property.
        /// </summary>
        public static readonly DependencyProperty BindableSelectionStartProperty =
            DependencyProperty.Register(
            "BindableSelectionStart",
            typeof(int),
            typeof(WorkspaceTextBox),
            new PropertyMetadata(OnBindableSelectionStartChanged));

        /// <summary>
        /// The bindable selection length property.
        /// </summary>
        public static readonly DependencyProperty BindableSelectionLengthProperty =
            DependencyProperty.Register(
            "BindableSelectionLength",
            typeof(int),
            typeof(WorkspaceTextBox),
            new PropertyMetadata(OnBindableSelectionLengthChanged));

        public static DependencyProperty CaretOffsetProperty = 
            DependencyProperty.Register(
            "CaretOffset", 
            typeof(int), 
            typeof(WorkspaceTextBox), 
            new PropertyMetadata(0, CaretOffsetChangedCallback));

        public static DependencyProperty ShowLineNumbersProperty =
            DependencyProperty.Register(
                "ShowLineNumbers", 
                typeof(bool), 
                typeof(WorkspaceTextBox), 
                new PropertyMetadata(false, ShowLineNumbersChangedCallback));

        /// <summary>
        /// Indicates whether the change came from the UI.
        /// </summary>
        private bool changeFromUI;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceTextBox"/> class.
        /// </summary>
        public WorkspaceTextBox()
        {
            this.TextArea.TextView.LinkTextForegroundBrush = (Brush)this.FindResource("AccentColorBrush");

            this.TextArea.SelectionChanged += this.TextAreaOnSelectionChanged;
            this.TextArea.Caret.PositionChanged += this.CaretPositionChanged;
            this.TextArea.DocumentChanged += this.TextAreaDocumentChanged;
        }

        /// <summary>
        /// Gets or sets the text box scroll viewer.
        /// </summary>
        /// <value>
        /// The text box scroll viewer.
        /// </value>
        public ScrollViewer TextBoxScrollViewer
        {
            get { return (ScrollViewer)this.GetValue(TextBoxScrollViewerProperty); }
            set { this.SetValue(TextBoxScrollViewerProperty, value); }
        }

        /// <summary>
        /// Carets the offset changed callback.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void CaretOffsetChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var textBox = obj as WorkspaceTextBox;
            if (textBox != null)
            {
                textBox.CaretOffset = (int)args.NewValue;
            }
        }
        
        private static void ShowLineNumbersChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var textBox = obj as WorkspaceTextBox;
            if (textBox != null)
            {
                textBox.ShowLineNumbers = (bool)args.NewValue;
            }
        }

        /// <summary>
        /// Handles the DocumentChanged event of the TextArea control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void TextAreaDocumentChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the PositionChanged event of the Caret control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CaretPositionChanged(object sender, EventArgs e)
        {
            if (!ApplicationManager.ReflectSelectionChanges)
            {
                return;
            }

            var caret = sender as Caret;
            if (caret != null)
            {
                this.SetCurrentValue(CaretOffsetProperty, caret.Offset);
            }
        }

        /// <summary>
        /// Texts the area on selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void TextAreaOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            if (!ApplicationManager.ReflectSelectionChanges)
            {
                return;
            }

            if (this.BindableSelectionStart != this.SelectionStart)
            {
                this.changeFromUI = true;
                this.BindableSelectionStart = this.SelectionStart;
            }

            if (this.BindableSelectionLength != this.SelectionLength)
            {
                this.changeFromUI = true;
                this.BindableSelectionLength = this.SelectionLength;
            }
        }

        /// <summary>
        /// Gets or sets the bindable selection start.
        /// </summary>
        /// <value>The bindable selection start.</value>
        public int BindableSelectionStart
        {
            get
            {
                return (int)this.GetValue(BindableSelectionStartProperty);
            }

            set
            {
                this.SetValue(BindableSelectionStartProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the bindable selection.
        /// </summary>
        /// <value>The length of the bindable selection.</value>
        public int BindableSelectionLength
        {
            get
            {
                return (int)this.GetValue(BindableSelectionLengthProperty);
            }

            set
            {
                this.SetValue(BindableSelectionLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets/sets the caret position.
        /// </summary>
        public new int CaretOffset
        {
            get { return base.CaretOffset; }
            set { base.CaretOffset = value; }
        }
        
        /// <summary>
        /// Initializes the scroll viewer.
        /// </summary>
        public void InitializeScrollViewer()
        {
            this.TextBoxScrollViewer = DependencyObjectUtils.FindChildOfType<ScrollViewer>(this);
        }

        /// <summary>
        /// Called when [bindable selection start changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBindableSelectionStartChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var textBox = dependencyObject as WorkspaceTextBox;
            if (textBox == null)
            {
                return;
            }

            if (!textBox.changeFromUI)
            {
                var newValue = (int)args.NewValue;
                textBox.SelectionStart = newValue;
            }
            else
            {
                textBox.changeFromUI = false;
            }
        }

        /// <summary>
        /// Called when [bindable selection length changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBindableSelectionLengthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var textBox = dependencyObject as WorkspaceTextBox;
            if (textBox == null)
            {
                return;
            }

            if (!textBox.changeFromUI)
            {
                var newValue = (int)args.NewValue;
                textBox.SelectionLength = newValue;
            }
            else
            {
                textBox.changeFromUI = false;
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="info">The information.</param>
        public void RaisePropertyChanged(string info)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        /// <summary>
        /// The last used index
        /// </summary>
        private int lastUsedIndex = 0;

        /// <summary>
        /// Searches the string.
        /// </summary>
        /// <param name="searchString">The search string.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void SearchString(string searchString)
        {
            var editorText = this.Document.Text;

            if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(editorText))
            {
                this.lastUsedIndex = 0;
                return;
            }

            var nIndex = editorText.IndexOf(searchString, this.lastUsedIndex, StringComparison.OrdinalIgnoreCase);
            if (nIndex != -1)
            {
                this.Select(nIndex, searchString.Length);

                this.ScrollToSelection();

                this.lastUsedIndex = nIndex + searchString.Length;
            }
            else
            {
                this.lastUsedIndex = 0;
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Scrolls to selection.
        /// </summary>
        public void ScrollToSelection()
        {
            if (this.Document == null)
            {
                return;
            }

            var documentLine = this.Document.GetLineByOffset(this.CaretOffset);
            if (documentLine != null)
            {
                this.ScrollTo(documentLine.LineNumber, this.TextArea.Selection.StartPosition.Column);
            }
        }

        /// <summary>
        /// Resets the session.
        /// </summary>
        public void ResetSession()
        {
            this.lastUsedIndex = 0;
        }
    }
}
