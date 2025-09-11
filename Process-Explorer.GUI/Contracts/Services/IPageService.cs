using System;

namespace Process_Explorer.GUI.Contracts.Services;

public interface IPageService
{
    Type GetPageType(string key);
}
