﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PXLed
{
    public partial class App : Application
    {
        public static Config Config { get; private set; }

        MainWindow mainWindow;

        private void Application_Startup(object sender, StartupEventArgs args)
        {
            Config = new Config("config.json");

            mainWindow = new();
            mainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            mainWindow.StopCurrentEffect();
        }

        public static void ShowError(Exception exception)
        {
            MessageBox.Show($"{exception.Message}\n\n{exception.StackTrace}", $"Exception occured in {exception.Source}", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowMessage(string msg)
        {
            MessageBox.Show(msg, $"Message", MessageBoxButton.OK);
        }
    }
}
