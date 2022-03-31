using System.Collections.Generic;
using UnityEngine;
using Utilities2D;
using System.Threading;
using System;

public class StoneScript : MonoBehaviour
{
    //the attached rigidbody of the stone
    private Rigidbody2D _rb;

    [SerializeField]
    private PhysicsMaterial2D _defaultMaterial;
    [SerializeField]
    private PhysicsMaterial2D _noFrictionMaterial;

    private bool _noFrictionNextUpdate = false;
    private DateTime _timeToUpdateFrictionToNormalAgain = DateTime.MaxValue;
    private bool _noRotationNextUpdate = false;
    private DateTime _timeToUpdateRotationToNormalAgain = DateTime.MaxValue;


    //at that velocity or higher, it can happen, that the stone gets stuck in a wall or in another stone
    private float criticalSpeed = 5f; //is only a rough value -> could be higher (it is not tested)

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

    }
    void FixedUpdate()
    {
        ManageHighVelocity();
        ManageRigidbodyOptions();
    }

    private void ManageRigidbodyOptions()
    {
        if (_noFrictionNextUpdate)
        {
            _rb.sharedMaterial = _noFrictionMaterial;
            _noFrictionNextUpdate = false;
        }
        if (_noRotationNextUpdate)
        {
            _rb.freezeRotation = true;
            _noRotationNextUpdate = false;
        }

        DateTime now = DateTime.UtcNow;

        if( _timeToUpdateRotationToNormalAgain < now)
        {
            _rb.freezeRotation = false;
            _timeToUpdateRotationToNormalAgain = DateTime.MaxValue;
            //Debug.Log($"{name} normal Rotation {_timeToUpdateFrictionToNormalAgain.ToString("mm:ss: ffff")}");
        }
        if ( _timeToUpdateFrictionToNormalAgain < now)
        {
            _rb.sharedMaterial = _defaultMaterial;
            _timeToUpdateFrictionToNormalAgain = DateTime.MaxValue;
            //Debug.Log($"{name} normal Friction {_timeToUpdateFrictionToNormalAgain.ToString("mm:ss: ffff")}");
        }
    }

    private void ManageHighVelocity()
    {
        //if the velocity gets over the critical speed it should enable the more performance intensive mode,
        //that doesnt let any stone get stuck in walls or stuck in other stones
        _rb.collisionDetectionMode = (_rb.velocity.magnitude > criticalSpeed) ?
            CollisionDetectionMode2D.Continuous :
            CollisionDetectionMode2D.Discrete;
    }

    public void SetNoFriction(int timeInMs)
    {
        _timeToUpdateFrictionToNormalAgain = DateTime.UtcNow;
        _timeToUpdateFrictionToNormalAgain = _timeToUpdateFrictionToNormalAgain.AddMilliseconds(timeInMs);
        _noFrictionNextUpdate = true;
    }

    public void FreezeRotation(int timeInMs)
    {
        _timeToUpdateRotationToNormalAgain = DateTime.UtcNow;
        _timeToUpdateRotationToNormalAgain = _timeToUpdateRotationToNormalAgain.AddMilliseconds(timeInMs);
        _noRotationNextUpdate = true;
    }
}