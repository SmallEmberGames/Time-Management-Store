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

    [SerializeField] private Vector2 m_randomWaitTime;

    void Start()
    {
        AddCustomer();
    }

    private void AddCustomer()
    {
        if (!m_customerLines.HasASpot())
        {
            return;
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
        newCustomer.SetOrder(new PickupObject[]
        {
            objectOne,
            objectTwo
        });

        StartCoroutine(WaitForNextCustomer());
    }

    IEnumerator WaitForNextCustomer()
    {
        float waitTime = Random.Range(m_randomWaitTime.x, m_randomWaitTime.y);
        yield return new WaitForSeconds(waitTime);

        AddCustomer();
    }
}
