using Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 快捷键工具
/// </summary>
public sealed class ShortcutkeyUtility 
{
    private static float _curPressTime;
    private static float _pressTime = 0.3f;
    private static bool _isPressCtrl;
    private static bool _isPressShift;
    private static bool _isPressAlt;

    public static bool IsPressGroupKey => _isPressCtrl || _isPressShift || _isPressAlt;//正在点击组合键时，禁止触发单键事件
    private readonly static Dictionary<KeyCode, KeyActionGroup> _ketEventDictionary = new Dictionary<KeyCode, KeyActionGroup>();

    private static bool _enable = true;


    static ShortcutkeyUtility()
    {
        CoreEngine.OnUpdate += Update;
    }
    /// <summary>
    /// 是否激活快捷键工具
    /// </summary>
    /// <param name="enable"></param>
    public static void SetEnable(bool enable)
    {
        _enable = enable;
    }

    #region Trigger
    private static void Update()
    {
        if (!_enable)
            return;
        var keys = _ketEventDictionary.Keys.ToList();
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            var key = keys[i];
            TriggerClickAndPress(key);
            TriggerKeyWithCtrl(key);
            TriggerKeyWithShift(key);
            TriggerKeyWithCtrlAndShift(key);
            TriggerKeyWithAlt(key);
            TriggerKeyWithAltAndCtrl(key);
        }
    }

    private static void TriggerClickAndPress(KeyCode key)
    {
        if (IsPressGroupKey)
            return;
        if (Input.GetKey(key))
        {
            _curPressTime += Time.deltaTime;
            if (_curPressTime >= _pressTime)
            {
                TriggerEvent(key, ClickType.Press);
                TriggerEvent(key, ClickType.ClickAndPress);
            }
        }
        if (Input.GetKeyUp(key))
        {
            if (_curPressTime < _pressTime)
            {
                TriggerEvent(key);
                TriggerEvent(key, ClickType.ClickAndPress);
            }

            _curPressTime = 0;
        }
    }


    private static void TriggerKeyWithCtrl(KeyCode key)
    {
        if (OnPressCtrl() && !OnPressAlt() && !OnPressShift())
        {
            if (Input.GetKeyDown(key))
                TriggerEvent(key, ClickType.ClickWithCtrl);
            if (Input.GetKey(key))
                TriggerEvent(key, ClickType.PressWithCtrl);
        }

    }
    private static void TriggerKeyWithShift(KeyCode key)
    {
        if (OnPressShift() && !OnPressCtrl() && !OnPressAlt())
        {
            if (Input.GetKeyDown(key))
                TriggerEvent(key, ClickType.ClickWithShift);
            if (Input.GetKey(key))
                TriggerEvent(key, ClickType.PressWithShift);
        }
    }


    private static void TriggerKeyWithCtrlAndShift(KeyCode key)
    {
        if (OnPressCtrl() && OnPressShift() && !OnPressAlt())
        {
            if (Input.GetKeyDown(key))
                TriggerEvent(key, ClickType.ClickWithCtrlAndShift);
            if (Input.GetKey(key))
                TriggerEvent(key, ClickType.PressWithCtrlAndShift);
        }
    }

    private static void TriggerKeyWithAlt(KeyCode key)
    {
        if (OnPressAlt() && !OnPressCtrl() && !OnPressShift())
        {
            if (Input.GetKeyDown(key))
                TriggerEvent(key, ClickType.ClickWithAlt);
            if (Input.GetKey(key))
                TriggerEvent(key, ClickType.PressWithAlt);
        }
    }


    private static void TriggerKeyWithAltAndCtrl(KeyCode key)
    {
        if (OnPressAlt() && OnPressCtrl() && !OnPressShift())
        {
            if (Input.GetKeyDown(key))
                TriggerEvent(key, ClickType.ClickWithCtrlAndAlt);
            if (Input.GetKey(key))
                TriggerEvent(key, ClickType.PressWithCtrlAndAlt);
        }
    }


    private static bool OnPressCtrl()
    {
        _isPressCtrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        return _isPressCtrl;
    }

    private static bool OnPressShift()
    {
        _isPressShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        return _isPressShift;
    }

    private static bool OnPressAlt()
    {
        _isPressAlt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        return _isPressAlt;
    }

    private static void TriggerEvent(KeyCode type, ClickType clickType = ClickType.Click)
    {
        if (!_ketEventDictionary.ContainsKey(type))
            return;
        _ketEventDictionary[type].Handle(clickType);
    }
    #endregion

    #region Register
    /// <summary>
    /// 添加一个全局的默认为点击按键的事件
    /// </summary>
    /// <param name="type">按键</param>
    /// <param name="action">事件</param>
    public static void Add(KeyCode type, Action action)
    {
        Add(type, ClickType.Click, action);
    }
    /// <summary>
    /// 添加一个指定点击类型的事件
    /// </summary>
    /// <param name="type">按键</param>
    /// <param name="clickType">点击类型</param>
    /// <param name="action">事件</param>
    public static void Add(KeyCode type, ClickType clickType, Action action)
    {
        var keyEvent = new KeyEventAction(action, clickType);
        if (_ketEventDictionary.ContainsKey(type))
        {
            _ketEventDictionary[type].AddAction(keyEvent);
        }
        else
        {
            var group = new KeyActionGroup();
            group.AddAction(keyEvent);
            _ketEventDictionary.Add(type, group);
        }
    }

    /// <summary>
    /// 移除一个按键的事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    public static void Remove(KeyCode type, Action action)
    {
        if (!_ketEventDictionary.ContainsKey(type))
            return;
        var group = _ketEventDictionary[type];
        group.Remove(action);
        if (group.Actions.Count == 0)
        {
            _ketEventDictionary.Remove(type);
        }
    }

    #endregion
}
public enum ClickType
{
    Click,//点击
    Press,//长按
    ClickAndPress,//长按或者点击
    ClickWithCtrl,//按住ctrl键点击
    PressWithCtrl,//按住ctrl键长按
    ClickWithShift,//按住Shift键点击
    PressWithShift,//按住Shift键长按
    ClickWithCtrlAndShift,//按住Ctrl和Shift键点击
    PressWithCtrlAndShift, //按住Ctrl和Shift键长按
    ClickWithAlt,
    PressWithAlt,
    ClickWithCtrlAndAlt,
    PressWithCtrlAndAlt,
}
public class KeyActionGroup
{
    public readonly List<KeyEventAction> Actions;

    public KeyActionGroup()
    {
        Actions = new List<KeyEventAction>();
    }
    public void AddAction(KeyEventAction action)
    {
        if (Actions.Contains(action))
            return;
        Actions.Add(action);
    }

    public void Remove(Action action)
    {
        for (int i = 0; i < Actions.Count; i++)
        {
            if (Actions[i].Action == action)
            {
                Actions.Remove(Actions[i]);
            }
        }
    }

    public void Handle(ClickType type)
    {
        if (Actions == null || Actions.Count <= 0)
            return;
        for (int i = Actions.Count - 1; i >= 0; i--)
        {
            if (i >= 0 && i < Actions.Count)
            {
                var action = Actions[i];
                if (action.ClickType == type)
                {
                    action.Handle();
                }
            }
        }
    }
}



public class KeyEventAction
{
    public readonly Action Action;
    public ClickType ClickType;
    public KeyEventAction(Action action, ClickType type)
    {
        Action = action;
        ClickType = type;
    }

    public void Handle()
    {
        Action?.Invoke();
    }
}

