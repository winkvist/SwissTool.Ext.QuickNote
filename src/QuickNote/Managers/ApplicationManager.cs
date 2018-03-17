// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationManager.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The application manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.Managers
{
    using System.Windows;

    using SwissTool.Ext.QuickNote.Models;
    using SwissTool.Framework.Infrastructure;

    /// <summary>
    /// The application manager.
    /// </summary>
    public static class ApplicationManager
    {
        /// <summary>
        /// Just a lock object.
        /// </summary>
        private static readonly object LockObject = new object();

        /// <summary>
        /// The backing field of the setting reflect selection changes.
        /// </summary>
        private static bool reflectSelectionChanges;

        /// <summary>
        /// Initializes static members of the <see cref="ApplicationManager"/> class. 
        /// </summary>
        static ApplicationManager()
        {
            reflectSelectionChanges = true;
        }

        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>The application.</value>
        internal static ApplicationBase Application { get; private set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        internal static AppSettings Settings { get; private set; }

        /// <summary>
        /// Gets or sets the main window.
        /// </summary>
        /// <value>
        /// The main window.
        /// </value>
        internal static Window MainWindow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [reflect selection changes].
        /// </summary>
        /// <value>
        /// <c>true</c> if [reflect selection changes]; otherwise, <c>false</c>.
        /// </value>
        internal static bool ReflectSelectionChanges
        {
            get
            {
                return reflectSelectionChanges;
            }

            set
            {
                lock (LockObject)
                {
                    reflectSelectionChanges = value;
                }
            }
        }

        /// <summary>
        /// Setups the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="settings">The settings.</param>
        internal static void Setup(ApplicationBase application, AppSettings settings)
        {
            Application = application;
            Settings = settings;

            if (Settings.WindowOpacity < 0.5)
            {
                Settings.WindowOpacity = 0.5;
            }
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        internal static void SaveSettings()
        {
            Application.SaveConfiguration(Settings);
        }
    }
}
