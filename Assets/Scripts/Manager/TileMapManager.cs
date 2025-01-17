using UnityEngine;
using System.Collections.Generic;

public class TileMapManager : MonoBehaviour
{
    [Header("Tile Chunks to Manage")]
    public List<GameObject> tileChunks = new List<GameObject>(); // List of chunks to manage

    [Header("Distance Settings")]
    public float activationDistance = 15f; // Distance at which the chunks should be activated
    public float deactivationDistance = 20f; // Distance at which the chunks should be deactivated

    public Transform player; // Reference to the player’s transform

    private void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference not assigned in the TileMapManager.");
        }
    }

    private void Update()
    {
        ManageTileChunkActivation();
    }

    private void ManageTileChunkActivation()
    {
        foreach (GameObject chunk in tileChunks)
        {
            if (chunk != null)
            {
                // Get the position of the chunk (assuming each chunk is a GameObject)
                float distanceToPlayer = Vector2.Distance(chunk.transform.position, player.position);

                // Activate chunk if it's within the activation distance
                if (distanceToPlayer <= activationDistance)
                {
                    if (!chunk.activeInHierarchy) // Only activate if not already active
                    {
                        chunk.SetActive(true);
                        Debug.Log($"Activated: {chunk.name}");
                    }
                }
                // Deactivate chunk if it's outside the deactivation distance
                else if (distanceToPlayer >= deactivationDistance)
                {
                    if (chunk.activeInHierarchy) // Only deactivate if it's currently active
                    {
                        chunk.SetActive(false);
                        Debug.Log($"Deactivated: {chunk.name}");
                    }
                }
            }
        }
    }
}
