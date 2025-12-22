using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Button m_button;

    private PickupObject m_object;
    private InventoryUI m_inventoryUI;

    public PickupObject Object
    {
        get { return m_object; } 
    }

    private void Awake()
    {
        m_button.onClick.AddListener(RemoveObject);
    }

    public void SetObject(PickupObject pickupObject, InventoryUI inventoryUI)
    {
        m_object = pickupObject;
        m_inventoryUI = inventoryUI;

        #region Set image
        m_object.GetSprite(out Sprite sprite, out Color color);
        m_button.image.sprite = sprite;
        m_button.image.color = color;
        #endregion
    }

    private void RemoveObject()
    {
        if (m_object == null || m_inventoryUI == null)
        {
            return;
        }

        m_inventoryUI.RemoveObject(this, m_object);
    }
}
