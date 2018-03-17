namespace SwissTool.Ext.QuickNote.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// The Editable Text Block.
    /// </summary>
    public partial class EditableTextBlock : TextBox
    {
        /// <summary>
        /// The old text
        /// </summary>
        private string previousText;

        /// <summary>
        /// The is in edit mode
        /// </summary>
        private bool isInEditMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableTextBlock"/> class.
        /// </summary>
        public EditableTextBlock()
        {
            this.InitializeComponent();
            this.isInEditMode = false;
        }
        
        /// <summary>
        /// Enters the edit mode.
        /// </summary>
        public void EnterEditMode()
        {
            this.isInEditMode = true;
            this.Focusable = true;
            this.previousText = this.Text;
            
            this.Focus();
            this.IsReadOnly = false;
            this.SelectAll();
        }

        /// <summary>
        /// Exits the edit mode.
        /// </summary>
        /// <param name="moveFocus">if set to <c>true</c> [move focus].</param>
        /// <param name="cancelRequest">if set to <c>true</c> [cancel request].</param>
        private void ExitEditMode(bool moveFocus, bool cancelRequest = false)
        {
            this.Focusable = false;
            this.IsReadOnly = true;
            this.isInEditMode = false;
            
            if (cancelRequest)
            {
                this.Text = this.previousText;
            }

            if (!moveFocus)
            {
                return;
            }

            var request = new TraversalRequest(FocusNavigationDirection.Next);
            var keyboardFocus = Keyboard.FocusedElement as UIElement;

            keyboardFocus?.MoveFocus(request);
        }

        /// <summary>
        /// Handles the OnMouseDoubleClick event of the EditableTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void EditableTextBlock_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            if (!this.isInEditMode)
            {
                this.EnterEditMode();    
            }
        }

        /// <summary>
        /// Handles the OnLostFocus event of the EditableTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EditableTextBlock_OnLostFocus(object sender, RoutedEventArgs e)
        {
            this.ExitEditMode(false);
        }

        /// <summary>
        /// Handles the OnKeyDown event of the EditableTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void EditableTextBlock_OnKeyDown(object sender, KeyEventArgs e)
        {
            var isCancelRequest = e.Key == Key.Escape;

            if (e.Key == Key.Enter || isCancelRequest)
            {
                this.ExitEditMode(true, isCancelRequest);
                e.Handled = true;
            }
        }
    }
}
