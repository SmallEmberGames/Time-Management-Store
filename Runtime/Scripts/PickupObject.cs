using System.Collections;
using UnityEngine;

public class PickupObject : ClickableObject
{
    //Every pickup script should have something that tels the player what your picking up. They should also be unique
    private PlayerInventory _inventory;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _inventory = PlayerInventory.Instance;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void GetSprite(out Sprite sprite, out Color color)
    {
        if (_spriteRenderer == null)
        {
            Debug.Log("No sprite Renderer");
        }

        sprite = _spriteRenderer.sprite;
        color = _spriteRenderer.color;
    }

    //This object can be pickup and held by the player if they have 
    public override void Action(PlayerMovement playerMovement)
    {
        if (!_inventory.IsInventoryFull())
        {
            StartCoroutine(AddObject(playerMovement));
        }
        else
        {

            base.Action(playerMovement);
        }
    }

    IEnumerator AddObject(PlayerMovement playerMovement)
    {
        yield return new WaitForSeconds(2); //For an pickup animation put get the animation and use the animation time here to wait for the animation to be done
        _inventory.AddObject(this);
        base.Action(playerMovement);
    }
}
