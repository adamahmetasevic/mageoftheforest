using UnityEngine;

public class EnemyLockedLever : LeverBase
{
    [SerializeField] private GameObject[] enemies; // List of enemies to check
    [SerializeField] private Animator leverAnimator; // Animator for the lever

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.F) && !isActivated && AreEnemiesDefeated())
        {
            ActivateLever();
        }
    }

    private bool AreEnemiesDefeated()
    {


        // Check if all enemies are either destroyed or inactive
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
            {
                return true;
            }
            if (enemy != null && enemy.activeInHierarchy)
            {
                // If any enemy is still alive and active, return false
                return false;
            }
        }
        return true;
    }

    public override void ActivateLever()
    {
        isActivated = true;

        // Trigger the lever's animation
        if (leverAnimator != null)
        {
            leverAnimator.SetTrigger("Activate");
        }
        else
        {
            Debug.LogWarning("Lever Animator is not assigned!");
        }

        Debug.Log("Enemy-locked lever activated!");
        OpenDoor(); // Call the base class method to open the door
    }
}
