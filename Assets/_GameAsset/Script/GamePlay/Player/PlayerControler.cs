using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform _orientationTransfrom;

    [Header("Movement Setting")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private KeyCode _movementKey;

    [Header("Jump setting")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private  float  _airMultiplier;
     [SerializeField] private  float  _airDrag;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _jumpForce;
    [SerializeField] private bool _canJump;

    [Header("Siliding Settings")]
    [SerializeField] private KeyCode _slideKey;

    [SerializeField] private float _slideMultiplier;
    [SerializeField] private float _slideDrag;

    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;
    private StateControler _stateController;

    private Rigidbody _playerRigidbody;
    private float _horizantalInput, _verticalInput;

    private Vector3 _movementDirection;
    private bool _isSliding = false; // boş bırakmak da false demektir bir şey farketmez 
    private void FixedUpdate()
    {
        SetPlayerMovement();

    }
    private void Awake()
    {
        _stateController = GetComponent<StateControler>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;
    }
    private void Update()
    {
        SetInputs();
        SetStates();
        SetPlayerDrag();
        LimitPlayerSpeed();
    }
    private void SetInputs()
    {
        _horizantalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(_slideKey))
        {
            _isSliding = true;

        }
        else if (Input.GetKeyDown(_movementKey))
        {
            _isSliding = false;
            
        }

        else if (Input.GetKey(_jumpKey) && _canJump && IsGrounded())
        {
            //  zıplama işlemi gerçekleşecek
            _canJump = false;
            SetPlayerJumping();
            Invoke(nameof(ResetJumping), _jumpCooldown);

        }

      
    }
    private void SetStates()
    {
        var _movementDirection = GetMovementDirection(); //varın Vector3 yazmakla hiçbir farkı yoktur Vector3 değişse bile bu staır etkilenmez
        var isGrounded = IsGrounded();
        var isSliding = IsSliding();
        var currentState = _stateController.GetCurrentState(); //karakter nasıl hareket ediyorsa karakterin statetini öyle belirler

        var newState = currentState switch
        {
            _ when _movementDirection == Vector3.zero && isGrounded && !_isSliding => PlayerState.Idle,
            _ when _movementDirection != Vector3.zero && isGrounded && !_isSliding => PlayerState.Move,
            _ when _movementDirection != Vector3.zero && isGrounded && _isSliding => PlayerState.Slide,
            _ when _movementDirection == Vector3.zero && isGrounded && _isSliding => PlayerState.SlideIdle,
            _ when !_canJump && !isGrounded => PlayerState.Jump,
            _ => currentState
        };
        if (newState != currentState)
        {
            _stateController.ChangeState(newState);
        }
  
    }

    private void SetPlayerMovement()
    {
        _movementDirection = _orientationTransfrom.forward * _verticalInput + _orientationTransfrom.right * _horizantalInput;

        float forceMultiplier = _stateController.GetCurrentState() switch
        {
            PlayerState.Move => 1f,
            PlayerState.Slide => _slideMultiplier,
            PlayerState.Jump => _airMultiplier,
            _ => 1f 
        };   
         _playerRigidbody.AddForce(_movementDirection.normalized * _movementSpeed *forceMultiplier, ForceMode.Force);
    }
    private void SetPlayerDrag()
    {
        _playerRigidbody.linearDamping = _stateController.GetCurrentState() switch
        {
            PlayerState.Move => _groundDrag,
            PlayerState.Slide => _slideDrag,
            PlayerState.Jump => _airDrag,

            _ => _playerRigidbody.linearDamping
        };
    
    }

    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        if (flatVelocity.magnitude > _movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _movementSpeed;
            _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, _playerRigidbody.linearVelocity.y, _playerRigidbody.linearVelocity.z);
        }
    }

    private void SetPlayerJumping()
    {
        _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        _playerRigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
    private void ResetJumping()
    {
        _canJump = true;
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundLayer);
    }
    private Vector3 GetMovementDirection()
    {
        return _movementDirection.normalized;
    }

   private bool IsSliding()
{
    return _isSliding;
}
}
   
