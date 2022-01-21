using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class bendingScript : MonoBehaviour
{
    private List<GameObject> currActionFieldCollisions;
    private List<GameObject> actualPlayerCollisions;

    private float mouseSmoothingInnerBorder = 0.1f;
    private float mouseSmoothingOuterBorder = 1f;


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

                Vector2 posToMoveTo = ((Vector2)this.transform.position + sidePointsForHoveringStones[i] + addedBounds);

                posToMoveTo.y += (addedBounds.y / 1.5f);

                //vector from the current stone to the hover position
                Vector2 vectorToPoint = vectorObjPos - posToMoveTo;

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

                Vector2 vecToPlayer = (Vector2)objectsToMove[i].gameObject.transform.position - (Vector2)this.transform.position;

                Vector2 objectPosition = objectsToMove[i].transform.position;

                Vector2 playerPos = transform.position;

                Vector2 objectBounds = objectsToMove[i].GetComponent<Collider2D>().bounds.size;

                Vector2 addedBounds = (vecToPlayer.normalized * objectBounds.magnitude) / 4;

                //move it slower if it has too less distance to avoid shaking
                float slowDownIfNearMult = MultiplierForObjectSlowDown(vecToObject[i].magnitude, 0.1f, 0.8f, false);
                objectsToMove[i].GetComponent<Rigidbody2D>().velocity = (vecToObject[i].normalized) * 5f * -1f * slowDownIfNearMult;




                Debug.DrawLine(objectPosition,
                    playerPos,  //objectsToMove[i].GetComponent<Collider2D>().bounds.size.magnitude),
                    Color.magenta, 0.5f);

                Debug.DrawLine(objectPosition,
                     objectPosition + ((vecToObject[i].normalized) * 2f * -1f * slowDownIfNearMult),  //objectsToMove[i].GetComponent<Collider2D>().bounds.size.magnitude),
                    Color.red, 0.5f);

                Debug.DrawLine(objectPosition,
                    objectPosition + addedBounds,  //objectsToMove[i].GetComponent<Collider2D>().bounds.size.magnitude),
                    Color.green, 0.5f);

                Debug.DrawLine(objectPosition,
                    playerPos+sidePointsForHoveringStones[i]+addedBounds,  //objectsToMove[i].GetComponent<Collider2D>().bounds.size.magnitude),
                    Color.blue, 0.5f);

            }
        }
    }
    private void PerformPullToPlayerAttack()
    {
        foreach (GameObject curr in currActionFieldCollisions)
        {
            //Vector2 dir = ((Vector2)this.transform.position - (Vector2)curr.transform.position).normalized; //weiter weg wenn obj größer TODO TODO TODO
            Vector2 distanceVec = ((Vector2)this.transform.position - (Vector2)curr.transform.position);// +(curr.GetComponent<Collider2D>().bounds.size.magnitude*dir));
            Vector2 actualSpeedVec = distanceVec.normalized;

            actualSpeedVec *= 2f * MultiplierForObjectSlowDown(distanceVec.magnitude, 2f, 2.5f, true);
            curr.GetComponent<Rigidbody2D>().velocity = actualSpeedVec;
            Debug.DrawLine(curr.transform.position, (Vector2)curr.transform.position - actualSpeedVec, Color.green, 0.1f);
        }
    }
    private void PerformPullToMouseAttack()
    {
        foreach (GameObject curr in currActionFieldCollisions)
        {
            Vector2 distanceClickedObj = (_worldPointWhereClicked - (Vector2)curr.transform.position);
            Vector2 actualSpeedVec = distanceClickedObj.normalized; // give direction

            Vector2 distanceObjPlayer = ((Vector2)this.transform.position - (Vector2)curr.transform.position);

            float dotMovement = Vector2.Dot(distanceClickedObj, distanceObjPlayer); //if > 0 = moving to player
            bool isMovingToPlayer = dotMovement > 0;

            actualSpeedVec *= 5f; // setting the speed

            if (isMovingToPlayer)
            {
                float innerBorderPlayer = 2f;
                float outerBorderPlayer = 2.2f;

                float speedMultForPlayerDistance = MultiplierForObjectSlowDown(distanceObjPlayer.magnitude, innerBorderPlayer, outerBorderPlayer, false); //between 1 and 0

                actualSpeedVec *= speedMultForPlayerDistance;
            }
            actualSpeedVec *= MultiplierForObjectSlowDown(distanceClickedObj.magnitude, mouseSmoothingInnerBorder, mouseSmoothingOuterBorder, false);
            curr.GetComponent<Rigidbody2D>().velocity = actualSpeedVec;
        }
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
            return calcNumberInBorderSystem / distBetweenBorders;
        }
        return 1;
    }
    private Vector2 ExtendVectorViaBoundsFromObectInDirectionOfAntoherVector(Vector2 givenVector, GameObject givenObjectForBounds,Vector2 dirToExtend)
    {
        float magOfObj = givenObjectForBounds.GetComponent<Collider2D>().bounds.size.magnitude/2;
        Vector2 vecNorm = givenObjectForBounds.GetComponent<Collider2D>().bounds.size.normalized;
        Vector2 result = vecNorm * dirToExtend.normalized * magOfObj;
        result += givenVector;
        Debug.Log($"givenVec:{givenVector}|originalBounds:{givenObjectForBounds.GetComponent<Collider2D>().bounds.size.magnitude}|result:{result}");
        return result;
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
                speedVec *= 100f;
                speedVec.y *= 0.5f;
                curr.GetComponent<Rigidbody2D>().AddForce(speedVec, ForceMode2D.Impulse);
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