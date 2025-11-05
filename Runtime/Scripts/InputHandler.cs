using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private PlayerMovement m_playerMovement;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            Debug.Log($"Mouse position return;");
            return;
        }

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint( mousePosition );
        Debug.Log($"Mouse position {worldPosition}");
        m_playerMovement.MoveToMousePosition(worldPosition);
    }

}
