// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationManager.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The application manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using SwissTool.Framework.UI.Enums;
using SwissTool.Framework.UI.Managers;

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

        internal static HighlightingManager HighlightingManager { get; private set; }

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

            SetupHightlightings();
        }

        private static void SetupHightlightings()
        {
            var uiHint = WindowManager.CurrentTheme.UiHint;

            var highlightingManager = new HighlightingManager();

            highlightingManager.RegisterHighlighting("None", new string[] { }, GetHighlightingDefinition(uiHint, "None.xshd"));
            
            highlightingManager.RegisterHighlighting("ActionScript3", new[] { ".as" }, GetHighlightingDefinition(uiHint, "AS3.xshd"));
            highlightingManager.RegisterHighlighting("ASP/XHTML", new[] { ".asp", ".aspx", ".asax", ".asmx", ".ascx", ".master" }, GetHighlightingDefinition(uiHint, "ASPX.xshd"));
            highlightingManager.RegisterHighlighting("BAT", new[] { ".bat", ".dos" }, GetHighlightingDefinition(uiHint, "DOSBATCH.xshd"));
            highlightingManager.RegisterHighlighting("Boo", new[] { ".boo" }, GetHighlightingDefinition(uiHint, "Boo.xshd"));
            highlightingManager.RegisterHighlighting("C#", new[] { ".cs" }, GetHighlightingDefinition(uiHint, "CSharp-Mode.xshd"));
            highlightingManager.RegisterHighlighting("C++", new[] { ".c", ".h", ".cc", ".cpp", ".hpp" }, GetHighlightingDefinition(uiHint, "CPP-Mode.xshd"));
            highlightingManager.RegisterHighlighting("Coco", new[] { ".atg" }, GetHighlightingDefinition(uiHint, "Coco-Mode.xshd"));
            highlightingManager.RegisterHighlighting("CSS", new[] { ".css" }, GetHighlightingDefinition(uiHint, "CSS-Mode.xshd"));
            highlightingManager.RegisterHighlighting("F#", new[] { ".fs" }, GetHighlightingDefinition(uiHint, "FSharp-Mode.xshd"));
            highlightingManager.RegisterHighlighting("HLSL", new[] { ".fx" }, GetHighlightingDefinition(uiHint, "HLSL.xshd"));
            highlightingManager.RegisterHighlighting("HTML", new[] { ".htm", ".html" }, GetHighlightingDefinition(uiHint, "HTML-Mode.xshd"));
            highlightingManager.RegisterHighlighting("INI", new[] { ".cfg", ".conf", ".ini", ".iss" }, GetHighlightingDefinition(uiHint, "INI.xshd"));
            highlightingManager.RegisterHighlighting("Java", new[] { ".java" }, GetHighlightingDefinition(uiHint, "Java-Mode.xshd"));
            highlightingManager.RegisterHighlighting("JavaScript", new[] { ".js" }, GetHighlightingDefinition(uiHint, "JavaScript-Mode.xshd"));
            highlightingManager.RegisterHighlighting("Json", new[] { ".json" }, GetHighlightingDefinition(uiHint, "Json.xshd"));
            highlightingManager.RegisterHighlighting("LOG", new[] { ".log" }, GetHighlightingDefinition(uiHint, "Log.xshd"));
            highlightingManager.RegisterHighlighting("MarkDown", new[] { ".md" }, GetHighlightingDefinition(uiHint, "MarkDown-Mode.xshd"));
            highlightingManager.RegisterHighlighting("Pascal", new[] { ".pas" }, GetHighlightingDefinition(uiHint, "Pascal.xshd"));
            highlightingManager.RegisterHighlighting("Patch", new[] { ".patch", ".diff" }, GetHighlightingDefinition(uiHint, "Patch-Mode.xshd"));
            highlightingManager.RegisterHighlighting("PHP", new[] { ".php" }, GetHighlightingDefinition(uiHint, "PHP-Mode.xshd"));
            highlightingManager.RegisterHighlighting("PLSQL", new[] { ".plsql" }, GetHighlightingDefinition(uiHint, "PLSQL.xshd"));
            highlightingManager.RegisterHighlighting("PowerShell", new[] { ".ps1", ".psm1", ".psd1" }, GetHighlightingDefinition(uiHint, "PowerShell.xshd"));
            highlightingManager.RegisterHighlighting("Python", new[] { ".py", ".pyw" }, GetHighlightingDefinition(uiHint, "Python-Mode.xshd"));
            highlightingManager.RegisterHighlighting("Ruby", new[] { ".rb" }, GetHighlightingDefinition(uiHint, "Ruby.xshd"));
            highlightingManager.RegisterHighlighting("Scheme", new[] { ".sls", ".sps", ".ss", ".scm" }, GetHighlightingDefinition(uiHint, "scheme.xshd"));
            highlightingManager.RegisterHighlighting("Squirrel", new[] { ".nut" }, GetHighlightingDefinition(uiHint, "squirrel.xshd"));
            highlightingManager.RegisterHighlighting("TeX", new[] { ".tex" }, GetHighlightingDefinition(uiHint, "Tex-Mode.xshd"));
            highlightingManager.RegisterHighlighting("TSQL", new[] { ".sql" }, GetHighlightingDefinition(uiHint, "TSQL-Mode.xshd"));
            highlightingManager.RegisterHighlighting("TXT", new[] { ".txt" }, GetHighlightingDefinition(uiHint, "TXT.xshd"));
            highlightingManager.RegisterHighlighting("VB", new[] { ".vb" }, GetHighlightingDefinition(uiHint, "VB-Mode.xshd"));
            highlightingManager.RegisterHighlighting("VTL", new[] { ".vtl", ".vm" }, GetHighlightingDefinition(uiHint, "vtl.xshd"));
            highlightingManager.RegisterHighlighting("XML", (".xml;.xsl;.xslt;.xsd;.manifest;.config;.addin;" +
                                                             ".xshd;.wxs;.wxi;.wxl;.proj;.csproj;.vbproj;.ilproj;" +
                                                             ".booproj;.build;.xfrm;.targets;.xaml;.xpt;" +
                                                             ".xft;.map;.wsdl;.disco;.ps1xml;.nuspec").Split(';'),
                GetHighlightingDefinition(uiHint, "XML-Mode.xshd"));
            highlightingManager.RegisterHighlighting("XmlDoc", null, GetHighlightingDefinition(uiHint, "XmlDoc.xshd"));

            HighlightingManager = highlightingManager;
        }

        private static IHighlightingDefinition GetHighlightingDefinition(UiHint uiHint, string resourceName)
        {
            using (var stream = typeof(ApplicationManager).Assembly.GetManifestResourceStream($"SwissTool.Ext.QuickNote.Resources.Highlighting.{uiHint}.{resourceName}"))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Could not find embedded resource");
                }

                using (XmlReader reader = new XmlTextReader(stream))
                {
                    return HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
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
