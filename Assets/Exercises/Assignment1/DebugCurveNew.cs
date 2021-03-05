using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AfGD.Assignment1;

namespace AfGD.Assignment1
{
    public enum DrawType { NONE = 0, BEHIND = 1, INFRONT = 2, BOTH = 3, FULL = 4 };
    
    public class DebugCurveNew : MonoBehaviour
    {

        private List<Vector3> controlPoints;
        List<CurveSegment> curveSegments;
        private CurveType curveType = CurveType.HERMITE;

        [Header("Debug varaibles")]
        [Range(2, 100)]
        public int debugSegments = 20;
        public bool closedLoop = false;
        public bool drawPath = true;
        public Color pathColor = Color.magenta;
        public bool drawTangents = true;
        public float tangentScale = 1.0f;
        public Color tangentColor = Color.green;
        private bool hasPath;

        void Update()
        {
            if (hasPath)
            {
                if (drawPath)
                    foreach (CurveSegment curve in curveSegments)
                        DrawCurveSegments(curve, pathColor, debugSegments);
                if (drawTangents)
                    foreach (CurveSegment curve in curveSegments)
                        DrawTangents(curve, tangentColor, debugSegments, tangentScale);
            }
            else
                GetPath();

        }

        bool Init()
        {
            int cpCount = controlPoints.Count;
            if (cpCount  < 1) 
                return false;

            curveSegments = new List<CurveSegment>();

            if (closedLoop)
            {
                for (int i = 0; i < cpCount; i++)
                {
                    int a = (i + 0) % cpCount;
                    int b = (i + 1) % cpCount;
                    int c = (i + 2) % cpCount;
                    int d = (i + 3) % cpCount;
                    curveSegments.Add(new CurveSegment(controlPoints[a], controlPoints[b], controlPoints[c], controlPoints[d], curveType));
                }
            }
            else
            {
                for (int i = 0; i < cpCount - 3; i++)
                {
                    int a = i + 0;
                    int b = i + 1;
                    int c = i + 2;
                    int d = i + 3;
                    curveSegments.Add(new CurveSegment(controlPoints[a], controlPoints[b], controlPoints[c], controlPoints[d], curveType));
                }
            }
            return true;
        }

        public static void DrawCurveSegments(CurveSegment curve,
            Color color, int segments = 50)
        {
            float fSegments = segments;
            for (int i = 0; i < fSegments; i++)
            {
                float u1 = i / fSegments;
                float u2 = (i+1) / fSegments;
                Vector3 p1 = curve.Evaluate(u1);
                Vector3 p2 = curve.Evaluate(u2);
                Debug.DrawLine(p1, p2, color);
            }
        }

        public static void DrawTangents(CurveSegment curve,
            Color color, int segments = 50, float scale = 0.1f)
        {
            float fSegments = segments;
            for (int i = 0; i < fSegments; i++)
            {
                float u = i / fSegments;
                Vector3 p = curve.Evaluate(u);
                Vector3 ptan = curve.EvaluateDv(u);
                Debug.DrawLine(p, p + ptan * scale, color);
            }
        }
        void GetPath()
        {
            controlPoints = this.GetComponent<PathFinding>().GetPath();
            hasPath = controlPoints.Count > 0;
            if (hasPath)
                Init();
        }
    }
}