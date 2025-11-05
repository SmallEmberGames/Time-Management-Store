using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 m_position;
    private Vector3 m_lastPosition;
    private NavMeshAgent m_agent;
    private bool m_walking;

    private void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.updateRotation = false;
        m_agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (!m_walking)
        {
            return;
        }

        //If the player is at the position of the mouse click or if the player isn't moving because of and obstacle
        if (transform.position == m_position || m_lastPosition == transform.position)
        {
            return;
        }

        m_agent.SetDestination(m_position);
    }


    public void MoveToMousePosition(Vector3 position)
    {
        m_position = new Vector3(position.x, position.y, 0);
        m_walking = true;
    }
}
