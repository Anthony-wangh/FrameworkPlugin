﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void TimerCallback(object param);

/// <summary>
/// Timers.inst.StartCoroutine(timerDelegate);
/// Timers.inst.Add(0.050f, 0, timerDelegate);
/// Timers.inst.Add(0.1f, 1, timerDelegate);
/// Timers.inst.Add(3, 1, (object param) => {  });
/// Timers.inst.AddUpdate(this.Render);
/// Timers.inst.Exists(timerDelegate)
/// Timers.inst.Remove(timerDelegate);
/// </summary>
public class Timers
{
    public static int repeat;
    public static float time;
    public static GameObject gameObject;

    Dictionary<TimerCallback, Anymous_T> _items;
    Dictionary<TimerCallback, Anymous_T> _toAdd;
    List<Anymous_T> _toRemove;
    List<Anymous_T> _pool;
    float _lastTime;

    TimersEngine _engine;

    private static Timers _inst;
    public static Timers Inst
    {
        get
        {
            if (_inst == null)
                _inst = new Timers();
            return _inst;
        }
    }

    public Timers()
    {
        _inst = this;
        gameObject = new GameObject("[Timers]")
        {
            hideFlags = HideFlags.HideInHierarchy
        };
        gameObject.SetActive(true);
        Object.DontDestroyOnLoad(gameObject);

        _engine = gameObject.AddComponent<TimersEngine>();

        _items = new Dictionary<TimerCallback, Anymous_T>();
        _toAdd = new Dictionary<TimerCallback, Anymous_T>();
        _toRemove = new List<Anymous_T>();
        _pool = new List<Anymous_T>(100);
        _lastTime = Time.time;
    }

    public void Add(float interval, int repeat, TimerCallback callback)
    {
        Add(interval, repeat, callback, null);
    }

    /**
     * @interval in seconds
     * @repeat 0 indicate loop infinitely, otherwise the run count
     **/
    public void Add(float interval, int repeat, TimerCallback callback, object callbackParam)
    {
        if (callback == null)
        {
            Debug.LogWarning("timer callback is null, " + interval + "," + repeat);
            return;
        }

        if (_items.TryGetValue(callback, out Anymous_T t))
        {
            t.Set(interval, repeat, callback, callbackParam);
            t.elapsed = 0;
            t.deleted = false;
            return;
        }

        if (_toAdd.TryGetValue(callback, out t))
        {
            t.Set(interval, repeat, callback, callbackParam);
            return;
        }

        t = GetFromPool();
        t.interval = interval;
        t.repeat = repeat;
        t.callback = callback;
        t.param = callbackParam;
        _toAdd[callback] = t;
    }

    public void CallLater(TimerCallback callback)
    {
        Add(0.001f, 1, callback);
    }

    public void CallLater(TimerCallback callback, object callbackParam)
    {
        Add(0.001f, 1, callback, callbackParam);
    }

    public void AddUpdate(TimerCallback callback)
    {
        Add(0.001f, 0, callback);
    }

    public void AddUpdate(TimerCallback callback, object callbackParam)
    {
        Add(0.001f, 0, callback, callbackParam);
    }

    public void StartCoroutine(IEnumerator routine)
    {
        _engine.StartCoroutine(routine);
    }

    public bool Exists(TimerCallback callback)
    {
        if (_toAdd.ContainsKey(callback))
            return true;

        if (_items.TryGetValue(callback, out Anymous_T at))
            return !at.deleted;

        return false;
    }

    public void Remove(TimerCallback callback)
    {
        if (_toAdd.TryGetValue(callback, out Anymous_T t))
        {
            _toAdd.Remove(callback);
            ReturnToPool(t);
        }

        if (_items.TryGetValue(callback, out t))
            t.deleted = true;
    }

    private Anymous_T GetFromPool()
    {
        Anymous_T t;
        int cnt = _pool.Count;
        if (cnt > 0)
        {
            t = _pool[cnt - 1];
            _pool.RemoveAt(cnt - 1);
            t.deleted = false;
            t.elapsed = 0;
        }
        else
            t = new Anymous_T();
        return t;
    }

    private void ReturnToPool(Anymous_T t)
    {
        t.callback = null;
        _pool.Add(t);
    }

    public void Update()
    {
        time = Time.time;
        float elapsed = time - _lastTime;
        if (Time.timeScale != 0)
            elapsed /= Time.timeScale;
        _lastTime = time;

        Dictionary<TimerCallback, Anymous_T>.Enumerator iter;

        if (_items.Count > 0)
        {
            iter = _items.GetEnumerator();
            while (iter.MoveNext())
            {
                Anymous_T i = iter.Current.Value;
                if (i.deleted)
                {
                    _toRemove.Add(i);
                    continue;
                }

                i.elapsed += elapsed;
                if (i.elapsed < i.interval)
                    continue;

                i.elapsed -= i.interval;
                if (i.elapsed < 0 || i.elapsed > 0.03f)
                    i.elapsed = 0;

                if (i.repeat > 0)
                {
                    i.repeat--;
                    if (i.repeat == 0)
                    {
                        i.deleted = true;
                        _toRemove.Add(i);
                    }
                }
                repeat = i.repeat;
                if (i.callback != null)
                {
                    try
                    {
                        i.callback(i.param);
                    }
                    catch (System.Exception e)
                    {
                        i.deleted = true;
                        Debug.Log("timer callback failed, " + i.interval + "," + i.repeat);
                        Debug.LogException(e);
                    }
                }
            }
            iter.Dispose();
        }

        int len = _toRemove.Count;
        if (len > 0)
        {
            for (int k = 0; k < len; k++)
            {
                Anymous_T i = _toRemove[k];
                if (i.deleted && i.callback != null)
                {
                    _items.Remove(i.callback);
                    ReturnToPool(i);
                }
            }
            _toRemove.Clear();
        }

        if (_toAdd.Count > 0)
        {
            iter = _toAdd.GetEnumerator();
            while (iter.MoveNext())
                _items.Add(iter.Current.Key, iter.Current.Value);
            iter.Dispose();
            _toAdd.Clear();
        }
    }
}

class Anymous_T
{
    public float interval;
    public int repeat;
    public TimerCallback callback;
    public object param;

    public float elapsed;
    public bool deleted;

    public void Set(float interval, int repeat, TimerCallback callback, object param)
    {
        this.interval = interval;
        this.repeat = repeat;
        this.callback = callback;
        this.param = param;
    }
}

class TimersEngine : MonoBehaviour
{
    void Update()
    {
        Timers.Inst.Update();
    }
}
