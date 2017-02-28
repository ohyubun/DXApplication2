using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;
using DXApplication2.Common;
using DXApplication2.Main.Properties;
using DXApplication2.Main.ViewModels;
using DXApplication2.Main.Views;
using DXApplication2.Modules.ViewModels;
using DXApplication2.Modules.Views;
using System.ComponentModel;
using System.Windows;
using System;
using AppModules = DXApplication2.Common.Modules;

namespace DXApplication2.Main
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ApplicationThemeHelper.UpdateApplicationThemeName();
            DaoService = new DaoService
            {
                Name="Robert",
                 Documents ="Test",
                 Id=1

            };
            new Bootstrapper().Run(DaoService,ChangeTitle);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            ApplicationThemeHelper.SaveApplicationThemeName();
            base.OnExit(e);
        }
        public DaoService DaoService { get; private set; }
        private int index = 1;
        public void ChangeTitle(string title)
        {
            App.Current.MainWindow.Title = $"{title},MainWindow";
            DaoService.Id++;
        }
    }
    public class Bootstrapper
    {
        const string StateVersion = "1.0";
        Action<string> _changTitle;
        DaoService _daoService;
        public virtual void Run(DaoService daoService,Action<string> changTitle)
        {
            _daoService = daoService;
            _changTitle = changTitle;
            ConfigureTypeLocators();
            RegisterModules();
            if (!RestoreState())
                InjectModules();
            ConfigureNavigation();
            ShowMainWindow();
        }

        protected IModuleManager Manager { get { return ModuleManager.DefaultManager; } }
        protected virtual void ConfigureTypeLocators()
        {
            var mainAssembly = typeof(MainViewModel).Assembly;
            var modulesAssembly = typeof(ModuleViewModel).Assembly;
            var modulesAssembly1 = typeof(ChildViewModel1).Assembly;
            var assemblies = new[] { mainAssembly, modulesAssembly, modulesAssembly1 };
            ViewModelLocator.Default = new ViewModelLocator(assemblies);
            ViewLocator.Default = new ViewLocator(assemblies);
        }
        protected virtual void RegisterModules()
        {
            Manager.Register(Regions.MainWindow, new Module(AppModules.Main, MainViewModel.Create, typeof(MainView)));
            Manager.Register(Regions.Navigation, new Module(AppModules.Module1, () => new NavigationItem("Module1")));
            Manager.Register(Regions.Navigation, new Module(AppModules.Module2, () => new NavigationItem("Module2")));
            Manager.Register(Regions.Navigation, new Module(AppModules.ChildView1, () => new NavigationItem("ChildView1")));
            Manager.Register(Regions.Documents, new Module(AppModules.Module1, () => ModuleViewModel.Create("Module1", "Module1 Content", _daoService, _changTitle), typeof(ModuleView)));
            Manager.Register(Regions.Documents, new Module(AppModules.Module2, () => ModuleViewModel.Create("Module2", "Module2 Content", _daoService, _changTitle), typeof(ModuleView)));
            Manager.Register(Regions.Documents, new Module(AppModules.ChildView1, () => ChildViewModel1.Create("ChildView1", "ChildView1 Content", _daoService, _changTitle), typeof(ChildView1)));

        }
        protected virtual bool RestoreState()
        {
#if !DEBUG
            if (Settings.Default.StateVersion != StateVersion) return false;
            return Manager.Restore(Settings.Default.LogicalState, Settings.Default.VisualState);
#else
            return false;
#endif
        }
        protected virtual void InjectModules()
        {
            Manager.Inject(Regions.MainWindow, AppModules.Main);
            Manager.Inject(Regions.Navigation, AppModules.Module1);
            Manager.Inject(Regions.Navigation, AppModules.Module2);
            Manager.Inject(Regions.Navigation, AppModules.ChildView1);
        }
        protected virtual void ConfigureNavigation()
        {
            Manager.GetEvents(Regions.Navigation).Navigation += OnNavigation;
            Manager.GetEvents(Regions.Documents).Navigation += OnDocumentsNavigation;
        }
        protected virtual void ShowMainWindow()
        {
            App.Current.MainWindow = new MainWindow();
            App.Current.MainWindow.Show();
            App.Current.MainWindow.Closing += OnClosing;
        }
        void OnNavigation(object sender, NavigationEventArgs e)
        {
            if (e.NewViewModelKey == null) return;
            Manager.InjectOrNavigate(Regions.Documents, e.NewViewModelKey);
        }
        void OnDocumentsNavigation(object sender, NavigationEventArgs e)
        {
            Manager.Navigate(Regions.Navigation, e.NewViewModelKey);
        }
        void OnClosing(object sender, CancelEventArgs e)
        {
            string logicalState;
            string visualState;
            Manager.Save(out logicalState, out visualState);
            Settings.Default.StateVersion = StateVersion;
            Settings.Default.LogicalState = logicalState;
            Settings.Default.VisualState = visualState;
            Settings.Default.Save();
        }
    }
}