using Framework.Core.Interface;
using System;
using System.Collections.Generic;

public interface IFsmManager : IManager
{
    Dictionary<Type, IFsmState> FsmStateList { get; set; }
    void Register(IFsmState fsm);
    void ChangeState<T>(params object[] param) where T : IFsmState;

    void ChangeState(IFsmState fsm, params object[] param);
}
