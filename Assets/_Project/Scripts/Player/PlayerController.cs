using _Project.Scripts.Inputs;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace _Project.Scripts.Player
{
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController controller;
        [SerializeField] private InputReader inputReader;
        
        [Header("Rigs")]
        [SerializeField] private TwoBoneIKConstraint leftArmRig;
        [SerializeField] private TwoBoneIKConstraint rightArmRig;
        [SerializeField] private MultiRotationConstraint leftHandRig;
        [SerializeField] private MultiRotationConstraint rightHandRig;
        
        
        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 2.0f;
        [SerializeField] private float runSpeed = 5.0f;
        [SerializeField] private float rotationSpeed = 10.0f;

        [Header("Smoothing")]
        [SerializeField] private float speedSmoothTime = 0.1f;
        
        [Header("Head Aiming")]
        [SerializeField] private Transform lookTarget;
        [SerializeField] private float lookDistance = 10f;
        
        [Header("Hand IK Settings")]
        [SerializeField] private float handRayDistance = 0.75f;
        [SerializeField] private float rigLerpSpeed = 5f;
        [SerializeField] private Vector3 rotationOffset;

        private Camera _mainCamera;
        private Vector2 _currentVelocity;
        private Vector2 _velocitySmoothVelocity;
    
        private readonly int _xVelHash = Animator.StringToHash("X_Velocity");
        private readonly int _yVelHash = Animator.StringToHash("Y_Velocity");

        private void Awake()
        {
            // Cache main camera
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleMovement();
            HandleHeadTracking();
            HandleArmIK(leftArmRig, leftHandRig, -transform.right);
            HandleArmIK(rightArmRig, rightHandRig, transform.right);
        }

        private void HandleMovement()
        {
            // Get input values
            var input = inputReader.Movement;
            var isRunning = inputReader.IsRunning;
            var targetSpeed = isRunning ? runSpeed : walkSpeed;
            
            // Smooth damp animator parameters
            _currentVelocity = Vector2.SmoothDamp(
                _currentVelocity, 
                input * targetSpeed, 
                ref _velocitySmoothVelocity, 
                speedSmoothTime
            );
            
            if (input.sqrMagnitude > 0.01f)
            {
                // Rotate the character relative to the camera
                HandleRotation();
            }

            // Update animator parameters
            animator.SetFloat(_xVelHash, _currentVelocity.x);
            animator.SetFloat(_yVelHash, _currentVelocity.y);
        }

        private void HandleRotation()
        {
            var camForward = _mainCamera.transform.forward;
            camForward.y = 0;
                
            var targetRot = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }

        private void HandleHeadTracking()
        {
            // Positions the target point in front of the camera for the head to track
            var targetPos = _mainCamera.transform.position + _mainCamera.transform.forward * lookDistance;
            lookTarget.position = targetPos;
        }

        private void HandleArmIK(TwoBoneIKConstraint armRig, MultiRotationConstraint handRig, Vector3 rayDirection)
        {
            var rayOrigin = transform.position + Vector3.up * 1.5f;
    
            Debug.DrawRay(rayOrigin, rayDirection * handRayDistance, Color.red);

            if (Physics.Raycast(rayOrigin, rayDirection, out var hit, handRayDistance))
            {
                var targetWeight = Mathf.Lerp(armRig.weight, 1f, Time.deltaTime * rigLerpSpeed);
        
                armRig.weight = targetWeight;
                handRig.weight = targetWeight;
                armRig.data.target.position = hit.point;
        
                var lookRot = Quaternion.LookRotation(-hit.normal, transform.up);
                armRig.data.target.rotation = lookRot * Quaternion.Euler(rotationOffset);
            }
            else
            {
                var targetWeight = Mathf.Lerp(armRig.weight, 0f, Time.deltaTime * rigLerpSpeed);
        
                armRig.weight = targetWeight;
                handRig.weight = targetWeight;
            }
        }
    }
}