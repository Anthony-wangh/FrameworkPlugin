using Framework.Common;
using Framework.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Manager
{
    /// <summary>
    /// 定时器 
    /// </summary>
    public class TimerManager : ManagerBase
    {
        private readonly List<Timer> _timerList = new List<Timer>();
        private readonly List<Timer> _addTimer = new List<Timer>();


        protected override void OnInit()
        {
            base.OnInit();
            CoreEngine.OnUpdate += Update;
        }

        ///TimerMgr.Inst.Add(3f, () => {}, loop: true);
        public Timer Add(float duration, Action complete = null, Action<float> update = null,
            bool loop = false, bool ignoreTimeScale = false, GameObject owner = null)
        {
            var timer = new Timer(duration, complete, update, loop, ignoreTimeScale, owner);
            _addTimer.Add(timer);
            return timer;
        }

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            if (_addTimer.Count > 0)
            {
                _timerList.AddRange(_addTimer);
                _addTimer.Clear();
            }

            foreach (Timer timer in _timerList)
            {
                timer.Update();
            }

            _timerList.RemoveAll(t => t.IsDone || t.IsOwnerDestroyed);
        }
    }

    public class Timer
    {
        private readonly float _duration;
        private readonly bool _isLoop;
        private readonly bool _ignore;
        private Action _onComplete;
        private Action<float> _onUpdate;
        private readonly bool _hasOwner;
        private readonly GameObject _owner;
        public bool IsOwnerDestroyed => _hasOwner && _owner == null;
        public bool IsDone { get; private set; }
        private bool _isPaused;
        private float CurTime => _ignore ? Time.realtimeSinceStartup : Time.time;
        private float _elapsedTime; //计时器执行的时间
        private float _updateTime;
        public Timer(float duration, Action complete = null, Action<float> update = null,
            bool loop = false, bool ignoreTimeScale = false, GameObject owner = null)
        {
            _duration = duration;
            _owner = owner;
            _hasOwner = owner != null;
            _ignore = ignoreTimeScale;
            _isLoop = loop;
            _onComplete = complete;
            _onUpdate = update;
            _updateTime = CurTime;
            _elapsedTime = 0;
        }

        public void Update()
        {
            if (IsDone || IsOwnerDestroyed)
                return;


            if (_isPaused)
            {
                _updateTime = CurTime;
                return;
            }

            _elapsedTime += CurTime - _updateTime;
            _updateTime = CurTime;
            if (_elapsedTime > _duration)
                _elapsedTime = _duration;

            _onUpdate?.Invoke(_elapsedTime);

            if (_elapsedTime >= _duration)
            {
                _onComplete?.Invoke();

                if (_isLoop)
                {
                    _elapsedTime = 0;
                }
                else
                {
                    Dispose();
                }
            }
        }

        public void Pause()
        {
            if (IsDone || IsOwnerDestroyed)
                return;

            _isPaused = true;
        }

        public void Resume()
        {
            if (IsDone || IsOwnerDestroyed)
                return;

            _isPaused = false;
        }

        public void Cancel()
        {
            Dispose();
        }

        public void Dispose()
        {
            IsDone = true;
            _onComplete = null;
            _onUpdate = null;
        }
    }
}
