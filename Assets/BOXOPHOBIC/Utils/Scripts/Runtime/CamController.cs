#if ENABLE_INPUT_SYSTEM
#define USE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine;

namespace Boxophobic.Utility
{
    public class CamController : MonoBehaviour
    {
        public float movementSpeed = 5f;
        public float accelerationMultiplier = 2f;
        public float sensitivity = 2f;
        private float yaw = 0f;
        private float pitch = 0f;
#if USE_INPUT_SYSTEM
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction shiftAction;
#endif
        void OnEnable()
        {
#if USE_INPUT_SYSTEM
            var map = new InputActionMap("Cam Controller");
            lookAction = map.AddAction("look", binding: "<Mouse>/delta");
            moveAction = map.AddAction("move");
            shiftAction = map.AddAction("shift");
            moveAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");
            shiftAction.AddBinding("<Keyboard>/leftShift");
            shiftAction.AddBinding("<Keyboard>/rightShift");
            lookAction.Enable();
            moveAction.Enable();
            shiftAction.Enable();
#endif
        }
        void OnDisable()
        {
#if USE_INPUT_SYSTEM
            lookAction?.Disable();
            moveAction?.Disable();
            shiftAction?.Disable();
#endif
        }
        void Start()
        {
            yaw = transform.eulerAngles.y;
            float rawPitch = transform.eulerAngles.x;
            // Normalize from 0-360 to -180-180 so e.g. 350 becomes -10
            pitch = rawPitch > 180f ? rawPitch - 360f : rawPitch;
        }
        void Update()
        {
            float currentSpeed = movementSpeed;
            float horizontal = 0f;
            float vertical = 0f;
            float mouseX = 0f;
            float mouseY = 0f;
#if USE_INPUT_SYSTEM
            bool shifting = shiftAction.ReadValue<float>() > 0f;
            if (shifting) currentSpeed *= accelerationMultiplier;
            var move = moveAction.ReadValue<Vector2>();
            horizontal = move.x;
            vertical = move.y;
            var look = lookAction.ReadValue<Vector2>();
            mouseX = look.x * 0.1f;
            mouseY = look.y * 0.1f;
#else
            bool shifting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (shifting) currentSpeed *= accelerationMultiplier;
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
#endif
            transform.Translate(horizontal * currentSpeed * Time.deltaTime, 0f, vertical * currentSpeed * Time.deltaTime);
            yaw += sensitivity * mouseX;
            pitch -= sensitivity * mouseY;
            pitch = Mathf.Clamp(pitch, -90f, 90f);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}