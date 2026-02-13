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
    
        private Vector2 _currentVelocity;
        private Vector2 _velocitySmoothVelocity;
    
        private readonly int _xVelHash = Animator.StringToHash("X_Velocity");
        private readonly int _yVelHash = Animator.StringToHash("Y_Velocity");
        
        private void Update()
        {
            Move();
        }

        private void Move()
        {
            // 1. Girdileri Al
            float moveX = inputReader.Movement.x; // A-D veya Sol-Sağ
            float moveY = inputReader.Movement.y;   // W-S veya Yukarı-Aşağı
            bool isRunning = inputReader.IsRunning;

            Vector2 input = new Vector2(moveX, moveY);
            float targetSpeed = isRunning ? runSpeed : walkSpeed;

            // 2. Animator Parametrelerini Yumuşat (SmoothDamp en akıcı geçişi sağlar)
            // Giriş yoksa 0'a, varsa hedef hıza doğru pürüzsüz geçiş yapar
            _currentVelocity = Vector2.SmoothDamp(
                _currentVelocity, 
                input * targetSpeed, 
                ref _velocitySmoothVelocity, 
                speedSmoothTime
            );

            // 3. Karakteri Kameraya Göre Döndür (Opsiyonel ama TPS için gerekli)
            if (input.sqrMagnitude > 0.01f)
            {
                Vector3 camForward = Camera.main.transform.forward;
                camForward.y = 0;
                Quaternion targetRot = Quaternion.LookRotation(camForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
            }

            // 4. Animator'ı Güncelle
            animator.SetFloat(_xVelHash, _currentVelocity.x);
            animator.SetFloat(_yVelHash, _currentVelocity.y);
        }
    }
}