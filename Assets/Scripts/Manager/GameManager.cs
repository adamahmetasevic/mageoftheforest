using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Game Objects to Manage")]
    public List<GameObject> objectsToManage = new List<GameObject>();

    [Header("Distance Settings")]
    public float activationDistance = 15f;
    public float deactivationDistance = 20f;

    public Transform player;

    [Header("Boss References")]
    public GameObject treeBossObject; // Reference to the TreeBoss object

    private void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference not assigned in the GameManager.");
        }

        // Initial setup of boss states
        SetupBossStates();
    }

    private void SetupBossStates()
    {
        // Remove TreeBoss from managed objects if it's defeated
        if (treeBossObject != null)
        {
            if (IsBossDefeated("TreeBoss"))
            {
                treeBossObject.SetActive(false);
                // Remove from managed objects list if it exists there
                if (objectsToManage.Contains(treeBossObject))
                {
                    objectsToManage.Remove(treeBossObject);
                }
            }
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
                // Skip if this is a defeated boss
                if (IsBossObject(obj) && IsBossDefeated(GetBossName(obj)))
                {
                    continue;
                }

                float distanceToPlayer = Vector2.Distance(obj.transform.position, player.position);

                if (distanceToPlayer <= activationDistance)
                {
                    if (!obj.activeInHierarchy)
                    {
                        obj.SetActive(true);
                    }
                }
                else if (distanceToPlayer >= deactivationDistance)
                {
                    if (obj.activeInHierarchy)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }
    }

    private bool IsBossObject(GameObject obj)
    {
        // Check if the object is a boss
        return obj == treeBossObject; // Add more boss checks here if needed
    }

    private string GetBossName(GameObject obj)
    {
        // Return the appropriate boss name based on the GameObject
        if (obj == treeBossObject) return "TreeBoss";
        // Add more boss names here if needed
        return "";
    }

    private bool IsBossDefeated(string bossName)
    {
        // Check PlayerPrefs for boss defeat status
        return PlayerPrefs.GetInt($"{bossName}Defeated", 0) == 1;
    }

    // Method to mark a boss as defeated (can be called from other scripts)
    public void MarkBossAsDefeated(string bossName)
    {
        PlayerPrefs.SetInt($"{bossName}Defeated", 1);
        PlayerPrefs.Save();

        // Handle the defeated boss
        switch (bossName)
        {
            case "TreeBoss":
                if (treeBossObject != null)
                {
                    treeBossObject.SetActive(false);
                    if (objectsToManage.Contains(treeBossObject))
                    {
                        objectsToManage.Remove(treeBossObject);
                    }
                }
                break;
            // Add cases for other bosses here
        }
    }
    public void HandleDefeatedBossesAfterLoad(GameData loadedData)
{
    // Check for the TreeBoss
    if (treeBossObject != null && loadedData.defeatedBosses.Contains("TreeBoss"))
    {
        treeBossObject.SetActive(false);
        if (objectsToManage.Contains(treeBossObject))
        {
            objectsToManage.Remove(treeBossObject);
        }
    }

    // Add similar checks for other bosses as needed
}

}