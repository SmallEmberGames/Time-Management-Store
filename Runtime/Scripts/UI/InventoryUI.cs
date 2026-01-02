using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Slot m_inventorySlot;
    private PlayerInventory m_playerInventory;
    private RectTransform m_rectTransform;
    private List<Slot> m_inventorySlots;

    private float m_padding;
    private float m_spacing;
    private float m_slotSize;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_slotSize = m_inventorySlot.GetComponent<RectTransform>().sizeDelta.x;
        HorizontalLayoutGroup horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        m_padding = horizontalLayoutGroup.padding.left + horizontalLayoutGroup.padding.right;
        m_spacing = horizontalLayoutGroup.spacing;

        m_inventorySlots = new List<Slot>();
    }

    private void Start()
    {
        m_playerInventory = PlayerInventory.Instance;
    }

    public void SetSlots(int slots)
    {
        float inventorySize = m_padding + (slots * m_slotSize);

        for (int i = 0; i < slots; i++)
        {
            if (i == slots - 1)
            {
                break;
            }

            inventorySize += m_spacing;
        }

        m_rectTransform.sizeDelta = new Vector2(inventorySize, m_rectTransform.sizeDelta.y);
    }

    public void AddObject(PickupObject pickupObject)
    {
        Slot newSlot = Instantiate(m_inventorySlot, m_rectTransform);
        newSlot.SetObject(pickupObject, this);
        m_inventorySlots.Add(newSlot);
        Debug.Log("AddObject UI");
    }

    public void RemoveSlot(PickupObject obj)
    {
        for (int i = 0; i < m_inventorySlots.Count; i++)
        {
            Slot slot = m_inventorySlots[i];
            if (slot.Object == obj)
            {
                m_inventorySlots.Remove(slot);
                Destroy(slot.gameObject);
                break;
            }
        }
    }

    public void RemoveObject(Slot slot, PickupObject obj)
    {
        m_playerInventory.RemoveObject(obj);
        m_inventorySlots.Remove(slot);
        Destroy(slot.gameObject);
    }
}
