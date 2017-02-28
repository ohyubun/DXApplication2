using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.POCO;
using DXApplication2.Common;
using System;
using System.Windows;

namespace DXApplication2.Modules.ViewModels
{
    public class ChildViewModel1: ModuleViewModel
    {
        public  DelegateCommand ShowMessageCommand { get; set; }

        public static new ModuleViewModel Create(string caption, string content, DaoService daoService, Action<string> changeTitle)
        {
            return ViewModelSource.Create(() => new ChildViewModel1()
            {
                Caption = caption,
                Content = content,
                DaoService = daoService,
                ChangeTitle = changeTitle
            });
        }

        public ChildViewModel1()
        {
            ShowMessageCommand = new DelegateCommand(showMessage);
        }

        private void showMessage()
        {
            MessageBoxResult result = DXMessageBox.Show(this.Content, "Error Message", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) DXMessageBox.Show("'YES' button clicked", "Result");
            else DXMessageBox.Show("'NO' button clicked", "Result");

        }
    }
}