using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AfGD;
using AfGD.Assignment1;

namespace AfGD.Assignment1
{
    public class MoveAlongPathNew : MonoBehaviour
    {
        [Header("The controlpoints are stored under a parent object")]
        public GameObject controlPointParent;

        [Header("Base speed of moving object")]
        public float baseSpeed = 1.0f;

        private List<Vector3> controlPoints;
        private List<CurveSegment> curveSegments;
        private float time;
        private float t;
        private int curveIndex = 0;
        private DebugCurve debugCurve;
        private bool hasPath;

        void Start()
        {
            Init();
        }
        void Update()
        {
            if (hasPath)
                MoveObjectAlongPath();
            else
                GetPath();
        }

        void Init()
        {
            hasPath = false;
            time = 0;
            t = 0;

            curveSegments = new List<CurveSegment>();
        }

        void CreateCurveSegments()
        {
            this.transform.position = controlPoints[1];

            int cpCount = controlPoints.Count;

            for (int i = 0; i < cpCount - 3; i++)
            {
                int a = i + 0;
                int b = i + 1;
                int c = i + 2;
                int d = i + 3;
                curveSegments.Add(new CurveSegment(controlPoints[a], controlPoints[b], controlPoints[c], controlPoints[d], CurveType.HERMITE));
            }
        }

        void MoveObjectAlongPath()
        {
            time += Time.deltaTime;
            int tempIndex = Mathf.FloorToInt(time);
            curveIndex = tempIndex % controlPoints.Count;
            t = time % 1;

            Vector3 targetPos = curveSegments[curveIndex].Evaluate(t);

            transform.position = targetPos;
        }

        void GetPath()
        {
            controlPoints = controlPointParent.GetComponent<PathFinding>().GetPath();
            hasPath = controlPoints.Count > 0;
            if (hasPath)
                CreateCurveSegments();
            else
                controlPointParent.GetComponent<PathFinding>().Run();
        }
    }
}

