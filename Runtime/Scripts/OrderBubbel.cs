using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class OrderBubbel : MonoBehaviour
{
    [Header("OrderBubble")]
    [SerializeField] private GridLayoutGroup m_gridLayout;
    [SerializeField] private RawImage m_orderObject;

    [Header("Pay")]
    [SerializeField] private Texture m_payTexture;
    [SerializeField] private Color m_payColor;

    private Dictionary<PickupObject, RawImage> m_orderObjects;
    private bool m_orderComplete;
    private bool m_isLineCustomer;

    public bool OrderComplete
    {
        get { return m_orderComplete; }
    }

    private void Awake()
    {
        #region Check what type of customer this is
        m_isLineCustomer = transform.parent.TryGetComponent<CustomerController>(out CustomerController customerController);

#if UNITY_EDITOR
        //If is isn't a line customer then it should be a table check if this isn't an order bubbel that is somewhere it shouldn't
        if (!m_isLineCustomer && !transform.parent.TryGetComponent<Tabel>(out Tabel tabel))
        {
            Debug.LogWarning($"<color=ff0000ff>[Warning]</color> OrderBubbel: ({name}) doesn't have CustomerController.cs or Tabel.cs on the same object");
        }
#endif
        #endregion

        gameObject.SetActive(false);
    }

    public void SetOrder(PickupObject[] orderObjects)
    {
        m_orderObjects = new Dictionary<PickupObject, RawImage>();

        for (int i = 0; i < orderObjects.Length; i++)
        {
            PickupObject pickupObject = orderObjects[i];
            pickupObject.GetSprite(out Sprite sprite, out Color color);
            
            RawImage currentOrder = Instantiate(m_orderObject, m_gridLayout.transform);
            currentOrder.texture = sprite.texture;
            currentOrder.color = color;

            m_orderObjects.Add(pickupObject, currentOrder);
        }
    }

    public bool ServedOrder(PickupObject orderObject, out bool orderComplete)
    {
        orderComplete = false;
        if (m_orderObjects.ContainsKey(orderObject))
        {
            RawImage currentOrderObject = m_orderObjects[orderObject];

            if (m_orderObjects.Count == 1)
            {
                //This is then the last object from there order so the order is complete

                if (m_isLineCustomer)
                {
                    //Update so it will say they need to pay first
                    currentOrderObject.texture = m_payTexture;
                    currentOrderObject.color = m_payColor;
                }
                else //tabelCustomer
                {
                    //Turn off the orderbubbel and let them enjoy there food
                    Destroy(currentOrderObject.gameObject);
                    gameObject.SetActive(false);
                }
                m_orderComplete = true;
                orderComplete = m_orderComplete;
                return true;
            }

            Destroy(currentOrderObject.gameObject);
            m_orderObjects.Remove(orderObject);
            return true;
        }

        return false;
    }

    public void ResetBubbel()
    {
        m_orderComplete = false;
        m_orderObjects.Clear();
    }

    public void SetToPayForTableOrder()
    {
        m_orderComplete = true;
        RawImage pay = Instantiate(m_orderObject, m_gridLayout.transform);
        pay.texture = m_payTexture;
        pay.color = m_payColor;
    }
}
