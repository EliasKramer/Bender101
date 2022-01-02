using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class bendingScript : MonoBehaviour
{
    private List<GameObject> currActionFieldCollisions;
    private List<GameObject> actualPlayerCollisions;

    private System.DateTime _lastTimeStompAttack;
    private float _stompAttackMinDelayInMs = 500f;
    private bool _stompAttackDelayIsEnough = true;
    private float _forceForStone = 6f;
    //private Vector2 _vectorForStone = new Vector2(0, _forceForStone);

    private System.DateTime _lastTimePushAttack;
    private float _pushAttackMinDelay = 500f;
    private bool _pushAttackDelayIsEnough = true;
    private Vector2 _pushAttackForce = new Vector2(0, 1500f);

    private bool _jumpButtonIsDown = false;
    private bool _mouseDown = false;
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
        actualPlayerCollisions = new List<GameObject> ();
    }
    // Update is called once per frame
    void Update()
    {
        CheckDelays();
        //Debug.Log("not main count = " + currCollisions.Count);
        if (_jumpButtonIsDown && _stompAttackDelayIsEnough)
        {
            foreach (GameObject curr in currActionFieldCollisions)
            {
                curr.GetComponent<Rigidbody2D>().velocity = new Vector2(0, _forceForStone);
            }
            _stompAttackDelayIsEnough = false;
        }

        if (!_pushAttackActive && _pullAttackActive)
        {
            _worldPointWhereClicked = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            if (!_altMode1Active)
            {
                foreach (GameObject curr in currActionFieldCollisions)
                {
                    Vector2 distanceVec = ((Vector2)this.transform.position - (Vector2)curr.transform.position);
                    //Debug.Log($"distancevec: {distanceVec.magnitude}");
                    Vector2 actualSpeedVec = distanceVec.normalized;
                    Vector2 boundsSize = curr.gameObject.GetComponent<Collider2D>().bounds.size;
                    float distance = boundsSize.x;
                    if (distance < boundsSize.y)
                    {
                        distance = boundsSize.y;
                    }
                    distance *= 0.8f;
                    if (distance < 2f)
                    {
                        distance = 2f;
                    }
                    Debug.Log($"Boundssize {boundsSize}||distance {distance}");
                    actualSpeedVec *= 2f * (distanceVec.magnitude - distance);
                    //Debug.Log($"forcevec = {distanceVec}");
                    curr.GetComponent<Rigidbody2D>().velocity = actualSpeedVec;
                }
            }
            else
            {
                foreach (GameObject curr in currActionFieldCollisions)
                {
                    Debug.Log(curr.name);
                    if (!actualPlayerCollisions.Contains(curr))
                    {
                        Vector2 distanceVec = (_worldPointWhereClicked - (Vector2)curr.transform.position);

                        //Debug.Log($"distancevec: {distanceVec.magnitude}");
                        Vector2 actualSpeedVec = distanceVec.normalized;
                        actualSpeedVec *= 2f * (distanceVec.magnitude - 0.4f);
                        //Debug.Log($"forcevec = {distanceVec}");
                        curr.GetComponent<Rigidbody2D>().velocity = actualSpeedVec;
                    }
                }
            }
        }

        //Instantiate(this._actionField, posForActionField, Quaternion.identity);


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
    }
    public void JumpButtonAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jumpButtonIsDown = true;
        }
        if (context.canceled)
        {
            _jumpButtonIsDown = false;
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
        if(collision.collider.tag == "Stone")
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
    public void PushAttack(InputAction.CallbackContext context)
    {
        if (context.performed) //taste unten
        {
            _pushAttackActive = true;
        }
        if (context.canceled) // taste wieder oben
        {
            _pushAttackActive = false;
            _worldPointWhereClicked = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if (_pushAttackDelayIsEnough && !_pullAttackActive)
            {
                foreach (GameObject curr in currActionFieldCollisions)
                {
                    if (!_altMode1Active)
                    {
                        Vector2 speedVec = (_worldPointWhereClicked - (Vector2)this.transform.position).normalized;
                        speedVec *= 10f;
                        speedVec.y *= 0.5f;
                        Debug.Log($"forcevec = {speedVec}");
                        curr.GetComponent<Rigidbody2D>().velocity = speedVec;
                    }
                    else
                    {
                        Vector2 speedVec = (_worldPointWhereClicked - (Vector2)curr.transform.position).normalized;
                        speedVec *= 10f;
                        speedVec.y *= 0.5f;
                        Debug.Log($"forcevec = {speedVec}");
                        curr.GetComponent<Rigidbody2D>().velocity = speedVec;
                    }
                }
                _pushAttackDelayIsEnough = false;
            }

            //Instantiate(this._actionField, posForActionField, Quaternion.identity);
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
    /*
    private void FireLaser()
    {
        if (!_fireLaser || Time.time - this._lastBulletFired < FIRE_DELAY)
        {
            return;
        }
        var spawnPos = this.transform.position + new Vector3(0, 0.7f);
        Instantiate(this._laserPrefab, spawnPos, Quaternion.identity);
        this._lastBulletFired = Time.time;
    }


    public void FireLaser(InputAction.CallbackContext context)
    {
        if (context.performed) //taste unten
        {
            _fireLaser = true;
        }
        if (context.canceled) // taste wieder oben
        {
            _fireLaser = false;
        }
    }
    */

}
