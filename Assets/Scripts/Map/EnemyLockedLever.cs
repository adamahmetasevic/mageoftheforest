using UnityEngine;

public class EnemyLockedLever : LeverBase
{
    public GameObject[] enemies; // List of enemies to check

    private void Update()
    {
        if (!isActivated && AreEnemiesDefeated())
        {
            ActivateLever();
        }
    }

    private bool AreEnemiesDefeated()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null) // If any enemy is still alive, return false
            {
                return false;
            }
        }
        return true;
    }

    public override void ActivateLever()
    {
        isActivated = true;
        Debug.Log("Enemy-locked lever activated!");
        OpenDoor(); // Call the base class method to open the door
    }
}
