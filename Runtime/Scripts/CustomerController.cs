using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    [Header("OrderBubble")]
    [SerializeField] private Canvas m_orderCanvas;
    [SerializeField] private GridLayoutGroup m_gridLayout;
    [SerializeField] private RawImage m_orderObject;

    [Header("Pay")]
    [SerializeField] private Texture m_payTexture;
    [SerializeField] private Color m_payColor;

    private Vector2 m_walkingPoint;
    private NavMeshAgent m_agent;
    private bool m_walking;
    private Dictionary<PickupObject, RawImage> m_orderObjects;
    private Vector2 m_startPoint;
    private bool m_orderComplete;

    private void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.updateRotation = false;
        m_agent.updateUpAxis = false;

        m_startPoint = new Vector2(transform.position.x, transform.position.y);
        m_orderCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!m_walking || m_walkingPoint == null)
        {
            return;
        }

        Vector2 playerPosition = transform.position;
        //If the player is at the position of the mouse click or if the player isn't moving because of and obstacle
        if (Vector2.Distance(playerPosition, m_walkingPoint) < 0.1f)
        {
            ShowOrder(true);
            m_walking = false;

            if (m_orderComplete)
            {
                Destroy(gameObject);
            }
            return;
        }

        m_agent.SetDestination(m_walkingPoint);
    }

    public void SetWalkingPoint(Vector3 point)
    {
        if (m_walking)
        {
            return;
        }

        m_walkingPoint = new Vector2(point.x, point.y);
        m_walking = true;
    }

    public void SetOrder(PickupObject[] orderObjects)
    {
        m_orderObjects = new Dictionary<PickupObject, RawImage>();

        for (int i = 0; i < orderObjects.Length; i++)
        {
            PickupObject pickupObject = orderObjects[i];
            pickupObject.GetSprite(out Sprite sprite, out Color color);
            RawImage currentOrder = null;
            if (i == 0)
            {
                currentOrder = m_orderObject;
            }
            else
            {
                currentOrder = Instantiate(m_orderObject, m_gridLayout.transform);
            }

            currentOrder.texture = sprite.texture;
            currentOrder.color = color;

            m_orderObjects.Add(pickupObject, currentOrder);
        }
    }

    public bool ServedOrder(PickupObject orderObject)
    {
        if (m_orderObjects.ContainsKey(orderObject))
        {
            RawImage currentOrderObject = m_orderObjects[orderObject];

            if (m_orderObjects.Count == 1)
            {
                //This is then the lost object from there order so the order is complete

                //Update so it will say they need to pay first
                currentOrderObject.texture = m_payTexture;
                currentOrderObject.color = m_payColor;
                m_orderComplete = true;
                return true;
            }

            currentOrderObject.gameObject.SetActive(false);
            m_orderObjects.Remove(orderObject);
            return true;
        }

        return false;
    }

    public bool Pay()
    {
        if (!m_orderComplete)
        {
            return false;
        }

        //Todo Give point to player for finishing order
        ShowOrder(false);
        SetWalkingPoint(m_startPoint);
        return true;
    }

    private void ShowOrder(bool show)
    {
        m_orderCanvas.gameObject.SetActive(show);
    }
}
