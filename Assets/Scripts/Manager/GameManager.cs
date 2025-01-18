using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Game Objects to Manage")]
    public List<GameObject> objectsToManage = new List<GameObject>(); // List of objects to enable/disable based on distance

    [Header("Distance Settings")]
    public float activationDistance = 15f; // Distance at which the objects should be activated
    public float deactivationDistance = 20f; // Distance at which the objects should be deactivated

    public Transform player; // Reference to the player’s transform

    private void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference not assigned in the GameManager.");
        }
    }

    private void Update()
    {
        ManageObjectActivation();
    }

    private void ManageObjectActivation()
    {
        foreach (GameObject obj in objectsToManage)
        {
            if (obj != null)
            {
                float distanceToPlayer = Vector2.Distance(obj.transform.position, player.position);

                // Activate object if it's within the activation distance
                if (distanceToPlayer <= activationDistance)
                {
                    if (!obj.activeInHierarchy) // Only activate if not already active
                    {
                        obj.SetActive(true);
                        //Debug.Log($"Activated: {obj.name}");
                    }
                }
                // Deactivate object if it's outside the deactivation distance
                else if (distanceToPlayer >= deactivationDistance)
                {
                    if (obj.activeInHierarchy) // Only deactivate if it's currently active
                    {
                        obj.SetActive(false);
                        //Debug.Log($"Deactivated: {obj.name}");
                    }
                }
            }
        }
    }
}
