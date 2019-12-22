using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Mover ballPlayer;
    [SerializeField] private Hole[] holeList;
    [SerializeField] private Mover[] ballList;
    [SerializeField] private Spline[] splineList;
    [SerializeField] private LineRenderer[] lineList;
    [SerializeField] private int[] lineCountPart;
    
    [Header("Control")]
    [SerializeField] private float minShiftForDirection = 20.0f;
    
    private bool mouseButtonDown = false;
    private Vector3 positionClick;
    private Vector3 positionShift;
    
    private MoveDirection shiftMouseDirection = MoveDirection.not;

    public static LevelManager Instance;

    private bool inWorkState = false;

    public bool InWorkState => inWorkState;
    
    public Spline[] SplineList => splineList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDrawGizmos()
    {
        bool findProblem = false;

        if (splineList.Length != lineList.Length)
        {
            Debug.LogError("Not the same size: SplineList, LineList");
            findProblem = true;
        } else if (splineList.Length != lineCountPart.Length)
        {
            Debug.LogError("Not the same size: SplineList, LineCountPart");
            findProblem = true;
        }
        
        if (!findProblem)
        {
            foreach (var value in SplineList)
            {
                if (value == null)
                {
                    findProblem = true;
                    break;
                }
                else if (!value.InWorkState)
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
        
        if (inWorkState)
        {
            for (int i = 0; i < lineList.Length; i++)
            {
                var activeSpline = splineList[i];
                var activeLine = lineList[i];
                int activeCountPart = lineCountPart[i];

                if (activeLine == null || activeCountPart == 0)
                {
                    Debug.LogError("Not set: LineList[" + i + "]");
                    continue;
                } else if (activeCountPart == 0)
                {
                    Debug.LogError("Not set: LineCountPart[" + i + "]");
                    continue;
                }

                activeLine.positionCount = activeCountPart + 1;

                for (int j = 0; j < activeCountPart; j++)
                {
                    var setPosition = activeSpline.GetPosition((float) j / activeCountPart) + Vector3.forward * 1.5f;
                    activeLine.SetPosition(j, setPosition);
                }
                
                var lastPosition = activeSpline.GetPosition(1.0f) + Vector3.forward * 1.5f;
                activeLine.SetPosition(activeCountPart, lastPosition);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseButtonDown = true;
            positionClick = Input.mousePosition;
        } else if (Input.GetMouseButton(0))
        {
            if (mouseButtonDown)
            {
                positionShift = Input.mousePosition - positionClick;
                if (Mathf.Abs(positionShift.x) > minShiftForDirection)
                {
                    if (positionShift.x > 0)
                    {
                        shiftMouseDirection = MoveDirection.right;
                    }
                    else if (positionShift.x < 0)
                    {
                        shiftMouseDirection = MoveDirection.left;
                    }
                } else if (Mathf.Abs(positionShift.y) > minShiftForDirection)
                {
                    if (positionShift.y > 0)
                    {
                        shiftMouseDirection = MoveDirection.up;
                    }
                    else if (positionShift.y < 0)
                    {
                        shiftMouseDirection = MoveDirection.down;
                    }
                }

                if (shiftMouseDirection != MoveDirection.not)
                {
                    ballPlayer?.Move(shiftMouseDirection);

                    mouseButtonDown = false;
                    positionClick = Vector3.zero;
                    positionShift = Vector3.zero;
                    shiftMouseDirection = MoveDirection.not;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (mouseButtonDown)
            {
                mouseButtonDown = false;
                positionClick = Vector3.zero;
                positionShift = Vector3.zero;
            }
        }
    }

    public float NextObjectIncDirection(Mover ball)
    {
        float findResult = 1;
        
        foreach (var value in holeList)
        {
            if (value.Active)
            {
                if (value.SplineNumber == ball.SplineNumber)
                {
                    if (value.SplineRelatePosition > ball.SplineRelatePosition && value.SplineRelatePosition < findResult)
                    {
                        findResult = value.SplineRelatePosition;
                    }
                }
            }
        }

        foreach (var value in ballList)
        {
            if (value.Active)
            {
                if (value != ball)
                {
                    if (value.SplineNumber == ball.SplineNumber)
                    {
                        if (value.SplineRelatePosition > ball.SplineRelatePosition && value.SplineRelatePosition < findResult)
                        {
                            findResult = value.SplineRelatePosition;
                        }
                    }
                }
            }
        }
        
        if (ball != ballPlayer && ballPlayer)
        {
            if (ballPlayer.SplineNumber == ball.SplineNumber)
            {
                if (ballPlayer.SplineRelatePosition > ball.SplineRelatePosition && ballPlayer.SplineRelatePosition < findResult)
                {
                    findResult = ballPlayer.SplineRelatePosition;
                }
            }
        }

        return findResult;
    }
    
    public float NextObjectDecDirection(Mover ball)
    {
        float findResult = 0;
        
        foreach (var value in holeList)
        {
            if (value.Active)
            {
                if (value.SplineNumber == ball.SplineNumber)
                {
                    if (value.SplineRelatePosition < ball.SplineRelatePosition && value.SplineRelatePosition > findResult)
                    {
                        findResult = value.SplineRelatePosition;
                    }
                }
            }
        }
        
        foreach (var value in ballList)
        {
            if (value.Active)
            {
                if (value != ball)
                {
                    if (value.SplineNumber == ball.SplineNumber)
                    {
                        if (value.SplineRelatePosition < ball.SplineRelatePosition && value.SplineRelatePosition > findResult)
                        {
                            findResult = value.SplineRelatePosition;
                        }
                    }
                }
            }
        }

        if (ball != ballPlayer && ballPlayer)
        {
            if (ballPlayer.SplineNumber == ball.SplineNumber)
            {
                if (ballPlayer.SplineRelatePosition < ball.SplineRelatePosition && ballPlayer.SplineRelatePosition > findResult)
                {
                    findResult = ballPlayer.SplineRelatePosition;
                }
            }
        }

        return findResult;
    }

    public bool BallReachedTarget(Mover ball, MoveSplineDirection moveDirection)
    {
        bool reachedTarget = false;

        foreach (var value in holeList)
        {
            if (value.Active)
            {
                if (value.SplineNumber == ball.SplineNumber)
                {
                    if (value.SplineRelatePosition == ball.SplineRelatePosition)
                    {
                        reachedTarget = true;

                        value.Active = false;
                        ball.Active = false;

                        BallFallInHole(ball);
                        break;
                    }
                }
            }
        }

        if (!reachedTarget)
        {
            foreach (var value in ballList)
            {
                if (value.Active)
                {
                    if (value != ball)
                    {
                        if (value.SplineNumber == ball.SplineNumber)
                        {
                            if (value.SplineRelatePosition == ball.SplineRelatePosition)
                            {
                                reachedTarget = true;
                                
                                BallKickBall(ball, value, moveDirection);
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (!reachedTarget)
        {
            if (ball != ballPlayer && ballPlayer)
            {
                if (ball.SplineRelatePosition == ballPlayer.SplineRelatePosition)
                {
                    reachedTarget = true;
                    
                    BallKickBall(ball, ballPlayer, moveDirection);
                }
            }
        }

        return reachedTarget;
    }

    private void BallFallInHole(Mover ball)
    {
        ball.transform.position -= new Vector3(0, 0, -2.3f);
        
        SoundManager.Instance?.PlaySoundByIndex(3, ball.transform.position);
    }

    private void BallKickBall(Mover ball1, Mover ball2, MoveSplineDirection moveDirection)
    {
        ball2.Move(moveDirection);
        
        SoundManager.Instance?.PlaySoundByIndex(4, ball1.transform.position);
    }

    public void BallCanNotMove(Mover ball, MoveDirection moveDirection)
    {
        Debug.Log("Need create wave in " + moveDirection);
    }
}

public enum MoveSplineDirection
{
    not,
    increment,
    decrement
}

public enum MoveDirection
{
    not,
    left,
    right,
    up,
    down
}
