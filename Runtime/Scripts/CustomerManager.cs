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
    [SerializeField] private float m_timeBetweenWaves;

    private List<CustomerWave> m_waveQue = new List<CustomerWave>();
    private int m_currentActiveCustomerCount;
    private bool m_spawningWave;
    private bool m_canSpawn = true;

    public void AddWave(CustomerWave wave)
    {
        m_waveQue.Add(wave);

        if (!m_spawningWave)
        {
            SpawnWave();
        }
    }

    public void StopSpawning()
    {
        m_canSpawn = false;
        if (m_currentActiveCustomerCount <= 0)
        {
            Debug.Log("End level");
        }
    }

    public void RemoveCustomer()
    {
        m_currentActiveCustomerCount--;
        if (!m_canSpawn && m_currentActiveCustomerCount <= 0)
        {
            Debug.Log("End level");
        }
    }

    private void SpawnWave()
    {
        m_spawningWave = true;
        if (m_waveQue.Count != 0 && m_canSpawn)
        {
            CustomerType[] customersTypes = m_waveQue[0].customerType;
            for (int i = 0; i < customersTypes.Length; i++)
            {
                CustomerType customerType = customersTypes[i];
                if (customerType == CustomerType.Line)
                {
                    AddLineCustomer();
                }
                else if (customerType == CustomerType.Table)
                {
                    AddTabelCustomer();
                }
            }
            m_waveQue.RemoveAt(0);
            StartCoroutine(HelperWait.ActionAfterWait(m_timeBetweenWaves, SpawnWave));
        }
        else
        {
            m_spawningWave = false;
        }
    }

    //Todo implement that it will run this method if the restaurant is empty
    //This is for if the player is fast and can keep up so the restaurant keeps having customers
    private void AddRandomCustomer()
    {
        if (!m_canSpawn)
        {
            return;
        }

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
        m_currentActiveCustomerCount++;

        Vector3 customerWalkTo = m_customerLines.GetSpot(newCustomer);
        newCustomer.SetWalkingPoint(customerWalkTo);
        newCustomer.DoneAction = RemoveCustomer;

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
        m_currentActiveCustomerCount++;

        tabel.AddCustomerToSpot(newCustomer, spawnIndex);
        newCustomer.SetWalkingPoint(customerWalkTo, tabel);
        newCustomer.DoneAction = RemoveCustomer;
    }
}
