using pdxpartyparrot.Core.Audio;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class MusicEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private AudioClip _music;

        public override bool WaitForComplete => false;

        public override bool IsDone => true;

        public override void OnStart()
        {
            if(EffectsManager.Instance.EnableAudio) {
                AudioManager.Instance.PlayMusic(_music);
            }
        }

        public override void OnStop()
        {
            AudioManager.Instance.StopAllMusic();
        }
    }
}
