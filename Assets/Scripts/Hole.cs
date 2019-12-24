using UnityEngine;

namespace Marbleous
{
    public class Hole : MonoBehaviour
    {
        [SerializeField] private bool active = true;

        [SerializeField] int splineNumber = -1;
        [SerializeField] float splineRelatePosition = 0.0f;

        private bool startLevel = false;

        public int SplineNumber
        {
            get { return splineNumber; }
        }

        public float SplineRelatePosition
        {
            get { return splineRelatePosition; }
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        private void OnDrawGizmos()
        {
            if (!startLevel)
            {
                bool findProblem = false;
                var levelManager = FindObjectOfType<LevelManager>();

                if (levelManager != null)
                {
                    if (!levelManager.InWorkState)
                    {
                        findProblem = true;
                    }
                }
                else
                {
                    findProblem = true;
                }

                if (!findProblem)
                {
                    bool findNearestPosition = false;

                    float minDistance = 100;
                    int splineNumber = 0;
                    int routeNumber = 0;

                    for (int i = 0; i < levelManager.SplineList.Length; i++)
                    {
                        for (int j = 0; j <= levelManager.SplineList[i].RoutesCount; j++)
                        {
                            Vector3 curRoutePosition = levelManager.SplineList[i].GetRoutePosition(j);
                            float distanceTo = (transform.position - curRoutePosition).magnitude;
                            if (distanceTo < 10)
                            {
                                if (distanceTo < minDistance)
                                {
                                    splineNumber = i;
                                    routeNumber = j;

                                    minDistance = distanceTo;

                                    findNearestPosition = true;
                                }
                            }
                        }
                    }

                    if (findNearestPosition)
                    {
                        this.splineNumber = splineNumber;
                        splineRelatePosition =
                            levelManager.SplineList[splineNumber].GetRouteRelatePosition(routeNumber);

                        transform.position = levelManager.SplineList[splineNumber].GetRoutePosition(routeNumber);
                    }
                }
            }
        }
    }
}
