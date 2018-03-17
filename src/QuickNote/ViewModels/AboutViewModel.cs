// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutViewModel.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   Defines the AboutViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.ViewModels
{
    using SwissTool.Ext.QuickNote.Models;
    using SwissTool.Framework.UI.Infrastructure;

    /// <summary>
    /// The about view model.
    /// </summary>
    public class AboutViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public AboutModel Model { get; set; }

        /// <summary>
        /// Gets the version display string.
        /// </summary>
        public string VersionDisplayString
        {
            get
            {
                if (this.Model == null)
                {
                    return string.Empty;
                }

                return $"Version {this.Model.Version.Major}.{this.Model.Version.Minor}.{this.Model.Version.Build}";
            }
        }
    }
}
