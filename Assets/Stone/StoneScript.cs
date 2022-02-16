using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneScript : MonoBehaviour
{
    //The collider of the stone
    private Collider2D _attachedCollider;
    //the last position that doesnt overlap with the platform
    private Vector2 _lastSafePosition;
    private float _maxVelocity = 2f;
    //a reference to the platform the player can move on
    private Collider2D _platform;
    //the contact filter. it should only find the platform, so that could be improved
    private static ContactFilter2D _filter = new ContactFilter2D();
    //the attached rigidbody of the stone
    private Rigidbody2D _rb;

    private Vector2 _lastPos;
    private Vector2 _predictedPos;
    private Vector2 _lastRememberedPos;
    private float criticalSpeed = 5f;
    void Start()
    {
        _attachedCollider = GetComponent<Collider2D>();
        _lastSafePosition = transform.position;
        _platform = GameObject.Find("MetaData").GetComponent<MetaDataScript>().Platform;
        _filter.NoFilter();
        _rb = GetComponent<Rigidbody2D>();
        _lastPos = transform.position;
        _predictedPos = transform.position;
        _predictedPos = transform.position;
    }
    void FixedUpdate()
    {
        Vector2 currPos = transform.position;

        //Debug.DrawLine(_lastRememberedPos, _predictedPos, Color.blue, 5f);


        _lastRememberedPos = _predictedPos;
        _predictedPos = currPos + _rb.velocity * Time.deltaTime;
        _lastPos = currPos;

        float radiusCircle = 0.2f;
        Collider2D[] overlappingCollidersCheckBefore =  Physics2D.OverlapCircleAll(_predictedPos, radiusCircle);

        bool doesContainPlatform = false;
        foreach(Collider2D curr in overlappingCollidersCheckBefore)
        {
            if(curr == _platform)
            {
                doesContainPlatform = true;
            }
        }

        bool didBugBack = false;


        if (_rb.velocity.magnitude > criticalSpeed)
        {
            List<Collider2D> overlapping = new List<Collider2D>();
            _attachedCollider.OverlapCollider(_filter, overlapping);

            float distance = Physics2D.Distance(_attachedCollider, _platform).distance;

            if (overlapping.Contains(_platform) && distance < -0.1f)
            {

                Vector2 velocityBefore = _rb.velocity;
                _rb.velocity = Vector2.zero;


                transform.position = _lastSafePosition;
                _rb.velocity = velocityBefore.normalized * criticalSpeed;
                didBugBack = true;
            }
            else
            {
                _lastSafePosition = transform.position;
            }
        }
        if (doesContainPlatform && didBugBack)
        {
            Debug.Log($"Next pos will be in wall - circle prediction with {radiusCircle}");
            Debug.DrawLine(_predictedPos, (_predictedPos - currPos).normalized * radiusCircle, Color.magenta, 5f); // prototype. fuck you and see you tomorrow
        }

        if (_platform.OverlapPoint(_predictedPos) && didBugBack)
        {
            Debug.Log("Next pos will be in wall - point prediction");
        }
    }
}