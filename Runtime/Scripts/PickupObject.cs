using System.Collections;
using UnityEngine;

public class PickupObject : ClickableObject
{
    //Every pickup script should have something that tels the player what your picking up. They should also be unique
    private PlayerInventory m_inventory;
    private SpriteRenderer m_spriteRenderer;

    private void Start()
    {
        m_inventory = PlayerInventory.Instance;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void GetSprite(out Sprite sprite, out Color color)
    {
        if (m_spriteRenderer == null)
        {
            Debug.Log("No sprite Renderer");
        }

        sprite = m_spriteRenderer.sprite;
        color = m_spriteRenderer.color;
    }

    //This object can be pickup and held by the player if they have 
    public override void Action(PlayerMovement playerMovement)
    {
        if (!m_inventory.IsInventoryFull())
        {
            StartCoroutine(ExampleCoroutine(playerMovement));
        }
        else
        {

            base.Action(playerMovement);
        }
    }

    IEnumerator ExampleCoroutine(PlayerMovement playerMovement)
    {
        yield return new WaitForSeconds(2); //For an pickup animation put get the animation and use the animation time here to wait for the animation to be done
        m_inventory.AddObject(this);
        Debug.Log("playerMovement");
        base.Action(playerMovement);
    }
}
