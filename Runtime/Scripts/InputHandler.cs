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
            return;
        }

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(mousePosition));

        if (!rayHit) 
        {
            return;
        }

        if (rayHit.collider.gameObject.TryGetComponent<ClickableObject>(out ClickableObject clickableObject))
        {
            if (clickableObject.gameObject.TryGetComponent<FirstSpot>(out FirstSpot firstSpot))
            {
                if (!firstSpot.HasCustomer)
                {
                    return;
                }
            }
            m_playerMovement.MoveToMousePosition(clickableObject);
        }
    }

}
