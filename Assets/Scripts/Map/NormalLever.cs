using UnityEngine;

public class NormalLever : LeverBase
{
    [SerializeField] private Animator leverAnimator; // Reference to the Animator component

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.F) && !isActivated)
        {
            ActivateLever();
        }
    }

    public override void ActivateLever()
    {
        isActivated = true;

        // Trigger the animation
        if (leverAnimator != null)
        {
            leverAnimator.SetTrigger("Activate");
        }
        else
        {
            Debug.LogWarning("Lever Animator is not assigned!");
        }

        Debug.Log("Normal lever activated!");
        OpenDoor();
    }
}
