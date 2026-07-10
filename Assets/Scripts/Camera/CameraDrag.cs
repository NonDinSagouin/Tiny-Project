using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class CameraDrag : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField] private float zoomMin = 1f;
    [SerializeField] private float zoomMax = 20f;
    [SerializeField] private float zoomSmoothSpeed = 10f;
    [SerializeField] private float zoomSensitivity = 0.1f;
    
    private bool isDragging = false;
    private Vector3 dragOrigin;
    private Vector3 dragOffset;
    private float targetOrthographicSize;

    private Vector3 GetMousePosition => mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

    private void Awake()
    {
        mainCamera = Camera.main;
        targetOrthographicSize = mainCamera.orthographicSize;
    }

    public void OnDrag(CallbackContext context)
    {
        if (context.started) dragOrigin = GetMousePosition;
        isDragging = context.started ||context.performed;
    }

    public void OnZoom(CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 zoomDelta = context.ReadValue<Vector2>();
        float zoomAmount = zoomDelta.y * zoomSensitivity;
        targetOrthographicSize = Mathf.Clamp(targetOrthographicSize - zoomAmount, zoomMin, zoomMax);
    }

    void LateUpdate()
    {
        mainCamera.orthographicSize = Mathf.Lerp(
            mainCamera.orthographicSize,
            targetOrthographicSize,
            zoomSmoothSpeed * Time.deltaTime
        );

        if (!isDragging) return;

        dragOffset = GetMousePosition - transform.position;
        transform.position = dragOrigin - dragOffset;
    }

}
