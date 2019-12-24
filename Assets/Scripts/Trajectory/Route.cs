using UnityEngine;

namespace Trajectory
{
    public class Route : MonoBehaviour
    {
        [SerializeField] private Transform[] controlPoints;
        [SerializeField] private float length = 0.0f;

        private Vector3 currentPosition;
        private Vector3 prevPosition;

        private bool inWorkState = false;

        public bool InWorkState => inWorkState;

        private void OnDrawGizmos()
        {
            bool findProblem = false;
            if (controlPoints.Length < 4)
            {
                findProblem = true;
            }
            else
            {
                foreach (var value in controlPoints)
                {
                    if (value == null)
                    {
                        findProblem = true;
                        break;
                    }
                }
            }

            if (inWorkState && findProblem)
            {
                inWorkState = false;
            } else if (!inWorkState && !findProblem)
            {
                inWorkState = true;
            }
        
            if (InWorkState)
            {
                float t0 = 0.0f;
                float t1 = 1.0f;
                float stepCalculate = (t1 - t0) / 10;
                float t = t0;

                do
                {
                    t += stepCalculate;
                    if (t > t1) t = t1;

                    currentPosition = GetPosition(t);

                    Gizmos.DrawSphere(currentPosition, 0.5f);
                } while (t < 1);

                Gizmos.color = Color.black;
                Gizmos.DrawLine(controlPoints[0].position, controlPoints[1].position);
                Gizmos.DrawLine(controlPoints[2].position, controlPoints[3].position);
            }
        }

        public Transform[] ControlPoints
        {
            get => controlPoints;
        }

        private void Init()
        {
            UpdateRouteLength();
        }

        private void UpdateRouteLength()
        {
            float lengthCalculate = 0.0f;
            const int StepCount = 50;
            Vector3 point = GetBezierPosition(0);
        
            for (int i = 0; i < StepCount; ++i)
            {
                Vector3 nextPoint = GetBezierPosition((float)(i + 1) / StepCount);
                lengthCalculate += (point - nextPoint).magnitude;
                point = nextPoint;
            }
        
            length = lengthCalculate;
        }

        public Vector3 GetBezierPosition(float t)
        {
            return  Mathf.Pow(1 - t, 3) * controlPoints[0].position +
                    3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position +
                    3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position +
                    Mathf.Pow(t, 3) * controlPoints[3].position;
        }

        public Vector3 GetPosition(float t)
        {
            float totalLength = GetLength(true);
            const int StepCount = 50;

            float currentLength = 0.0f;
            Vector3 pointA = GetBezierPosition(0.0f);
            for (var i = 1; i < StepCount; ++i)
            {
                Vector3 pointB = GetBezierPosition((float) i / StepCount);
                float segmentLength = (pointB - pointA).magnitude;

                if (t >= currentLength/totalLength && t < (currentLength + segmentLength)/totalLength)
                {
                    float alpha = (t - currentLength / totalLength) /
                                  ((currentLength + segmentLength) / totalLength - currentLength / totalLength);
                    Vector3 result = pointA + (pointB - pointA) * alpha; 
                    return result;
                }
            
                currentLength += segmentLength;
                pointA = pointB;
            }

            return GetBezierPosition(1.0f);
        }

        public float GetLength(bool update = false)
        {
            if (length == 0.0f || update)
            {
                Init();
            }
            return length;
        }
    }
}