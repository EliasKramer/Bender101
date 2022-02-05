using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class bendingScript : MonoBehaviour
{
    private List<GameObject> currActionFieldCollisions;
    private List<GameObject> actualPlayerCollisions;

    private float mouseSmoothingInnerBorder = 0.1f;
    private float mouseSmoothingOuterBorder = 3f;


    private System.DateTime _lastTimeStompAttack;
    private float _stompAttackMinDelayInMs = 500f;
    private bool _stompAttackDelayIsEnough = true;
    private float _forceForStone = 6f;

    private System.DateTime _lastTimePushAttack;
    private float _pushAttackMinDelay = 500f;
    private bool _pushAttackDelayIsEnough = true;

    private System.DateTime _lastTimeStompAttackAttempt;
    private float _durationBeforeStompAttackGetsHoverAttack = 200f;
    private bool _stompIsHover = false;

    private bool _jumpButtonIsDown = false;
    private Vector2 _worldPointWhereClicked;
    private bool _altMode1Active = false;
    private bool _pushAttackActive = false;
    private bool _pullAttackActive = false;

    static private float _speedForMovingObjects = 4f;
    static private float _innerSafetyZoneRadiusAroundThePlayerRadius = 2f;
    static private float _outerSafetyZoneRadiusAroundThePlayerRadius = _innerSafetyZoneRadiusAroundThePlayerRadius + 0.2f;
    [SerializeField]
    private GameObject _actionField;
    public Camera _cam;
    void Start()
    {
        currActionFieldCollisions = new List<GameObject>();
        _lastTimeStompAttack = DateTime.UtcNow;
        actualPlayerCollisions = new List<GameObject>();
    }
    void Update()
    {
        CheckDelays();
        PerformAttacks();
    }
    private void CheckDelays()
    {
        System.DateTime now = DateTime.UtcNow;
        if (!_stompAttackDelayIsEnough && ((now - _lastTimeStompAttack).TotalMilliseconds >= _stompAttackMinDelayInMs))
        {
            _lastTimeStompAttack = now;
            _stompAttackDelayIsEnough = true;
        }
        if (!_pushAttackDelayIsEnough && ((now - _lastTimePushAttack).TotalMilliseconds >= _pushAttackMinDelay))
        {
            _lastTimePushAttack = now;
            _pushAttackDelayIsEnough = true;
        }
        _stompIsHover = (DateTime.UtcNow - _lastTimeStompAttackAttempt).TotalMilliseconds > _durationBeforeStompAttackGetsHoverAttack;
    }
    private void PerformAttacks()
    {
        _worldPointWhereClicked = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (_jumpButtonIsDown && _stompIsHover && !_pullAttackActive && !_pushAttackActive)
        {
            PerformHoverAttack();
        }
        else if (!_pushAttackActive && _pullAttackActive)
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
                objectsToMove[i].GetComponent<Rigidbody2D>().velocity = (vecToObject[i].normalized) * 5f * -1f * slowDownIfNearMult;
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
            curr.GetComponent<Rigidbody2D>().velocity = actualspeed.normalized * 4f * MultiplierForObjectSlowDown(actualspeed.magnitude, 0.1f, 0.2f, false);
        }
    }
    private void PerformPullToMouseAttack()
    {
        foreach (GameObject curr in currActionFieldCollisions)
        {

            /*
            Vector2 vectorColliderDistance = colliderDistance.pointB - colliderDistance.pointA;



            float distanceToFloatOver = 1.5f;


            actualSpeedVec *= _speedForMovingObjects; // setting the speed

            bool isMoving90DegMode = false;
            //---
            if (speedMultForPlayerDistance > distanceToFloatOver) // if it slows down more than half then it should apply a force in a 90� angle of the current distance to the player. so it moves around the player
            {
                actualSpeedVec *= speedMultForPlayerDistance;
                Debug.Log("point 1");
            }
            else
            {
                Vector2 rightAngleVec = new Vector2();
                rightAngleVec.x = vecPlayerObj.y;
                rightAngleVec.y = vecPlayerObj.x;
                bool mouseIsOnTheOppositeSideOfTheCurrentObject = true;
                if (vecPlayerObj.x > 0 && distanceClickedPlayer.x < 0)
                {
                    rightAngleVec.x *= -1f;

                }
                else if (vecPlayerObj.x <= 0 && distanceClickedPlayer.x >= 0)
                {
                    rightAngleVec.y *= -1f;
                }
                else
                {
                    mouseIsOnTheOppositeSideOfTheCurrentObject = false;
                    actualSpeedVec *= speedMultForPlayerDistance;
                }

                if (mouseIsOnTheOppositeSideOfTheCurrentObject)
                {
                    actualSpeedVec = rightAngleVec.normalized * _speedForMovingObjects;

                    Debug.DrawLine(curr.transform.position,
                     (Vector2)curr.transform.position + rightAngleVec,
                     Color.green, 0.1f);
                    isMoving90DegMode = true;

                }
            }
            //---



            //if(!isMovingToPlayer || !isMoving90DegMode)
            //{
                actualSpeedVec *= MultiplierForObjectSlowDown(distanceClickedObj.magnitude, mouseSmoothingInnerBorder, mouseSmoothingOuterBorder, false);

                Debug.DrawLine(curr.transform.position,
                 (Vector2)curr.transform.position + actualSpeedVec,
                 Color.red, 0.1f);
                Debug.Log("point 7");
            //}
            */
            ///---------------------------------------------------- new try. clean -------------------------------------------------------------------

            // get the inital vector where the object shall head to
            Vector2 distanceClickedObj = (_worldPointWhereClicked - (Vector2)curr.transform.position);

            //this will be used later to determin wether you clicked left or right in relation to the player
            Vector2 distanceClickedPlayer = (Vector2)this.transform.position - _worldPointWhereClicked;

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
                //in order to calculate a vector that is 90� turned it must first switch the x and y values
                Vector2 rightAngleVec = new Vector2();
                rightAngleVec.x = vecPlayerObj.y;
                rightAngleVec.y = vecPlayerObj.x;

                // to finally get a 90� of a vector you must multiply one of your values by -1. 
                // depending on where you do it, the vector will go left or right (90�)
                // for example: if the point where we want to move is on the left and the current obj is on the right,
                //              it shall move 90� up
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
            actualSpeedVec *= _speedForMovingObjects;



            //applying the velocity and slowing it down if it goes towards the player.
            curr.GetComponent<Rigidbody2D>().velocity = GetSpeedWithAntiPlayerCollision(curr, actualSpeedVec, 0.1f, true); ;
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
            //the distance between the tow colliders
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
        //if it is not moving towards the player we can just give the speed it wants to move back.
        //no need for slowing down
        return speedVecTryingToApply;
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
        //innerBorderToObject += ObjSize.magnitude;
        //outerBorderToObect += ObjSize.magnitude;

        //if you dont want negative speed
        if (distanceToObject < innerBorderToObject && !neagativeSpeedForObject)
        {
            return 0;
        }
        if (distanceToObject < outerBorderToObect)
        {
            float calcNumberInBorderSystem = distanceToObject - innerBorderToObject;
            //Debug.Log($"distaceTo Object: {distanceToObject}, calcNumberInBorderSystem {calcNumberInBorderSystem}, return: {calcNumberInBorderSystem / distBetweenBorders}");
            return calcNumberInBorderSystem / distBetweenBorders;
        }
        return 1;
    }
    private void PerformStompAttack()
    {
        if (_stompAttackDelayIsEnough && !_stompIsHover)
        {
            foreach (GameObject curr in currActionFieldCollisions)
            {
                curr.GetComponent<Rigidbody2D>().velocity = new Vector2(0, _forceForStone);
            }
            _stompAttackDelayIsEnough = false;
        }
    }
    private void PerformRegularPushAttack()
    {
        _worldPointWhereClicked = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        if (_pushAttackDelayIsEnough && !_pullAttackActive)
        {
            foreach (GameObject curr in currActionFieldCollisions)
            {
                Vector2 speedVec;
                if (!_altMode1Active)
                {
                    speedVec = (_worldPointWhereClicked - (Vector2)this.transform.position).normalized;

                }
                else
                {
                    speedVec = (_worldPointWhereClicked - (Vector2)curr.transform.position).normalized;
                }
                speedVec *= 25f;
                speedVec.y *= 0.2f;
                curr.GetComponent<Rigidbody2D>().velocity += speedVec;
            }
            _pushAttackDelayIsEnough = false;
        }
    }
    public void JumpButtonAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _lastTimeStompAttackAttempt = DateTime.UtcNow;
            _jumpButtonIsDown = true;
        }
        if (context.canceled)
        {
            _jumpButtonIsDown = false;
            PerformStompAttack();

        }
    }
    public void PushAttack(InputAction.CallbackContext context)
    {
        if (context.performed) //taste unten
        {
            _pushAttackActive = true;
        }
        if (context.canceled) // taste wieder oben
        {
            _pushAttackActive = false;
            PerformRegularPushAttack();
        }
    }
    public void PullAttack(InputAction.CallbackContext context)
    {
        if (context.performed) //taste unten
        {
            _pullAttackActive = true;
        }
        if (context.canceled) // taste wieder oben
        {
            _pullAttackActive = false;
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
}