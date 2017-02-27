using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DXApplication1.Common;
using System;

namespace DXApplication1.Modules.ViewModels
{
    public class ModuleViewModel : IDocumentModule, ISupportState<ModuleViewModel.Info>
    {
        public virtual string Caption { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual string Content { get; set; }
        public virtual Action<string> ChangeTitle { get; set; }
        public virtual DelegateCommand ChangeTitleCommand { get; set; }
        public virtual DaoService DaoService { get; set; }

        protected void changeTitle()
        {
            Console.WriteLine($"Before calling mainWindow : DaoService,id={DaoService.Id}");
            ChangeTitle(Content);
            Console.WriteLine($"After calling mainWindow : DaoService,id={DaoService.Id}");

        }
        public static ModuleViewModel Create(string caption, string content, DaoService daoService,Action<string> changeTitle)
        {        
            return ViewModelSource.Create(() => new ModuleViewModel()
            {
                Caption = caption,
                Content = content,
                DaoService= daoService,
                ChangeTitle = changeTitle
            });
        }

        protected ModuleViewModel()
        {
            ChangeTitleCommand = new DelegateCommand(changeTitle);
        }

        #region Serialization
        [Serializable]
        public class Info
        {
            public string Content { get; set; }
            public string Caption { get; set; }
            //public Action<string> ChangeTitle { get; set; }
        }
        Info ISupportState<Info>.SaveState()
        {
            return new Info()
            {
                Content = this.Content,
                Caption = this.Caption,
                //ChangeTitle = ChangeTitle
            };
        }
        void ISupportState<Info>.RestoreState(Info state)
        {
            this.Content = state.Content;
            this.Caption = state.Caption;
            //ChangeTitle = ChangeTitle;
        }
        #endregion
    }
}
