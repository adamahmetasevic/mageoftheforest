using UnityEngine;
using System.Collections;

public class DelayLever : LeverBase
{
    [SerializeField] private Animator leverAnimator; // Reference to the Animator component
    [SerializeField] private float delayTime = 3f;   // Time in seconds before the door closes
    [SerializeField] private float resetDelay = 0.5f; // Time to wait after closing before allowing reactivation

    private bool isInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.F) && !isActivated)
        {
            ActivateLever();
        }
    }

    public override void ActivateLever()
    {
        if (isActivated) return;

        isActivated = true;

        // Trigger the lever's activation animation
        if (leverAnimator != null)
        {
            leverAnimator.SetTrigger("Activate");
        }
        else
        {
            Debug.LogWarning("Lever Animator is not assigned!");
        }

        Debug.Log("Lever activated! Door will open instantly and close after " + delayTime + " seconds.");
        OpenDoor();
        StartCoroutine(CloseDoorWithDelay());
    }

    private IEnumerator CloseDoorWithDelay()
    {
        yield return new WaitForSeconds(delayTime);
        leverAnimator.SetTrigger("Deactivate");
        CloseDoor();
        
        Debug.Log("Door closed after delay!");

        // Wait a short time before allowing reactivation
        yield return new WaitForSeconds(resetDelay);
        isActivated = false;
    }
}