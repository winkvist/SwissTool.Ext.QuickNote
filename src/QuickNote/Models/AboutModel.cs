// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutModel.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The about model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote.Models
{
    using System;
    using System.Reflection;

    using SwissTool.Framework.Extensions;

    /// <summary>
    /// The about model.
    /// </summary>
    public class AboutModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutModel"/> class.
        /// </summary>
        public AboutModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutModel"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public AboutModel(Assembly assembly)
            : this()
        {
            this.Title = assembly.GetAssemblyName();
            this.Description = assembly.GetAssemblyDescription();
            this.Version = assembly.GetAssemblyVersion();
            this.Copyright = assembly.GetAssemblyCopyright();
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the copyright.
        /// </summary>
        /// <value>
        /// The copyright.
        /// </value>
        public string Copyright { get; set; }
    }
}
