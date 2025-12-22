using UnityEngine;

public class Register : ClickableObject
{
    [SerializeField] private CustomerLines m_customerLines;

    //This object can be pickup and held by the player if they have 
    public override void Action(PlayerMovement playerMovement)
    {
        if (m_customerLines.CustomersPay())
        {
            //animation or bell
        }

        base.Action(playerMovement);
    }
}
