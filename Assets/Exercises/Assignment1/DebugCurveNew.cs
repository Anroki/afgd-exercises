using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AfGD.Assignment1;

namespace AfGD.Assignment1
{
    public enum DrawType { NONE = 0, BEHIND = 1, INFRONT = 2, BOTH = 3, FULL = 4 };
    
    [RequireComponent(typeof(LineRenderer))]
    public class DebugCurveNew : MonoBehaviour
    {

        private List<Vector3> controlPoints;
        List<CurveSegment> curveSegments;
        private CurveType curveType = CurveType.CATMULLROM;
        public DrawType drawType = DrawType.NONE;
        public float lineWidth = 1.0f;
        public AnimationCurve lineCurve;

        [Header("Debug varaibles")]
        [Range(2, 100)]
        public int debugSegments = 20;
        public float drawLength = 10;
        public bool closedLoop = false;
        public bool drawPath = true;
        public Color pathColor = Color.magenta;
        public bool drawTangents = true;
        public float tangentScale = 1.0f;
        public Color tangentColor = Color.green;
        private float currentCurveLength;
        private List<Vector3> linePoints;
        private LineRenderer lineRenderer;

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (Application.isEditor)
            {
                if (!Init())
                    return;
            }
            if (drawPath && drawType == DrawType.FULL)
                foreach (CurveSegment curve in curveSegments)
                    DrawCurveSegments(curve, pathColor, debugSegments);
            if (drawTangents && drawType == DrawType.FULL)
                foreach (CurveSegment curve in curveSegments)
                    DrawTangents(curve, tangentColor, debugSegments, tangentScale);

        }

        bool Init()
        {
            controlPoints = new List<Vector3>();
            linePoints = new List<Vector3>();
            lineRenderer = gameObject.GetComponent<LineRenderer>();
            lineRenderer.widthCurve = lineCurve;
            lineRenderer.widthMultiplier = lineWidth;

            controlPoints = this.GetComponent<PathFinding>().GetPath();

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

        public void DrawCurveFromPosition(float u, int curveIndex)
        {
            CurveSegment previousCurve;
            CurveSegment nextCurve;
            CurveSegment currentCurve = curveSegments[curveIndex];


            if (curveIndex == 0)
                previousCurve = curveSegments[curveSegments.Count - 1];
            else
                previousCurve = curveSegments[curveIndex - 1];


            if (curveIndex == curveSegments.Count - 1)
                nextCurve = curveSegments[0];
            else
                nextCurve = curveSegments[curveIndex + 1];


            if (drawType == DrawType.BEHIND || drawType == DrawType.BOTH)
            {
                currentCurveLength = 0;
                List<Vector3> tempPoints = new List<Vector3>();
                tempPoints.AddRange(GetCurvePointsBehind(currentCurve, u, pathColor));
                tempPoints.AddRange(GetCurvePointsBehind(previousCurve, 1.0f, pathColor));
                tempPoints.Reverse();
                linePoints.AddRange(tempPoints);
            }

            if(drawType == DrawType.INFRONT || drawType == DrawType.BOTH)
            {
                currentCurveLength = 0;
                GetCurvePointsInFront(currentCurve, u, pathColor);
                GetCurvePointsInFront(nextCurve, 0, pathColor);
            }
            lineRenderer.positionCount = linePoints.Count;
            lineRenderer.SetPositions(linePoints.ToArray());
        }

        public void GetCurvePointsInFront(CurveSegment curve, float u0, Color color, int segments = 25)
        {
            float fSegments = segments;
            for (int i = 0; i < fSegments; i++)
            {
                float u = u0 + i / fSegments;
                if (u > 1.0f || currentCurveLength >= drawLength)
                    break;
                
                Vector3 p1 = curve.Evaluate(u);
                
                if (linePoints.Count > 1)
                {
                    Vector3 p0 = linePoints[linePoints.Count-2];
                    currentCurveLength += Vector3.Distance(p0, p1);
                }
                linePoints.Add(p1);
            }
        }
        public List<Vector3> GetCurvePointsBehind(CurveSegment curve, float u0, Color color, int segments = 25)
        {
            float fSegments = segments;
            List<Vector3> tempPoints = new List<Vector3>();
            for (int i = 0; i < fSegments; i++)
            {
                float u = u0 - i / fSegments;
                if (u < 0.0f || currentCurveLength >= drawLength)
                    break;

                Vector3 p1 = curve.Evaluate(u);

                if (tempPoints.Count > 1)
                {
                    Vector3 p0 = tempPoints[tempPoints.Count - 2];
                    currentCurveLength += Vector3.Distance(p0, p1);
                }
                tempPoints.Add(p1);
            }
            return tempPoints;
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

        public void SetDrawType(DrawType drawType)
        {
            this.drawType = drawType;
        }
    }
}