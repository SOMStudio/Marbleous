using System.Collections;
using Trajectory;
using UnityEngine;

namespace Marbleous
{
    public class Mover : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private float speed = 10.0f;
        [SerializeField] private bool moving = false;
        [SerializeField] private bool active = true;

        [Header("Spline")]
        [SerializeField] int splineNumber = -1;
        [SerializeField] float splineRelatePosition = 0.0f;

        [Header("Shift")]
        [SerializeField] private AnimationCurve shiftCurve;
        [SerializeField] private float shiftMultiplier = 1;
        [SerializeField] private float timeDuration = 1;

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

        private void Awake()
        {
            startLevel = true;
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
                        ;
                    }
                }
            }
        }

        private MoveSplineDirection CanMoveDirection(float splineRPosition, int splineN, MoveDirection moveDirection)
        {
            var result = MoveSplineDirection.not;

            var levelManager = LevelManager.Instance;
            Spline activeSpline = levelManager.SplineList[splineN];
            Vector3 activeSplinePosition = activeSpline.GetPosition(splineRPosition);
            float activeSplineRelatePosition = splineRPosition;

            bool findResult = false;
            if (!findResult && activeSplineRelatePosition < 1)
            {
                float checkSplineRelatePos = activeSplineRelatePosition + .1f;

                if (checkSplineRelatePos <= 1)
                {
                    Vector3 checkSplinePos = activeSpline.GetPosition(checkSplineRelatePos);
                    Vector3 directShift = checkSplinePos - activeSplinePosition;

                    switch (moveDirection)
                    {
                        case MoveDirection.right when Mathf.RoundToInt(directShift.x) > 0:
                            result = MoveSplineDirection.increment;
                            findResult = true;
                            break;
                        case MoveDirection.left when Mathf.RoundToInt(directShift.x) < 0:
                            result = MoveSplineDirection.increment;
                            findResult = true;
                            break;
                        case MoveDirection.up when Mathf.RoundToInt(directShift.y) > 0:
                            result = MoveSplineDirection.increment;
                            findResult = true;
                            break;
                        case MoveDirection.down when Mathf.RoundToInt(directShift.y) < 0:
                            result = MoveSplineDirection.increment;
                            findResult = true;
                            break;
                    }
                }
            }

            if (!findResult && activeSplineRelatePosition > 0)
            {
                float checkSplineRelatePos = activeSplineRelatePosition - .1f;

                if (checkSplineRelatePos >= 0)
                {
                    Vector3 checkSplinePos = activeSpline.GetPosition(checkSplineRelatePos);
                    Vector3 directShift = checkSplinePos - activeSplinePosition;

                    switch (moveDirection)
                    {
                        case MoveDirection.right when Mathf.RoundToInt(directShift.x) > 0:
                            result = MoveSplineDirection.decrement;
                            findResult = true;
                            break;
                        case MoveDirection.left when Mathf.RoundToInt(directShift.x) < 0:
                            result = MoveSplineDirection.decrement;
                            findResult = true;
                            break;
                        case MoveDirection.up when Mathf.RoundToInt(directShift.y) > 0:
                            result = MoveSplineDirection.decrement;
                            findResult = true;
                            break;
                        case MoveDirection.down when Mathf.RoundToInt(directShift.y) < 0:
                            result = MoveSplineDirection.decrement;
                            findResult = true;
                            break;
                    }
                }
            }

            return result;
        }

        public void Move(MoveDirection moveDirection)
        {
            if (!active) return;
            if (moving) return;

            var levelManager = LevelManager.Instance;
            bool findResult = false;

            // check active spline
            var moveSplineDirection = CanMoveDirection(splineRelatePosition, splineNumber, moveDirection);
            if (moveSplineDirection != MoveSplineDirection.not)
            {
                findResult = true;
                Move(moveSplineDirection);
            }

            // check intersect spline
            if (!findResult)
            {
                var activePosition = levelManager.SplineList[splineNumber].GetPosition(splineRelatePosition);
                var splineList = levelManager.SplineList;
                for (int i = 0; i < splineList.Length; i++)
                {
                    if (i != splineNumber)
                    {
                        var activeSpline = splineList[i];
                        for (int j = 0; j < activeSpline.RoutesCount + 1; j++)
                        {
                            var checkPosition = activeSpline.GetRoutePosition(j);
                            if (activePosition == checkPosition)
                            {
                                var checkRelatePosition = activeSpline.GetRouteRelatePosition(j);
                                moveSplineDirection = CanMoveDirection(checkRelatePosition, i, moveDirection);
                                if (moveSplineDirection != MoveSplineDirection.not)
                                {
                                    splineNumber = i;
                                    splineRelatePosition = checkRelatePosition;
                                }
                            }

                            if (moveSplineDirection != MoveSplineDirection.not) break;
                        }
                    }

                    if (moveSplineDirection != MoveSplineDirection.not) break;
                }

                if (moveSplineDirection != MoveSplineDirection.not)
                {
                    findResult = true;
                    Move(moveSplineDirection);
                }
            }

            if (!findResult)
            {
                LevelManager.Instance.BallCanNotMove(this, moveDirection);
            }
        }

        public void Move(MoveSplineDirection moveSplineDirection)
        {
            if (!active) return;
            if (moving) return;

            if (moveSplineDirection == MoveSplineDirection.increment)
            {
                StartCoroutine(GoByRouteToInc());
            }
            else if (moveSplineDirection == MoveSplineDirection.decrement)
            {
                StartCoroutine(GoByRouteToDec());
            }

            //SoundManager.Instance?.PlaySoundByIndex(2, transform.position);
        }

        private IEnumerator GoByRouteToInc()
        {
            var levelManager = LevelManager.Instance;
            var activeSpline = levelManager.SplineList[splineNumber];
            float lenthSpline = activeSpline.GetLength();
            float targetPosition = levelManager.NextObjectIncDirection(this);

            moving = true;

            while (moving)
            {
                splineRelatePosition += Time.deltaTime * speed / lenthSpline;
                if (splineRelatePosition > targetPosition)
                {
                    if (targetPosition == 1 && activeSpline.Cycle)
                    {
                        splineRelatePosition = 0;
                        targetPosition = levelManager.NextObjectIncDirection(this);
                    }
                    else
                    {
                        splineRelatePosition = targetPosition;
                        moving = false;
                    }
                }

                transform.position = activeSpline.GetPosition(splineRelatePosition);

                yield return new WaitForEndOfFrame();
            }

            if (!LevelManager.Instance.BallReachedTarget(this, MoveSplineDirection.increment))
            {
                if (targetPosition != 1)
                {
                    Move(MoveSplineDirection.increment);
                }
            }
        }

        private IEnumerator GoByRouteToDec()
        {
            var levelManager = LevelManager.Instance;
            var activeSpline = levelManager.SplineList[splineNumber];
            float lenthSpline = activeSpline.GetLength();
            float targetPosition = levelManager.NextObjectDecDirection(this);

            moving = true;

            while (moving)
            {
                splineRelatePosition -= Time.deltaTime * speed / lenthSpline;

                if (splineRelatePosition < targetPosition)
                {
                    if (targetPosition == 0 && activeSpline.Cycle)
                    {
                        splineRelatePosition = 1;
                        targetPosition = levelManager.NextObjectDecDirection(this);
                    }
                    else
                    {
                        splineRelatePosition = targetPosition;
                        moving = false;
                    }
                }

                transform.position = activeSpline.GetPosition(splineRelatePosition);

                yield return new WaitForEndOfFrame();
            }

            if (!LevelManager.Instance.BallReachedTarget(this, MoveSplineDirection.decrement))
            {
                if (targetPosition != 0)
                {
                    Move(MoveSplineDirection.decrement);
                }
            }
        }
        
        public void Shake(MoveDirection moveDirection)
        {
            if (!active) return;
            if (moving) return;

            
            StartCoroutine(ShakeToRight(moveDirection));
            
        }

        private void PlaySoundShake()
        {
            SoundManager.Instance.PlaySoundByIndex(3, transform.position);
        }

        private Vector3 MoveDirectionToVector3(MoveDirection moveDirection)
        {
            Vector3 result = Vector3.zero;
            
            switch (moveDirection)
            {
                case MoveDirection.left:
                    result = Vector3.left;
                    break;
                case MoveDirection.right:
                    result = Vector3.right;
                    break;
                case MoveDirection.up:
                    result = Vector3.up;
                    break;
                case MoveDirection.down:
                    result = Vector3.down;
                    break;
            }

            return result;
        }
        
        private IEnumerator ShakeToRight(MoveDirection moveDirection)
        {
            Vector3 startPosition = transform.position;
            float timeForShake = 0.0f;
            Vector3 shiftDirection = MoveDirectionToVector3(moveDirection);

            moving = true;

            Invoke(nameof(PlaySoundShake), timeDuration / 3);
            
            while (moving)
            {
                timeForShake += Time.deltaTime / timeDuration;

                if (timeForShake >= 1)
                {
                    timeForShake = 1;
                    
                    moving = false;

                    PlaySoundShake();
                }

                transform.position = startPosition + shiftMultiplier * shiftCurve.Evaluate(timeForShake) * shiftDirection;

                yield return new WaitForEndOfFrame();
            }

            moving = false;
        }
    }
}
