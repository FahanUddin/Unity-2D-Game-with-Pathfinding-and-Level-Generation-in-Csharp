using UnityEngine;
using System.Collections.Generic;

public class EnemyPathFFollow : MonoBehaviour
{
    public float speed = 4f;
    public float waypointTolerance = 0.2f;

    private List<Vector2> path;
    private int currentWaypoint = 0;
    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        currentWaypoint = 0;

        path = PathfindingManager.Instance.FindPath(transform.position, target.position);
    }

    public bool HasPath()
    {
        return path != null && path.Count > 0;
    }


    void Update()
    {
        if (target == null || path == null || path.Count == 0)
            return;

        Vector2 targetWaypoint = path[currentWaypoint];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetWaypoint) < waypointTolerance)
        {
            currentWaypoint++;
            if (currentWaypoint >= path.Count)
            {

                path = null;
            }
        }
    }
}
