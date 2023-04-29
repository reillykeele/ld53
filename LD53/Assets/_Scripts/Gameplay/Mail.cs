using System;
using System.Collections.Generic;
using LD53.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Util.Attributes;
using Util.Helpers;
using Util.Systems;

namespace LD53.Gameplay
{
    public enum MailType
    {
        Blue = 0,
        Red = 1
    }

    [RequireComponent(typeof(Collider2D))]
    public class Mail : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private float _moveDelta = 1f;
        [SerializeField] private float _rotationDelta = 1f;
        [SerializeField] private float _fadeDelta = 1f;

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
        [SerializeField, ReadOnly] public bool HasPostage;
        [SerializeField, ReadOnly] public bool HasReceivedStamp;
        [SerializeField, ReadOnly] public bool HasReturnToSenderStamp;
        [SerializeField, ReadOnly] public MailType MailType;
        [SerializeField, ReadOnly] public int SortOrder;

        [Header("Spawn Animation")]
        [SerializeField] private float _spawnMoveDuration = 1f;
        [SerializeField] private float _spawnRotationDuration = 1f;
        [SerializeField] private LeanTweenType _spawnMoveEase = LeanTweenType.notUsed;
        [SerializeField] private LeanTweenType _spawnRotationEase = LeanTweenType.notUsed;
        [SerializeField, ReadOnly] public Vector3 GoalPosition;
        [SerializeField, ReadOnly] public Quaternion GoalRotation;
        [SerializeField, ReadOnly] public LTDescr _spawnMoveTween;
        [SerializeField, ReadOnly] public LTDescr _spawnRotateTween;
        [SerializeField, ReadOnly] public bool _spawnMoveFinished = false;
        [SerializeField, ReadOnly] public bool _spawnRotateFinished = false;

        private Collider2D _collider;

        private Camera _camera;
        private Vector3 _pivotOffset;

        // temp
        private SpriteRenderer[] _spriteRenderers;

        [Header("Debug")]
        [SerializeField, ReadOnly] private bool _isHovering = false;
        [SerializeField, ReadOnly] private bool _isDragging = false;
        [SerializeField, ReadOnly] private bool _isSpawning = false;

        void Awake()
        {
            // setup components
            _collider =  GetComponent<Collider2D>();

            // temp
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            _receivedStamp.gameObject.Disable();
            _returnToSenderStamp.gameObject.Disable();
        }

        void Start()
        {
            _camera = Camera.main;

            // setup mail
            _address.color = MailType switch
            {
                MailType.Blue => Color.blue,
                MailType.Red => Color.red,
                
                _ => Color.black
            };

            if (HasPostage == false) _postage.gameObject.Disable();

            _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, 0f);

            // spawn animation
            _isSpawning = true;
            _spawnMoveTween = LeanTween.move(gameObject, GoalPosition, _spawnMoveDuration)
                .setEase(_spawnMoveEase)
                .setIgnoreTimeScale(false);
            _spawnMoveTween.setOnComplete(FinishSpawnMove);

            _spawnRotateTween = LeanTween.rotate(gameObject, GoalRotation.eulerAngles, _spawnRotationDuration)
                .setEase(_spawnRotationEase)
                .setIgnoreTimeScale(false);
            _spawnRotateTween.setOnComplete(FinishSpawnRotate);

            UpdateSortOrder();
        }

        void Update()
        {
            if (GameSystem.Instance.IsPlaying() == false) return;

            // highlight
            if (_isDragging)
            {
                _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, Mathf.MoveTowards(_highlight.color.a, 1f, _fadeDelta));
            }
            else if (_isHovering)
            {
                _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, Mathf.MoveTowards(_highlight.color.a, 0.5f, _fadeDelta));
            }
            else
            {
                _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, Mathf.MoveTowards(_highlight.color.a, 0f, _fadeDelta));
            }

            // movement
            if (_isDragging)
            {
                if (transform.rotation != Quaternion.identity)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, _rotationDelta);

                if (_isSpawning)
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
            // TODO: Highlight effect
            _isHovering = true;

            _outline.gameObject.Disable();
        }

        void OnMouseExit()
        {
            _isHovering = false;

            if (_isDragging == false)
                _outline.gameObject.Enable();
        }

        void OnMouseUp()
        {
            _isDragging = false;

            var overlaps = new List<Collider2D>();
            _collider.OverlapCollider(new ContactFilter2D(), overlaps);

            foreach (var col in overlaps)
            {
                if (col.TryGetComponent<IReceivable>(out var receivable))
                {
                    // try and receive
                    if (receivable.Receive(this))
                        return;
                }
            }

            // reset sort order to top of stack
            SortOrder = GameManager.Instance.GetHighestSortingOrder() + 1;
            UpdateSortOrder();

            if (_isHovering == false)
                _outline.gameObject.Enable();
        }

        void OnMouseDown()
        {
            _isDragging = true;

            SortOrder = Int16.MaxValue;
            UpdateSortOrder();

            _pivotOffset = transform.position - GetMousePosition();

            _outline.gameObject.Disable();
        }

        void OnMouseDrag()
        {
            var newPosition = GetMousePosition() + _pivotOffset;
            transform.position = Vector3.MoveTowards(transform.position, newPosition, _moveDelta);
        }

        private Vector3 GetMousePosition()
        {
            // TODO Change to use input action from input reader probably
            var mousePos = Mouse.current.position.value;

            var pos = _camera.ScreenToWorldPoint(mousePos);
            pos.z = 0f;

            return pos;
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

        private void UpdateSortOrder()
        {
            foreach (var render in _spriteRenderers)
            {
                render.sortingOrder = SortOrder;
            }
        }

        private void FinishSpawnMove()
        {
            _spawnMoveFinished = true;

            if (_spawnRotateFinished) _isSpawning = false;
        }

        private void FinishSpawnRotate()
        {
            _spawnRotateFinished = true;

            if (_spawnMoveFinished) _isSpawning = false;
        }

        private void StopSpawnAnimation()
        {
            LeanTween.cancel(_spawnMoveTween.id);
            LeanTween.cancel(_spawnRotateTween.id);

            _isSpawning = false;
        }


    }
}

