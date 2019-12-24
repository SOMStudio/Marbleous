using System;
using System.Collections.Generic;
using UnityEngine;

namespace Trajectory
{
    public class Spline : MonoBehaviour
    {
        [SerializeField] private Route[] routes;
        [SerializeField] private bool cycle = false;
        [SerializeField] private bool joinControl = true;
        [SerializeField] private float splineLength = 0.0f;
    
        private List<float> routesLength = new List<float>();
        private List<float> dependLength = new List<float>();

        private bool inWorkState = false;

        public bool InWorkState => inWorkState;

        public bool Cycle => cycle;

        public int RoutesCount => routes.Length;

        private void OnDrawGizmos()
        {
            bool findProblem = false;
            if (routes[0] == null)
            {
                findProblem = true;
            }
            else
            {
                for (int i = 1; i < routes.Length; i++)
                {
                    if (routes[i] == null || routes[i] == routes[i - 1] || !routes[i].InWorkState)
                    {
                        findProblem = true;
                        break;
                    }
                }
            }

            if (inWorkState && findProblem)
            {
                inWorkState = false;
            }
            else if (!inWorkState && !findProblem)
            {
                inWorkState = true;
            }

            if (inWorkState)
            {
                for (int i = 1; i < routes.Length; i++)
                {
                    routes[i].ControlPoints[0].position = routes[i - 1].ControlPoints[3].position;
                    routes[i].ControlPoints[0].gameObject.SetActive(false);

                    Vector3 dirLine = Vector3.zero;
                    float lengthDirLine = 0.0f;

                    if (i == routes.Length - 1)
                    {
                        if (cycle)
                        {
                            routes[i].ControlPoints[3].position = routes[0].ControlPoints[0].position;
                            routes[i].ControlPoints[3].gameObject.SetActive(false);

                            if (joinControl)
                            {
                                dirLine = (routes[0].ControlPoints[0].position - routes[0].ControlPoints[1].position)
                                    .normalized;
                                lengthDirLine = (routes[i].ControlPoints[2].position - routes[i].ControlPoints[3].position)
                                    .magnitude;
                                routes[i].ControlPoints[2].position =
                                    routes[i].ControlPoints[3].position + dirLine * lengthDirLine;
                            }
                        }
                    }

                    if (joinControl)
                    {
                        dirLine = (routes[i - 1].ControlPoints[3].position - routes[i - 1].ControlPoints[2].position)
                            .normalized;
                        lengthDirLine = (routes[i].ControlPoints[1].position - routes[i].ControlPoints[0].position)
                            .magnitude;
                        routes[i].ControlPoints[1].position = routes[i].ControlPoints[0].position + dirLine * lengthDirLine;
                    }
                }

                GetLength(true);
            }
        }
    
        private void Init()
        {
            UpdateRoutesLength();

            UpdateDependLength();
        }

        private void UpdateDependLength()
        {
            float lengthCalculate = 0.0f;
            if (dependLength.Count == 0)
            {
                dependLength.Add(0.0f);
            }
        
            for (int i = 1; i < routes.Length; i++)
            {
                if (i == dependLength.Count)
                {
                    dependLength.Add(0.0f);
                }

                lengthCalculate += routesLength[i - 1] / splineLength;
                dependLength[i] = lengthCalculate;
            }
        
            if (dependLength.Count == routes.Length)
                dependLength.Add(1.0f);
        }

        private void UpdateRoutesLength()
        {
            splineLength = 0.0f;

            for (int i = 0; i < routes.Length; i++)
            {
                if (i == routesLength.Count)
                {
                    routesLength.Add(0.0f);
                }

                routesLength[i] = routes[i].GetLength(true);
                splineLength += routesLength[i];
            }
        }

        private int GetRoutFromSpline(float t)
        {
            for (int i = 0; i < routes.Length; i++)
            {
                if (t >= dependLength[i] && t <= dependLength[i + 1])
                {
                    return i;
                }
            }

            throw new ArgumentOutOfRangeException("param: t (Function: GetRoutFromSpline)");
        }

        private Vector3 GetPositionFromRoutSpline(float t)
        {
            int numberRout = GetRoutFromSpline(t);
            float depandTime = t - dependLength[numberRout];
            float time = depandTime / (dependLength[numberRout + 1] - dependLength[numberRout]);

            return routes[numberRout].GetPosition(time);
        }

        public Vector3 GetRoutePosition(int numberRoute)
        {
            if (numberRoute < routes.Length)
            {
                return routes[numberRoute].GetBezierPosition(0);
            }
            else if (numberRoute == routes.Length)
            {
                return routes[numberRoute - 1].GetBezierPosition(1);
            }
        
            throw new ArgumentOutOfRangeException("param: numberRoute (Function: GetRoutePosition)");
        }

        public float GetRouteRelatePosition(int numberRoute)
        {
            if (numberRoute < dependLength.Count)
            {
                return dependLength[numberRoute];
            }
        
            throw new ArgumentOutOfRangeException("param: numberRoute (Function: GetRouteRelatePosition)");
        }

        public Vector3 GetPosition(float t)
        {
            return GetPositionFromRoutSpline(t);
        }

        public float GetLength(bool update = true)
        {
            if (splineLength == 0.0f || update)
            {
                Init();
            }
            return splineLength;
        }
    }
}
