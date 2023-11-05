using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Components.Cameras
{
    public class Movable3DCamera : MonoBehaviour
    {
        [SerializeField] GameObject _gameObject;
        public GameObject GameObject => _gameObject;

        [SerializeField] Transform _transform;
        public Transform Transform => _transform;

        [SerializeField] Camera _camera;
        public Camera Camera => _camera;

        [Header("Follow")]
        [SerializeField] Transform _follow;
        public Transform Follow
        {
            get => _follow;
            set => _follow = value;
        }

        [SerializeField] float _followDistance = 4;
        public float FollowDistance
        {
            get => _followDistance;
            set => _followDistance = value;
        }

        [SerializeField] Vector3 _followPosition;
        public Vector3 FollowPosition
        {
            get => _followPosition;
            set => _followPosition = value;
        }

        [Header("Movement Lerp")]
        [SerializeField] bool _lerpPosition = true;
        public bool LerpPosition
        {
            get => _lerpPosition;
            set => _lerpPosition = value;
        }

        [SerializeField] float _movementLerpSpeed = 10f;
        public float MovementLerpSpeed
        {
            get => _movementLerpSpeed;
            set => _movementLerpSpeed = value;
        }

        [SerializeField] bool _lerpRotation = true;
        public bool LerpRotation
        {
            get => _lerpRotation;
            set => _lerpRotation = value;
        }

        [SerializeField] float _rotateLerpSpeed = 10f;
        public float RotateLerpSpeed
        {
            get => _rotateLerpSpeed;
            set => _rotateLerpSpeed = value;
        }

        [Header("Mouse Rotation")]
        [SerializeField] bool _rotateByMouseEnabled = true;
        public bool RotateByMouseEnabled
        {
            get => _rotateByMouseEnabled;
            set => _rotateByMouseEnabled = value;
        }

        [SerializeField] float _mouseSentensity = 180f;
        public float MouseSentensity
        {
            get => _mouseSentensity;
            set => _mouseSentensity = value;
        }

        [Header("Free Movement")]
        [SerializeField] bool _freeMovementEnabled = true;
        public bool FreeMovementEnabled
        {
            get => _freeMovementEnabled;
            set => _freeMovementEnabled = value;
        }

        [SerializeField] float _freeMovementSpeed = 10f;
        public float FreeMovementSpeed
        {
            get => _freeMovementSpeed;
            set => _freeMovementSpeed = value;
        }

        [Header("Mouse Visible")]
        [SerializeField] bool _mouseVisible = false;
        public bool MouseVisible
        {
            get => _mouseVisible;
            set
            {
                _mouseVisible = value;
                Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = value;
            }
        }

        public Vector3 TargetPosition { get; set; }
        public Quaternion TargetRotation { get; set; }

        protected virtual void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;

            // Initialize mouse bisible
            MouseVisible = MouseVisible;

            // Initial target is current position
            TargetPosition = Transform.position;
            TargetRotation = Transform.rotation;
        }

        protected virtual void Update()
        {
            if (RotateByMouseEnabled)
            {
                // Get Mouse Input
                float mouseX = Input.GetAxis("Mouse X") * MouseSentensity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * MouseSentensity * Time.deltaTime;

                // Create a Quaternion representing the rotation around Y axis (mouse X movement)
                Quaternion yRotation = Quaternion.Euler(0, mouseX, 0);

                // Create a Quaternion representing the rotation around X axis (mouse Y movement)
                Quaternion xRotation = Quaternion.Euler(-mouseY, 0, 0);

                // Apply the rotations
                TargetRotation *= yRotation * xRotation;

                // Apply rotation blocks
                var euler = TargetRotation.eulerAngles;
                euler.z = 0;
                euler.x = euler.x % 360;
                if (euler.x > 180) euler.x -= 360;
                euler.x = Mathf.Clamp(euler.x, 0, 80);
                TargetRotation = Quaternion.Euler(euler);
            }
            if (FreeMovementEnabled && !Follow)
            {
                // Free move camera by movement speed
                TargetPosition += (
                        Transform.forward * Input.GetAxis("Vertical") +
                        Transform.right * Input.GetAxis("Horizontal") +
                        Vector3.up * System.Convert.ToInt32(Input.GetKey(KeyCode.Space)) +
                        Vector3.down * System.Convert.ToInt32(Input.GetKey(KeyCode.LeftShift))
                    ) * FreeMovementSpeed * Time.deltaTime;
            }
            if (Follow)
            {
                TargetPosition = Follow.position +
                    Follow.right * FollowPosition.x +
                    Follow.up * FollowPosition.y +
                    Follow.forward * FollowPosition.z
                    - Transform.forward * FollowDistance;
            }


        }

        private void FixedUpdate()
        {
            float scrollDelta = Input.mouseScrollDelta.y;

            if (scrollDelta != 0)
            {
                var value = scrollDelta * -1;
                if (value < 0 && _followDistance < 10) return;

                _followDistance += value;
            }
        }

        protected virtual void LateUpdate()
        {
            if (LerpPosition)
            {
                Transform.position = new Vector3(
                        Mathf.Lerp(Transform.position.x, TargetPosition.x, Mathf.Abs(TargetPosition.x - Transform.position.x) * Time.deltaTime * MovementLerpSpeed),
                        Mathf.Lerp(Transform.position.y, TargetPosition.y, Mathf.Abs(TargetPosition.y - Transform.position.y) * Time.deltaTime * MovementLerpSpeed),
                        Mathf.Lerp(Transform.position.z, TargetPosition.z, Mathf.Abs(TargetPosition.z - Transform.position.z) * Time.deltaTime * MovementLerpSpeed)
                    );
            }
            else
            {
                Transform.position = TargetPosition;
            }

            if (LerpRotation)
            {
                Transform.rotation = Quaternion.Lerp(Transform.rotation, TargetRotation, RotateLerpSpeed * Time.deltaTime);
            }
            else
            {
                Transform.rotation = TargetRotation;
            }
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (Selection.gameObjects.Contains(GameObject))
            {
                if (Follow)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(Follow.position +
                        Follow.right * FollowPosition.x +
                        Follow.up * FollowPosition.y +
                        Follow.forward * FollowPosition.z, 0.05f);
                }
            }
        }
#endif

        protected virtual void Reset()
        {
            _gameObject = gameObject;
            _transform = transform;
            TryGetComponent(out _camera);
        }
    }
}