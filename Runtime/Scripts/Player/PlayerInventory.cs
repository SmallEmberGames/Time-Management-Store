using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [SerializeField] private InventoryUI m_inventoryUI;
    [SerializeField] private int m_inventorySlots;

    private List<PickupObject> _inventoryObjects = new List<PickupObject>();
    private int _currentSlotsTaken;

    public PickupObject[] InventoryObjects
    {
        get { return _inventoryObjects.ToArray(); }
    }

    public int InventorySlots
    {
        get { return m_inventorySlots; }
    }

    private void Awake()
    {
        Instance = this;
        m_inventoryUI.SetSlots(m_inventorySlots);
    }

    public bool IsInventoryFull()
    {
        bool isInventoryFull = _currentSlotsTaken == m_inventorySlots;
        if (!isInventoryFull)
        {
            _currentSlotsTaken++;
        }

        return isInventoryFull;
    }

    public void AddObject(PickupObject obj)
    {
        _inventoryObjects.Add(obj);
        m_inventoryUI.AddObject(obj);
    }

    public void RemoveObject(PickupObject obj)
    {
        m_inventoryUI.RemoveSlot(obj);

        for (int i = 0; i < _inventoryObjects.Count; i++)
        {
            if (_inventoryObjects[i] == obj)
            {
                _inventoryObjects.RemoveAt(i);
                _currentSlotsTaken--;
                break;
            }
        }
    }
}
