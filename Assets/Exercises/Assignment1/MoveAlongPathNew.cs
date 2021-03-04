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

        void Start()
        {
            Init();
            CreateCurveSegments();
        }
        void Update()
        {
            MoveObjectAlongPath();
        }

        void Init()
        {
            time = 0;
            t = 0;

            controlPoints = controlPointParent.GetComponent<PathFinding>().GetPath();
            curveSegments = new List<CurveSegment>();
        }

        void CreateCurveSegments()
        {
            Transform[] cpTransform = controlPointParent.GetComponentsInChildren<Transform>().Where(t => t != controlPointParent.transform).ToArray();

            foreach (Transform transform in cpTransform)
            {
                controlPoints.Add(transform.position);
            }

            this.transform.position = controlPoints[1];

            int cpCount = controlPoints.Count;

            for (int i = 0; i < cpCount - 3; i++)
            {
                int a = i + 0;
                int b = i + 1;
                int c = i + 2;
                int d = i + 3;
                curveSegments.Add(new CurveSegment(controlPoints[a], controlPoints[b], controlPoints[c], controlPoints[d], CurveType.CATMULLROM));
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

        void triggerEffect()
        {
            Debug.Log("Trigger effect!");
        }
    }
}

