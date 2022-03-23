using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities2D;
using System.Linq;
public class BendingScript : MonoBehaviour
{
    private List<GameObject> currActionFieldCollisions;
    private List<GameObject> actualPlayerCollisions;

    [SerializeField]
    GameObject Platforms;
    private Collider2D _platformCompositeCollider;
    private Collider2D _platformGridCollider;

    private float mouseSmoothingInnerBorder = 0.1f;
    private float mouseSmoothingOuterBorder = 3f;

    private float _stompDelayInMs = 400f;
    private float _timeForHover = 200f;
    private Delay _stompDelay;

    private float _pushDelayInMs = 500f;
    private Delay _pushDelay;

    private float _pullDelayInMs = 500f;
    private Delay _pullDelay;

    private bool _testStateIsOn = false;

    private Vector2 _currentMousePos;
    private Vector2 _startMousePos;
    private bool _altMode1Active = false;

    [SerializeField]
    public float SpeedForStoneShooting = 20f;
    public float SpeedForStoneMovement = 4f;
    public float SpeedForstoneStomp = 6f;

    static private float _innerSafetyZoneRadiusAroundThePlayerRadius = 2f;
    static private float _outerSafetyZoneRadiusAroundThePlayerRadius = _innerSafetyZoneRadiusAroundThePlayerRadius + 0.2f;

    /// <summary>
    /// temporarily used variables
    /// </summary>
    private List<GameObject> _tempObjectsToMoveOutOfTheWay = new List<GameObject>();
    private List<GameObject> _tempObjectsToShoot = new List<GameObject>();
    private float _tempHeightForMovingOutOfTheWay;
    private Vector2 _tempDirectionToApply;
    private bool _tempStartedSlicingAction;
    private long _tempCurrentFixedUpdateIterationCount;
    private float _tempPushForceForSlicing = 5;
    //end of temps
    public Camera _cam;

    private long _fixedUpdateIterationCount;
    void Start()
    {
        _fixedUpdateIterationCount = 0;

        currActionFieldCollisions = new List<GameObject>();
        actualPlayerCollisions = new List<GameObject>();

        _stompDelay = new Delay(_stompDelayInMs, false);
        _pushDelay = new Delay(_pushDelayInMs, false);
        _pullDelay = new Delay(_pullDelayInMs, false);
        _platformCompositeCollider = Platforms.GetComponent<CompositeCollider2D>();
    }
    void FixedUpdate()
    {
        PerformAttacks();

        CheckForObjectMovement();

        GameObject test = null;
        try
        {
            test = currActionFieldCollisions[0];
        }
        catch
        { }
        if (test != null)
        {
            TestMethod(currActionFieldCollisions[0], this.transform.position);
        }
        _fixedUpdateIterationCount++;
    }

