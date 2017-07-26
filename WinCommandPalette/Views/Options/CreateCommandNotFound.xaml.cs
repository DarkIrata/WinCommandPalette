﻿using System;
using System.Windows.Controls;
using WinCommandPalette.Plugin.CommandBase;
using WinCommandPalette.Plugin.CreateCommand;

namespace WinCommandPalette.Views.Options
{
    /// <summary>
    /// Interaktionslogik für CreateCommandNotFound.xaml
    /// </summary>
    public partial class CreateCommandNotFound : UserControl, ICreateCommand
    {
        public CreateCommandNotFound()
        {
            this.InitializeComponent();
        }

        public string CommandTypeName => throw new NotImplementedException();

        public void ClearAll()
        {
        }

        public ICommandBase GetCommand()
        {
            return null;
        }

        public void ShowCommand(ICommandBase command)
        {
        }
    }
}
