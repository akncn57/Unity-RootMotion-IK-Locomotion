using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Inputs
{
    public class InputReader : MonoBehaviour, IA_Character.IMovementActions
    {
        public Vector2 Movement { get; set; }
        public bool IsRunning { get; set; }
        public bool IsCrouching { get; set; }
        
        private IA_Character _controls;

        private void Awake()
        {
            _controls = new IA_Character();
            _controls.Movement.SetCallbacks(this);
            _controls.Movement.Enable();
        }

        private void OnDestroy()
        {
            _controls?.Dispose();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Movement = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Debug.Log("OnLook");
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            Debug.Log("OnCrouch");
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            Debug.Log("OnJump");
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            Debug.Log("OnRun");
        }
    }
}