using System;
using System.Collections.Generic;
using LD53.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Util.Attributes;
using Util.Helpers;

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

        [Header("bruh")]
        [SerializeField] private SpriteRenderer _outline;
        [SerializeField] private SpriteRenderer _highlight;

        [Header("bruh2")]
        [SerializeField, ReadOnly] public MailType MailType;
        [SerializeField, ReadOnly] public int SortOrder;

        private Collider2D _collider;

        private Camera _camera;
        private Vector3 _pivotOffset;

        // temp
        private SpriteRenderer[] _spriteRenderers;

        [Header("Debug")]
        [SerializeField, ReadOnly] private bool _isHovering = false;
        [SerializeField, ReadOnly] private bool _isDragging = false;
        [SerializeField, ReadOnly] private Quaternion _originalRotation;

        void Awake()
        {
            // setup components
            _collider =  GetComponent<Collider2D>();

            // temp
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        void Start()
        {
            _camera = Camera.main;

            _originalRotation = transform.rotation;

            _spriteRenderers[1].color = MailType switch
            {
                MailType.Blue => Color.blue,
                MailType.Red => Color.red,
                
                _ => Color.white
            };

            _highlight.color = new Color(_highlight.color.r, _highlight.color.g, _highlight.color.b, 0f);

            UpdateSortOrder();
        }

        void Update()
        {
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

            if (_isDragging)
            {
                if (transform.rotation != Quaternion.identity)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, _rotationDelta);
            } 
            else if (transform.rotation != _originalRotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _originalRotation, _rotationDelta);
            }
        }

        void OnMouseEnter()
        {
            // TODO: Highlight effect
            _isHovering = true;

            _outline.gameObject.Disable();
        }

        void OnMouseExit()
        {
            // TODO: Highlight effect
            _isHovering = _isDragging;

            if (_isHovering == false)
                _outline.gameObject.Enable();
        }

        void OnMouseUp()
        {
            _isDragging = false;

            var overlaps = new List<Collider2D>();
            _collider.OverlapCollider(new ContactFilter2D(), overlaps);

            foreach (var col in overlaps)
            {
                if (col.TryGetComponent<DropArea>(out var dropArea))
                {
                    dropArea.Receive(this);
                    // TODO: Transition out
                    Destroy(gameObject);
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
            var mousePos = Mouse.current.position.value;

            var pos = _camera.ScreenToWorldPoint(mousePos);
            pos.z = 0f;

            return pos;
        }

        private void UpdateSortOrder()
        {
            foreach (var render in _spriteRenderers)
            {
                render.sortingOrder = SortOrder;
            }
        }
    }
}
