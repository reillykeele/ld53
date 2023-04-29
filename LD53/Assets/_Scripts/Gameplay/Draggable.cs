using LD53.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD53.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class Draggable : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private float _moveDelta = 1f;

        private Camera _camera;
        private Vector3 _pivotOffset;

        void Start()
        {
            _camera = Camera.main;
        }

        void OnMouseEnter()
        {
            Debug.Log("mouse enter");
        }

        void OnMouseExit()
        {
            Debug.Log("mouse exit");
        }

        void OnMouseDown()
        {
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
