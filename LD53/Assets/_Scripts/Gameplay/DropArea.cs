using UnityEngine;
using Util.Attributes;
using Util.Systems;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class DropArea : MonoBehaviour, IReceivable
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _highlight;

        [Header("Configuration")]
        [SerializeField] private MailType _acceptedMailType;
        [SerializeField] private float _fadeDelta = 1f;

        [Header("Debug")]
        [SerializeField, ReadOnly] public bool IsHovering = false;

        void Awake()
        {
            _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, 0f);
        }

        void Update()
        {
            if (GameSystem.Instance.IsPlaying() == false) return;

            if (IsHovering)
            {
                _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, Mathf.MoveTowards(_highlight.color.a, 0.5f, _fadeDelta));
            }
            else
            {
                _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, Mathf.MoveTowards(_highlight.color.a, 0.0f, _fadeDelta));
            }
        }

        public bool Receive(Mail mail)
        {
            // TODO: Process mail
            if (mail.HasPostage && mail.HasReceivedStamp && mail.HasReturnToSenderStamp == false && mail.MailType == _acceptedMailType)
            {
                GameManager.Instance.AddScore(1);
            }
            else
            {
                Debug.Log("bleh");
            }

            mail.DepositInBin();
            return true;
        }

        public void Highlight()
        {
            IsHovering = true;
        }

        public void Unhighlight()
        {
            IsHovering = false;
        }
    }
}
