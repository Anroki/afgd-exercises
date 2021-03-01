using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AfGD;

public class MoveAlongPath : MonoBehaviour
{
    public GameObject controlPointParent;
    private List<Vector3> controlPoints;
    private List<CurveSegment> curveSegments;
    private float time;

    void Start()
    {
        time = 0;
        controlPoints = new List<Vector3>();
        curveSegments = new List<CurveSegment>();

        Transform[] cpTransform = controlPointParent.GetComponentsInChildren<Transform>();

        foreach (Transform transform in cpTransform)
        {
            controlPoints.Add(transform.position);
        }
        curveSegments.Add(new CurveSegment(controlPoints[1], controlPoints[2], controlPoints[3], controlPoints[4], CurveType.CATMULLROM));
        curveSegments.Add(new CurveSegment(controlPoints[2], controlPoints[3], controlPoints[4], controlPoints[1], CurveType.CATMULLROM));
        curveSegments.Add(new CurveSegment(controlPoints[3], controlPoints[4], controlPoints[1], controlPoints[2], CurveType.CATMULLROM));
        curveSegments.Add(new CurveSegment(controlPoints[4], controlPoints[1], controlPoints[2], controlPoints[3], CurveType.CATMULLROM));
    }


    void Update()
    {
        time += Time.deltaTime;
        int tempIndex = Mathf.FloorToInt(time);
        int curveIndex = tempIndex % 4;
        float t = time % 1;
        Debug.Log(curveIndex);
        transform.position = curveSegments[curveIndex].Evaluate(t);
    }
}
