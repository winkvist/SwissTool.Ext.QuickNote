// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Media;

    using SwissTool.Ext.QuickNote.Managers;
    using SwissTool.Ext.QuickNote.Models;
    using SwissTool.Framework.Commanding;
    using SwissTool.Framework.UI.Infrastructure;
    using SwissTool.Framework.Utilities.Serialization;

    /// <summary>
    /// The settings view model.
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            this.AcceptCommand = new RelayCommand(o => this.Accept());
            this.ResetWindowLocationCommand = new RelayCommand(o => this.ResetWindowLocation());

            this.Settings = ApplicationManager.Settings;
            this.SettingsCopy = JsonUtils.Clone(ApplicationManager.Settings);
        }

        /// <summary>
        /// Occurs when [request apply settings].
        /// </summary>
        public event Action RequestApplySettings;

        /// <summary>
        /// Gets or sets the accept command.
        /// </summary>
        /// <value>The accept command.</value>
        public ICommand AcceptCommand { get; set; }

        /// <summary>
        /// Gets or sets the reset window location command.
        /// </summary>
        /// <value>
        /// The reset window location command.
        /// </value>
        public ICommand ResetWindowLocationCommand { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public AppSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the settings copy.
        /// </summary>                                
        /// <value>The settings copy.</value>
        public AppSettings SettingsCopy { get; set; }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public FontFamily FontFamily
        {
            get
            {
                return new FontFamily(this.SettingsCopy.FontFamily);
            }

            set
            {
                this.SettingsCopy.FontFamily = value.FamilyNames.First().Value;
            }
        }

        /// <summary>
        /// Gets the font sizes.
        /// </summary>
        /// <value>
        /// The font sizes.
        /// </value>
        public IEnumerable<int> FontSizes
        {
            get
            {
                for (var i = 1; i <= 20; i++)
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is maintain workspace states enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is maintain workspace states enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsMaintainWorkspaceStatesEnabled
        {
            get
            {
                return this.SettingsCopy.MaintainWorkspaceStates;
            }
        }

        /// <summary>
        /// Gets the window location label.
        /// </summary>
        /// <value>
        /// The window location label.
        /// </value>
        public string WindowLocationLabel
        {
            get
            {
                if (this.SettingsCopy.WindowPositionX <= 0 && this.SettingsCopy.WindowPositionY <= 0)
                {
                    return "[ Center Screen ]";
                }

                return $"[ {this.SettingsCopy.WindowPositionX} , {this.SettingsCopy.WindowPositionY} ]";
            }
        }

        /// <summary>
        /// Accepts this instance.
        /// </summary>
        private void Accept()
        {
            this.SaveChanges();
            this.Close();
        }

        private void ResetWindowLocation()
        {
            this.SettingsCopy.WindowPositionX = 0.0;
            this.SettingsCopy.WindowPositionY = 0.0;

            this.NotifyPropertyChanged(nameof(this.WindowLocationLabel));
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        private void SaveChanges()
        {
            ApplicationManager.Settings.MaintainWorkspaceStates = this.SettingsCopy.MaintainWorkspaceStates;
            ApplicationManager.Settings.WindowOpacity = this.SettingsCopy.WindowOpacity;
            ApplicationManager.Settings.WorkspaceStateSaveInterval = this.SettingsCopy.WorkspaceStateSaveInterval;
            ApplicationManager.Settings.FontSize = this.SettingsCopy.FontSize;
            ApplicationManager.Settings.FontFamily = this.SettingsCopy.FontFamily;
            ApplicationManager.Settings.EnableSyntaxHighlighting = this.SettingsCopy.EnableSyntaxHighlighting;
            ApplicationManager.Settings.WindowPositionX = this.SettingsCopy.WindowPositionX;
            ApplicationManager.Settings.WindowPositionY = this.SettingsCopy.WindowPositionY;
            ApplicationManager.Settings.ShowLineNumbers = this.SettingsCopy.ShowLineNumbers;

            ApplicationManager.SaveSettings();

            this.RequestApplySettings?.Invoke();
        }
    }
}
