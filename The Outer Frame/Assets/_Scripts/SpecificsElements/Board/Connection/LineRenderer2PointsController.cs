using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineRenderer2PointsController : MonoBehaviour
{
    [SerializeField] Transform point1;
    [SerializeField] Transform point2;
    LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        Vector3 localPointA = transform.InverseTransformPoint(point1.gameObject.transform.position);
        Vector3 localPointB = transform.InverseTransformPoint(point2.gameObject.transform.position);

        lineRenderer.SetPosition(0, localPointA);
        lineRenderer.SetPosition(1, localPointB);
    }
}
