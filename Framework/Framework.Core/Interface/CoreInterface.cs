using System;
using Framework.Core.Message;
using UnityEngine;

namespace Framework.Core.Interface
{
    public interface IInit
    {
        void Init();
    }
    public interface IControl : IDisposable, IMessage, IInit
    {
        void UnsubscribeAll();
    }

    public interface IManager : IInit
    {
    }

    public interface IModel : IDisposable, IInit
    {
    }

    public interface IService : IInit
    {
    }

    public interface IUtility : IInit
    {
    }

    
}
