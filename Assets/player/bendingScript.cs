using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class bendingScript : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> currCollisions;
    public Collider2D coll;
    private System.DateTime _lastTimeStompAttack;
    private float _stompAttackMinDelayInMs = 500f;
    private float _forceForStone = 1000f;
    private bool _jumpButtonIsDown = false;
    void Start()
    {
        currCollisions = new List<GameObject>();
        coll = GetComponent<CircleCollider2D>();
        _lastTimeStompAttack = DateTime.UtcNow;
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("not main count = " + currCollisions.Count);
        if (_jumpButtonIsDown && DelayIsEnough())
        {
            foreach (GameObject curr in currCollisions)
            {
                curr.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, _forceForStone));
                //Debug.Log($"curr:{curr.gameObject.name}");
            }
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
    public void addCollidedStone(GameObject gObj)
    {
        currCollisions.Add(gObj);
    }
    public void remCollidedStone(GameObject gObj)
    {
        currCollisions.Remove(gObj);
    }
}
