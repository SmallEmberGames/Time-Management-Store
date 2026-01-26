public class FirstSpot : ClickableObject
{
    private CustomerController m_customerController;
    private PlayerInventory m_playerInventory;

    public CustomerController CustomerController
    {
        get {return m_customerController;}
        set { m_customerController = value; }
    }

    public bool HasCustomer
    {
        get { return m_customerController != null; }
    }

    private void Start()
    {
        m_playerInventory = PlayerInventory.Instance;
    }

    public override void Action(PlayerMovement playerMovement)
    {
        //Get customer order and check if we have something in the players inventory that matches
        if (m_customerController == null)
        {
            return;
        }

        PickupObject[] customerOrders = m_playerInventory.InventoryObjects;
        for (int i = 0; i < customerOrders.Length; i++)
        {
            if (m_customerController.OrderBubbel.ServedOrder(customerOrders[i], out bool orderComplete))
            {
                //The player has this order item so we can remove it from the customer order 
                m_playerInventory.RemoveObject(customerOrders[i]);
            }
        }

        base.Action(playerMovement);
    }
}
