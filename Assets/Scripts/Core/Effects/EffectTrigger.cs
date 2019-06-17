using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects
{
    public class EffectTrigger : MonoBehaviour
    {
        [Serializable]
        public class ReorderableList : ReorderableList<EffectTrigger>
        {
        }

        [SerializeField]
        private ReorderableList _triggerOnComplete = new ReorderableList();

        [SerializeField]
        [ReorderableList]
        private EffectTriggerComponent.ReorderableList _components = new EffectTriggerComponent.ReorderableList();

        [SerializeField]
        [ReadOnly]
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        private Coroutine _effectWaiter;

#region Unity Lifecycle
        private void Awake()
        {
            RunOnComponents(c => c.Initialize());
        }

        private void Update()
        {
            float dt = UnityEngine.Time.deltaTime;

            RunOnComponents(c => {
                if(!c.IsDone) {
                    c.OnUpdate(dt);
                }
            });
        }
#endregion

#region Components
        [CanBeNull]
        public T GetEffectTriggerComponent<T>() where T: EffectTriggerComponent
        {
            foreach(var component in _components.Items) {
                T tc = component as T;
                if(tc != null) {
                    return tc;
                }
            }
            return null;
        }

        public void GetEffectTriggerComponents<T>(ICollection<T> components) where T: EffectTriggerComponent
        {
            components.Clear();
            foreach(var component in _components.Items) {
                T tc = component as T;
                if(tc != null) {
                    components.Add(tc);
                }
            }
        }

        private void RunOnComponents(Action<EffectTriggerComponent> f)
        {
            foreach(var component in _components.Items) {
                f(component);
            }
        }
#endregion

        public void Trigger(Action callback=null)
        {
            _isRunning = true;

            RunOnComponents(c => c.OnStart());

            _effectWaiter = StartCoroutine(EffectWaiter(callback));
        }

        // forcefully stops the trigger early
        public void StopTrigger()
        {
            if(null != _effectWaiter) {
                StopCoroutine(_effectWaiter);
                _effectWaiter = null;
            }

            RunOnComponents(c => c.OnStop());

            _isRunning = false;
        }

        public void ResetTrigger()
        {
            RunOnComponents(c => c.OnReset());
        }

        private IEnumerator EffectWaiter(Action callback)
        {
            WaitForSeconds wait = new WaitForSeconds(0.05f);

            // wait for components (if we should)
            while(true) {
                bool done = true;
                foreach(EffectTriggerComponent component in _components.Items) {
                    if(!component.WaitForComplete || component.IsDone) {
                        continue;
                    }

                    done = false;
                    break;
                }

                if(done) {
                    break;
                }

                yield return wait;
            }

            // invoke our callback
            // (don't wait for further effects)
            _effectWaiter = null;
            callback?.Invoke();

            if(EffectsManager.Instance.EnableDebug) {
                Debug.Log($"Trigger {_triggerOnComplete.Items.Count} more effects from {name}");
            }

            // trigger further effects
            foreach(var onCompleteEffect in _triggerOnComplete.Items) {
                onCompleteEffect.Trigger();
            }

            // wait for those effects before we call ourself not running
            while(true) {
                bool done = true;
                foreach(EffectTrigger onCompleteEffect in _triggerOnComplete.Items) {
                    if(!onCompleteEffect.IsRunning) {
                        continue;
                    }

                    done = false;
                    break;
                }

                if(done) {
                    break;
                }

                yield return wait;
            }

            _isRunning = false;
        }
    }
}
