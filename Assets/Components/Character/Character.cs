using Components.Cameras;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] GameObject _gameObject;
    public GameObject GameObject => _gameObject;

    [SerializeField] Transform _transform;
    public Transform Transform => _transform;

    [SerializeField] Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;

    [SerializeField] Animator _animator;
    public Animator Animator => _animator;

    [SerializeField] float _movementSpeed = 1;
    public float MovementSpeed => _movementSpeed;

    [SerializeField] float _walkSpeedScale = 0.4f;
    public float WalkSpeedScale => _walkSpeedScale;

    [SerializeField] float _runSpeedScale = 1.4f;
    public float RunSpeedScale => _runSpeedScale;

    [SerializeField] float _backwarMoveSpeedScale = 0.7f;
    public float BackwarMoveSpeedScale => _backwarMoveSpeedScale;

    [SerializeField] float _jumpForce = 3f;
    public float JumpForce => _jumpForce;

    [SerializeField] float _jumpAddForceTime = 0.225f;
    public float JumpAddForceTime => _jumpAddForceTime;

    [SerializeField] Vector3 _groundedColliderPosition;
    public Vector3 GroundedColliderPosition => _groundedColliderPosition;

    [SerializeField] float _groundedColliderRadius = 0.2f;
    public float GroundedColliderRadius => _groundedColliderRadius;

    [SerializeField] LayerMask _groundedMask = 0;
    public LayerMask GroundedMask => _groundedMask;

    private bool isGrounded;
    public bool IsGrounded
    {
        get => isGrounded;
        set
        {
            isGrounded = value;
            Animator.SetBool("IsGrounded", value);
        }
    }

    private float lastJumpedTime = float.MinValue;

    void Start()
    {
        MainCamera.Current.Follow = Transform;
    }

    void Update()
    {
        ResetMoveAnimations();

        float speed = MovementSpeed;
        float direction = Input.GetAxis("Vertical");
        if (direction != 0)
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                // Walk
                speed *= WalkSpeedScale;
                Animator.SetBool($"Walk{(direction > 0 ? "Forward" : "Backward")}", true);
            }
            else if (Input.GetKey(KeyCode.LeftShift) && direction > 0)
            {
                // Sprint
                speed *= RunSpeedScale;
                Animator.SetBool($"Sprint", true);
            }
            else
            {
                // Run
                Animator.SetBool($"Run{(direction > 0 ? "Forward" : "Backward")}", true);
            }
        }
        if (direction < 0) speed *= BackwarMoveSpeedScale;
        Vector3 velocity = Transform.forward * direction * speed;
        velocity.y = Rigidbody.velocity.y;
        Rigidbody.velocity = velocity;

        IsGrounded = CalculateIsGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded && Time.time - lastJumpedTime > JumpAddForceTime + 0.1f)
        {
            lastJumpedTime = Time.time;
            Animator.SetTrigger("Jump");
            StartCoroutine(CallJumpRoutine());
        }
        Transform.rotation = Quaternion.Euler(0, MainCamera.Current.Transform.rotation.eulerAngles.y, 0);
    }

    private bool CalculateIsGrounded()
    {
        return Physics.CheckSphere(Transform.position + GroundedColliderPosition, GroundedColliderRadius, GroundedMask);
    }

    private IEnumerator CallJumpRoutine()
    {
        yield return new WaitForSeconds(JumpAddForceTime);
        Rigidbody.velocity += Vector3.up * JumpForce;
    }

    private void ResetMoveAnimations()
    {
        // Reset all move animations
        Animator.SetBool("WalkForward", false);
        Animator.SetBool("WalkBackward", false);
        Animator.SetBool("RunForward", false);
        Animator.SetBool("RunBackward", false);
        Animator.SetBool("Sprint", false);
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if (Selection.gameObjects.Contains(GameObject))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Transform.position + GroundedColliderPosition, GroundedColliderRadius);
        }
    }
#endif

    private void Reset()
    {
        _gameObject = gameObject;
        _transform = transform;
        TryGetComponent(out _animator);
        TryGetComponent(out _rigidbody);
    }
}
