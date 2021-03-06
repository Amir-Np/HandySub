﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using HandyControl.Controls;
using HandyControl.Tools;
using HandySub.Language;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using ApplicationTheme = HandyControl.Tools.ApplicationTheme;

namespace HandySub
{
    public partial class App : Application
    {
        public static string[] WindowsContextMenuArgument = {string.Empty, string.Empty};

        private readonly List<string> wordsToRemove =
            ". hdtv exm RMT DD5 YTS TURKISH VIDEOFLIX Gisaengchung KOREAN 8CH BluRay Hdcam HDCAM - XviD AC3 EVO WEBRip FGT MP3 CMRG Pahe 10bit 720p 1080p 480p WEB-DL H264 H265 x264 x265 800MB 900MB HEVC PSA RARBG 6CH 2CH CAMRip Rip AVS RMX HDTV RMTeam mSD SVA MkvCage MeGusta TBS AMZN DDP5.1 DDP5 SHITBOX NITRO WEB DL 1080 720 480 MrMovie BWBP NTG "
                .Split(' ').ToList();

        public App()
        {
            var cachePath = $"{AppDomain.CurrentDomain.BaseDirectory}Cache";
            if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);

            ProfileOptimization.SetProfileRoot(cachePath);
            ProfileOptimization.StartProfile("Profile");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            GlobalData.Init();
            LocalizationManager.Instance.LocalizationProvider = new ResxProvider();
            LocalizationManager.Instance.CurrentCulture =
                new CultureInfo(GlobalData.Config.UILang);
            ConfigHelper.Instance.SetLang(GlobalData.Config.UILang);
            if (GlobalData.Config.Theme != ApplicationTheme.Light)
                UpdateTheme(GlobalData.Config.Theme);

            UpdateAccentColor(GlobalData.Config.Accent);

            ConfigHelper.Instance.SetWindowDefaultStyle();
            ConfigHelper.Instance.SetNavigationWindowDefaultStyle();

            //init Appcenter Crash Reporter
            AppCenter.Start("3770b372-60d5-49a1-8340-36a13ae5fb71",
                typeof(Analytics), typeof(Crashes));

            if (e.Args.Length > 0)
            {
                var NameFromContextMenu = RemoveJunkString(Path.GetFileNameWithoutExtension(e.Args[0]));

                WindowsContextMenuArgument[0] = NameFromContextMenu;
                WindowsContextMenuArgument[1] = e.Args[0].Replace(Path.GetFileName(e.Args[0]), "");
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var boot = new Bootstrapper();
            boot.Run();

            if (GlobalData.Config.IsFirstRun)
            {
                GlobalData.Config.IsFirstRun = false;
                GlobalData.Save();
            }
        }

        public string RemoveJunkString(string stringToClean)
        {
            var cleaned = Regex.Replace(stringToClean, "\\b" + string.Join("\\b|\\b", wordsToRemove) + "\\b", " ");
            cleaned = Regex.Replace(cleaned, @"S[0-9].{1}E[0-9].{1}", ""); // remove SXXEXX ==> X is 0-9
            cleaned = Regex.Replace(cleaned, @"s[0-9].{1}e[0-9].{1}", ""); // remove SXXEXX ==> X is 0-9
            cleaned = Regex.Replace(cleaned, @"(\[[^\]]*\])|(\([^\)]*\))", ""); // remove between () and []
            cleaned = Regex.Replace(cleaned, "[ ]{2,}",
                " "); // remove space [More than 2 space] and replace with one space

            return cleaned.Trim();
        }

        public void UpdateAccentColor(Brush accent)
        {
            if (accent != null && ThemeManager.Current.ActualAccentColor != accent)
            {
                ThemeManager.Current.ActualAccentColor = accent;

                if (accent.GetType() == typeof(LinearGradientBrush))
                {
                    var brush = (LinearGradientBrush) accent;
                    var solid = new SolidColorBrush(brush.GradientStops[1].Color);
                    ModernWpf.ThemeManager.Current.AccentColor = solid.Color;
                }
                else
                {
                    Color color = ((SolidColorBrush) accent).Color;
                    ModernWpf.ThemeManager.Current.AccentColor = color;
                }
            }
            else if (ThemeManager.Current.ActualAccentColor != accent && accent == null)
            {
                var brush = ResourceHelper.GetResource<Brush>("PrimaryBrush");
                ThemeManager.Current.ActualAccentColor = brush;

                var linBrush = (LinearGradientBrush) brush;
                var solid = new SolidColorBrush(linBrush.GradientStops[1].Color);
                ModernWpf.ThemeManager.Current.AccentColor = solid.Color;
            }
        }

        public void UpdateTheme(ApplicationTheme theme)
        {
            if (ThemeManager.Current.ApplicationTheme != theme)
            {
                ThemeManager.Current.ApplicationTheme = theme;
                ModernWpf.ThemeManager.Current.ApplicationTheme = theme == ApplicationTheme.Dark
                    ? ModernWpf.ApplicationTheme.Dark
                    : ModernWpf.ApplicationTheme.Light;
            }
        }
    }
}