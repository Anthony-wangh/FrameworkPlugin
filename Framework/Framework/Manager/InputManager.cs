using Framework.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 手势管理
/// </summary>
public class InputManager
{
    private const float LongPressTime = 1f;
    /// <summary>
    /// Mouse Down
    /// </summary>
    public static Action Down;
    /// <summary>
    /// MouseUp
    /// </summary>
    public static Action Up;
    /// <summary>
    /// MouseClick
    /// </summary>
    public static Action Click;
    /// <summary>
    /// MousePress
    /// Mouse press longTime more than one millon
    /// </summary>
    public static Action LongPress;
    /// <summary>
    /// 开始拖拽
    /// </summary>
    public static Action BeginDrag;
    /// <summary>
    /// 正在拖拽，只有在BeginDrag触发时才有效
    /// </summary>
    public static Action Drag;
    /// <summary>
    /// 结束拖拽
    /// </summary>
    public static Action EndDrag;
    /// <summary>
    /// 双击
    /// </summary>
    public static Action DoubleClick;
    /// <summary>
    /// 正在拖拽，不需要触发条件
    /// </summary>
    public static Action Press;

    private static float _pressTime;
    private static float _clickOffset;
    private static Vector2 _pos;
    private static Vector2 _deltaPos;
    private static float _dragOffset;
    private static bool _hasDrag;
    private static float _clickTime = 0;
    /// <summary>
    /// 鼠标正在点击得按键
    /// </summary>
    public static int MouseId { get; private set; }
    private static int[] GetMouseButtonNum = { 0, 1, 2 };
    /// <summary>
    /// 鼠标当前位置
    /// </summary>
    public static Vector2 Pos => _pos;
    /// <summary>
    /// 鼠标位置变化值
    /// </summary>
    public static Vector2 DeltaPos => _deltaPos;
    /// <summary>
    /// 是否点中UI
    /// </summary>
    public static bool HitUI { get; private set; }
    //双指
    private static bool _isDoubleTouch = false;

    //用于区分 移动端 2个手指放开 然后变成一个手指 的问题
    private static bool _mouseDown = false; 

    static InputManager()
    {
        _pos = Input.mousePosition;
        _clickOffset = Application.isMobilePlatform ? 30.0f : 10.0f;
        CoreEngine.OnUpdate += Update;
    }

    private static void Update()
    {
        if (MouseDown())
        {
            HitUI = IsPointerOverUgui();
            _pos = Input.mousePosition;
            _dragOffset = 0;
            _pressTime = Time.time;
            _mouseDown = true;
            Down?.Invoke();
        }
        else if (MousePress() && _mouseDown)
        {
            Vector2 pos = Input.mousePosition;
            Press?.Invoke();
            if (_pos != pos)
            {
                _deltaPos = pos - _pos;
                _dragOffset += Vector2.Distance(_pos, pos);
                _pos = pos;
                if (_dragOffset > _clickOffset && Input.GetMouseButton(0))
                {
                    if (!_hasDrag)
                    {
                        _hasDrag = true;
                        OnBeginDrag();
                    }

                    OnDrag();
                }
            }
            else
            {
                _deltaPos = new Vector2(0, 0);

                if (_dragOffset < _clickOffset && Time.time - _pressTime > LongPressTime && !_isDoubleTouch)
                {
                    LongPress?.Invoke();
                    _pressTime = float.NaN;
                }
            }
        }
        else if (MouseUp() && _mouseDown)
        {
            Up?.Invoke();
            if (!HitUI && !float.IsNaN(_pressTime) && _dragOffset < _clickOffset && !_isDoubleTouch)
            {
                if (Time.time - _clickTime < 0.4f)
                {
                    _clickTime = 0;
                    DoubleClick?.Invoke();
                }
                else
                {
                    Click?.Invoke();
                }
                _clickTime = Time.time;
            }
            if (_hasDrag)
            {
                _hasDrag = false;
                OnEndDrag();
            }
            _deltaPos = new Vector2(0, 0);
        }
        else
        {
            _deltaPos = new Vector2(0, 0);
            HitUI = false;
            _hasDrag = false;
            _dragOffset = 0;
            _mouseDown = false;
        }

        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 1)
                _isDoubleTouch = true;

            if (Input.touchCount == 0)
                _isDoubleTouch = false;
        }
    }

    private static void OnBeginDrag()
    {
        if (!Application.isMobilePlatform && !Input.GetMouseButton(0) && _isDoubleTouch)
            return;

        BeginDrag?.Invoke();
    }

    private static void OnDrag()
    {
        if (!Application.isMobilePlatform && !Input.GetMouseButton(0) && _isDoubleTouch)
            return;

        Drag?.Invoke();
    }

    private static void OnEndDrag()
    {
        if (!Application.isMobilePlatform && !Input.GetMouseButtonUp(0) && _isDoubleTouch)
            return;

        EndDrag?.Invoke();
    }

    private static bool MouseDown()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began)
            {
                MouseId = 0;
                return true;
            }
            return false;
        }
        for (int i = 0; i < 3; i++)
        {
            if (Input.GetMouseButtonDown(i))
            {
                MouseId = i;
                return true;
            }
        }
        return false;
    }

    private static bool MouseUp()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1)
            {
                return Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled;
            }

            return false;
        }
        foreach (var i in GetMouseButtonNum)
        {
            if (Input.GetMouseButtonUp(i))
                return true;
        }
        return false;
    }

    private static bool MousePress()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1)
            {
                return Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary;
            }

            return false;
        }
        foreach (var i in GetMouseButtonNum)
        {
            if (Input.GetMouseButton(i))
                return true;
        }
        return false;
    }
    /// <summary>
    /// 鼠标是否正在点击UI
    /// </summary>
    /// <returns></returns>
    public static bool IsPointerOverUgui()
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventDataCurrentPosition =
            new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    /// <summary>
    /// 鼠标滚轮或者手机端二指缩放值
    /// </summary>
    /// <returns></returns>
    public static float GetAxisScrollWheel()
    {
        if (!Application.isMobilePlatform)
        {
            return Input.GetAxis("Mouse ScrollWheel") * 500;
        }
        if (Input.touchCount == 2)
        {
            Touch touch = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            float distance = Vector2.Distance(touch.position, touch2.position);
            if (touch.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                _curDist = distance;
            }

            float offset = distance - _curDist;
            _curDist = distance;

            if ((touch.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) &&
                Vector2.Dot(touch.deltaPosition, touch2.deltaPosition) <= 0)
            {
                return offset;
            }
            return 0;
        }
        return 0;
    }

    private static float _curDist;
    /// <summary>
    /// 鼠标中键移动值或者移动端二指移动值
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetDoubleMove()
    {
        if (!Application.isMobilePlatform)
        {
            if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
                return _deltaPos;
            return Vector2.zero;
        }
        if (Input.touchCount == 2)
        {
            Touch touch = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch.phase == TouchPhase.Moved &&
                touch2.phase == TouchPhase.Moved &&
                Vector2.Dot(touch.deltaPosition, touch2.deltaPosition) > 0)
                return (touch.deltaPosition + touch2.deltaPosition) * 0.5f;
            return Vector2.zero;
        }
        return Vector2.zero;
    }
    /// <summary>
    /// 单击鼠标移动值或者移动端单指移动值
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetSingleMove()
    {
        if (!Application.isMobilePlatform)
        {
            if (Input.GetMouseButton(0))
                return _deltaPos;
            return Vector2.zero;
        }
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
                return _deltaPos;
            return Vector2.zero;
        }
        return Vector2.zero;
    }
}
