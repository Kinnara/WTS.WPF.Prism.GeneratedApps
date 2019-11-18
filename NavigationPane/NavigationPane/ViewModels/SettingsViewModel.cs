﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

using NavigationPane.Contracts.Services;
using NavigationPane.Models;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace NavigationPane.ViewModels
{
    public class SettingsViewModel : BindableBase, INavigationAware
    {
        private readonly AppConfig _config;
        private readonly IThemeSelectorService _themeSelectorService;
        private AppTheme _theme;
        private string _versionDescription;
        private ICommand _setThemeCommand;
        private ICommand _privacyStatementCommand;

        public AppTheme Theme
        {
            get { return _theme; }
            set { SetProperty(ref _theme, value); }
        }

        public string VersionDescription
        {
            get { return _versionDescription; }
            set { SetProperty(ref _versionDescription, value); }
        }

        public ICommand SetThemeCommand => _setThemeCommand ?? (_setThemeCommand = new DelegateCommand<string>(OnSetTheme));

        public ICommand PrivacyStatementCommand => _privacyStatementCommand ?? (_privacyStatementCommand = new DelegateCommand(OnPrivacyStatement));

        public SettingsViewModel(AppConfig config, IThemeSelectorService themeSelectorService)
        {
            _config = config;
            _themeSelectorService = themeSelectorService;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            VersionDescription = GetVersionDescription();
            Theme = _themeSelectorService.GetCurrentTheme();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private string GetVersionDescription()
        {
            var appName = "NavigationPane";
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var versionInfo = FileVersionInfo.GetVersionInfo(assemblyLocation);
            return $"{appName} - {versionInfo.FileVersion}";
        }

        private void OnSetTheme(string themeName)
        {
            var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
            _themeSelectorService.SetTheme(theme);
        }

        private void OnPrivacyStatement()
        {
            // There is an open Issue on this
            // https://github.com/dotnet/corefx/issues/10361
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = _config.PrivacyStatement,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;
    }
}
