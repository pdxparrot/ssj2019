﻿#if USE_SPINE
using System;

using pdxpartyparrot.Core.Animation;

using Spine;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SpineAnimationEffectTriggerComponent : EffectTriggerComponent
    {
        public class EventArgs : System.EventArgs
        {
            public TrackEntry TrackEntry { get; set; }
        }

#region Events
        public event EventHandler<EventArgs> StartEvent;
        public event EventHandler<EventArgs> CompleteEvent;
#endregion

        [SerializeField]
        private SpineAnimationHelper _spineAnimation;

        [SerializeField]
        private string _spineAnimationName = "default";

        [SerializeField]
        private int _spineAnimationTrack;

        [SerializeField]
        private bool _waitForComplete = true;

        public override bool WaitForComplete => _waitForComplete;

        public override bool IsDone => null == _trackEntry || _trackEntry.IsComplete;

        private TrackEntry _trackEntry;

        public override void OnStart()
        {
            if(EffectsManager.Instance.EnableAnimation) {
                if(EffectsManager.Instance.EnableDebug) {
                    Debug.Log($"Triggering Spine animation {_spineAnimationName} on track {_spineAnimationTrack}");
                }

                _trackEntry = _spineAnimation.SetAnimation(_spineAnimationTrack, _spineAnimationName, false);
                _trackEntry.Complete += OnComplete;

                StartEvent?.Invoke(this, new EventArgs{
                    TrackEntry = _trackEntry,
                });
            } else {
                // TODO: set a timer or something to timeout when we'd normally be done
            }
        }

        public override void OnStop()
        {
            // TODO: any way to force-stop the animation?

            Complete();
        }

        public override void OnReset()
        {
            _spineAnimation.ResetAnimation();
        }

        private void Complete()
        {
            if(null == _trackEntry) {
                return;
            }

            CompleteEvent?.Invoke(this, new EventArgs{
                TrackEntry = _trackEntry,
            });

            _trackEntry.Complete -= OnComplete;
            _trackEntry = null;
        }

        #region Event Handlers
        private void OnComplete(TrackEntry entry)
        {
            if(entry != _trackEntry) {
                // is this even possible?
                return;
            }

            Complete();
        }
#endregion
    }
}
#endif