    private void CheckForObjectMovement()
    {
        if (_tempStartedSlicingAction)
        {
            if (_fixedUpdateIterationCount == _tempCurrentFixedUpdateIterationCount)
            {
                //remove items that were null because slicer removed them if they were too small
                _tempObjectsToShoot = _tempObjectsToShoot.Where(x => x != null).ToList();
                _tempObjectsToMoveOutOfTheWay = _tempObjectsToMoveOutOfTheWay.Where(x => x != null).ToList();

                _tempPushForceForSlicing = _tempObjectsToShoot.Count() * 5;
                foreach (GameObject curr in _tempObjectsToMoveOutOfTheWay)
                {

                    curr.GetComponent<StoneScript>().SetNoFriction();
                    //curr.GetComponent<Rigidbody2D>().freezeRotation = true;

                    if (curr.transform.position.y < _tempHeightForMovingOutOfTheWay)
                    {
                        //Debug.Log(curr.Key.name + " unten");
                        Debug.DrawLine(curr.transform.position, (Vector2)curr.transform.position + Vector2.down * 5f, Color.red, 5);
                        curr.GetComponent<Rigidbody2D>().velocity = Vector2.down * 3f;
                        curr.GetComponent<Rigidbody2D>().freezeRotation = true;
                    }
                    else
                    {
                        //Debug.Log(curr.Key.name + " oben");
                        Debug.DrawLine(curr.transform.position, (Vector2)curr.transform.position + Vector2.up * 5f, Color.green, 5);
                        curr.GetComponent<Rigidbody2D>().velocity = Vector2.up * 3f;
                    }

                }
                //remove deleted items from objectsToShoot
                _tempObjectsToShoot = _tempObjectsToShoot.Where(x => x != null).ToList();
                Debug.Log("check: current:" + _fixedUpdateIterationCount + " == saved:" + _tempCurrentFixedUpdateIterationCount);
            }
            else if (_fixedUpdateIterationCount == _tempCurrentFixedUpdateIterationCount +1)
            {
                foreach(GameObject curr in _tempObjectsToShoot)
                {
                    curr.GetComponent<StoneScript>().SetNoFriction();
                    curr.GetComponent<Rigidbody2D>().freezeRotation = true;
                    //curr.GetComponent<Rigidbody2D>().gravityScale = 0;

                    curr.GetComponent<Rigidbody2D>().velocity = Vector2.up * 6f;
                }
            }
            else if (_fixedUpdateIterationCount > _tempCurrentFixedUpdateIterationCount + 10)
            {
                if (_tempObjectsToShoot.Count == 0)
                {
                    _tempStartedSlicingAction = false;
                    _tempPushForceForSlicing = 5;
                    _tempCurrentFixedUpdateIterationCount = -1;
                    _tempObjectsToMoveOutOfTheWay.Clear();
                }
                else
                {
                    GameObject farthestObjectToShoot = _tempObjectsToShoot[0];
                    farthestObjectToShoot.GetComponent<StoneScript>().SetNoFriction();
                    farthestObjectToShoot.GetComponent<Rigidbody2D>().freezeRotation = true;

                    ApplyPushVelocity(farthestObjectToShoot, _tempDirectionToApply, _tempPushForceForSlicing);

                    Debug.Log($"obj to shoot {farthestObjectToShoot} iteration:{_fixedUpdateIterationCount}");

                    _tempObjectsToShoot.RemoveAt(0);
                    _tempPushForceForSlicing -= 5;
                }
            }
            /*
                float distance = (this.transform.position - curr.Key.transform.position).magnitude;
            ApplyPushVelocity(curr.Key, _tempDirectionToApply, Mathf.Pow(distance, 2));

            _tempPushForceForSlicing;
            foreach (KeyValuePair<GameObject, float> curr in _tempObjectsToShoot)
            {                
                float distance = (this.transform.position - curr.Key.transform.position).magnitude;
                ApplyPushVelocity(curr.Key, _tempDirectionToApply, Mathf.Pow(distance, 2));
            }*/
        }
    }

    private void PerformAttacks()
    {
        _currentMousePos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        if(_stompDelay == null)
        {
            Debug.Log($"stompDelay is null");
        }

        if (_pullDelay == null)
        {
            Debug.Log($"_pullDelay is null");
        }
        if (_pushDelay == null)
        {
            Debug.Log($"_pushDela is null");
        }
        if (_stompDelay.IsDoingAction && (_stompDelay.ActionDurationInMs > _timeForHover) && !_pullDelay.IsDoingAction && !_pushDelay.IsDoingAction)
        {
            PerformHoverAttack();
        }
        else if (!_pushDelay.IsDoingAction && _pullDelay.IsDoingAction)
        {
            if (!_altMode1Active)
            {
                PerformPullToPlayerAttack();
            }
            else
            {
                PerformPullToMouseAttack();
            }
        }
    }

