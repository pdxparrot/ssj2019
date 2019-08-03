using System;
using System.Collections.Generic;
using System.Text;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class HighScoresMenu : MenuPanel
    {
        [Serializable]
        private struct ExtraColumnEntry
        {
            public string id;

            public TextMeshProUGUI columnText;
        }

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        [Tooltip("Units per-second to scroll")]
        private float _scrollRate = 100.0f;

        [SerializeField]
        [CanBeNull]
        private TextMeshProUGUI _rankColumnText;

        [SerializeField]
        [CanBeNull]
        private TextMeshProUGUI _nameColumnText;

        [SerializeField]
        private TextMeshProUGUI _scoreColumnText;

        [SerializeField]
        [CanBeNull]
        private TextMeshProUGUI _playerCountColumnText;

        [SerializeField]
        private ExtraColumnEntry[] _extraColumns;

#region Unity Lifecycle
        private void Awake()
        {
            if(null != _rankColumnText) {
                _rankColumnText.text = string.Empty;
            }

            if(null != _nameColumnText) {
                _nameColumnText.text = string.Empty;
            }

            _scoreColumnText.text = string.Empty;

            if(null != _playerCountColumnText) {
                _playerCountColumnText.text = string.Empty;
            }

            foreach(ExtraColumnEntry extraColumn in _extraColumns) {
                extraColumn.columnText.text = string.Empty;
            }
        }

        private void OnEnable()
        {
            Dictionary<string, StringBuilder> columns = new Dictionary<string, StringBuilder>();
            HighScoreManager.Instance.HighScoresText(columns);

            if(null != _rankColumnText) {
                _rankColumnText.text = columns.GetOrAdd("rank").ToString();
            }

            if(null != _nameColumnText) {
                _nameColumnText.text = columns.GetOrAdd("name").ToString();
            }

            _scoreColumnText.text = columns.GetOrAdd("score").ToString();

            if(null != _playerCountColumnText) {
                _playerCountColumnText.text = columns.GetOrAdd("playerCount").ToString();
            }

            foreach(ExtraColumnEntry extraColumn in _extraColumns) {
                extraColumn.columnText.text = columns.GetOrAdd(extraColumn.id).ToString();
            }

            _scrollRect.verticalNormalizedPosition = 1.0f;
        }
#endregion

#region Event Handlers
        public override void OnMove(InputAction.CallbackContext context)
        {
            //_scrollRect.verticalNormalizedPosition = Mathf.MoveTowards(_scrollRect.verticalNormalizedPosition, 0.0f, 1.0f);
        }
#endregion
    }
}
