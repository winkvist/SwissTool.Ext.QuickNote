// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The extension settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.Models
{
    /// <summary>
    /// The extension settings class.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettings"/> class.
        /// </summary>
        public AppSettings()
        {
            this.WindowPositionX = 0.0;
            this.WindowPositionY = 0.0;
            this.WindowHeight = 320;
            this.WindowWidth = 700;
            this.MaintainWorkspaceStates = true;
            this.WindowOpacity = 1.0;
            this.WorkspaceStateSaveInterval = 5;
            this.FontFamily = "Consolas";
            this.FontSize = 12;
            this.EnableSyntaxHighlighting = false;
            this.ShowLineNumbers = false;
        }

        /// <summary>
        /// Gets or sets the window position Y.
        /// </summary>
        /// <value>The window position Y.</value>
        public double WindowPositionY { get; set; }

        /// <summary>
        /// Gets or sets the window position X.
        /// </summary>
        /// <value>The window position X.</value>
        public double WindowPositionX { get; set; }

        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        /// <value>The height of the window.</value>
        public double WindowHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        /// <value>The width of the window.</value>
        public double WindowWidth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workspace states should be maintained.
        /// </summary>
        /// <value>
        /// <c>true</c> if maintain workspace states; otherwise, <c>false</c>.
        /// </value>
        public bool MaintainWorkspaceStates { get; set; }

        /// <summary>
        /// Gets or sets the workspace state save interval.
        /// </summary>
        /// <value>
        /// The workspace state save interval.
        /// </value>
        public int WorkspaceStateSaveInterval { get; set; }

        /// <summary>
        /// Gets or sets the window opacity.
        /// </summary>
        /// <value>The window opacity.</value>
        public double WindowOpacity { get; set; }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public int FontSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable syntax highlighting].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable syntax highlighting]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSyntaxHighlighting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show line numbers].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show line numbers]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowLineNumbers { get; set; }
    }
}