    private void SetObjectSpeed(GameObject objectToMove, Vector2 attemtedVelocityToApply)
    {
        objectToMove.GetComponent<Rigidbody2D>().velocity = (attemtedVelocityToApply);
    }
    private void PerformHoverAttack()
    {
        //2 Points where Stones should move to and hover
        Vector2[] sidePointsForHoveringStones = { new Vector2(-2f, 0.75f), new Vector2(2f, 0.75f) };
        //2 stones which should be moved there
        GameObject[] objectsToMove = new GameObject[2];
        //the 2 vectors where the stones should move
        Vector2[] vecToObject = new Vector2[2];

        for (int i = 0; i < sidePointsForHoveringStones.Length; i++) // iterate through the two stone hover positions
        {
            foreach (GameObject currObj in currActionFieldCollisions) // iterate through every stone in the players range
            {
                //vector from the current stone to the player;
                Vector2 vectorToPlayer = (Vector2)currObj.gameObject.transform.position - (Vector2)this.transform.position;

                Vector2 vectorObjPos = currObj.transform.position;

                Vector2 objectBounds = currObj.GetComponent<Collider2D>().bounds.size;

                Vector2 addedBounds = (vectorToPlayer.normalized * objectBounds.magnitude) / 4;

                Vector2 posToMoveTo = CalculateMoreDistanceIfObjectIsBigger(currObj, sidePointsForHoveringStones[i], 0.25f, 0.3f);

                //vector from the current stone to the hover position
                Vector2 vectorToPoint = vectorObjPos - ((Vector2)this.transform.position + posToMoveTo);

                //try to finding a stone for each hover spot. if the hover position is on the left side, the stone that gets pulled to that position has to be on the left side too
                if ((vectorToPlayer.x < 0 && sidePointsForHoveringStones[i].x < 0) || ((vectorToPlayer.x >= 0 && sidePointsForHoveringStones[i].x >= 0)))
                {

                    //if the player doesnt have a stone for this hover position already or the stone we currently have is nearer to our hover position we change our current stone
                    if (objectsToMove[i] == null || vecToObject[i].magnitude > vectorToPoint.magnitude)
                    {
                        objectsToMove[i] = currObj;
                        vecToObject[i] = vectorToPoint;
                    }
                }
            }
            if (objectsToMove[i] != null) // if the player finds a stone, it should be moved
            {
                float slowDownIfNearMult = MultiplierForObjectSlowDown(vecToObject[i].magnitude, 0.1f, 0.8f, false);
                Vector2 movementVec = (vecToObject[i].normalized) * SpeedForStoneMovement * -1f * slowDownIfNearMult;
                SetObjectSpeed(objectsToMove[i], movementVec);
            }
        }
    }
    private Vector2 CalculateMoreDistanceIfObjectIsBigger(GameObject objectToMove, Vector2 vecFromThisPosToDestinationPos, float distanceMultiplier, float yMultiplier)
    {
        //vector from the current stone to the player;
        Vector2 vectorToPlayer = (Vector2)objectToMove.gameObject.transform.position - (Vector2)this.transform.position;

        //get the objects bounds size
        Vector2 objectBounds = objectToMove.GetComponent<Collider2D>().bounds.size;

        Vector2 addedBounds = (vectorToPlayer.normalized * objectBounds.magnitude) * distanceMultiplier;

        //get the position where the object shall move
        Vector2 posToMoveTo = vecFromThisPosToDestinationPos;

        //add the bounds to the position to move
        Vector2 retVal = posToMoveTo + addedBounds;

        //add a bit of height
        retVal.y += (yMultiplier * (objectBounds.y / 2));

        return retVal;
    }
    private void PerformPullToPlayerAttack()
    {
        //this method is for pulling lots of objects towards the player
        foreach (GameObject curr in currActionFieldCollisions)
        {
            //for each object we get the vector to the player
            Vector2 vectorPlayerToObj = ((Vector2)curr.transform.position - (Vector2)this.transform.position);

            //we want the objects to hover in a circle around the player.
            //therfore we take the direction to the player and give it the length of the safety radius
            Vector2 posWhereObjShallMoveTo = vectorPlayerToObj.normalized * _innerSafetyZoneRadiusAroundThePlayerRadius * -1f;

            //now we calculate the new point where the object shall move to
            //if the object is bigger it shall hover farther away
            Vector2 posWithSizeOfObjectCalculatedIn = CalculateMoreDistanceIfObjectIsBigger(curr, posWhereObjShallMoveTo, 0.4f, 0.1f) * -1f;

            //now calculate the speed
            Vector2 actualspeed = (Vector2)this.transform.position - 2 * (posWhereObjShallMoveTo) - posWithSizeOfObjectCalculatedIn - (Vector2)curr.transform.position;

            //set the veclocity to the calculated the point with a set speed. 
            //it should also slow down if it gets near the point to avoid shaking
            Vector2 finalMovement = actualspeed.normalized * SpeedForStoneMovement * MultiplierForObjectSlowDown(actualspeed.magnitude, 0.1f, 0.2f, false);
            SetObjectSpeed(curr, finalMovement);
        }
    }
    private void PerformPullToMouseAttack()
    {
        foreach (GameObject curr in currActionFieldCollisions)
        {
            // get the inital vector where the object shall head to
            Vector2 distanceClickedObj = (_currentMousePos - (Vector2)curr.transform.position);

            //this will be used later to determin wether you clicked left or right in relation to the player
            Vector2 distanceClickedPlayer = (Vector2)this.transform.position - _currentMousePos;

            // this will be the speed we will apply later. Before that we give it the general direction it shall head to.
            Vector2 actualSpeedVec = distanceClickedObj.normalized;

            // this is the distance between the two collider. first is the player and second is the object we are trying to move
            // it will be important later. as we try to get around the player if the destination is on the other side
            float colliderDistance = Physics2D.Distance(this.GetComponent<Collider2D>(), curr.GetComponent<Collider2D>()).distance;

            // the distance where the objects shall start to move around the player
            float distanceWhereObjectsShallStartToGoAroundPlayer = _innerSafetyZoneRadiusAroundThePlayerRadius + (_outerSafetyZoneRadiusAroundThePlayerRadius / 2);

            // is important later for calculating the angle for the vector for moving around the player
            Vector2 vecPlayerObj = ((Vector2)this.transform.position - (Vector2)curr.transform.position);

            // as soon as the distance from the object to the player is less than the distance where it shall start to move around the player
            // it shall move around the player
            if (colliderDistance <= distanceWhereObjectsShallStartToGoAroundPlayer)
            {
                //in order to calculate a vector that is 90° turned it must first switch the x and y values
                Vector2 rightAngleVec = new Vector2();
                rightAngleVec.x = vecPlayerObj.y;
                rightAngleVec.y = vecPlayerObj.x;

                // to finally get a 90° of a vector you must multiply one of your values by -1. 
                // depending on where you do it, the vector will go left or right (90°)
                // for example: if the point where we want to move is on the left and the current obj is on the right,
                //              it shall move 90° up
                if (vecPlayerObj.x > 0 && distanceClickedPlayer.x < 0)
                {
                    rightAngleVec.x *= -1f;
                    actualSpeedVec = rightAngleVec.normalized;

                }
                else if (vecPlayerObj.x <= 0 && distanceClickedPlayer.x >= 0)
                {
                    rightAngleVec.y *= -1f;
                    actualSpeedVec = rightAngleVec.normalized;

                }

                //then we apply the right angled vector as our direction where our speed shall go
            }

            //now we apply a velocity to our direction
            actualSpeedVec *= SpeedForStoneMovement;



            //applying the velocity and slowing it down if it goes towards the player.
            SetObjectSpeed(curr, GetSpeedWithAntiPlayerCollision(curr, actualSpeedVec, 0.3f, true));
        }
    }
    /// <summary>
    /// this method will reduce the speed we try to apply to an object, if it comes near the player
    /// </summary>
    /// <param name="obj">The object that we try to slow down</param>
    /// <param name="speedVecTryingToApply">The speed that we try to apply</param>
    /// <param name="minDistanceBetweenPlayerAndStone">this is a small buffer. if it is set to 0 the player and the object could collide a bit</param>
    /// <param name="allowNegativeSpeed">if it is true and the object gets too near it will push the object away from the player</param>
    /// <returns>It returns the the speed, that shall be applied to the object</returns>
    private Vector2 GetSpeedWithAntiPlayerCollision(GameObject obj, Vector2 speedVecTryingToApply, float minDistanceBetweenPlayerAndStone, bool allowNegativeSpeed)
    {
        //the basic idea is, that you take the object position, the point where it should head to and draw a vector = objPos + speedVecTryingToApply
        //then you take the borders of the player and object, add them and add a buffer to that = playerPos + allBounds
        //if you draw a vector from the object to that point (playerPos + allBounds) - objPos then you will have the second vector
        //then we calcuclate the angles from these vectors to the player->object vector
        //we got now 2 angles. if the speedangle we try to apply is in the bounds of the border angle it should slow down

        //the player position
        Vector2 playerPos = this.transform.position;
        //the object position
        Vector2 objPos = obj.transform.position;
        //the vector from the obj to the player
        Vector2 vecPlayerObject = objPos - playerPos;
        //the vector from the player to the destination
        Vector2 vecPlayerDestination = (objPos + speedVecTryingToApply) - playerPos;

        //the bounds of the player and object colliders
        Vector2 boundsPlayer = this.GetComponent<Collider2D>().bounds.size / 2;
        Vector2 boundsObject = obj.GetComponent<Collider2D>().bounds.size / 2;

        //adding the bounds together
        Vector2 allBounds = boundsPlayer + boundsObject;

        //setting the border vector in the right direction
        if (objPos.x < playerPos.x)
        {
            allBounds.x *= -1f;
        }
        if (playerPos.y > vecPlayerDestination.y)
        {
            allBounds.y *= -1f;
        }

        //add the buffer to the bounds
        allBounds = (allBounds.normalized) * (allBounds.magnitude + minDistanceBetweenPlayerAndStone);

        //this is the vector from the object to the border position
        Vector2 vecObjToSlowDownBorder = (playerPos + allBounds) - objPos;

        //this is the angle from the speed we try to apply in relation to the player->object vector
        float angleToPlayer = Vector2.Angle(speedVecTryingToApply, vecPlayerObject);
        //this is the angle from the border we calculated in relation to the player->object vector
        float angleToBorder = Vector2.Angle(vecObjToSlowDownBorder, vecPlayerObject);

        //if the object would continue this path it would hit the player, so we slow it down
        if (angleToPlayer > angleToBorder)
        {
            //the distance between the two colliders
            float colliderDistance = Physics2D.Distance(this.GetComponent<Collider2D>(), obj.GetComponent<Collider2D>()).distance;

            //calculating a multiplier by how much we should slow our object down
            float speedMultForPlayerDistance = MultiplierForObjectSlowDown(
                colliderDistance,
                _innerSafetyZoneRadiusAroundThePlayerRadius,
                _outerSafetyZoneRadiusAroundThePlayerRadius,
                allowNegativeSpeed);

            //if it is just slowing down we can just apply the slower speed
            if (speedMultForPlayerDistance >= 0)
            {
                return speedVecTryingToApply * speedMultForPlayerDistance;
            }
            //if it is already in the safetyzone of the player it should get out of there immediatly
            else
            {
                //goes in the opposite direction of the player
                return vecPlayerObject.normalized * speedMultForPlayerDistance * -1f;
            }
        }
        //the normal case. should slow down if it goes near the mouse to avoid shaking
        return speedVecTryingToApply * MultiplierForObjectSlowDown(
            (_currentMousePos - (Vector2)obj.transform.position).magnitude,
            mouseSmoothingInnerBorder,
            mouseSmoothingOuterBorder,
            true);
    }
    /// <summary>
    /// this mehtod takes the distance to an object and slows it down the more it comes to the inner Border. As soon as the outer Border is reached it slows the object down.
    /// </summary>
    /// <param name="distanceToObject"></param>
    /// <param name="innerBorderToObject"></param>
    /// <param name="outerBorderToObect"></param>
    /// <param name="neagativeSpeedForObject"></param>
    /// <returns></returns>
    private float MultiplierForObjectSlowDown(float distanceToObject, float innerBorderToObject, float outerBorderToObect, bool neagativeSpeedForObject)
    {
        float distBetweenBorders = outerBorderToObect - innerBorderToObject;

        //if you dont want negative speed
        if (distanceToObject < innerBorderToObject && !neagativeSpeedForObject)
        {
            return 0;
        }
        if (distanceToObject < outerBorderToObect)
        {
            float calcNumberInBorderSystem = distanceToObject - innerBorderToObject;
            float retVal = calcNumberInBorderSystem / distBetweenBorders;
            return retVal >= -1 ? retVal : -1;
        }
        return 1;
    }
    private void PerformStompAttack()
    {
        if (!(_stompDelay.ActionDurationInMs > _timeForHover))
        {
            if (_stompDelay.IsDoingAction)
            {
                foreach (GameObject curr in currActionFieldCollisions)
                {
                    SetObjectSpeed(curr, new Vector2(0, SpeedForstoneStomp));
                }
            }
        }
    }
    private void PerformRegularPushAttack()
    {
        _currentMousePos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        if (!_pullDelay.IsDoingAction)
        {
            if (_pushDelay.IsDoingAction)
            {
                if (_altMode1Active)
                {
                    foreach (GameObject curr in currActionFieldCollisions)
                    {
                        Vector2 direction;

                        direction = (_currentMousePos - (Vector2)curr.transform.position).normalized;

                        ApplyPushVelocity(curr, direction, 0);
                    }
                }
                else
                {
                    if (currActionFieldCollisions.Count != 0)
                    {
                        GameObject nearestObject = null;
                        float nearestDistance = float.MaxValue;
                        foreach (GameObject curr in currActionFieldCollisions)
                        {
                            Vector2 objToMousePos = _currentMousePos - (Vector2)curr.transform.position;
                            if (nearestObject == null || objToMousePos.magnitude <= nearestDistance)
                            {
                                nearestObject = curr;
                                nearestDistance = objToMousePos.magnitude;
                            }
                        }
                        Vector2 thisPos = transform.position;

                        Vector2 mouseToPlayer = _currentMousePos - thisPos;

                        Vector2 rightAngle = RightAngle(mouseToPlayer, true).normalized;

                        float thickness = 0.2f;

                        Vector2 playerUpStartPoint = thisPos + (rightAngle * (thickness / 2));

                        Vector2 playerDownStartPoint = thisPos + ((rightAngle * -1) * (thickness / 2));

                        float timeForDebug = 5f;
                        /*
                        Debug.DrawLine(playerUpStartPoint, playerUpStartPoint + mouseToPlayer, Color.green, timeForDebug);
                        Debug.DrawLine(playerDownStartPoint, playerDownStartPoint + mouseToPlayer, Color.green, timeForDebug);
                        Debug.DrawLine(thisPos, thisPos + mouseToPlayer, Color.grey, timeForDebug);
                        */

                        Pair2D sliceUpperLine = new Pair2D(playerUpStartPoint, playerUpStartPoint + mouseToPlayer);
                        Pair2D sliceLowerLine = new Pair2D(playerDownStartPoint, playerDownStartPoint + mouseToPlayer);
                        List<Slicer2D.Slice2D> upperSlices = Slicer2D.Slicing.LinearSliceAll(sliceUpperLine);
                        List<Slicer2D.Slice2D> lowerSlices = Slicer2D.Slicing.LinearSliceAll(sliceLowerLine);

                        List<GameObject> slicerOriginObjects = new List<GameObject>();

                        foreach (Slicer2D.Slice2D slice in upperSlices)
                        {
                            slicerOriginObjects.Add(slice.originGameObject);
                        }
                        foreach (Slicer2D.Slice2D slice in lowerSlices)
                        {
                            slicerOriginObjects.Add(slice.originGameObject);
                        }

                        //lowerSlices[0].originGameObject
                        /*
                        Pair2 sliceUpperLine = new Pair2(playerUpStartPoint, playerUpStartPoint + mouseToPlayer);
                        Pair2 sliceLowerLine = new Pair2(playerDownStartPoint, playerDownStartPoint + mouseToPlayer);

                        List<Slicer2D.Slice2D> upperSlices = Slicer2D.Slicing.LinearCutSliceAll(LinearCut.Create(sliceUpperLine, 0.04f));
                        List<Slicer2D.Slice2D> lowerSlices = Slicer2D.Slicing.LinearCutSliceAll(LinearCut.Create(sliceLowerLine, 0.04f));
                        */

                        /*
                        foreach (Slicer2D.Slice2D slice in upperSlices)
                        {
                            foreach (GameObject curr in slice.GetGameObjects())
                            {
                                Debug.Log($"origin: {slice.originGameObject.name} upper curr: {curr.name}");
                            }
                        }*/
                        List<GameObject> objectsToGetOutOfTheWay = new List<GameObject>();
                        List<GameObject> objectsToShoot = new List<GameObject>();

                        Median averageYLevelForShootingObjects = new Median();

                        foreach (Slicer2D.Slice2D slice in lowerSlices)
                        {
                            foreach (GameObject curr in slice.GetGameObjects())
                            {
                                //curr.GetComponent<Rigidbody2D>().gravityScale = 0;
                                Vector2 vecToCurr = (Vector2)curr.transform.position - thisPos;
                                float angleToCurr = Vector2.Angle(mouseToPlayer, vecToCurr);

                                float rightAngleDistanceToCurr = Mathf.Sin(Mathf.Deg2Rad * angleToCurr) * vecToCurr.magnitude;
                                float parallelDistToCurr = Mathf.Cos(Mathf.Deg2Rad * angleToCurr) * vecToCurr.magnitude;
                                /*
                                Debug.DrawLine(curr.transform.position,
                                    (Vector2)curr.transform.position + (RightAngle(mouseToPlayer.normalized, true) * rightAngleDistanceToCurr),
                                    Color.blue,
                                    timeForDebug);*/
                                if (rightAngleDistanceToCurr <= (thickness / 2))
                                {
                                    /*Debug.Log($"-------name:{curr.name}--------");
                                    Debug.Log($"angle:{angleToCurr}");
                                    Debug.Log($"righAng:{rightAngleDistanceToCurr}");
                                    Debug.Log($"parallelDist:{parallelDistToCurr}");
                                    Debug.DrawLine(
                                        thisPos,
                                        thisPos + vecToCurr,
                                        Color.magenta,
                                        timeForDebug);*/
                                    averageYLevelForShootingObjects.Add(curr.transform.position.y);
                                    objectsToShoot.Add(curr);
                                }
                                else
                                {
                                    objectsToGetOutOfTheWay.Add(curr);
                                }
                            }
                        }
                        foreach (Slicer2D.Slice2D slice in upperSlices)
                        {
                            foreach (GameObject curr in slice.GetGameObjects())
                            {
                                objectsToGetOutOfTheWay.Add(curr);
                            }
                        }
                        foreach (GameObject curr in slicerOriginObjects)
                        {
                            if (objectsToGetOutOfTheWay.Contains(curr))
                            {
                                objectsToGetOutOfTheWay.Remove(curr);
                            }
                        }
                        foreach (GameObject curr in objectsToGetOutOfTheWay)
                        {
                            _tempObjectsToMoveOutOfTheWay.Add(curr);
                        }
                        foreach (GameObject curr in objectsToShoot)
                        {
                            _tempObjectsToShoot.Add(curr);
                        }
                        _tempDirectionToApply = mouseToPlayer.normalized;
                        _tempHeightForMovingOutOfTheWay = averageYLevelForShootingObjects.GetMedian();
                        _tempStartedSlicingAction = true;
                        _tempCurrentFixedUpdateIterationCount = _fixedUpdateIterationCount;

                        float GetDistanceToPlayer(GameObject obj)
                        {
                            return (this.transform.position - obj.transform.position).magnitude;
                        }

                        _tempObjectsToShoot.Sort((firstObj, secondObj) =>
                        {
                            return (GetDistanceToPlayer(firstObj).CompareTo(GetDistanceToPlayer(secondObj))) * -1;
                        });
                    }
                }
            }
        }
    }
    private Vector2 RightAngle(Vector2 vec, bool swapX)
    {
        float tempX = vec.x;
        vec.x = vec.y;
        vec.y = tempX;

        if (swapX)
        {
            vec.x *= -1;
        }
        else
        {
            vec.y *= -1;
        }
        return vec;
    }
    private void DrawPoint(Vector2 point)
    {
        float size = 0.1f;
        float time = 5f;
        Color color = Color.red;
        Debug.DrawLine(point, point + (Vector2.right * size), color, time);
        Debug.DrawLine(point, point + (Vector2.left * size), color, time);
        Debug.DrawLine(point, point + (Vector2.up * size), color, time);
        Debug.DrawLine(point, point + (Vector2.down * size), color, time);

    }
    private void ApplyPushVelocity(GameObject obj, Vector2 dir, float additionalForce)
    {
        dir *= SpeedForStoneShooting;
        //dir.y *= 0.2f;
        dir += (dir.normalized * additionalForce);
        Debug.DrawLine(obj.transform.position,
            (Vector2)obj.transform.position + dir.normalized,
            Color.yellow, 50f);
        Debug.Log($"{obj.name} applied {dir.magnitude} push velo");
        SetObjectSpeed(obj, dir);
    }
    public void StompAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _stompDelay.StartAction();
        }
        if (context.canceled)
        {
            PerformStompAttack();
            _stompDelay.StopAction();
        }
    }
    public void PushAttack(InputAction.CallbackContext context)
    {
        if (context.performed) //taste unten
        {
            _startMousePos = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _pushDelay.StartAction();
        }
        if (context.canceled) // taste wieder oben
        {
            PerformRegularPushAttack();
            Pair2D pair = new Pair2D(_startMousePos, _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            Slicer2D.Slicing.LinearSliceAll(pair);
            _pushDelay.StopAction();
        }
    }
    public void PullAttack(InputAction.CallbackContext context)
    {
        if (context.performed) //taste unten
        {
            _pullDelay.StartAction();
        }
        if (context.canceled) // taste wieder oben
        {
            _pullDelay.StopAction();
        }
    }
    public void AltMode1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _altMode1Active = true;
        }
        if (context.canceled)
        {
            _altMode1Active = false;
        }
    }
    public void addCollidedStone(GameObject gObj)
    {
        currActionFieldCollisions.Add(gObj);
    }
    public void remCollidedStone(GameObject gObj)
    {
        currActionFieldCollisions.Remove(gObj);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Stone")
        {
            actualPlayerCollisions.Add(collision.gameObject);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Stone")
        {
            actualPlayerCollisions.Remove(collision.gameObject);
        }
    }
    public void TestStateActivation(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _testStateIsOn = !_testStateIsOn;
            Time.timeScale *= 0.5f;
            Time.fixedDeltaTime *= 0.5f;
        }
    }
    public void TestMethod(GameObject givenObject, Vector2 toPointTo)
    {
        //takes an object and goes in the direction of the point, till the collider bounds are reached
        if (_testStateIsOn)
        {

            Vector2 bounds = givenObject.GetComponent<Collider2D>().bounds.size / 2;
            Vector2 objectPos = toPointTo;
            Vector2 collPos = givenObject.transform.position;

            Vector2 offset = ((collPos - objectPos) * -1f).normalized * bounds.magnitude;

            Debug.DrawLine(collPos,
                objectPos,
                Color.grey,
                0.1f);
            Debug.DrawLine(collPos,
                collPos + offset,
                Color.red,
                0.1f);

            Vector2 offSetBefore = offset;

            if (offset.x < 0)
            {
                bounds.x *= -1;
            }
            if (offset.y < 0)
            {
                bounds.y *= -1;
            }

            Debug.DrawLine(collPos,
                collPos + bounds,
                Color.red,
                0.1f);

            float diffX = diff(offset.x, bounds.x);
            float diffY = diff(offset.y, bounds.y);
            //Debug.Log($"after offset:{offset} bounds:{bounds} diffX: {diffX} diffY: {diffY}");

            float multX = 1;
            float multY = 1;
            if (positivVal(offset.y) > positivVal(bounds.y))
            {
                multY = bounds.y / offset.y;

            }

            if (positivVal(offset.x) > positivVal(bounds.x))
            {
                multX = bounds.x / offset.x;

            }
            //Debug.Log($"multx {multX} multy {multY} ");

            //float optinalMult = multY == 1 || multX == 1 ? 1 : 2;

            /*            Debug.DrawLine(collPos,
                                           collPos + (offset * multY * multX),
                                           Color.blue,
                                           0.1f);
                        DrawBounds(givenObject);
            */

            /*

            Vector2 offsetDir = offset.normalized;
            //Debug.Log($"before offset:{offset} bounds:{bounds}");
            if (offset.x > bounds.x || offset.x < (bounds.x * -1)) //other
            {
                if (offset.x >= 0)
                {
                    offset.x = bounds.x;
                }
                if(offset.x < 0)
                {
                    offset.x = bounds.x * -1f;
                }
            }

            float multX = offset.x / offSetBefore.x;
            offset *= (1-multX);

            //offset = offset.magnitude * offsetDir;
            Debug.DrawLine(playerPos,
                playerPos + offset,
                Color.blue,
                0.1f);
            */
        }
    }
    private float positivVal(float val)
    {
        if (val < 0)
        {
            return val * -1f;
        }
        return val;
    }
    private float diff(float a, float b)
    {
        return b - a;
    }
    private void DrawBounds(GameObject obj)
    {
        Vector2 bounds = obj.GetComponent<Collider2D>().bounds.size / 2;
        Vector2 midPoint = obj.transform.position;

        Vector2 boundsReal = midPoint;
        boundsReal.x += bounds.x;
        boundsReal.y += bounds.y;
        Vector2 rightUp = boundsReal;

        boundsReal = midPoint;
        boundsReal.x += bounds.x;
        boundsReal.y -= bounds.y;
        Vector2 rightDown = boundsReal;

        boundsReal = midPoint;
        boundsReal.x -= bounds.x;
        boundsReal.y += bounds.y;
        Vector2 leftUp = boundsReal;


        boundsReal = midPoint;
        boundsReal.x -= bounds.x;
        boundsReal.y -= bounds.y;
        Vector2 leftDown = boundsReal;


        Debug.DrawLine(
            leftUp,
            rightUp,
            Color.red,
            0.1f);
        Debug.DrawLine(
            rightUp,
            rightDown,
            Color.red,
            0.1f);
        Debug.DrawLine(
            rightDown,
            leftDown,
            Color.red,
            0.1f);
        Debug.DrawLine(
            leftDown,
            leftUp,
            Color.red,
            0.1f);
    }
}