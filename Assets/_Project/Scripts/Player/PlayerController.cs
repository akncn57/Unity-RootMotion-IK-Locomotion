using _Project.Scripts.Inputs;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController controller;
        [SerializeField] private InputReader inputReader;
        
        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 2.0f;
        [SerializeField] private float runSpeed = 5.0f;
        [SerializeField] private float rotationSpeed = 10.0f;

        [Header("Smoothing")]
        [SerializeField] private float speedSmoothTime = 0.1f;
        
        [Header("Head Aiming")]
        [SerializeField] private Transform lookTarget;
        [SerializeField] private float lookDistance = 10f;
        [Range(0, 180)] public float maxRotationAngle = 90f;

        
    
        private Vector2 _currentVelocity;
        private Vector2 _velocitySmoothVelocity;
    
        private readonly int _xVelHash = Animator.StringToHash("X_Velocity");
        private readonly int _yVelHash = Animator.StringToHash("Y_Velocity");
        
        private void Update()
        {
            Move();
            HeadTrack();
        }

        private void Move()
        {
            // Get input values
            var moveX = inputReader.Movement.x;
            var moveY = inputReader.Movement.y;
            var isRunning = inputReader.IsRunning;
            var input = new Vector2(moveX, moveY);
            var targetSpeed = isRunning ? runSpeed : walkSpeed;
            
            // Smooth damp animator parameters
            _currentVelocity = Vector2.SmoothDamp(
                _currentVelocity, 
                input * targetSpeed, 
                ref _velocitySmoothVelocity, 
                speedSmoothTime
            );

            // Rotate the character relative to the camera
            if (input.sqrMagnitude > 0.01f)
            {
                var camForward = Camera.main.transform.forward;
                camForward.y = 0;
                
                var targetRot = Quaternion.LookRotation(camForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
            }

            // Update animator parameters
            animator.SetFloat(_xVelHash, _currentVelocity.x);
            animator.SetFloat(_yVelHash, _currentVelocity.y);
        }

        private void HeadTrack()
        {
            var targetPos = Camera.main.transform.position + Camera.main.transform.forward * lookDistance;
            lookTarget.position = targetPos;
        }
    }
}