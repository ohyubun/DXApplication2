using System;

namespace DXApplication1.Common
{
    public interface IDocumentModule
    {
        string Caption { get; }
        bool IsActive { get; set; }
        DaoService DaoService { get; set; }
        Action<string> ChangeTitle { get; set; }
    }
}
