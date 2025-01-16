using UnityEngine;

public abstract class LeverBase : MonoBehaviour
{
    public GameObject door; // Reference to the door GameObject
    protected bool isActivated = false;

    public abstract void ActivateLever();

    protected void OpenDoor()
    {
        if (door != null)
        {
            // Disable the BoxCollider2D component
            BoxCollider2D boxCollider = door.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
            else
            {
                Debug.LogWarning("No BoxCollider2D found on the door!");
            }

            // Play the door's animation
            Animator doorAnimator = door.GetComponent<Animator>();
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("Open"); // Trigger the 'Open' animation
            }
            else
            {
                Debug.LogWarning("No Animator found on the door!");
            }

            Debug.Log("Door opened!");
        }
        else
        {
            Debug.LogWarning("Door is not assigned!");
        }
    }
}
