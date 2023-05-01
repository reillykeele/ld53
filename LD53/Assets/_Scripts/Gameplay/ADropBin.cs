using UnityEngine;
using Util.Attributes;
using Util.Systems;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class ADropBin : MonoBehaviour, IReceivable
    {
        [Header("Components")]
        [SerializeField] protected SpriteRenderer _highlight;

        [Header("Configuration")]
        [SerializeField] protected float _fadeDelta = 1f;

        [Header("Debug")]
        [SerializeField, ReadOnly] public bool IsHovering = false;

        [Header("Audio")]
        protected AudioSource _audioSource;
        [SerializeField] protected AudioClip _acceptClip;
        [SerializeField] protected AudioClip _rejectClip;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

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
            if (ShouldAccept(mail))
                AcceptMail(mail);
            else
                RejectMail(mail);

            mail.DepositInBin();
            return true;
        }

        protected abstract bool ShouldAccept(Mail mail);

        protected abstract void AcceptMail(Mail mail);

        protected abstract void RejectMail(Mail mail);

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
