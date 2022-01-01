using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class bendingScript : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> currCollisions;
    private Collider2D coll;

    private System.DateTime _lastTimeStompAttack;
    private float _stompAttackMinDelayInMs = 500f;
    private bool _stompAttackDelayIsEnough = true;
    private float _forceForStone = 1500f;
    //private Vector2 _vectorForStone = new Vector2(0, _forceForStone);

    private System.DateTime _lastTimePushAttack;
    private float _pushAttackMinDelay = 500f;
    private bool _pushAttackDelayIsEnough = true;
    private Vector2 _pushAttackForce = new Vector2(0, 1500f);

    private bool _jumpButtonIsDown = false;
    private bool _mouseDown = false;
    private Vector2 _worldPointWhereClicked;
    [SerializeField]
    private GameObject _actionField;
    public Camera _cam;
    void Start()
    {
        currCollisions = new List<GameObject>();
        coll = GetComponent<CircleCollider2D>();
        _lastTimeStompAttack = DateTime.UtcNow;
    }
    // Update is called once per frame
    void Update()
    {
        CheckDelays();
        //Debug.Log("not main count = " + currCollisions.Count);
        if (_jumpButtonIsDown && _stompAttackDelayIsEnough)
        {
            foreach (GameObject curr in currCollisions)
            {
                curr.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,_forceForStone));
            }
            _stompAttackDelayIsEnough=false;
        }
        if(_mouseDown && _pushAttackDelayIsEnough)
        {
            foreach (GameObject curr in currCollisions)
            {
                Vector2 forceVector = (_worldPointWhereClicked - (Vector2)this.transform.position).normalized;
                forceVector *= 1500f;
                Debug.Log($"forcevec = {forceVector}");
                curr.GetComponent<Rigidbody2D>().AddForce(forceVector);
            }
            _pushAttackDelayIsEnough=false;
        }

    }
    private void CheckDelays()
    {
        System.DateTime now = DateTime.UtcNow;
        if(!_stompAttackDelayIsEnough && ((now- _lastTimeStompAttack).TotalMilliseconds >= _stompAttackMinDelayInMs))
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
    private bool DelayIsEnough()
    {
        System.DateTime now = DateTime.UtcNow;
        if ((now - _lastTimeStompAttack).TotalMilliseconds >= _stompAttackMinDelayInMs)
        {
            _lastTimeStompAttack = now;
            return true;
        }
        else
        {
            return false;
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
        currCollisions.Add(gObj);
    }
    public void remCollidedStone(GameObject gObj)
    {
        currCollisions.Remove(gObj);
    }
    public void MouseDown(InputAction.CallbackContext context)
    {



        if (context.performed) //taste unten
        {
            _mouseDown = true;
        }
        if (context.canceled) // taste wieder oben
        {
            _mouseDown = false;
            _worldPointWhereClicked = _cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());


            //Instantiate(this._actionField, posForActionField, Quaternion.identity);
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
