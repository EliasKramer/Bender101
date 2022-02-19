using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneScript : MonoBehaviour
{
    //the attached rigidbody of the stone
    private Rigidbody2D _rb;

    //at that velocity or higher, it can happen, that the stone gets stuck in a wall or in another stone
    private float criticalSpeed = 5f; //is only a rough value -> could be higher (it is not tested)

    void Start()
    {
        //getting the attached rigidbody 
        _rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        //if the velocity gets over the critical speed it should enable the more performance intensive mode,
        //that doesnt let any stone get stuck in walls or stuck in other stones
        if (_rb.velocity.magnitude > criticalSpeed)
        {
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        else
        {
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        }
    }
}