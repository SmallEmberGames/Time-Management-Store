using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private CustomerController m_customerControllerPrefab;
    [SerializeField] private PickupObject[] m_pickupObjects;
    [SerializeField] private Transform[] m_spawnpoints;

    [SerializeField] private CustomerLines m_customerLines;
    [SerializeField] private CustomerTabels m_customerTabels;

    [SerializeField] private float m_timeBetweenTableCustomersSpawn;
    [SerializeField] private Vector2 m_randomWaitTime;

    void Start()
    {
        AddCustomer();
    }

    private void AddCustomer()
    {
        //Randomize which customer type will come 0 = line customer 1 = tabel customer
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            if (!AddLineCustomer())
            {
                //Add tabel customer
                AddTabelCustomer();
            }
        }
        else
        {
            if (!AddTabelCustomer())
            {
                AddLineCustomer();
            }
        }

        StartCoroutine(WaitForNextCustomer());
    }

    private bool AddTabelCustomer()
    {
        if (!m_customerTabels.CustomerTabel(out Tabel tabel, out int tabelIndex))
        {
            return false;
        }

        int spawnIndex = Random.Range(0, m_spawnpoints.Length);
        Vector3 newCustomerSpawn = m_spawnpoints[spawnIndex].position;

#if UNITY_EDITOR
        if (tabel.TabelSpots.Length == 0)
        {
            Debug.LogWarning($"<color=ff0000ff>[Warning]</color> Tabel: ({tabel.name}) tabel spots aren't set");
            return false;
        }
#endif

        //For tabel customers we are going to have one object per customer that is sitting
        List<PickupObject> pickupObjects = m_pickupObjects.ToList();
        PickupObject[] order = new PickupObject[tabel.TabelSpots.Length];
        for (int i = 0; i < tabel.TabelSpots.Length; i++)
        {
            Vector3 customerSpot = tabel.TabelSpots[i].position;
            StartCoroutine(SpawnTableCustomers(tabel, i, newCustomerSpawn, customerSpot));
            order[i] = pickupObjects[Random.Range(0, pickupObjects.Count)];
        }

        tabel.OrderBubbel.SetOrder(order);

        return true;
    }

    private bool AddLineCustomer()
    {
        if (!m_customerLines.HasASpot())
        {
            return false;
        }

        int spawnIndex = Random.Range(0, m_spawnpoints.Length);
        Vector3 newCustomerSpawn = m_spawnpoints[spawnIndex].position;
        CustomerController newCustomer = Instantiate(m_customerControllerPrefab, newCustomerSpawn, Quaternion.identity);
        Vector3 customerWalkTo = m_customerLines.GetSpot(newCustomer);
        newCustomer.SetWalkingPoint(customerWalkTo);

        //For line customers we are going to have two objects 
        List<PickupObject> pickupObjects = m_pickupObjects.ToList();
        PickupObject objectOne = pickupObjects[Random.Range(0, pickupObjects.Count)];
        pickupObjects.Remove(objectOne);
        PickupObject objectTwo = pickupObjects[Random.Range(0, pickupObjects.Count)];
        newCustomer.OrderBubbel.SetOrder(new PickupObject[]
        {
            objectOne,
            objectTwo
        });

        return true;
    }

    IEnumerator SpawnTableCustomers(Tabel tabel, int spawnIndex, Vector3 spawnpoint, Vector3 customerWalkTo)
    {
        if (spawnIndex != 0)
        {
            yield return new WaitForSeconds(m_timeBetweenTableCustomersSpawn * spawnIndex);
        }

        CustomerController newCustomer = Instantiate(m_customerControllerPrefab, spawnpoint, Quaternion.identity);
        tabel.AddCustomerToSpot(newCustomer, spawnIndex);
        newCustomer.SetWalkingPoint(customerWalkTo, tabel);
    }

    IEnumerator WaitForNextCustomer()
    {
        float waitTime = Random.Range(m_randomWaitTime.x, m_randomWaitTime.y);
        yield return new WaitForSeconds(waitTime);

        AddCustomer();
    }
}
