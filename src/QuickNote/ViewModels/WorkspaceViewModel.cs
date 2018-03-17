// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceViewModel.cs" company="Fredrik Winkvist">
//   Copyright © Winkvist.com. All rights reserved.
// </copyright>
// <summary>
//   The workspace view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;

    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Highlighting;

    using SwissTool.Ext.QuickNote.Managers;
    using SwissTool.Ext.QuickNote.Models;
    using SwissTool.Framework.Infrastructure;
    using SwissTool.Framework.UI.Commanding;
    using SwissTool.Framework.UI.Infrastructure;

    using Application = System.Windows.Application;
    using MessageBox = System.Windows.MessageBox;
    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
    using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

    /// <summary>
    /// The workspace view model.
    /// </summary>
    public class WorkspaceViewModel : ViewModelBase
    {
        /// <summary>
        /// The name of the workspace file.
        /// </summary>
        private string filename;

        /// <summary>
        /// The tab item label.
        /// </summary>
        private string label;

        /// <summary>
        /// Indicating if the user has made changes since last save.
        /// </summary>
        private bool hasChanges;

        /// <summary>
        /// The selection start.
        /// </summary>
        private int selectionStart;

        /// <summary>
        /// The selection length.
        /// </summary>
        private int selectionLength;

        /// <summary>
        /// The caret offset
        /// </summary>
        private int caretOffset;

        /// <summary>
        /// The syntax highlight
        /// </summary>
        private IHighlightingDefinition syntaxHighlighting;

        /// <summary>
        /// The syntax highlighting definitions
        /// </summary>
        private IEnumerable<IHighlightingDefinition> syntaxHighlightingDefinitions;

        /// <summary>
        /// The document
        /// </summary>
        private TextDocument document;

        /// <summary>
        /// The content
        /// </summary>
        private string content;

        /// <summary>
        /// The is in drag drop operation
        /// </summary>
        private bool isInDragDropOperation;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceViewModel"/> class.
        /// </summary>
        public WorkspaceViewModel()
        {
            this.CloseWorkspaceCommand = new RelayCommand(o => this.CloseWorkspace());
            this.SaveCommand = new RelayCommand(o => this.Save());
            this.SaveWorkspaceStateCommand = new RelayCommand(o => this.SaveWorkspaceState());
            this.SaveFileAsCommand = new RelayCommand(o => this.SaveFileAs());
            this.ClearWorkspaceCommand = new RelayCommand(o => this.ClearWorkspace());
            this.OpenFileCommand = new RelayCommand(o => this.OpenFile());
            this.OpenFileInNewTabCommand = new RelayCommand(o => this.OpenFileInNewTab());
            this.AddNewWorkspaceCommand = new RelayCommand(o => this.MainViewModel.AddWorkspace());
            this.HideMainWindowCommand = new RelayCommand(o => this.HideMainWindow());
            this.ShowSettingsWindowCommand = new RelayCommand(o => this.ShowSettingsWindow());
            this.SaveStateCommand = new RelayCommand(o => this.SaveState(), o => this.HasChanges);
            this.PrintCommand = new RelayCommand(o => this.Print());

            this.Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceViewModel"/> class.
        /// </summary>
        /// <param name="mainViewModel">
        /// The main view model.
        /// </param>
        public WorkspaceViewModel(MainViewModel mainViewModel)
            : this()
        {
            this.MainViewModel = mainViewModel;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in drag drop operation.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in drag drop operation; otherwise, <c>false</c>.
        /// </value>
        public bool IsInDragDropOperation
        {
            get
            {
                return this.isInDragDropOperation;

            }

            set
            {
                this.isInDragDropOperation = value;
                this.NotifyPropertyChanged(nameof(this.IsInDragDropOperation));
            }
        }

        /// <summary>
        /// Gets the syntax highlighting definitions.
        /// </summary>
        /// <value>
        /// The syntax highlighting definitions.
        /// </value>
        public IEnumerable<IHighlightingDefinition> SyntaxHighlightingDefinitions
        {
            get
            {
                if (this.syntaxHighlightingDefinitions == null)
                {
                    var list = new List<IHighlightingDefinition>(HighlightingManager.Instance.HighlightingDefinitions);
                    var item = list.FirstOrDefault(l => l.Name == "Text");

                    if (item != null)
                    {
                        list.Remove(item);
                        list.Insert(0, item);
                    }

                    this.syntaxHighlightingDefinitions = list;
                }

                return this.syntaxHighlightingDefinitions;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is syntax highlighting enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is syntax highlighting enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsSyntaxHighlightingEnabled
        {
            get
            {
                return ApplicationManager.Settings.EnableSyntaxHighlighting;
            }
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label
        {
            get
            {
                return this.label;
            }

            set
            {
                var oldValue = this.label;

                this.label = value;
                this.WorkspaceState.Label = value;

                if (oldValue != value)
                {
                    this.HasChanges = true;
                }

                this.NotifyPropertyChanged(nameof(this.Label));
            }
        }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string Filename
        {
            get
            {
                return this.filename;
            }

            set
            {
                this.filename = value;

                this.WorkspaceState.SourceFilename = value;

                if (value != null)
                {
                    var fileName = Path.GetFileName(value) ?? string.Empty;
                    if (fileName.Length > 15)
                    {
                        fileName = fileName.Substring(0, 15) + "...";
                    }

                    this.Label = fileName;
                }

                this.NotifyPropertyChanged(nameof(this.Filename));
                this.NotifyPropertyChanged(nameof(this.Label));
                this.NotifyPropertyChanged(nameof(this.IsVirtual));
                this.NotifyPropertyChanged(nameof(this.IsPhysical));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has changes.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </value>
        public bool HasChanges
        {
            get
            {
                return this.hasChanges;
            }

            set
            {
                this.hasChanges = value;
                this.WorkspaceState.HasChanges = value;

                this.NotifyPropertyChanged(nameof(this.HasChanges));
            }
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content
        {
            get
            {
                return this.content;
            }
            
            set
            {
                this.content = value;
            }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string FullDisplayName
        {
            get
            {
                return Path.GetFileName(this.filename) ?? "Untitled";
            }
        }

        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        public TextDocument Document
        {
            get
            {
                return this.document;
            }

            set
            {
                this.document = value;
                this.NotifyPropertyChanged(nameof(this.Document));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is close button visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is close button visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsCloseButtonVisible
        {
            get
            {
                return this.MainViewModel.Workspaces.Count > 1;
            }
        }

        /// <summary>
        /// Gets or sets the caret offset.
        /// </summary>
        /// <value>
        /// The caret offset.
        /// </value>
        public int CaretOffset
        {
            get
            {
                return this.caretOffset;
            }

            set
            {
                this.caretOffset = value;
                this.NotifyPropertyChanged(nameof(this.CaretOffset));
            }
        }

        /// <summary>
        /// Gets or sets the selection start.
        /// </summary>
        /// <value>The selection start.</value>
        public int SelectionStart
        {
            get
            {
                return this.selectionStart;
            }

            set
            {
                this.selectionStart = value;
                this.NotifyPropertyChanged(nameof(this.SelectionStart));
            }
        }

        /// <summary>
        /// Gets or sets the length of the selection.
        /// </summary>
        /// <value>The length of the selection.</value>
        public int SelectionLength
        {
            get
            {
                return this.selectionLength;
            }

            set
            {
                this.selectionLength = value;
                this.NotifyPropertyChanged(nameof(this.SelectionLength));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is virtual.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is virtual; otherwise, <c>false</c>.
        /// </value>
        public bool IsVirtual
        {
            get
            {
                return string.IsNullOrEmpty(this.filename);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is physical.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is physical; otherwise, <c>false</c>.
        /// </value>
        public bool IsPhysical
        {
            get
            {
                return !this.IsVirtual;
            }
        }

        /// <summary>
        /// Gets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public int FontSize
        {
            get
            {
                return ApplicationManager.Settings.FontSize;
            }
        }

        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string FontFamily
        {
            get
            {
                return ApplicationManager.Settings.FontFamily;
            }
        }

        /// <summary>
        /// Gets or sets the syntax highlighting.
        /// </summary>
        /// <value>
        /// The syntax highlighting.
        /// </value>
        public IHighlightingDefinition SyntaxHighlighting
        {
            get
            {
                return this.syntaxHighlighting;
            }
            set
            {
                if (!ApplicationManager.Settings.EnableSyntaxHighlighting)
                {
                    return;
                }

                this.syntaxHighlighting = value;

                if (value != null)
                {
                    this.WorkspaceState.SyntaxHighlightingDefinition = value.Name;
                }

                this.NotifyPropertyChanged(nameof(this.SyntaxHighlighting));
            }
        }

        /// <summary>
        /// Gets the main view model.
        /// </summary>
        /// <value>The main view model.</value>
        public MainViewModel MainViewModel { get; private set; }

        /// <summary>
        /// Gets or sets the state of the workspace.
        /// </summary>
        /// <value>
        /// The state of the workspace.
        /// </value>
        public WorkspaceState WorkspaceState { get; set; }

        /// <summary>
        /// Gets the open file command.
        /// </summary>
        /// <value>The open file command.</value>
        public ICommand OpenFileCommand { get; private set; }

        /// <summary>
        /// Gets the close workspace command.
        /// </summary>
        /// <value>The close workspace command.</value>
        public ICommand CloseWorkspaceCommand { get; private set; }

        /// <summary>
        /// Gets the clear workspace command.
        /// </summary>
        /// <value>The clear workspace command.</value>
        public ICommand ClearWorkspaceCommand { get; private set; }

        /// <summary>
        /// Gets the save command.
        /// </summary>
        /// <value>The save file command.</value>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Gets the save state command.
        /// </summary>
        /// <value>
        /// The save state command.
        /// </value>
        public ICommand SaveStateCommand { get; private set; }

        /// <summary>
        /// Gets the print command.
        /// </summary>
        /// <value>
        /// The print command.
        /// </value>
        public ICommand PrintCommand { get; private set; }

        /// <summary>
        /// Gets the save workspace state command.
        /// </summary>
        /// <value>
        /// The save workspace state command.
        /// </value>
        public ICommand SaveWorkspaceStateCommand { get; private set; }

        /// <summary>
        /// Gets the save file as command.
        /// </summary>
        /// <value>The save file as command.</value>
        public ICommand SaveFileAsCommand { get; private set; }

        /// <summary>
        /// Gets the open file in new tab command.
        /// </summary>
        /// <value>The open file in new tab command.</value>
        public ICommand OpenFileInNewTabCommand { get; private set; }

        /// <summary>
        /// Gets the add new workspace command.
        /// </summary>
        /// <value>The add new workspace command.</value>
        public ICommand AddNewWorkspaceCommand { get; private set; }

        /// <summary>
        /// Gets the hide main window command.
        /// </summary>
        /// <value>The hide main window command.</value>
        public ICommand HideMainWindowCommand { get; private set; }

        /// <summary>
        /// Gets the show settings window command.
        /// </summary>
        /// <value>The show settings window command.</value>
        public ICommand ShowSettingsWindowCommand { get; private set; }

        /// <summary>
        /// Gets or sets the print action.
        /// </summary>
        /// <value>
        /// The print action.
        /// </value>
        public Action PrintAction { get; set; }

        /// <summary>
        /// Resets the selection.
        /// </summary>
        public void ResetSelection()
        {
            this.NotifyPropertyChanged(nameof(this.SelectionStart));
            this.NotifyPropertyChanged(nameof(this.SelectionLength));
        }

        /// <summary>
        /// Refreshes the UI.
        /// </summary>
        public void RefreshUI()
        {
            this.NotifyPropertyChanged(nameof(this.FontSize));
            this.NotifyPropertyChanged(nameof(this.FontFamily));
            this.NotifyPropertyChanged(nameof(this.IsSyntaxHighlightingEnabled));
        }

        /// <summary>
        /// Notifies the workspace collection change.
        /// </summary>
        public void NotifyWorkspaceCollectionChange()
        {
            this.NotifyPropertyChanged(nameof(this.IsCloseButtonVisible));
        }

        /// <summary>
        /// Closes the workspace.
        /// </summary>
        public void CloseWorkspace()
        {
            var proceed = this.CheckForChanges();

            if (!proceed)
            {
                return;
            }

            this.MainViewModel.RemoveWorkspace(this);
        }

        /// <summary>
        /// Clears the workspace.
        /// </summary>
        /// <returns>A value indicating whether the workspace was cleared.</returns>
        public bool ClearWorkspace()
        {
            var proceed = this.CheckForChanges();

            if (!proceed)
            {
                return false;
            }

            return this.Reset();
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        /// <returns>A value indicating whether the workspace was reset.</returns>
        public bool Reset()
        {
            var workspaceStateFilePath = Path.Combine(ApplicationManager.Application.DataDirectory, "Workspaces", Path.GetRandomFileName());
                
            this.WorkspaceState = new WorkspaceState(workspaceStateFilePath);
            this.syntaxHighlighting = this.SyntaxHighlightingDefinitions.FirstOrDefault(l => l.Name == (this.WorkspaceState.SyntaxHighlightingDefinition ?? "Text"));

            this.Label = string.Empty;
            this.SelectionStart = 0;
            this.SelectionLength = 0;
            this.Filename = null;
            this.Document = new TextDocument();
            this.Document.TextChanged += this.OnDocumentTextChanged;

            this.HasChanges = false;
            this.WorkspaceState.ResetChangeState(true);

            return true;
        }

        /// <summary>
        /// Save file.
        /// </summary>
        /// <returns>Whether the file was saved.</returns>
        public bool Save()
        {
            if (this.IsVirtual)
            {
                if (!this.SaveFileAs())
                {
                    return false;
                }
            }
            else
            {
                // Save the file.
                File.WriteAllText(this.filename, this.content);

                // Reset the change tracking property.
                this.HasChanges = false;
            }

            return this.SaveWorkspaceState();
        }

        /// <summary>
        /// Prints this instance.
        /// </summary>
        public void Print()
        {
            using (new ApplicationWindowSession(ApplicationManager.MainWindow, true))
            {
                var dialog = new System.Windows.Controls.PrintDialog();

                var dialogResult = dialog.ShowDialog() ?? false;
                if (!dialogResult)
                {
                    return;
                }

                var flowDocument = new FlowDocument();

                var lines = this.Content.Split('\n');

                foreach (var line in lines)
                {
                    var myParagraph = new Paragraph { Margin = new Thickness(0) };
                    myParagraph.Inlines.Add(new Run(line));
                    flowDocument.Blocks.Add(myParagraph);
                }

                var paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;

                dialog.PrintDocument(paginator, this.FullDisplayName);
            }
        }

        /// <summary>
        /// Saves the state.
        /// </summary>
        /// <returns>Whether the state was saved.</returns>
        public bool SaveState()
        {
            return this.SaveWorkspaceState();
        }

        /// <summary>
        /// Saves the state of the workspace.
        /// </summary>
        /// <returns>Whether the state was saved.</returns>
        public bool SaveWorkspaceState()
        {
            this.MainViewModel.SaveWorkspaceState(this.WorkspaceState);

            if (this.IsVirtual)
            {
                // Reset the change tracking property.
                this.HasChanges = false;    
            }

            return true;
        }

        /// <summary>
        /// Save file as.
        /// </summary>
        /// <returns>Whether the file was saved.</returns>
        public bool SaveFileAs()
        {
            var saveFileDialog = new SaveFileDialog
                {
                    DefaultExt = ".txt",
                    Filter = "Text-Files (*.txt)|*.txt",
                    AddExtension = true,
                    OverwritePrompt = true
                };

            var result = saveFileDialog.ShowDialog();

            if (!result.HasValue || !result.Value)
            {
                return false;
            }

            var fileName = saveFileDialog.FileName;

            // Save the file.
            File.WriteAllText(fileName, this.Content);

            // Update the file name and reset the change tracking property.
            this.Filename = fileName;
            this.HasChanges = false;

            return this.SaveWorkspaceState();
        }

        /// <summary>
        /// Loads the specified file.
        /// </summary>
        /// <param name="filenameToLoad">The filename to load.</param>
        public void LoadFile(string filenameToLoad)
        {
            var fileContents = File.ReadAllText(filenameToLoad);

            this.Filename = filenameToLoad;
            this.Document.Text = fileContents;

            var highlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(this.Filename));
            if (highlighting != null)
            {
                this.SyntaxHighlighting = highlighting;
            }

            this.WorkspaceState.ResetChangeState();
            this.HasChanges = false;
        }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        public void OpenFile()
        {
            var openFileDialog = new OpenFileDialog
                {
                    DefaultExt = ".txt",
                    Filter = "Text-Files (*.txt)|*.txt"
                };

            var result = openFileDialog.ShowDialog();

            if (result == true)
            {
                this.LoadFile(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Opens the file in new tab.
        /// </summary>
        public void OpenFileInNewTab()
        {
            var openFileDialog = new OpenFileDialog
                {
                    DefaultExt = ".txt",
                    Filter = "Text-Files (*.txt)|*.txt"
                };

            var result = openFileDialog.ShowDialog();

            if (result == true)
            {
                var workspace = this.MainViewModel.AddWorkspace();
                workspace.LoadFile(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Hides the main window.
        /// </summary>
        public void HideMainWindow()
        {
            this.MainViewModel.Hide();
        }

        /// <summary>
        /// Shows the settings window.
        /// </summary>
        public void ShowSettingsWindow()
        {
            var view = new Views.SettingsView();
            var viewModel = new SettingsViewModel();
            viewModel.RequestClose += view.Close;
            viewModel.RequestApplySettings += this.MainViewModel.ApplySettings;
            view.DataContext = viewModel;
            view.ShowDialog();
            viewModel.RequestApplySettings -= this.MainViewModel.ApplySettings;
            viewModel.RequestClose -= view.Close;
        }

        /// <summary>
        /// Checks for changes.
        /// </summary>
        /// <returns>A value indicating whether the execution should proceed.</returns>
        public bool CheckForChanges()
        {
            if (this.IsVirtual)
            {
                if (this.HasChanges || this.WorkspaceState.ChangeState != ChangeState.New)
                {
                    var question = "The workspace has not been saved to a permanent file."
                                       + Environment.NewLine + "If you continue without saving, all changes will be lost."
                                       + Environment.NewLine + Environment.NewLine + "Save to file now?";
                    
                    var result = MessageBox.Show(Application.Current.MainWindow, question, "QuickNote", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Cancel)
                    {
                        return false;
                    }

                    if (result == MessageBoxResult.Yes)
                    {
                        return this.SaveFileAs();
                    }
                }
            }
            else
            {
                if (this.HasChanges)
                {
                    var question = "The text in the " + this.FullDisplayName + " file has changed."
                                   + Environment.NewLine + Environment.NewLine + "Do you want to save changes?";

                    var result = MessageBox.Show(Application.Current.MainWindow, question, "QuickNote", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Cancel)
                    {
                        return false;
                    }

                    if (result == MessageBoxResult.Yes)
                    {
                        return this.Save();
                    }
                }
            }

            return true;
        }

        // Not allowed in true MVVM fasion, but who gives a damn?
        private void OnDocumentTextChanged(object sender, EventArgs e)
        {
            var document = sender as TextDocument;
            if (document == null)
            {
                return;
            }

            this.Content = document.Text;
            this.HasChanges = true;
        }
    }
}
