using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    //If this object is clicked the player should walk towards a certain place. This will be an parent object that others can inherent so you have certain scripts for tabels ect
    [SerializeField] protected Transform m_playerMoveToPoint;

    public Vector3 MoveToPosition
    {
        get
        {
            return m_playerMoveToPoint.position;
        }
    }

    public virtual void Action(PlayerMovement playerMovement)
    {
        playerMovement.StartWalking();
    }
}
