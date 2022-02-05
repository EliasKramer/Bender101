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

    /// <summary>
    /// a start for the game
    /// </summary>
    void Start()
    {
        currActionFieldCollisions = new List<GameObject>();
        _lastTimeStompAttack = DateTime.UtcNow;
        actualPlayerCollisions = new List<GameObject>();
    }

    /// <summary>
    /// 
    /// </summary>
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
            if (speedMultForPlayerDistance > distanceToFloatOver) // if it slows down more than half then it should apply a force in a 90° angle of the current distance to the player. so it moves around the player
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
            float distanceWhereObjectsShallStartToGoAroundPlayer = (_outerSafetyZoneRadiusAroundThePlayerRadius - _innerSafetyZoneRadiusAroundThePlayerRadius) / 2;

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

                }
                else if (vecPlayerObj.x <= 0 && distanceClickedPlayer.x >= 0)
                {
                    rightAngleVec.y *= -1f;
                }

                //then we apply the right angled vector as our direction where our speed shall go
                actualSpeedVec = rightAngleVec.normalized;
            }

            //now we apply a velocity to our direction
            actualSpeedVec *= _speedForMovingObjects;

            //applying the velocity and slowing it down if it goes towards the player.
            curr.GetComponent<Rigidbody2D>().velocity = actualSpeedVec;// GetSpeedWithAntiPlayerCollision(curr,actualSpeedVec,true);
        }
    }
    private Vector2 GetSpeedWithAntiPlayerCollision(GameObject obj, Vector2 speedVecTryingToApply, bool allowNegativeSpeed)
    {
        ///this method will reduce the speed we try to apply to an object, if it comes near the player.

        Vector2 distanceClickedObj = (Vector2)obj.transform.position + speedVecTryingToApply;
        Vector2 distanceObjPlayer = this.transform.position - obj.transform.position;

        float colliderDistance = Physics2D.Distance(this.GetComponent<Collider2D>(), obj.GetComponent<Collider2D>()).distance;

        float angleToPlayer = Vector2.Angle(distanceClickedObj, distanceObjPlayer); //if > 0 = moving to player

        bool isMovingToPlayer = angleToPlayer < 70; //could be one more precisely this is a lil guesswork
        if (isMovingToPlayer)
        {
            float speedMultForPlayerDistance = MultiplierForObjectSlowDown(
                colliderDistance,
                _innerSafetyZoneRadiusAroundThePlayerRadius,
                _outerSafetyZoneRadiusAroundThePlayerRadius,
                allowNegativeSpeed);

            return speedVecTryingToApply * speedMultForPlayerDistance;
        }
        return speedVecTryingToApply;
    }
    private float MultiplierForObjectSlowDown(float distanceToObject, float innerBorderToObject, float outerBorderToObect, bool neagativeSpeedForObject)
    {
        //this mehtod takes the distance to an object and slows it down the more it comes to the inner Border. As soon as the outer Border is reached it slows the object down.

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