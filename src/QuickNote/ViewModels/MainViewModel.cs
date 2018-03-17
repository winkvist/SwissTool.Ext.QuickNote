// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The main view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Timers;
    using System.Windows.Forms;

    using SwissTool.Ext.QuickNote.Managers;
    using SwissTool.Ext.QuickNote.Models;
    using SwissTool.Framework.Enums;
    using SwissTool.Framework.UI.Infrastructure;
    using SwissTool.Framework.Utilities.Serialization;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// The main view model.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// The workspace states filename.
        /// </summary>
        private const string WorkspaceStatesFilename = "workspacestates.json";

        /// <summary>
        /// The save timer
        /// </summary>
        private readonly Timer saveTimer;

        /// <summary>
        /// The current workspace.
        /// </summary>
        private WorkspaceViewModel currentWorkspace;

        /// <summary>
        /// The window opacity.
        /// </summary>
        private double windowOpacity;

        /// <summary>
        /// The search string
        /// </summary>
        private string searchString;

        /// <summary>
        /// The is reordering
        /// </summary>
        private bool isReordering;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            this.saveTimer = new Timer(ApplicationManager.Settings.WorkspaceStateSaveInterval * 60000);
            this.saveTimer.Elapsed += this.SaveTimerElapsed;

            this.ApplySettings();

            this.Workspaces = new ObservableCollection<WorkspaceViewModel>();
            
            // Add one workspace.
            this.AddWorkspace();
        }

        /// <summary>
        /// The workspace changed event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public delegate void WorkspaceChangedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Occurs when [workspace changed].
        /// </summary>
        public event WorkspaceChangedEventHandler WorkspaceChanged;

        /// <summary>
        /// Occurs when [request hide].
        /// </summary>
        public event Action RequestHide;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is reordering.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is reordering; otherwise, <c>false</c>.
        /// </value>
        public bool IsReordering
        {
            get
            {
                return this.isReordering;
            }

            set
            {
                this.isReordering = value;
                this.NotifyPropertyChanged(nameof(this.IsReordering));
            }
        }

        /// <summary>
        /// Gets or sets the workspaces.
        /// </summary>
        /// <value>The workspaces.</value>
        public ObservableCollection<WorkspaceViewModel> Workspaces { get; set; }

        /// <summary>
        /// Gets or sets the current workspace.
        /// </summary>
        /// <value>The current workspace.</value>
        public WorkspaceViewModel CurrentWorkspace
        {
            get
            {
                return this.currentWorkspace;
            } 

            set
            {
                ApplicationManager.ReflectSelectionChanges = false;

                try
                {
                    this.PreviousWorkspace = this.currentWorkspace;
                    this.currentWorkspace = value;

                    this.NotifyPropertyChanged(nameof(this.CurrentWorkspace));
                    this.NotifyPropertyChanged(nameof(this.CurrentWorkspaceIndex));
                }
                finally
                {
                    ApplicationManager.ReflectSelectionChanges = true;
                }

                this.WorkspaceChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Gets or sets the window opacity.
        /// </summary>
        /// <value>The window opacity.</value>
        public double WindowOpacity
        {
            get
            {
                return this.windowOpacity;
            }

            set
            {
                this.windowOpacity = value;
                this.NotifyPropertyChanged(nameof(this.WindowOpacity));
            }
        }

        /// <summary>
        /// Gets the width of the window.
        /// </summary>
        /// <value>
        /// The width of the window.
        /// </value>
        public double WindowWidth
        {
            get
            {
                return ApplicationManager.Settings.WindowWidth;
            }
        }

        /// <summary>
        /// Gets the height of the window.
        /// </summary>
        /// <value>
        /// The height of the window.
        /// </value>
        public double WindowHeight
        {
            get
            {
                return ApplicationManager.Settings.WindowHeight;
            }
        }

        /// <summary>
        /// Gets or sets the window position x.
        /// </summary>
        /// <value>
        /// The window position x.
        /// </value>
        public double WindowPositionX
        {
            get
            {
                return ApplicationManager.Settings.WindowPositionX;
            }

            set
            {
                ApplicationManager.Settings.WindowPositionX = value;
                this.NotifyPropertyChanged(nameof(this.WindowPositionX));
            }
        }

        /// <summary>
        /// Gets or sets the window position y.
        /// </summary>
        /// <value>
        /// The window position y.
        /// </value>
        public double WindowPositionY
        {
            get
            {
                return ApplicationManager.Settings.WindowPositionY;
            }

            set
            {
                ApplicationManager.Settings.WindowPositionY = value;
                this.NotifyPropertyChanged(nameof(this.WindowPositionY));
            }
        }

        /// <summary>
        /// Gets or sets the recent workspace.
        /// </summary>
        /// <value>The recent workspace.</value>
        public WorkspaceViewModel PreviousWorkspace { get; set; }

        /// <summary>
        /// Gets or sets the index of the current workspace.
        /// </summary>
        /// <value>The index of the current workspace.</value>
        public int CurrentWorkspaceIndex
        { 
            get
            {
                return (this.Workspaces != null && this.CurrentWorkspace != null)
                           ? this.Workspaces.IndexOf(this.CurrentWorkspace)
                           : -1;
            }

            set
            {
                if (this.CurrentWorkspaceIndex == value)
                {
                    return;
                }

                if (value != -1)
                {
                    this.CurrentWorkspace = this.Workspaces[value];
                }

                this.NotifyPropertyChanged(nameof(this.CurrentWorkspaceIndex));
            }
        }

        /// <summary>
        /// Gets the number of pages.
        /// </summary>
        /// <value>The number of pages.</value>
        public int NumberOfPages 
        { 
            get
            {
                return this.Workspaces.Count;
            } 
        }

        /// <summary>
        /// Gets the workspace states file location.
        /// </summary>
        /// <value>The workspace states file location.</value>
        public string WorkspaceStatesFileLocation
        {
            get
            {
                return Path.Combine(ApplicationManager.Application.DataDirectory, WorkspaceStatesFilename);
            }
        }

        /// <summary>
        /// Gets or sets the search string.
        /// </summary>
        /// <value>
        /// The search string.
        /// </value>
        public string SearchString
        {
            get
            {
                return this.searchString;
            }

            set
            {
                this.searchString = value;
                this.NotifyPropertyChanged(nameof(this.SearchString));
            }
        }

        /// <summary>
        /// Determines whether the specified workspace view model is the current.
        /// </summary>
        /// <param name="workspaceViewModel">The workspace view model.</param>
        /// <returns>
        /// <c>true</c> if the view model is the current; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrentWorkspace(WorkspaceViewModel workspaceViewModel)
        {
            return this.currentWorkspace == workspaceViewModel;
        }

        /// <summary>
        /// Adds a new workspace.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <returns>The newly added workspace.</returns>
        public WorkspaceViewModel AddWorkspace(WorkspaceViewModel workspace = null)
        {
            // Create a new workspace.
            var workspaceViewModel = workspace ?? new WorkspaceViewModel(this);
            
            // Add the new workspace to the main collection.
            this.Workspaces.Add(workspaceViewModel);

            // Sets the newly created workspace as current.
            this.CurrentWorkspace = workspaceViewModel;

            this.NotifyWorkspaceCollectionChanged();

            return workspaceViewModel;
        }

        /// <summary>
        /// Adds the workspace.
        /// </summary>
        /// <param name="workspaceState">State of the workspace.</param>
        /// <param name="content">The content.</param>
        /// <returns>
        /// The newly added workspace.
        /// </returns>
        public WorkspaceViewModel AddWorkspace(WorkspaceState workspaceState, string content)
        {
            var hasChanges = workspaceState.HasChanges;

            var workspace = this.AddWorkspace(
                    new WorkspaceViewModel(this)
                        {
                            Filename = workspaceState.SourceFilename,
                            Label = workspaceState.Label,
                            WorkspaceState = workspaceState,
                        });

            var highlighting = workspace.SyntaxHighlightingDefinitions.FirstOrDefault(s => s.Name == workspaceState.SyntaxHighlightingDefinition);
            if (highlighting != null)
            {
                workspace.SyntaxHighlighting = highlighting;
            }

            workspace.Document.Text = content;
            workspace.HasChanges = hasChanges;

            return workspace;
        }

        /// <summary>
        /// Removes a specific workspace.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public void RemoveWorkspace(WorkspaceViewModel workspace)
        {
            var currentIndex = this.CurrentWorkspaceIndex;

            // Remove the workspace from the main workspace collection.
            this.Workspaces.Remove(workspace);

            this.DeleteWorkspaceState(workspace.WorkspaceState);

            if (this.Workspaces.Count == 0)
            {
                this.AddWorkspace();
            }

            this.NotifyWorkspaceCollectionChanged();

            // Goes to the previous workspace.
            this.GoToPreviousWorkspace(currentIndex);
        }

        /// <summary>
        /// Goes to next workspace.
        /// </summary>
        /// <param name="fromIndex">From index.</param>
        public void GoToNextWorkspace(int? fromIndex = null)
        {
            var index = fromIndex ?? this.CurrentWorkspaceIndex;

            this.CurrentWorkspaceIndex = (index + 1) % this.Workspaces.Count;
        }

        /// <summary>
        /// Goes to previous workspace.
        /// </summary>
        /// <param name="fromIndex">From index.</param>
        public void GoToPreviousWorkspace(int? fromIndex = null)
        {
            var index = fromIndex ?? this.CurrentWorkspaceIndex;
            var previousIndex = index - 1;

            if (previousIndex < 0)
            {
                previousIndex = this.Workspaces.Count - 1;
            }

            this.CurrentWorkspaceIndex = previousIndex;
        }

        /// <summary>
        /// Hides this instance.
        /// </summary>
        public void Hide()
        {
            this.RequestHide?.Invoke();
        }

        /// <summary>
        /// Saves the workspace states.
        /// </summary>
        public void SaveWorkspaceStates()
        {
            var workspaceStates = new List<WorkspaceState>();
            var workspaceStatesWasChanged = false;

            foreach (var workspace in this.Workspaces)
            {
                if (workspace.WorkspaceState.ChangeState == ChangeState.New)
                {
                    // New, unmodified. No need to save.
                    continue;
                }

                if (workspace.WorkspaceState.ChangeState != ChangeState.Unmodified)
                {
                    workspaceStatesWasChanged = true;

                    File.WriteAllText(workspace.WorkspaceState.TempStorageFilename, workspace.Content);
                }

                workspaceStates.Add(workspace.WorkspaceState);
            }
            
            if (workspaceStatesWasChanged)
            {
                JsonUtils.SerializeJsonFile(this.WorkspaceStatesFileLocation, workspaceStates);
                
                foreach (var workspaceState in workspaceStates)
                {
                    workspaceState.ResetChangeState();
                }
            }
        }

        /// <summary>
        /// Saves the state of the workspace.
        /// </summary>
        /// <param name="workspaceState">State of the workspace.</param>
        public void SaveWorkspaceState(WorkspaceState workspaceState)
        {
            var workspaceStates = new List<WorkspaceState>();

            foreach (var workspace in this.Workspaces)
            {
                if (workspace.WorkspaceState == workspaceState)
                {
                    // Save the new change status.
                    workspaceState.HasChanges = false;

                    var targetFileName = workspace.WorkspaceState.TempStorageFilename;
                    var targetDirectory = Path.GetDirectoryName(targetFileName);

                    if (targetDirectory == null)
                    {
                        throw new ArgumentNullException(nameof(targetDirectory));
                    }

                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    File.WriteAllText(targetFileName, workspace.Content);
                }

                workspaceStates.Add(workspace.WorkspaceState);
            }
            
            JsonUtils.SerializeJsonFile(this.WorkspaceStatesFileLocation, workspaceStates);

            workspaceState.ResetChangeState();
        }

        /// <summary>
        /// Deletes the state of the workspace.
        /// </summary>
        /// <param name="workspaceState">State of the workspace.</param>
        public void DeleteWorkspaceState(WorkspaceState workspaceState)
        {
            if (workspaceState.ChangeState == ChangeState.New)
            {
                return;
            }
            
            if (!string.IsNullOrEmpty(workspaceState.TempStorageFilename) && File.Exists(workspaceState.TempStorageFilename))
            {
                File.Delete(workspaceState.TempStorageFilename);
            }

            var workspaceStates = this.Workspaces.Select(workspace => workspace.WorkspaceState).ToList();

            JsonUtils.SerializeJsonFile(this.WorkspaceStatesFileLocation, workspaceStates);
        }
        
        /// <summary>
        /// Loads the workspace states and creates for the ones missing.
        /// </summary>
        public void LoadWorkspaceStates()
        {
            if (!File.Exists(this.WorkspaceStatesFileLocation))
            {
                return;
            }

            this.Workspaces.Clear();

            var workspaceStates = JsonUtils.DeserializeJsonFile<List<WorkspaceState>>(this.WorkspaceStatesFileLocation);

            foreach (var workspaceState in workspaceStates)
            {
                if (!string.IsNullOrEmpty(workspaceState.SourceFilename) && !File.Exists(workspaceState.SourceFilename))
                {
                    continue;
                }

                if (!File.Exists(workspaceState.TempStorageFilename))
                {
                    continue;
                }

                var tempFileContent = File.ReadAllText(workspaceState.TempStorageFilename);

                this.AddWorkspace(workspaceState, tempFileContent);
            }

            if (!this.Workspaces.Any())
            {
                this.AddWorkspace();
            }
        }

        /// <summary>
        /// Applies the settings.
        /// </summary>
        public void ApplySettings()
        {
            this.WindowOpacity = ApplicationManager.Settings.WindowOpacity;

            this.saveTimer.Interval = ApplicationManager.Settings.WorkspaceStateSaveInterval * 60000;
            
            if (ApplicationManager.Settings.MaintainWorkspaceStates)
            {
                this.saveTimer.Start();
            }

            this.CurrentWorkspace?.RefreshUI();

            if (ApplicationManager.Settings.WindowPositionX <= 0 && ApplicationManager.Settings.WindowPositionY <= 0)
            {
                var height = Screen.PrimaryScreen.Bounds.Height;
                var width = Screen.PrimaryScreen.Bounds.Width;

                this.WindowPositionX = (width - this.WindowWidth) / 2;
                this.WindowPositionY = (height - this.WindowHeight) / 2;
            }
        }

        /// <summary>
        /// Notifies the workspace collection changed.
        /// </summary>
        private void NotifyWorkspaceCollectionChanged()
        {
            foreach (var workspace in this.Workspaces)
            {
                workspace.NotifyWorkspaceCollectionChange();
            }
        }

        /// <summary>
        /// Saves the timer elapsed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void SaveTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.SaveWorkspaceStates();
            }
            catch
            {
                var host = ApplicationManager.Application.Host;

                host?.ShowBalloonToolTip("Save workspaces failed", "QuickNote was unable to save workspace states.", BalloonIcon.Warning);
            }
        }
    }
}
