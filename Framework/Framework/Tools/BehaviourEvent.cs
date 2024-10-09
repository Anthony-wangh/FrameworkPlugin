using System;
using UnityEngine;

namespace Framework.Tools
{
    /// <summary>
    /// MonoBehaviour生命周期事件
    /// </summary>
    public class BehaviourEvent : MonoBehaviour
    {
        public static BehaviourEvent Get(GameObject go)
        {
            BehaviourEvent behaviourEvent = go.GetComponent<BehaviourEvent>();
            if (behaviourEvent == null)
                behaviourEvent = go.AddComponent<BehaviourEvent>();
            return behaviourEvent;
        }
        public Action OnAwake;
        public Action OnObjEnable;
        public Action OnStart;
        public Action OnUpdate;
        public Action OnObjDisable;
        public Action OnObjDestroy;


        private void Awake()
        {
            OnAwake?.Invoke();
        }
        private void OnEnable()
        {
            OnObjEnable?.Invoke();
        }
        private void Start()
        {
            OnStart?.Invoke();
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void OnDisable()
        {
            OnObjDisable?.Invoke();
        }

        private void OnDestroy()
        {
            OnObjDestroy?.Invoke();

        }
    }
}
