using UnityEngine;

public class RopeGenerator : MonoBehaviour
{
    [Header("Rope Settings")]
    public GameObject ropeSegmentPrefab;
    public int segmentCount = 15;
    public float segmentSpacing = 2.3f; // Distance between segments 

    void Start()
    {
        Rigidbody2D previousRb = null;

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 spawnPos = transform.position - new Vector3(0, i * segmentSpacing, 0);
            GameObject segment = Instantiate(ropeSegmentPrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = segment.GetComponent<Rigidbody2D>();

            if (i == 0)
            {
                // First segment is fixed in place
                rb.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                HingeJoint2D joint = segment.GetComponent<HingeJoint2D>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedBody = previousRb;

                // Attach bottom of previous to top of current 
                joint.anchor = new Vector2(0f, 1.15f);
                joint.connectedAnchor = new Vector2(0f, -1.15f);
            }

            previousRb = rb;
        }
    }
}
