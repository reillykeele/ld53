using System;
using System.Collections.Generic;
using System.Linq;
using LD53.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Util.Attributes;
using Util.Helpers;
using Util.Systems;
using Random = UnityEngine.Random;

namespace LD53.Gameplay
{
    public enum MailType
    {
        Blue = 0,
        Red = 1,
        Yellow = 2,
        Purple = 3
    }

    [Serializable]
    public struct MailColorMap
    {
        public MailType MailType;
        public Color color;
    }

    [RequireComponent(typeof(Collider2D))]
    public class Mail : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        
        [SerializeField] private float _moveDelta = 1f;
        [SerializeField] private float _rotationDelta = 1f;
        [SerializeField] private float _fadeDelta = 1f;

        [SerializeField, Min(0f)] private float _minOverlapDistance = 0f;

        [Header("Components")]
        [SerializeField] private SpriteRenderer _address;
        [SerializeField] private SpriteRenderer _postage;
        [Space(5f)]
        [SerializeField] private SpriteRenderer _receivedStamp;
        [SerializeField] private SpriteRenderer _returnToSenderStamp;
        [Space(5f)]
        [SerializeField] private SpriteRenderer _outline;
        [SerializeField] private SpriteRenderer _highlight;

        [Header("Properties")] 
        [SerializeField, ReadOnly] public MailSpawner Spawner;
        [SerializeField, ReadOnly] public MailType MailType;
        [SerializeField, ReadOnly] public bool HasPostage;
        [SerializeField, ReadOnly] public bool HasReceivedStamp;
        [SerializeField, ReadOnly] public bool HasReturnToSenderStamp;

        [Header("Spawn Animation")]
        [SerializeField] private float _spawnMoveDuration = 1f;
        [SerializeField] private float _spawnRotationDuration = 1f;
        [Space(5f)]
        [SerializeField] private LeanTweenType _spawnMoveEase = LeanTweenType.notUsed;
        [SerializeField] private LeanTweenType _spawnRotationEase = LeanTweenType.notUsed;
        [Space(5f)]
        [SerializeField, ReadOnly] public Vector3 GoalPosition;
        [SerializeField, ReadOnly] public Quaternion GoalRotation;
        [Space(5f)]
        [SerializeField, ReadOnly] public LTDescr _spawnMoveTween;
        [SerializeField, ReadOnly] public LTDescr _spawnRotateTween;
        [Space(5f)]
        [SerializeField, ReadOnly] public bool _spawnMoveFinished = false;
        [SerializeField, ReadOnly] public bool _spawnRotateFinished = false;

        [Header("Deposit Animation")]
        [SerializeField] private float _scaleOutDuration = 1f;

        [Header("Other")] 
        [SerializeField] private List<MailColorMap> _colorMap;
        [SerializeField] private Sprite[] _postageStamps;

        [Header("Debug")]
        [SerializeField, ReadOnly] public bool IsHovering = false;
        [SerializeField, ReadOnly] public bool IsDragging = false;
        [SerializeField, ReadOnly] public bool IsSpawning = false;
        [SerializeField, ReadOnly] public bool IsDeposited = false;

        [Header("Audio")] 
        private AudioSource _audioSource;
        [SerializeField] private AudioClip _spawnAudioClip;

        private Collider2D _collider;

        private Camera _camera;
        private Vector3 _pivotOffset;

        private Collider2D[] _overlaps = new Collider2D[32];
        private ContactFilter2D _filter;

        void Awake()
        {
            _collider =  GetComponent<Collider2D>();

            _audioSource = GetComponent<AudioSource>();

            _filter = new ContactFilter2D() { useLayerMask = true, layerMask = LayerMask.GetMask("Bin") };
        }

        void Start()
        {
            _camera = Camera.main;

            Init();
        }

        public void Init()
        {
            // set properties to default
            IsHovering = false;
            IsDragging = false;
            IsDeposited = false;

            // setup mail
            _address.color = _colorMap.FirstOrDefault(x => MailType == x.MailType).color;

            if (HasPostage)
            {
                _postage.gameObject.Enable();

                _postage.sprite = _postageStamps[Random.Range(0, _postageStamps.Length)];
            }
            else
            {
                _postage.gameObject.Disable();
            }

            HasReceivedStamp = false;
            HasReturnToSenderStamp = false;

            _receivedStamp.gameObject.Disable();
            _returnToSenderStamp.gameObject.Disable();

            _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, 0f);

