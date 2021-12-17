using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bendingScript : MonoBehaviour
{
    // Start is called before the first frame update
    protected List<GameObject> currCollisions;
    public Collider2D coll;
    private System.DateTime _lastTimeStompAttack;
    private float _stompAttackMinDelayInMs = 500f;
    private float _forceForStone = 1000f;
    void Start()
    {
        currCollisions = new List<GameObject>();
        coll = GetComponent<CircleCollider2D>();
        _lastTimeStompAttack = DateTime.UtcNow;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("not main count = " + currCollisions.Count);
        if (Input.GetKeyDown("space") && delayIsEnough())
        {
            foreach(GameObject curr in currCollisions)
            {
                curr.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, _forceForStone));
                Debug.Log($"curr:{curr.gameObject.name}");
            }
        }
    }

    private bool delayIsEnough()
    {
        System.DateTime now = DateTime.UtcNow;
        if((now-_lastTimeStompAttack).TotalMilliseconds >= _stompAttackMinDelayInMs)
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
