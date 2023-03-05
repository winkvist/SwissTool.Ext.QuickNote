// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatternReplaceViewModel.cs" company="Fredrik Winkvist">
//   Copyright (c) Fredrik Winkvist. All rights reserved.
// </copyright>
// <summary>
//   The pattern replace view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Windows;
using SwissTool.Ext.QuickNote.Managers;

namespace SwissTool.Ext.QuickNote.ViewModels
{
    using System.Text;
    using System;
    using System.Windows.Input;
    using SwissTool.Framework.Commanding;
    using SwissTool.Framework.UI.Infrastructure;
    using System.Text.RegularExpressions;
    using System.ComponentModel;
    using ControlzEx.Standard;
    using System.Windows.Threading;

    /// <summary>
    /// The pattern replace view model.
    /// </summary>
    public class PatternReplaceViewModel : ViewModelBase
    {
        /// <summary>
        /// The source text
        /// </summary>
        private string sourceText;

        private string targetText;

        /// <summary>
        /// The pattern
        /// </summary>
        private string patternText;
        
        /// <summary>
        /// The preview text
        /// </summary>
        private string previewText;

        /// <summary>
        /// The compile progress
        /// </summary>
        private int compileProgress;

        private bool isCompileProgressVisible;

        /// <summary>
        /// The column separator
        /// </summary>
        private string columnSeparator = "\\t";

        /// <summary>
        /// The row separator
        /// </summary>
        private string rowSeparator = "\\n";

        private readonly BackgroundWorker backgroundWorker;

        public class TextCompileState
        {
            public Action<string> OnCompleted { get; set; }
            public Action<string> OnError { get; set; }
            public int? MaxLines { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternReplaceViewModel"/> class.
        /// </summary>
        public PatternReplaceViewModel()
        {
            this.AcceptCommand = new RelayCommand(o => this.Accept());

            this.backgroundWorker = new BackgroundWorker();
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            this.backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
            this.backgroundWorker.WorkerSupportsCancellation = true;
        }

        private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.CompileProgress = e.ProgressPercentage;
        }

        /// <summary>
        /// Gets or sets the accept command.
        /// </summary>
        /// <value>The accept command.</value>
        public ICommand AcceptCommand { get; set; }

        /// <summary>
        /// Gets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public int FontSize => ApplicationManager.Settings.FontSize;

        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string FontFamily => ApplicationManager.Settings.FontFamily;

        public event Action RequestPatternReplace;

        /// <summary>
        /// Gets or sets the column separator.
        /// </summary>
        /// <value>
        /// The column separator.
        /// </value>
        public string ColumnSeparator
        {
            get => this.columnSeparator;

            set
            {
                this.columnSeparator = value;
                this.NotifyPropertyChanged(nameof(this.ColumnSeparator));

                this.UpdatePreview();
            }
        }

        /// <summary>
        /// Gets or sets the row separator.
        /// </summary>
        /// <value>
        /// The row separator.
        /// </value>
        public string RowSeparator
        {
            get => this.rowSeparator;

            set
            {
                this.rowSeparator = value;
                this.NotifyPropertyChanged(nameof(this.RowSeparator));

                this.UpdatePreview();
            }
        }

        /// <summary>
        /// Gets or sets the source text.
        /// </summary>
        /// <value>
        /// The source text.
        /// </value>
        public string SourceText
        {
            get => this.sourceText;

            set
            {
                this.sourceText = value;
                this.NotifyPropertyChanged(nameof(this.SourceText));
            }
        }

        public string TargetText
        {
            get => this.targetText;

            set
            {
                this.targetText = value;
                this.NotifyPropertyChanged(nameof(this.TargetText));
            }
        }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>
        /// The pattern.
        /// </value>
        public string PatternText
        {
            get => this.patternText;

            set
            {
                this.patternText = value;
                this.NotifyPropertyChanged(nameof(this.PatternText));

                this.UpdatePreview();
            }
        }

