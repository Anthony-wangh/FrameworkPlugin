using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core.Message
{
    public class UnsubscribeOnDestroy : MonoBehaviour
    {
        private readonly HashSet<IUnsubscribeOnDestroy> _unsubscribes = new HashSet<IUnsubscribeOnDestroy>();

        public void AddUnsubscribe(IUnsubscribeOnDestroy unsubscribe)
        {
            _unsubscribes.Add(unsubscribe);
        }

        public void RemoveUnsubscribe(IUnsubscribeOnDestroy unsubscribe)
        {
            _unsubscribes.Remove(unsubscribe);
        }

        private void OnDestroy()
        {
            foreach (var unsubscribe in _unsubscribes)
            {
                unsubscribe.UnsubscribeOnDestroy();
            }

            _unsubscribes.Clear();
        }
    }

    public static class UnsubscribeExtension
    {
        public static IUnsubscribeOnDestroy UnsubscribeWhenGameObjectDestroyed(this IUnsubscribeOnDestroy unsubscribe, GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnsubscribeOnDestroy>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<UnsubscribeOnDestroy>();
            }

            trigger.AddUnsubscribe(unsubscribe);

            return unsubscribe;
        }
    }
}
