using UnityEngine;
using Util.Attributes;
using Util.Systems;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class ReceivedStampArea : MonoBehaviour, IReceivable
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _highlight;
        [SerializeField] private float _fadeDelta = 1f;

        [Header("Debug")]
        [SerializeField, ReadOnly] public bool IsHovering = false;

        [Header("Audio")]
        private AudioSource _audioSource;
        [SerializeField] private AudioClip _stampAudioClip;

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
            if (mail.HasReceivedStamp == false)
            {
                mail.StampReceived();

                _audioSource.PlayOneShot(_stampAudioClip);

                return true;
            }
            
            return false;
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