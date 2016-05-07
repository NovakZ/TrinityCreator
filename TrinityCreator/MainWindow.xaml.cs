﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using TrinityCreator.Database;
using TrinityCreator.Properties;
using TrinityCreator.DBC;
using System.Timers;

namespace TrinityCreator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App._MainWindow = this;

            // Check for updates
#if !DEBUG
            Updater.CheckLatestVersion();
#endif
            // presist settings on version change
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            // prepare lookup tool
            ContentGrid.ColumnDefinitions[2].Width = new GridLength(25);
            ContentGridSplitter.Visibility = Visibility.Collapsed;

            // Set emulator
            switch (Properties.Settings.Default.emulator)
            {
                case 0: // trinity335a
                    trinity335aRb.IsChecked = true;
                    break;
                case 1: // cMangos112
                    cMangos112Rb.IsChecked = true;
                    break;
            }


            // Load usable creators
            ItemTab.Content = new ItemPage();
            QuestTab.Content = new QuestPage();


            // view unfinished when set & on debug
            try
            {
                UpdatesBrowser.Navigate("https://github.com/RStijn/TrinityCreator/commits/master");
            }
            catch
            {
                /* too bad */
            }

            // Load randomTip
            tipTimer.Elapsed += ChangeRandomTip;
            tipTimer.Interval = 200; // don't change interval here
            tipTimer.Start();

        }

        static Random random = new Random();
        Timer tipTimer = new Timer();
        private double _lookupToolWidth;
        
        public void ShowLookupTool()
        {
            LookupToolExpander.IsExpanded = true;
        }

        private void configureMysql_Click(object sender, RoutedEventArgs e)
        {
            new DbConfigWindow().Show();
        }

        private void configureDbc_Click(object sender, RoutedEventArgs e)
        {
            new DBC.DbcConfigWindow().Show();
        }

        #region Settings
        // select the emulator
        private void trinity335aRb_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.emulator = 0;
            Properties.Settings.Default.Save();
        }
        private void cMangos112Rb_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.emulator = 1;
            Properties.Settings.Default.Save();
        }

        #endregion


        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            Donate();
        }

        private void WhyDonate_Click(object sender, RoutedEventArgs e)
        {
            var msg =
                string.Format(
                    "Trinity Creator is released as an open source project created by a passionate IT student, so donating is completely optional.{0}{0}" +
                    "If you're new to the emulation scene, you can use this program to create your own world and even release it as a public server.{0}" +
                    "Or if you're already running a profitable server, you'll be able to save a lot of development time when releasing new content.{0}{0}" +
                    "So, do you want to motivate me to implement more features or thank me for making things easier? Then toss me a few bucks! :)",
                    Environment.NewLine);
            var result = MessageBox.Show(msg, "Why donate?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
                Donate();
        }

        private void Donate()
        {
            Process.Start("https://paypal.me/RStijn");
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            new CreditsWindow().Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
        }

        private void LookupToolExpander_Expanded(object sender, RoutedEventArgs e)
        {

            if (_lookupToolWidth < 100)
                _lookupToolWidth = 300;
            ContentGrid.ColumnDefinitions[2].Width = new GridLength(_lookupToolWidth);
            ContentGridSplitter.Visibility = Visibility.Visible;
        }

        private void LookupToolExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            _lookupToolWidth = ContentGrid.ColumnDefinitions[2].Width.Value;
            ContentGrid.ColumnDefinitions[2].Width = new GridLength(25);
            ContentGridSplitter.Visibility = Visibility.Collapsed;
        }


        private void ChangeRandomTip(object sender, ElapsedEventArgs e)
        {
            tipTimer.Interval = 30000; // 30sec
            string[] allTips = Properties.Resources.RandomTips.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            string oldText = "";
            string newText = "";
            do
            {
                if (allTips.Length > 0)
                {
                    newText = allTips[random.Next(allTips.Length - 1)];
                    randomTipTxt.Dispatcher.Invoke(new Action(() =>
                    {
                        oldText = randomTipTxt.Text;
                        randomTipTxt.Text = newText;
                    }));
                }
            } while (oldText == newText);         
        }
    }
}