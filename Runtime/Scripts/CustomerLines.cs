using System.Collections.Generic;
using UnityEngine;

public class CustomerLines : MonoBehaviour
{
    [SerializeField] private LevelManager m_levelManager;

    [SerializeField] private Transform[] m_line1Spots;
    [SerializeField] private Transform[] m_line2Spots;
    [SerializeField] private Transform[] m_line3Spots;

    private List<CustomerController> m_line1CustomerController = new List<CustomerController>();
    private List<CustomerController> m_line2CustomerController = new List<CustomerController>();
    private List<CustomerController> m_line3CustomerController = new List<CustomerController>();

    private FirstSpot m_line1FirstSpot;
    private FirstSpot m_line2FirstSpot;
    private FirstSpot m_line3FirstSpot;

    private void Awake()
    {
        if (m_line1Spots.Length != 0)
        {
            m_line1FirstSpot = m_line1Spots[0].gameObject.GetComponent<FirstSpot>();
        }
        if (m_line2Spots.Length != 0)
        {
            m_line2FirstSpot = m_line2Spots[0].gameObject.GetComponent<FirstSpot>();
        }
        if (m_line3Spots.Length != 0)
        {
            m_line3FirstSpot = m_line3Spots[0].gameObject.GetComponent<FirstSpot>();
        }
    }

    public bool HasASpot()
    {
        return m_line3CustomerController.Count < m_line3Spots.Length || m_line2CustomerController.Count < m_line2Spots.Length || m_line1CustomerController.Count < m_line1Spots.Length;
    }

    public bool CustomersPay()
    {
        bool lineOne = CustomerLinePay(ref m_line1CustomerController, ref m_line1FirstSpot, m_line1Spots);
        bool lineTwo = CustomerLinePay(ref m_line2CustomerController, ref m_line2FirstSpot, m_line2Spots);
        bool lineThree = CustomerLinePay(ref m_line3CustomerController, ref m_line3FirstSpot, m_line3Spots);

        return lineOne || lineTwo || lineThree;
    }

    private bool CustomerLinePay(ref List<CustomerController> customerLine, ref FirstSpot firstSpot, Transform[] lineSpots)
    {
        if (customerLine.Count == 0)
        {
            return false;
        }
        bool customerDone = customerLine[0].Pay(m_levelManager);

        if (customerDone)
        {
            customerLine.RemoveAt(0);

            for (int i = 0; i < customerLine.Count; i++)
            {
                customerLine[i].SetWalkingPoint(lineSpots[i].position);
                if (i == 0)
                {
                    firstSpot.CustomerController = customerLine[i];
                }
            }
        }

        return customerDone;
    }

    public Vector3 GetSpot(CustomerController newCustomer)
    {
        if (m_line1CustomerController.Count == 0)
        {
            m_line1CustomerController.Add(newCustomer);
            m_line1FirstSpot.CustomerController = newCustomer;
            return m_line1Spots[0].position;
        }
        else if (m_line2CustomerController.Count == 0)
        {
            m_line2CustomerController.Add(newCustomer);
            m_line2FirstSpot.CustomerController = newCustomer;
            return m_line2Spots[0].position;
        }
        else if (m_line3CustomerController.Count == 0)
        {
            m_line3CustomerController.Add(newCustomer);
            m_line3FirstSpot.CustomerController = newCustomer;
            return m_line3Spots[0].position;
        }

        else if (m_line2CustomerController.Count < m_line1CustomerController.Count)
        {
            m_line2CustomerController.Add(newCustomer);
            return m_line2Spots[m_line2CustomerController.Count - 1].position;
        }
        else if (m_line3CustomerController.Count < m_line2CustomerController.Count)
        {
            m_line3CustomerController.Add(newCustomer);
            return m_line3Spots[m_line3CustomerController.Count - 1].position;
        }
        else
        {
            m_line1CustomerController.Add(newCustomer);
            return m_line1Spots[m_line1CustomerController.Count - 1].position;
        }
    }
}
