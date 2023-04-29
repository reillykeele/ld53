using System;
using System.Collections.Generic;
using LD53.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Util.Attributes;

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

        [SerializeField, ReadOnly] public MailType MailType;

        private Collider2D _collider;

        private Camera _camera;
        private Vector3 _pivotOffset;

        // temp
        private SpriteRenderer[] _spriteRenderers;

        [Header("Debug")]
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
        }

        void Update()
        {
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
            _spriteRenderers[0].color = Color.yellow;
        }

        void OnMouseExit()
        {
            // TODO: Highlight effect
            _spriteRenderers[0].color = Color.white;
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
        }

        void OnMouseDown()
        {
            // TODO: Figure out what is on top maybe? Is that done implicitly?

            _isDragging = true;

            _pivotOffset = transform.position - GetMousePosition();
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
    }
}
