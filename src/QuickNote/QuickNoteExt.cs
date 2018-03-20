// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickNoteExt.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The QuickNote extension
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SwissTool.Ext.QuickNote
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Xml;

    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;

    using SwissTool.Ext.QuickNote.Managers;
    using SwissTool.Ext.QuickNote.Models;
    using SwissTool.Ext.QuickNote.ViewModels;
    using SwissTool.Framework.Commanding;
    using SwissTool.Framework.Enums;
    using SwissTool.Framework.Infrastructure;
    using SwissTool.Framework.UI.Managers;

    /// <summary>
    /// The QuickNote extension
    /// </summary>
    public class QuickNoteExt : ExtensionBase
    {
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public AppSettings Settings { get; private set; }
        
        /// <summary>
        /// Gets or sets the main view model.
        /// </summary>
        /// <value>The main view model.</value>
        internal MainViewModel MainViewModel { get; set; }

        /// <summary>
        /// Gets or sets the main view.
        /// </summary>
        /// <value>The main view.</value>
        private Views.MainView MainView { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            this.InitiateMenuItems();
            this.InitiateActions();

            this.Icon = new BitmapImage(new Uri("/QuickNote;component/Images/App.png", UriKind.RelativeOrAbsolute));

            this.Settings = this.LoadConfiguration<AppSettings>();

            ApplicationManager.Setup(this, this.Settings);

            IHighlightingDefinition customHighlighting;
            using (var s = this.GetType().Assembly.GetManifestResourceStream("SwissTool.Ext.QuickNote.Controls.PlainTextHighlighting.xshd"))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not find embedded resource");
                }

                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            HighlightingManager.Instance.RegisterHighlighting("Text", new string[] { }, customHighlighting);

            var viewModel = new MainViewModel();
            var view = new Views.MainView { DataContext = viewModel };
            viewModel.RequestHide += view.Hide;
            
            this.MainView = view;
            this.MainViewModel = viewModel;

            ApplicationManager.MainWindow = this.MainView;
            
            if (ApplicationManager.Settings.MaintainWorkspaceStates)
            {
                try
                {
                    viewModel.LoadWorkspaceStates();
                }
                catch (Exception ex)
                {

                    MessageBox.Show(string.Format("Unable to load the previous workspace states: {0}", ex.Message), "Loading failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        /// <summary>
        /// Shows the main window.
        /// </summary>
        public void ShowHideMainWindow()
        {
            if (this.MainView.IsVisible)
            {
                this.MainView.Hide();
            }
            else
            {
                this.MainView.Show();
                WindowManager.SetFocus(this.MainView);
            }
        }

        /// <summary>
        /// Shows the about window.
        /// </summary>
        public void ShowAboutWindow()
        {
            var model = new AboutModel(Assembly.GetExecutingAssembly());

            WindowManager.ShowDialog<Views.AboutView>(new AboutViewModel { Model = model });
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
        /// Initiates the menu items.
        /// </summary>
        private void InitiateMenuItems()
        {
            this.AddMenuItems(
                new ExtensionMenuItem
                    {
                        Name = "Show/hide window",
                        Description = "Shows or hides the main window",
                        Icon = new BitmapImage(new Uri(this.GetIconPath("visible"), UriKind.RelativeOrAbsolute)),
                        Command = new RelayCommand((o) => this.ShowHideMainWindow())
                    },
                new ExtensionMenuItem
                    {
                        Name = "Settings",
                        Description = "Displays the settings window",
                        Icon = new BitmapImage(new Uri(this.GetIconPath("services"), UriKind.RelativeOrAbsolute)),
                        Command = new RelayCommand((o) => this.ShowSettingsWindow())
                    },
                new ExtensionMenuItem
                    {
                        Name = "About",
                        Description = "Displays the about window",
                        Icon = new BitmapImage(new Uri(this.GetIconPath("help"), UriKind.RelativeOrAbsolute)),
                        Command = new RelayCommand((o) => this.ShowAboutWindow())
                    });
        }

        /// <summary>
        /// Gets the icon path.
        /// </summary>
        /// <param name="iconName">Name of the icon.</param>
        /// <returns>The icon path.</returns>
        private string GetIconPath(string iconName)
        {
            var uiHint = WindowManager.CurrentTheme.UiHint;
            var path = $"/QuickNote;component/Resources/Icons/{uiHint}/24x24/{iconName}.png";

            return path;
        }

        /// <summary>
        /// Initiates the actions.
        /// </summary>
        private void InitiateActions()
        {
            this.AddActions(
                new ExtensionHotKeyAction
                    {
                        Identifier = "ShowHideWindowHotKey",
                        Name = "Show/hide window",
                        Description = "Shows or hides the main window",
                        Icon = new BitmapImage(new Uri("/QuickNote;component/Images/Switch.png", UriKind.RelativeOrAbsolute)),
                        Command = new RelayCommand(o => this.ShowHideMainWindow()),
                        DefaultHotKey = new ActionHotKey(HotKeyModifier.Control, HotKeyModifier.None, HotKey.Q)
                    });
        }
    }
}
