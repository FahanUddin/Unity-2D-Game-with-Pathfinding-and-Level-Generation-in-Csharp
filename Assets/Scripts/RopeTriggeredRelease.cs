using UnityEngine;
using System.Collections.Generic;

public class RopeTriggeredRelease : MonoBehaviour
{
    public GameObject ropeSegmentPrefab;
    public int segmentCount = 15;
    public float segmentHeight = 1f;

    private bool ropeGenerated = false;

    public void GenerateRope()
    {
        if (ropeGenerated) return;
        ropeGenerated = true;

        List<Rigidbody2D> ropeBodies = new List<Rigidbody2D>();

        //  Instantiate all segments and disable physics temporarily
        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 spawnPos = transform.position - new Vector3(0, i * segmentHeight, 0);
            GameObject segment = Instantiate(ropeSegmentPrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = segment.GetComponent<Rigidbody2D>();
            rb.isKinematic = true; // prevent falling or jittering
            ropeBodies.Add(rb);
        }

        // Connect hinge joints
        for (int i = 0; i < ropeBodies.Count; i++)
        {
            if (i == 0)
            {
                ropeBodies[i].bodyType = RigidbodyType2D.Static; // anchor
            }
            else
            {
                HingeJoint2D joint = ropeBodies[i].GetComponent<HingeJoint2D>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedBody = ropeBodies[i - 1];
                joint.anchor = new Vector2(0f, 1.15f);
                joint.connectedAnchor = new Vector2(0f, -1.15f);
            }
        }

        // Re-enable physics after setup
        for (int i = 1; i < ropeBodies.Count; i++)
        {
            ropeBodies[i].isKinematic = false;
        }
    }
}
