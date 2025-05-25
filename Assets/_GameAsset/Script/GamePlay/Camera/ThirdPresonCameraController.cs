using UnityEngine;

public class ThirdPresonCameraController : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _orientationTrasform;
    [SerializeField] private Transform _playerVisualTransform;
    [Header ("Settings")]
    [SerializeField] private float _rotationSpeed;

    private void Update()
    {
      
        Vector3 viewDirection = _playerTransform.position - new Vector3(transform.position.x, _playerTransform.position.y, transform.position.z);

        _orientationTrasform.forward = viewDirection.normalized;

        float horizantalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 InputDirection = _orientationTrasform.forward * verticalInput + _orientationTrasform.right * horizantalInput;

        if (InputDirection != Vector3.zero)
        {
            _playerVisualTransform.forward = Vector3.Slerp(_playerVisualTransform.forward, InputDirection.normalized, Time.deltaTime * _rotationSpeed);
            _playerVisualTransform.forward = InputDirection.normalized;
      }
       
    }
    

}