            // spawn animation
            if (IsSpawning)
            {
                _spawnMoveTween = LeanTween.move(gameObject, GoalPosition, _spawnMoveDuration)
                    .setEase(_spawnMoveEase)
                    .setIgnoreTimeScale(false);
                _spawnMoveTween.setOnComplete(FinishSpawnMove);

                _spawnRotateTween = LeanTween.rotate(gameObject, GoalRotation.eulerAngles, _spawnRotationDuration)
                    .setEase(_spawnRotationEase)
                    .setIgnoreTimeScale(false);
                _spawnRotateTween.setOnComplete(FinishSpawnRotate);

                _audioSource.PlayOneShot(_spawnAudioClip);
            }
        }
        void Update()
        {
            if (GameSystem.Instance.IsPlaying() == false) return;

            // highlight
            if (IsDragging)
            {
                _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, Mathf.MoveTowards(_highlight.color.a, 1f, _fadeDelta));
            }
            else if (IsHovering)
            {
                _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, Mathf.MoveTowards(_highlight.color.a, 0.5f, _fadeDelta));
            }
            else
            {
                _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, Mathf.MoveTowards(_highlight.color.a, 0f, _fadeDelta));
            }

            // movement
            if (IsDragging)
            {
                if (transform.rotation != Quaternion.identity)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, _rotationDelta);

                if (IsSpawning)
                    StopSpawnAnimation();
            }
            // else if (transform.rotation != GoalRotation)
            // {
            //     transform.rotation = Quaternion.RotateTowards(transform.rotation, GoalRotation, _rotationDelta);
            // }
        }

        #region Mouse Handling

        void OnMouseEnter()
        {
            if (GameSystem.Instance.IsPlaying() == false) return;

            // TODO: Highlight effect
            IsHovering = true;

            _outline.gameObject.Disable();
        }

        void OnMouseExit()
        {
            if (GameSystem.Instance.IsPlaying() == false) return;

            IsHovering = false;

            if (IsDragging == false)
                _outline.gameObject.Enable();
        }

        void OnMouseUp()
        {
            if (GameSystem.Instance.IsPlaying() == false) return;

            IsDragging = false;

            IReceivable receivable;
            var bins = Physics2D.OverlapAreaNonAlloc(new Vector2(-25, -25), new Vector2(25, 25), _overlaps, _filter.layerMask);
            for (int i = 0; i < bins; i++)
            {
                if (_overlaps[i].TryGetComponent<IReceivable>(out receivable))
                {
                    receivable.Unhighlight();
                }
            }

            var col = GetNearestOverlappingCollider();
            if (col?.TryGetComponent<IReceivable>(out receivable) == true)
            {
                receivable.Receive(this);
            }

            // reset sort order to top of stack
            transform.position = new Vector3(transform.position.x, transform.position.y, GameManager.Instance.GetTopZ());

            if (IsHovering == false)
                _outline.gameObject.Enable();
        }

        void OnMouseDown()
        {
            if (GameSystem.Instance.IsPlaying() == false) return;

            IsDragging = true;

            transform.position = new Vector3(transform.position.x, transform.position.y, GameManager.Instance.GetClippingPlaneZ() + 0.1f);
            
            _pivotOffset = transform.position - GetMousePosition();

            _outline.gameObject.Disable();
        }

        void OnMouseDrag()
        {
            if (GameSystem.Instance.IsPlaying() == false) return;

            var newPosition = GetMousePosition() + _pivotOffset;
            newPosition.z = transform.position.z;
            transform.position = Vector3.MoveTowards(transform.position, newPosition, _moveDelta);

            IReceivable receivable;
            var bins = Physics2D.OverlapAreaNonAlloc(new Vector2(-25, -25), new Vector2(25, 25), _overlaps, _filter.layerMask);
            for (int i = 0; i < bins; i++)
            {
                if (_overlaps[i].TryGetComponent<IReceivable>(out receivable))
                {
                    receivable.Unhighlight();
                }
            }

            var col = GetNearestOverlappingCollider();
            if (col?.TryGetComponent<IReceivable>(out receivable) == true)
            {
                receivable.Highlight();
            }
        }

        private Vector3 GetMousePosition()
        {
            // TODO Change to use input action from input reader probably
            var mousePos = Mouse.current.position.value;

            var pos = _camera.ScreenToWorldPoint(mousePos);
            pos.z = 0f;

            return pos;
        }

        private Collider2D GetNearestOverlappingCollider()
        {
            var numOverlaps = _collider.OverlapCollider(_filter, _overlaps);

            Collider2D col = null;
            for (int i = 0; i < numOverlaps; i++)
            {
                if (col == null)
                {
                    var dist = _collider.Distance(_overlaps[i]);

                    col = dist.distance <= -_minOverlapDistance ? _overlaps[i] : null;
                    continue;
                }

                var dist1 = _collider.Distance(col);
                var dist2 = _collider.Distance(_overlaps[i]);

                if (dist2.distance < dist1.distance)
                    col = _overlaps[i];
            }

            return col;
        }

        #endregion
        
        public void StampReceived()
        {
            HasReceivedStamp = true;

            _receivedStamp.gameObject.Enable();
        }

        public void StampReturnToSender()
        {
            HasReturnToSenderStamp = true;

            _returnToSenderStamp.gameObject.Enable();
        }

        public void DepositInBin()
        {
            // TODO Play particle effect and sound effect
            IsDeposited = true;

            var lt = LeanTween.scale(gameObject, Vector3.zero, _scaleOutDuration)
                .setEase(_spawnMoveEase)
                .setIgnoreTimeScale(false);
            lt.setOnComplete(Destroy);
        }

        private void FinishSpawnMove()
        {
            _spawnMoveFinished = true;

            if (_spawnRotateFinished) IsSpawning = false;
        }

        private void FinishSpawnRotate()
        {
            _spawnRotateFinished = true;

            if (_spawnMoveFinished) IsSpawning = false;
        }

        private void StopSpawnAnimation()
        {
            LeanTween.cancel(_spawnMoveTween.id);
            LeanTween.cancel(_spawnRotateTween.id);

            IsSpawning = false;
        }

        private void Destroy() => Spawner.Release(this);
    
    }
}

