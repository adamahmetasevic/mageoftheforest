using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapChunkManager : MonoBehaviour
{
    [Header("Tilemap Settings")]
    public Tilemap originalTilemap;
    public Vector2Int chunkSize = new Vector2Int(10, 10);

    [Header("Distance Settings")]
    public float activationDistance = 15f;
    public float deactivationDistance = 20f;

    public Transform player;

    private List<GameObject> chunks = new List<GameObject>();
    private Vector2Int chunksInView = new Vector2Int(3, 3);

    private void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference not assigned in the TilemapChunkManager.");
        }

        if (originalTilemap == null)
        {
            Debug.LogError("Original Tilemap reference not assigned.");
            return;
        }

        CreateChunksForEntireTilemap();
    }

    private void Update()
    {
        ManageTileChunkActivation();
    }

    private void CreateChunksForEntireTilemap()
    {
        BoundsInt bounds = originalTilemap.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x += chunkSize.x)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y += chunkSize.y)
            {
                CreateChunk(x, y);
            }
        }
    }

    private void CreateChunk(int x, int y)
    {
        GameObject chunk = new GameObject($"Tilemap Chunk {x / chunkSize.x}-{y / chunkSize.y}");
        chunk.transform.SetParent(transform);
        
        // Add Grid component to the chunk
        Grid grid = chunk.AddComponent<Grid>();
        grid.cellSize = originalTilemap.layoutGrid.cellSize;

        Tilemap chunkTilemap = chunk.AddComponent<Tilemap>();
        TilemapRenderer chunkRenderer = chunk.AddComponent<TilemapRenderer>();
        
        // Copy renderer settings from original
        chunkRenderer.sortOrder = originalTilemap.GetComponent<TilemapRenderer>().sortOrder;
        chunkRenderer.sortingLayerID = originalTilemap.GetComponent<TilemapRenderer>().sortingLayerID;
        chunkRenderer.sortingOrder = originalTilemap.GetComponent<TilemapRenderer>().sortingOrder;

        // Add Rigidbody2D
        Rigidbody2D rb = chunk.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        rb.simulated = true;
        rb.useFullKinematicContacts = true;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Add CompositeCollider2D
        CompositeCollider2D compositeCollider = chunk.AddComponent<CompositeCollider2D>();
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        compositeCollider.generationType = CompositeCollider2D.GenerationType.Synchronous;

        // Add TilemapCollider2D
        TilemapCollider2D tilemapCollider = chunk.AddComponent<TilemapCollider2D>();
        tilemapCollider.usedByComposite = true;

        // Set the layer to "Ground"
        chunk.layer = LayerMask.NameToLayer("Ground");

        // Position the chunk correctly in world space
        chunk.transform.position = originalTilemap.CellToWorld(new Vector3Int(x, y, 0));

        // Create the chunk's bounds
        BoundsInt chunkBounds = new BoundsInt(new Vector3Int(x, y, 0), 
            new Vector3Int(chunkSize.x, chunkSize.y, 1));

        // Apply the tiles from the original Tilemap to this chunk
        TilemapChunkData chunkData = new TilemapChunkData(originalTilemap, chunkBounds);
        chunkData.ApplyToTilemap(chunkTilemap);

        // Add chunk to the list
        chunks.Add(chunk);
        chunk.SetActive(false);
    }


    private void ManageTileChunkActivation()
    {
        foreach (GameObject chunk in chunks)
        {
            if (chunk != null)
            {
                Vector3 chunkCenter = chunk.transform.position;
                float distanceToPlayer = Vector2.Distance(chunkCenter, player.position);

                if (distanceToPlayer <= activationDistance)
                {
                    if (!chunk.activeInHierarchy)
                    {
                        chunk.SetActive(true);
                        Debug.Log($"Activated: {chunk.name}");
                    }
                }
                else if (distanceToPlayer >= deactivationDistance)
                {
                    if (chunk.activeInHierarchy)
                    {
                        chunk.SetActive(false);
                        Debug.Log($"Deactivated: {chunk.name}");
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        foreach (GameObject chunk in chunks)
        {
            if (chunk != null)
            {
                Gizmos.color = chunk.activeInHierarchy ? Color.green : Color.red;
                Gizmos.DrawWireCube(chunk.transform.position, 
                    new Vector3(chunkSize.x, chunkSize.y, 1));
            }
        }
    }
}

public class TilemapChunkData
{
    private Tilemap sourceTilemap;
    private BoundsInt chunkBounds;

    public TilemapChunkData(Tilemap source, BoundsInt chunkBounds)
    {
        this.sourceTilemap = source;
        this.chunkBounds = chunkBounds;
    }

    public void ApplyToTilemap(Tilemap chunkTilemap)
    {
        TileBase[] tiles = new TileBase[chunkBounds.size.x * chunkBounds.size.y * chunkBounds.size.z];
        int i = 0;

        foreach (Vector3Int pos in chunkBounds.allPositionsWithin)
        {
            TileBase tile = sourceTilemap.GetTile(pos);
            if (tile != null)
            {
                Vector3Int localPos = new Vector3Int(
                    pos.x - chunkBounds.x,
                    pos.y - chunkBounds.y,
                    pos.z - chunkBounds.z
                );
                chunkTilemap.SetTile(localPos, tile);
            }
            i++;
        }
    }
}