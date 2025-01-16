using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LightningBoltBase : MonoBehaviour
{
    public Transform firePoint; // Starting point of the lightning
    public int segmentCount = 10; // Number of segments for the lightning
    public float jaggedness = 0.5f; // Max offset for jaggedness
    public float duration = 0.2f; // Duration the lightning stays
    public LineRenderer lineRenderer; // Main LineRenderer for the lightning

    public Light2D lightningLight; // Reference to the Light 2D component
    public float lightTravelSpeed = 50f; // Speed at which the light moves along the path

    public GameObject smallArcPrefab; // Prefab for smaller arcs (must have a LineRenderer)
    public int smallArcCountPerSegment = 3; // Number of smaller arcs to generate per segment
    public float smallArcOffset = 0.3f; // Max offset for smaller arcs
    public float damage = 10f; 

    protected Vector3[] lightningPositions; // Stores the positions of the LineRenderer segments
    protected Vector3 targetPosition; // End position of the lightning (mouse cursor or enemy position)
    protected bool isLightAtEnd = false;

    // Method to initialize lightning
    public virtual void Initialize(Vector3 target)
    {
        targetPosition = target;
        GenerateLightning();
        GenerateSmallArcs();
        StartCoroutine(MoveLightAlongPath());
        Fire();
    }

    private void Fire()
    {
        // Raycast along the lightning segments
        for (int i = 0; i < lightningPositions.Length - 1; i++)
        {
            Vector3 start = lightningPositions[i];
            Vector3 end = lightningPositions[i + 1];

            RaycastHit2D hit = Physics2D.Linecast(start, end, LayerMask.GetMask("Enemy"));

            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage((int)damage, DamageType.Lightning);
                    Debug.Log($"Lightning hit: {enemy.name} for {damage} damage");
                }
            }
        }
    }

    void GenerateLightning()
    {
        lineRenderer.positionCount = segmentCount;
        lightningPositions = new Vector3[segmentCount];

        Vector3 start = firePoint.position;
        Vector3 end = targetPosition;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            point += new Vector3(
                Random.Range(-jaggedness, jaggedness),
                Random.Range(-jaggedness, jaggedness),
                0f
            );

            lightningPositions[i] = point;
            lineRenderer.SetPosition(i, point);
        }
    }

    void GenerateSmallArcs()
    {
        for (int i = 0; i < lightningPositions.Length - 1; i++)
        {
            Vector3 start = lightningPositions[i];
            Vector3 end = lightningPositions[i + 1];

            for (int j = 0; j < smallArcCountPerSegment; j++)
            {
                Vector3 midpoint = (start + end) / 2;
                midpoint += new Vector3(
                    Random.Range(-smallArcOffset, smallArcOffset),
                    Random.Range(-smallArcOffset, smallArcOffset),
                    0f
                );

                GameObject smallArc = Instantiate(smallArcPrefab, transform);
                LineRenderer arcRenderer = smallArc.GetComponent<LineRenderer>();

                if (arcRenderer != null)
                {
                    arcRenderer.positionCount = 3;
                    arcRenderer.SetPosition(0, start);
                    arcRenderer.SetPosition(1, midpoint);
                    arcRenderer.SetPosition(2, end);
                }

                Destroy(smallArc, duration);
            }
        }
    }

    IEnumerator MoveLightAlongPath()
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            int index = Mathf.Clamp(Mathf.FloorToInt(t * (segmentCount - 1)), 0, segmentCount - 2);
            float segmentT = (t * (segmentCount - 1)) - index;

            Vector3 lightPosition = Vector3.Lerp(lightningPositions[index], lightningPositions[index + 1], segmentT);
            lightningLight.transform.position = lightPosition;

            if (t >= 1f && !isLightAtEnd)
            {
                isLightAtEnd = true;
                DealDamage(); // Call DealDamage when light reaches the end
                Debug.Log("Light reached end, attempting to deal damage");
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    // Abstract method for dealing damage when light reaches the end
    protected virtual void DealDamage() { }
}
