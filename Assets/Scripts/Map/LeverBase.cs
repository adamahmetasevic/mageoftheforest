using UnityEngine;

public abstract class LeverBase : MonoBehaviour
{
    public GameObject door; // Reference to the door GameObject
    protected bool isActivated = false;
    private Animator doorAnimator; // Declare doorAnimator without initializing

    private void Awake()
    {
        // Initialize doorAnimator in the Awake method
        if (door != null)
        {
            doorAnimator = door.GetComponent<Animator>();
            if (doorAnimator == null)
            {
                Debug.LogWarning("No Animator found on the door!");
            }
        }
        else
        {
            Debug.LogWarning("Door is not assigned!");
        }
    }

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

    protected void CloseDoor()
    {
        isActivated = false;
        if (door != null)
        {
            // Enable the BoxCollider2D component
            BoxCollider2D boxCollider = door.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.enabled = true;
                isActivated = false;
            }
            else
            {
                Debug.LogWarning("No BoxCollider2D found on the door!");
            }

            // Play the door's animation
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("Close"); // Trigger the 'Close' animation
            }
            else
            {
                Debug.LogWarning("No Animator found on the door!");
            }

            Debug.Log("Door closed!");
        }
        else
        {
            Debug.LogWarning("Door is not assigned!");
        }
    }
}
