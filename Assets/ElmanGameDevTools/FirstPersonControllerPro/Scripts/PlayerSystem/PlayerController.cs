using UnityEngine;

namespace ElmanGameDevTools.PlayerSystem
{
    [RequireComponent(typeof(CharacterController))]
    [AddComponentMenu("Elman Game Dev Tools/Player System/Player Controller (Walk Only)")]
    public class PlayerController : MonoBehaviour
    {
        [Header("REFERENCES")]
        public CharacterController controller;
        public Transform playerCamera;

        [Header("MOVEMENT SETTINGS")]
        public float speed = 6f;
        public float gravity = -25f;

        [Header("MOUSE LOOK")]
        public float sensitivity = 2f;
        public float maxLookUpAngle = 90f;
        public float maxLookDownAngle = -90f;

        [Header("CAMERA INERTIA")]
        [Range(1f, 30f)] public float cameraWeight = 12f;

        [Header("CAMERA TILT")]
        public bool enableCameraTilt = true;
        public float tiltAmount = 2f;
        public float turnTiltAmount = 1.5f;
        public float tiltSmoothness = 8f;
        public float maxTotalTilt = 5f;

        [Header("HEAD BOB")]
        public bool enableHeadBob = true;
        [Range(0.01f, 0.15f)] public float bobAmountX = 0.04f;
        [Range(0.01f, 0.15f)] public float bobAmountY = 0.05f;
        public float walkBobFrequency = 12f;
        public float bobSmoothness = 10f;

        [Header("GROUND CHECK")]
        public LayerMask groundLayer = 1;
        public float groundCheckDistance = 0.5f;

        private Vector3 _velocity;
        private bool _isGrounded;

        private float _targetYaw;
        private float _targetPitch;
        private float _currentYaw;
        private float _currentPitch;
        private float _smoothInputX;
        private float _currentTilt;

        private float _headBobTimer;
        private float _cameraBaseHeight;

        private void Start()
        {
            if (controller == null)
                controller = GetComponent<CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _cameraBaseHeight = playerCamera.localPosition.y;

            _targetYaw = transform.eulerAngles.y;
            _targetPitch = playerCamera.localEulerAngles.x;
            _currentYaw = _targetYaw;
            _currentPitch = _targetPitch;
        }

        private void Update()
        {
            CheckGround();
            HandleMovement();
            HandleMouseLook();
            HandleCameraTilt();

            if (enableHeadBob)
                HandleHeadBob();
        }

        private void CheckGround()
        {
            Vector3 origin = transform.position + Vector3.up * controller.radius;
            _isGrounded = Physics.SphereCast(
                origin,
                controller.radius * 0.8f,
                Vector3.down,
                out _,
                groundCheckDistance,
                groundLayer
            ) || controller.isGrounded;

            if (_isGrounded && _velocity.y < 0f)
                _velocity.y = -5f;
        }

        private void HandleMovement()
        {
            Vector3 input =
                transform.right * Input.GetAxis("Horizontal") +
                transform.forward * Input.GetAxis("Vertical");

            if (input.magnitude > 1f)
                input.Normalize();

            controller.Move(input * speed * Time.deltaTime);

            _velocity.y += gravity * Time.deltaTime;
            controller.Move(_velocity * Time.deltaTime);
        }

        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            _smoothInputX = Mathf.Lerp(_smoothInputX, mouseX, Time.deltaTime * cameraWeight);

            _targetYaw += mouseX;
            _targetPitch -= mouseY;
            _targetPitch = Mathf.Clamp(_targetPitch, maxLookDownAngle, maxLookUpAngle);

            float smooth = Mathf.Clamp01(Time.deltaTime * cameraWeight);
            _currentYaw = Mathf.Lerp(_currentYaw, _targetYaw, smooth);
            _currentPitch = Mathf.Lerp(_currentPitch, _targetPitch, smooth);

            transform.rotation = Quaternion.Euler(0f, _currentYaw, 0f);
            playerCamera.localRotation = Quaternion.Euler(_currentPitch, 0f, _currentTilt);
        }

        private void HandleCameraTilt()
        {
            if (!enableCameraTilt)
            {
                _currentTilt = 0f;
                return;
            }

            float keyboardTilt = -Input.GetAxis("Horizontal") * tiltAmount;
            float mouseTilt = -_smoothInputX * turnTiltAmount;
            float targetTilt = Mathf.Clamp(
                keyboardTilt + mouseTilt,
                -maxTotalTilt,
                maxTotalTilt
            );

            _currentTilt = Mathf.Lerp(
                _currentTilt,
                targetTilt,
                Time.deltaTime * tiltSmoothness
            );
        }

        private void HandleHeadBob()
        {
            float moveMag = new Vector2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            ).magnitude;

            if (!_isGrounded || moveMag < 0.1f)
            {
                _headBobTimer = 0f;
                playerCamera.localPosition = Vector3.Lerp(
                    playerCamera.localPosition,
                    new Vector3(0f, _cameraBaseHeight, 0f),
                    Time.deltaTime * bobSmoothness
                );
                return;
            }

            _headBobTimer += Time.deltaTime * walkBobFrequency;

            Vector3 bobPos = new Vector3(
                Mathf.Cos(_headBobTimer * 0.5f) * bobAmountX,
                _cameraBaseHeight + Mathf.Sin(_headBobTimer) * bobAmountY,
                0f
            );

            playerCamera.localPosition = Vector3.Lerp(
                playerCamera.localPosition,
                bobPos,
                Time.deltaTime * bobSmoothness
            );
        }
    }
}
