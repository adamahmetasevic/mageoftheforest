using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LightningBolt : MonoBehaviour
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

    private Vector3[] lightningPositions; // Stores the positions of the LineRenderer segments
    private Vector3 targetPosition; // End position of the lightning (mouse cursor)

    void Start()
    {
        // Ensure LineRenderer is attached
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is missing!");
            return;
        }

        // Ensure Light2D is attached
        lightningLight = GetComponentInChildren<Light2D>();
        if (lightningLight == null)
        {
            Debug.LogWarning("No Light2D component found! Lightning will not illuminate.");
        }

        // Get the mouse position in world space
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0f;

        // Generate the lightning
        GenerateLightning();

        // Generate smaller arcs
        GenerateSmallArcs();

        // Start moving the light along the lightning path
        if (lightningLight != null)
        {
            StartCoroutine(MoveLightAlongPath());
        }

        // Destroy after duration
        Destroy(gameObject, duration);
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
                // Calculate midpoint and add offset
                Vector3 midpoint = (start + end) / 2;
                midpoint += new Vector3(
                    Random.Range(-smallArcOffset, smallArcOffset),
                    Random.Range(-smallArcOffset, smallArcOffset),
                    0f
                );

                // Create a small arc prefab
                GameObject smallArc = Instantiate(smallArcPrefab, transform);
                LineRenderer arcRenderer = smallArc.GetComponent<LineRenderer>();

                if (arcRenderer != null)
                {
                    // Set up the positions of the arc
                    arcRenderer.positionCount = 3; // Start, midpoint, and end
                    arcRenderer.SetPosition(0, start);
                    arcRenderer.SetPosition(1, midpoint);
                    arcRenderer.SetPosition(2, end);
                }

                // Destroy the arc after a short time
                Destroy(smallArc, duration);
            }
        }
    }

    IEnumerator MoveLightAlongPath()
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            // Calculate the current position of the light along the path
            float t = timeElapsed / duration;
            int index = Mathf.Clamp(Mathf.FloorToInt(t * (segmentCount - 1)), 0, segmentCount - 2);
            float segmentT = (t * (segmentCount - 1)) - index;

            // Interpolate between the current and next segment
            Vector3 lightPosition = Vector3.Lerp(lightningPositions[index], lightningPositions[index + 1], segmentT);

            // Update the light position
            lightningLight.transform.position = lightPosition;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Turn off the light after completing the path
        lightningLight.intensity = 0f;
    }
}
