// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainView.xaml.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MainView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.Views
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;

    using SwissTool.Ext.QuickNote.Controls;
    using SwissTool.Ext.QuickNote.Managers;
    using SwissTool.Ext.QuickNote.ViewModels;
    using SwissTool.Framework.UI.Utilities.Visual;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        /// <summary>
        /// The starting point of a drag operation.
        /// </summary>
        private Point startDrag;

        /// <summary>
        /// Prevent close.
        /// </summary>
        private bool preventClose = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            this.Initialize();

            this.Closing += this.WindowClosing;
            this.DataContextChanged += this.OnDataContextChanged;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public MainViewModel ViewModel
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            this.Left = ApplicationManager.Settings.WindowPositionX;
            this.Top = ApplicationManager.Settings.WindowPositionY;
            this.Height = ApplicationManager.Settings.WindowHeight;
            this.Width = ApplicationManager.Settings.WindowWidth;
        }

        /// <summary>
        /// Previews the window mouse wheel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        private void PreviewWindowMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                var viewModel = this.DataContext as MainViewModel;

                if (viewModel == null)
                {
                    return;
                }
                
                if (e.Delta > 0)
                {
                    viewModel.GoToNextWorkspace();
                }
                else
                {
                    viewModel.GoToPreviousWorkspace();
                }
            }
        }

        /// <summary>
        /// Previews the window key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void PreviewWindowKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) && Keyboard.IsKeyDown(Key.F4))
            {
                this.preventClose = true;
                return;
            }

            var viewModel = this.DataContext as MainViewModel;
            var handled = false;

            if (viewModel == null)
            {
                return;
            }

            if (e.Key == Key.F2)
            {
                this.RenameCurrentTab();

                handled = true;
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control))
            {
                if (e.Key == Key.Tab)
                {
                    viewModel.GoToPreviousWorkspace();

                    handled = true;
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.T:
                        viewModel.AddWorkspace();
                        handled = true;
                        break;
                    case Key.F4:
                        viewModel.CurrentWorkspace.CloseWorkspace();
                        handled = true;
                        break;
                    case Key.Tab:
                        viewModel.GoToNextWorkspace();
                        handled = true;
                        break;
                    case Key.N:
                        viewModel.CurrentWorkspace.ClearWorkspace();
                        handled = true;
                        break;
                    case Key.S:
                        viewModel.CurrentWorkspace.Save();
                        handled = true;
                        break;
                    case Key.O:
                        viewModel.CurrentWorkspace.OpenFile();
                        break;
                    case Key.P:
                        viewModel.CurrentWorkspace.Print();
                        break;
                }
            }
        
            e.Handled = handled;
        }

        /// <summary>
        /// Renames the current tab.
        /// </summary>
        private void RenameCurrentTab()
        {
            var viewModel = this.DataContext as MainViewModel;
            if (viewModel == null)
            {
                return;
            }

            var tabControl = this.FindName("MainTabControl") as CustomTabControl;
            if (tabControl == null)
            {
                return;
            }

            var selectedTabItem = tabControl.ItemContainerGenerator.ContainerFromItem(viewModel.CurrentWorkspace) as CustomTabItem;
            if (selectedTabItem == null)
            {
                return;
            }

            var textBox = DependencyObjectUtils.FindChild<EditableTextBlock>(selectedTabItem, "LabelTextBox");
            if (textBox != null)
            {
                textBox.EnterEditMode();
            }
        }

        /// <summary>
        /// Previews the window mouse left button down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PreviewWindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Alt)
            {
                if (!this.MainTabControl.IsMouseCaptured)
                {
                    this.MainTabControl.CaptureMouse();
                    this.Cursor = Cursors.SizeAll;

                    var currentPosition = e.GetPosition(this.MainTabControl);

                    this.startDrag = new Point
                    {
                        X = currentPosition.X,
                        Y = currentPosition.Y
                    };
                }
            }
        }

        /// <summary>
        /// Previews the window mouse left button up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PreviewWindowMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.MainTabControl.IsMouseCaptured)
            {
                this.MainTabControl.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;

                this.startDrag = new Point(-1, -1);

                var settings = ApplicationManager.Settings;

                settings.WindowPositionX = this.Left;
                settings.WindowPositionY = this.Top;

                ApplicationManager.SaveSettings();
            }
        }

        /// <summary>
        /// Previews the window mouse move.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void PreviewWindowMouseMove(object sender, MouseEventArgs e)
        {
            if (this.MainTabControl.IsMouseCaptured)
            {
                var currentPosition = e.GetPosition(this.MainTabControl);

                this.Left = this.Left + currentPosition.X - this.startDrag.X;
                this.Top = this.Top + currentPosition.Y - this.startDrag.Y;
            }
        }

        /// <summary>
        /// Previews the tab item mouse left button down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PreviewTabItemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Image || (e.OriginalSource.GetType().Name == "ButtonChrome"))
            {
                // Just a workaround to make the close button clickable.
                return;
            }

            if (e.Source is TabItem)
            {
                var tabItem = e.Source as TabItem;

                var workspaceViewModel = tabItem.DataContext as WorkspaceViewModel;

                if (workspaceViewModel == null)
                {
                    return;
                }

                var mainViewModel = this.ViewModel;

                if (mainViewModel == null)
                {
                    return;
                }

                mainViewModel.CurrentWorkspace = workspaceViewModel;

                if (e.LeftButton == MouseButtonState.Pressed)
                {

                    mainViewModel.IsReordering = true;

                    this.CurrentDragSource = workspaceViewModel;

                    DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.Move);

                    mainViewModel.IsReordering = false;

                    this.CurrentDragSource = null;
                    this.CurrentDragTarget = null;

                    foreach (var workspace in this.ViewModel.Workspaces)
                    {
                        workspace.IsInDragDropOperation = false;
                    }    
                }
            }
        }

        /// <summary>
        /// Windows the size changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void WindowSizeChanged(object sender, RoutedEventArgs e)
        {
            var settings = ApplicationManager.Settings;

            settings.WindowHeight = this.Height;
            settings.WindowWidth = this.Width;
            
            ApplicationManager.SaveSettings();
        }

        /// <summary>
        /// Windows the closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.preventClose)
            {
                this.preventClose = false;
                e.Cancel = true;
                return;
            }

            var viewModel = this.DataContext as MainViewModel;

            if (viewModel == null)
            {
                return;
            }

            if (ApplicationManager.Settings.MaintainWorkspaceStates)
            {
                try
                {
                    viewModel.SaveWorkspaceStates();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to save the current workspace states: {ex.Message}", "Saving failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                var changedViewModels = viewModel.Workspaces.Where(w => w.HasChanges);
                if (changedViewModels.Select(cvm => cvm.CheckForChanges()).Any(continueExecution => !continueExecution))
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Contents the tab control drag over.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void ContentTabControlDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                if ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.Move;
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Contents the tab control drop.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void ContentTabControlDrop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                return;
            }

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length < 0)
            {
                return;
            }

            var fileToLoad = files.First();

            var workspaceViewModel = this.DataContext as WorkspaceViewModel;
            if (workspaceViewModel == null)
            {
                return;
            }

            var existingWorkspace = workspaceViewModel.MainViewModel.Workspaces.FirstOrDefault(w => w.Filename == fileToLoad);
            if (existingWorkspace != null)
            {
                workspaceViewModel.MainViewModel.CurrentWorkspace = existingWorkspace;

                return;
            }

            if ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
            {
                // Copy command
                var newWorkspaceViewModel = workspaceViewModel.MainViewModel.AddWorkspace();
                newWorkspaceViewModel.LoadFile(fileToLoad);
            }
            else
            {
                // Move command
                if (workspaceViewModel.ClearWorkspace())
                {
                    // Load the file contents into the current workspace.
                    workspaceViewModel.LoadFile(fileToLoad);
                }
            }
        }

        /// <summary>
        /// Windows the got focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void WindowGotFocus(object sender, RoutedEventArgs e)
        {
            var foundTextBox = DependencyObjectUtils.FindChild<WorkspaceTextBox>(this.MainTabControl, "TextEditor");
            Keyboard.Focus(foundTextBox);
            foundTextBox.Focus();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.SearchBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void UIElement_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textEditor = DependencyObjectUtils.FindChild<WorkspaceTextBox>(this.MainTabControl, "TextEditor");
            if (textEditor == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                textEditor.ResetSession();

                this.SearchTextBox.Text = textEditor.SelectedText;
                this.SearchTextBox.SelectionStart = this.SearchTextBox.Text.Length;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => this.SearchTextBox.Focus()));
            }
            else
            {
                textEditor.SelectionLength = 0;
                textEditor.Focus(); 
            }
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            this.SearchBorder.Visibility = Visibility.Collapsed;
        }

        private void SearchTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            if (e.Key == Key.Escape)
            {
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = string.Empty;
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Enter)
            {
                this.SearchString();
            }
            else if (e.Key == Key.F3)
            {
                this.SearchString();
                e.Handled = true;
            }
        }

        private void SearchString()
        {
            var textBox = DependencyObjectUtils.FindChild<WorkspaceTextBox>(this.MainTabControl, "TextEditor");
            if (textBox == null)
            {
                return;
            }

            try
            {
                textBox.SearchString(this.SearchTextBox.Text);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ((Storyboard)this.FindResource("NoSearchMatchesStoryboard")).Begin(this);
            }
        }

        private void MainView_OnDeactivated(object sender, EventArgs e)
        {
            this.SearchBorder.Visibility = Visibility.Collapsed;
        }

        private void OnFindCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.SearchBorder.Visibility = Visibility.Visible;
        }

        private void EventSetter_OnHandler(object sender, SelectionChangedEventArgs e)
        {
            this.SearchBorder.Visibility = Visibility.Collapsed;
        }

        private WorkspaceViewModel currentDragSource;

        private WorkspaceViewModel CurrentDragSource
        {
            get
            {
                return this.currentDragSource;
            }

            set
            {
                var newValue = value;
                var oldValue = this.currentDragSource;

                if (newValue != oldValue)
                {
                    if (newValue != null)
                    {
                        newValue.IsInDragDropOperation = true;
                    }

                    if (oldValue != null)
                    {
                        oldValue.IsInDragDropOperation = false;    
                    }
                }

                this.currentDragSource = newValue;
            }
        }

        private WorkspaceViewModel currentDragTarget;

        private WorkspaceViewModel CurrentDragTarget
        {
            get
            {
                return this.currentDragTarget;
            }

            set
            {
                var newValue = value;
                var oldValue = this.currentDragTarget;

                if (newValue != oldValue)
                {
                    if (newValue != null)
                    {
                        newValue.IsInDragDropOperation = true;
                    }

                    if (oldValue != null && this.CurrentDragSource != oldValue)
                    {
                        oldValue.IsInDragDropOperation = false;
                    }
                }

                this.currentDragTarget = newValue;
            }
        }

        private void TabItemDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effects = (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey
                            ? DragDropEffects.Copy : DragDropEffects.Move;
                e.Handled = true;
                return;
            }

            if (this.ViewModel == null)
            {
                return;
            }

            var tabItemTarget = e.Source as CustomTabItem;
            var tabItemSource = e.Data.GetData(typeof(CustomTabItem)) as CustomTabItem;

            if (tabItemTarget == null || tabItemSource == null)
            {
                return;
            }

            this.CurrentDragTarget = tabItemTarget.DataContext as WorkspaceViewModel;

            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void TabItemDrop(object sender, DragEventArgs e)
        {
            if (this.ViewModel == null)
            {
                return;
            }

            var tabItemTarget = e.Source as CustomTabItem;
            var tabItemSource = e.Data.GetData(typeof(CustomTabItem)) as CustomTabItem;

            if (tabItemTarget == null || tabItemSource == null)
            {
                return;
            }

            if (!tabItemTarget.Equals(tabItemSource))
            {
                var tabControl = this.FindName("MainTabControl") as CustomTabControl;

                if (tabControl == null)
                {
                    return;
                }

                var sourceViewModel = tabItemSource.DataContext as WorkspaceViewModel;
                var targetViewModel = tabItemTarget.DataContext as WorkspaceViewModel;

                if (sourceViewModel == null || targetViewModel == null)
                {
                    return;
                }

                var sourceIndex = this.ViewModel.Workspaces.IndexOf(sourceViewModel);
                var targetIndex = this.ViewModel.Workspaces.IndexOf(targetViewModel);

                this.ViewModel.Workspaces.Move(sourceIndex, targetIndex);
            }
        }


        private void MainTabControl_OnPreviewDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }
            
            e.Effects = (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey
                            ? DragDropEffects.Copy : DragDropEffects.Move;

            e.Handled = true;
        }

        private void TabControl_OnPreviewDrop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                this.ViewModel.SaveWorkspaceStates();

                return;
            }

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length < 0)
            {
                return;
            }

            var fileToLoad = files.First();

            var existingWorkspace = this.ViewModel.Workspaces.FirstOrDefault(w => w.Filename == fileToLoad);
            if (existingWorkspace != null)
            {
                this.ViewModel.CurrentWorkspace = existingWorkspace;

                return;
            }

            if ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey)
            {
                // Copy command
                var newWorkspaceViewModel = this.ViewModel.AddWorkspace();
                newWorkspaceViewModel.LoadFile(fileToLoad);
            }
            else
            {
                // Move command
                if (this.ViewModel.CurrentWorkspace.ClearWorkspace())
                {
                    // Load the file contents into the current workspace.
                    this.ViewModel.CurrentWorkspace.LoadFile(fileToLoad);
                }
            }    
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.RenameCurrentTab();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.WorkspaceChanged += this.ViewModelOnWorkspaceChanged;
            }
        }

        private void ViewModelOnWorkspaceChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = DependencyObjectUtils.FindChild<WorkspaceTextBox>(this.MainTabControl, "TextEditor");
            if (textEditor != null)
            {
                textEditor.ScrollToSelection();
            }
        }

    }
}
