using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tabel : ClickableObject
{
    [SerializeField] private Transform[] m_tabelSpots;
    [SerializeField] private OrderBubbel m_orderBubbel;
    [SerializeField] private Vector2 m_randomEatingTime;

    private PlayerInventory _playerInventory;
    private CustomerLines _customerLines;
    private Dictionary<CustomerController, TabelCustomer> _tabelCustomers = new Dictionary<CustomerController, TabelCustomer>();
    private bool _orderDone;

    #region public gets and sets
    public OrderBubbel OrderBubbel
    {
        get { return m_orderBubbel; }
    }

    public Transform[] TabelSpots
    {
        get { return m_tabelSpots; }
    }

    public bool TabelTaken
    {
        get { return _tabelCustomers.Count != 0; }
    }

    public CustomerLines CustomerLines
    {
        set { _customerLines = value; }
    }
    #endregion

    public struct TabelCustomer
    {
        public int spotIndex;
        public bool isSitting;
    }

    private void Start()
    {
        _playerInventory = PlayerInventory.Instance;
    }

    public override void Action(PlayerMovement playerMovement)
    {
        PickupObject[] customerOrders = _playerInventory.InventoryObjects;
        for (int i = 0; i < customerOrders.Length; i++)
        {
            if (m_orderBubbel.ServedOrder(customerOrders[i], out bool orderComplete))
            {
                //The player has this order item so we can remove it from the customer order 
                _playerInventory.RemoveObject(customerOrders[i]);
            }
            if (!_orderDone && orderComplete)
            {
                _orderDone = true;
                Debug.Log("Order tabel complete");
                StartCoroutine(EatingTime());
            }
        }

        base.Action(playerMovement);
    }

    public void AddCustomerToSpot(CustomerController customer, int spotIndex)
    {
        TabelCustomer newTabelCustomer = new TabelCustomer()
        {
            spotIndex = spotIndex,
            isSitting = false,
        };

        _tabelCustomers.Add(customer, newTabelCustomer);
    }

    public void CustomerIsSitting(CustomerController customer)
    {
        if (!_tabelCustomers.ContainsKey(customer) || m_orderBubbel.OrderComplete)
        {
            return;
        }

        TabelCustomer tabelCustomer = _tabelCustomers[customer];
        tabelCustomer.isSitting = true;
        _tabelCustomers[customer] = tabelCustomer;

        #region Check if they are all sitting
        bool notAllSitting = false;
        foreach (var value in _tabelCustomers.Values)
        {
            if (!value.isSitting)
            {
                notAllSitting = true;
                break;
            }
        }

        if (!notAllSitting) //If they are all sitting
        {
            //Order
            m_orderBubbel.gameObject.SetActive(true);
        }
        #endregion
    }

    public void TabelPayed(CustomerController customer)
    {
        foreach (var key in _tabelCustomers.Keys)
        {
            if (customer != key)
            {
                key.hasPayed();
            }
        }

        _tabelCustomers.Clear();
        m_orderBubbel.ResetBubbel();
        _orderDone = false;
    }

    IEnumerator EatingTime()
    {
        float waitTime = Random.Range(m_randomEatingTime.x, m_randomEatingTime.y);
        Debug.Log($"[Tabel.cs] eat time {waitTime}");
        yield return new WaitForSeconds(waitTime);

        //Done eating check if you can pay
        if (_customerLines.HasASpot())
        {
            //Decide who is going to pay
            CustomerController[] customers = _tabelCustomers.Keys.ToArray();
            int randomCustomerIndex = Random.Range(0, customers.Length);
            CustomerController customer = customers[randomCustomerIndex];

            Debug.Log($"[Tabel.cs] customer goes to pay {customer.name}");
            //Let them move to the spot
            Vector3 customerWalkTo = _customerLines.GetSpot(customer);
            customer.SetWalkingPoint(customerWalkTo);

            //Set there order bubbel to pay
            customer.OrderBubbel.SetToPayForTableOrder();
        }
        else
        {
            Debug.Log($"[Tabel.cs] No pay spot avaiable");
        }
    }
}
