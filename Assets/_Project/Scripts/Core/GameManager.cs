using UnityEngine;

namespace _Project.Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            // Application.targetFrameRate = 60;

        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}