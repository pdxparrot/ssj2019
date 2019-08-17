using System;

using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Game.Level;

using UnityEngine;
using UnityEngine.UI;

namespace pdxpartyparrot.ssj2019.Level
{
    public abstract class BaseLevel : LevelHelper
    {
        [Space(10)]

        [SerializeField]
        private Image _fullScreenImage;

        [SerializeField]
        private Collider2D _cameraBounds;

        [Space(10)]

        [SerializeField]
        private FadeEffectTriggerComponent _enterFadeEffectTrigger;

        [SerializeField]
        private FadeEffectTriggerComponent _exitFadeEffectTrigger;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            if(null != _enterFadeEffectTrigger) {
                _enterFadeEffectTrigger.Image = _fullScreenImage;
            }

            if(null != _exitFadeEffectTrigger) {
                _exitFadeEffectTrigger.Image = _fullScreenImage;
            }
        }
#endregion

#region Events
        protected override void GameStartClientEventHandler(object sender, EventArgs args)
        {
            base.GameStartClientEventHandler(sender, args);

            GameManager.Instance.Viewer.SetBounds(_cameraBounds);
        }
#endregion
    }
}
