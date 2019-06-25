﻿using Cinemachine;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class CinemachineViewerShakeEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private CinemachineImpulseSource _impulseSource;

        [SerializeField]
        private bool _waitForComplete = true;

        public override bool WaitForComplete => _waitForComplete;

        [SerializeField]
        [ReadOnly]
        private bool _isPlaying;

        public override bool IsDone => !_isPlaying;

        public override void OnStart()
        {
            if(EffectsManager.Instance.EnableViewerShake) {
                _impulseSource.GenerateImpulse();
            }

            Assert.IsTrue(_impulseSource.m_ImpulseDefinition.m_TimeEnvelope.Duration >= 0);

            _isPlaying = true;
            TimeManager.Instance.RunAfterDelay(_impulseSource.m_ImpulseDefinition.m_TimeEnvelope.Duration, () => _isPlaying = false);
        }
    }
}