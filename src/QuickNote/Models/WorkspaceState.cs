// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceState.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Defines the WorkspaceState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.Models
{
    /// <summary>
    /// The change state.
    /// </summary>
    public enum ChangeState
    {
        /// <summary>
        /// Indicates a new instance.
        /// </summary>
        New,

        /// <summary>
        /// Indicates that the state has changed.
        /// </summary>
        Modified,

        /// <summary>
        /// Indicates that the state has not changed.
        /// </summary>
        Unmodified
    }

    /// <summary>
    /// The workspace state.
    /// </summary>
    public class WorkspaceState
    {
        /// <summary>
        /// The source filename
        /// </summary>
        private string sourceFilename;

        /// <summary>
        /// The label
        /// </summary>
        private string label;

        /// <summary>
        /// The syntax highlighting definition
        /// </summary>
        private string syntaxHighlightingDefinition;

        /// <summary>
        /// The has changes
        /// </summary>
        private bool hasChanges;

        /// <summary>
        /// The temp storage filename
        /// </summary>
        private string tempStorageFilename;

        /// <summary>
        /// The change state
        /// </summary>
        private ChangeState changeState;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceState"/> class.
        /// </summary>
        public WorkspaceState()
        {
            this.changeState = ChangeState.Unmodified;
            this.SyntaxHighlightingDefinition = "None";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceState"/> class.
        /// </summary>
        /// <param name="tempStorageFilename">The temp storage filename.</param>
        public WorkspaceState(string tempStorageFilename)
            : this()
        {
            this.TempStorageFilename = tempStorageFilename;
            this.changeState = ChangeState.New;
        }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string SourceFilename
        {
            get
            {
                return this.sourceFilename;
            } 
            
            set
            {
                var oldValue = this.sourceFilename;
                var newValue = value;

                this.sourceFilename = value;

                this.MaintainChangeState(oldValue, newValue);
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
                var newValue = value;

                this.label = value;

                this.MaintainChangeState(oldValue, newValue);
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
                var oldValue = this.hasChanges;
                var newValue = value;

                this.hasChanges = value;

                this.MaintainChangeState(oldValue, newValue);
            }
        }

        /// <summary>
        /// Gets or sets the temp storage filename.
        /// </summary>
        /// <value>The temp storage filename.</value>
        public string TempStorageFilename
        {
            get
            {
                return this.tempStorageFilename;
            } 

            set
            {
                var oldValue = this.hasChanges;
                var newValue = value;

                this.tempStorageFilename = value;

                this.MaintainChangeState(oldValue, newValue);
            }
        }

        /// <summary>
        /// Gets or sets the temp storage filename.
        /// </summary>
        /// <value>The temp storage filename.</value>
        public string SyntaxHighlightingDefinition
        {
            get
            {
                return this.syntaxHighlightingDefinition;
            }

            set
            {
                var oldValue = this.syntaxHighlightingDefinition;
                var newValue = value;

                this.syntaxHighlightingDefinition = value;

                this.MaintainChangeState(oldValue, newValue);
            }
        }

        /// <summary>
        /// Gets the change state of the workspace state.
        /// </summary>
        /// <value>
        /// The state of the change.
        /// </value>
        public ChangeState ChangeState
        {
            get
            {
                return this.changeState;
            }
        }

        /// <summary>
        /// Resets the state of the change.
        /// </summary>
        /// <param name="isNew">if set to <c>true</c> [is new].</param>
        public void ResetChangeState(bool isNew = false)
        {
            this.changeState = isNew ? ChangeState.New : ChangeState.Unmodified;
        }

        /// <summary>
        /// Maintains the state of the change.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void MaintainChangeState(object oldValue, object newValue)
        {
            if (oldValue != newValue)
            {
                this.changeState = ChangeState.Modified;
            }
        }
    }
}
