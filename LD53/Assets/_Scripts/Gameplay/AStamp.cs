using UnityEngine;
using Util.Attributes;
using Util.Systems;

namespace LD53.Gameplay
{
    public abstract class AStamp : MonoBehaviour, IReceivable
    {
        [Header("Components")]
        [SerializeField] protected SpriteRenderer _highlight;
        [SerializeField] protected float _fadeDelta = 1f;

        [Header("Debug")]
        [SerializeField, ReadOnly] public bool IsHovering = false;

        [Header("Audio")]
        protected AudioSource _audioSource;
        [SerializeField] protected AudioClip _stampAudioClip;

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
            if (ShouldStamp(mail) == false) 
                return false;
            
            StampMail(mail);
            return true;
        }

        protected abstract bool ShouldStamp(Mail mail);

        protected abstract void StampMail(Mail mail);

        protected abstract void IgnoreMail(Mail mail);

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
