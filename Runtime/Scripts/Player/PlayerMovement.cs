using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private List<WalkingPoint> m_walkingPointsList;
    private NavMeshAgent m_agent;
    private bool m_walking;
    private bool m_action;

    public struct WalkingPoint
    {
        public Vector2 point;
        public ClickableObject clickable;

        public WalkingPoint(Vector2 point, ClickableObject clickable) : this()
        {
            this.point = point;
            this.clickable = clickable;
        }
    };

    //Add a list of things you need to walk to and follow that list

    private void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.updateRotation = false;
        m_agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (!m_walking || m_walkingPointsList == null || m_walkingPointsList.Count == 0)
        {
            return;
        }

        Vector2 movePoint = m_walkingPointsList[0].point;
        Vector2 playerPosition = transform.position;

        //If the player is at the position of the mouse click or if the player isn't moving because of and obstacle
        if (Vector2.Distance(playerPosition, movePoint) < 0.1f)
        {
            m_walking = false;
            //Do action at that position and start walking again if there is another walking point
            m_action = true;
            m_walkingPointsList[0].clickable.Action(this);
            m_walkingPointsList.RemoveAt(0);
            return;
        }

        m_agent.SetDestination(movePoint);
    }

    public void StartWalking()
    {
        if (m_walkingPointsList.Count == 0)
        {
            m_action = false;
            return;
        }

        m_walking = true;
    }

    public void MoveToMousePosition(ClickableObject clickableObject)
    {
        if (m_walkingPointsList == null)
        {
            m_walkingPointsList = new List<WalkingPoint>();
        }

        Vector3 moveToPosition = clickableObject.MoveToPosition;
        Vector2 walkingPoint = new Vector2(moveToPosition.x, moveToPosition.y);
        m_walkingPointsList.Add(new WalkingPoint(walkingPoint, clickableObject));

        if (!m_walking && !m_action)
        {
            m_walking = true;
        }


    }
}
