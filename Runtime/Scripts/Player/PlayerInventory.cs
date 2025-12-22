using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [SerializeField] private InventoryUI m_inventoryUI;
    [SerializeField] private int m_inventorySlots;

    private List<PickupObject> m_inventoryObjects = new List<PickupObject>();
    private int m_currentSlotsTaken;

    public PickupObject[] InventoryObjects
    {
        get { return m_inventoryObjects.ToArray(); }
    }

    public int InventorySlots
    {
        get { return m_inventorySlots; }
    }

    private void Awake()
    {
        Instance = this;
        //TODO: Set inventory spaces for the slots
        m_inventoryUI.SetSlots(m_inventorySlots);
    }

    public bool IsInventoryFull()
    {
        bool isInventoryFull = m_currentSlotsTaken == m_inventorySlots;
        if (!isInventoryFull)
        {
            m_currentSlotsTaken++;
        }

        return isInventoryFull;
    }

    public void AddObject(PickupObject obj)
    {
        m_inventoryObjects.Add(obj);
        //TODO add visually
        m_inventoryUI.AddObject(obj);
    }

    public void RemoveObject(PickupObject obj)
    {
        m_inventoryUI.RemoveSlot(obj);

        for (int i = 0; i < m_inventoryObjects.Count; i++)
        {
            if (m_inventoryObjects[i] == obj)
            {
                m_inventoryObjects.RemoveAt(i);
                break;
            }
        }
    }
}
