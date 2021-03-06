﻿using HandySub.ViewModels;
using ModernWpf.Controls;
using System.Windows.Controls;

namespace HandySub.Views
{
    /// <summary>
    ///     Interaction logic for Subscene
    /// </summary>
    public partial class Subscene : UserControl
    {
        public Subscene()
        {
            InitializeComponent();
            Helper.Current.AddAutoSuggestBoxContextMenu(autoBox);
        }

        private void AutoSuggestBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            Helper.Current.LoadHistory(sender, args, autoBox);
        }

        private void AutoSuggestBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.QueryText))
            {
                Helper.Current.AddHistory(args.QueryText);
                SubsceneViewModel.Instance.SearchText = args.QueryText;
                SubsceneViewModel.Instance.OnSearchStarted();
            }
        }
    }
}