        /// <summary>
        /// Gets or sets the preview text.
        /// </summary>
        /// <value>
        /// The preview text.
        /// </value>
        public string PreviewText
        {
            get => this.previewText;

            set
            {
                this.previewText = value;
                this.NotifyPropertyChanged(nameof(this.PreviewText));
            }
        }

        public int CompileProgress
        {
            get => this.compileProgress;

            set
            {
                this.compileProgress = value;
                this.NotifyPropertyChanged(nameof(this.CompileProgress));
            }
        }

        public bool IsCompileProgressVisible
        {
            get => isCompileProgressVisible;

            set
            {
                this.isCompileProgressVisible = value;
                this.NotifyPropertyChanged(nameof(this.IsCompileProgressVisible));
            }
        }

        private void UpdatePreview()
        {
            var state = new TextCompileState
            {
                MaxLines = 100,
                OnCompleted = result =>
                {
                    this.PreviewText = result;
                },
                OnError = error =>
                {
                    this.PreviewText = error;
                }
            };
            
            if (!this.backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync(state);
            }
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            var state = e.Argument as TextCompileState;
            if (state == null)
            {
                return;
            }
            
            this.IsCompileProgressVisible = true;

            CompileText(
                result =>
                {
                    this.IsCompileProgressVisible = false;
                    state.OnCompleted(result);
                },
                error =>
                {
                    this.IsCompileProgressVisible = false;
                    state.OnError(error);
                },
                progress => this.backgroundWorker.ReportProgress(progress)
            );
        }

        private void CompileText(Action<string> onCompleted, Action<string> onError, Action<int> onProgressChanged, int? maxLines = null)
        {
            try
            {
                var resultCount = maxLines ?? int.MaxValue;

                var regex = new Regex(@"\{.*?\}");
                var matches = regex.Matches(this.PatternText);
                var parameterPlaceholderCount = matches.Count;
                var parameterPlaceholders = Enumerable.Range(0, parameterPlaceholderCount).Select(e => "{" + e + "}").ToArray();

                var lines = this.SourceText.Split(new[] { Regex.Unescape(this.RowSeparator) }, resultCount, StringSplitOptions.RemoveEmptyEntries);

                var stringBuilder = new StringBuilder();

                for (var i = 0; i < lines.Length; i++)
                {
                    var progress = (int)(Math.Floor((decimal)i / (decimal)lines.Length * 100));
                    onProgressChanged?.Invoke(progress);

                    var line = lines[i];

                    var trimmedLine = line.TrimEnd('\r');
                    if (string.IsNullOrWhiteSpace(trimmedLine))
                    {
                        stringBuilder.AppendLine(string.Empty);
                        continue;
                    }

                    var parts = trimmedLine.Split(new[] { Regex.Unescape(this.ColumnSeparator) }, StringSplitOptions.None);

                    if (parameterPlaceholderCount > parts.Length)
                    {
                        // Fill in missing parameters to avoid formatting issues
                        var arrayClone = (string[])parameterPlaceholders.Clone();
                        parts.CopyTo(arrayClone, 0);
                        parts = arrayClone;
                    }

                    try
                    {
                        var formattedString = string.Format(this.PatternText, parts);
                        stringBuilder.AppendLine(formattedString);
                    }
                    catch (Exception ex)
                    {
                        onProgressChanged?.Invoke(0);
                        onError(ex.Message);
                        // Probably unmatched parameter pairs
                        return;
                    }

                    //Thread.Sleep(1);
                }

                var result = stringBuilder.ToString();

                onProgressChanged?.Invoke(100);
                onCompleted(result);
            }
            catch (Exception ex)
            {
                onProgressChanged?.Invoke(0);
                onError(ex.Message);
            }
        }

        /// <summary>
        /// Accepts this instance.
        /// </summary>
        private void Accept()
        {
            var state = new TextCompileState
            {
                MaxLines = null,
                OnCompleted = result =>
                {
                    this.TargetText = result;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.RequestPatternReplace?.Invoke();
                        this.Close();
                    });
                },
                OnError = error =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, error, "Pattern replace", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            };

            if (!this.backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync(state);
            }
        }
    }
}
