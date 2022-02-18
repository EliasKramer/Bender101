using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneScript : MonoBehaviour
{
    //the strategy for undermeshing prevention (the strategy for not getting the stone stuck)
    private static ModeForStoneAntimesh _antiMeshMode = ModeForStoneAntimesh.SlowDownIfNearWall;

    //The collider of the stone
    private Collider2D _attachedCollider;

    //the last position that doesnt overlap with the platform
    private Vector2 _lastSafePosition;

    //a reference to the platform the player can move on
    private Collider2D _platform;

    //the attached rigidbody of the stone
    private Rigidbody2D _rb;

    //this is the distance that the stone needs to overlap with the
    //platform to get teleported back
    private static float _amountOfStuckness = -0.2f;

    //the critical speed is the maximum velocity that we can go without getting stuck in an object
    private float criticalSpeed = 5f; //is only a rough value -> could be higher (it is not tested)

    void Start()
    {
        _attachedCollider = GetComponent<Collider2D>();
        //the "first" last safe position should be the spawn point
        _lastSafePosition = transform.position;
        //grab the platform collider from the meta data file
        _platform = GameObject.Find("MetaData").GetComponent<MetaDataScript>().Platform;
        //getting the attached rigidboy
        _rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        //this if statement should provide a bit of performance boost
        //it should only take measurements, if the speed is high enough to get stuck in walls
        if (_rb.velocity.magnitude > criticalSpeed)
        {
            //calculates the next position of the stone and
            //gets a array of points, that define the outline of the future collider position
            Vector2[] allCurrentColliderPoints = GetRoughColliderPointsForPoint(_attachedCollider, (Vector2)transform.position + _rb.velocity * Time.deltaTime);

            //if the future collider will be overlapping with the platform, the following will happen
            if (DoesAtLeastCollideWithOnePoint(_platform, allCurrentColliderPoints))
            {
                //if this mode is enabled, the stone will slow down, if it is near a oncoming collision
                if(_antiMeshMode == ModeForStoneAntimesh.SlowDownIfNearWall)
                {
                    _rb.velocity = _rb.velocity.normalized * criticalSpeed;
                }
                //to completely prevent a overlapping of the colliders, we an
                //performance intensive collider on.
                _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
            else
            {
                //if our collider will not overlap a platform in the future,
                //the cheaper collision detection mode gets enabled
                _rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            }

            //if this mode is enabled, the stone shall be teleported to the last safe location, if it gets stuck in a wall
            if(_antiMeshMode == ModeForStoneAntimesh.TpBackIfStuckInWall)
            {
                TpBackIfStuck();
            }
        }
    }
    /// <summary>
    /// Remembers the last safe position, where the stone didnt collide with the platform. 
    /// If it does collide with the platform it will tp the stone to the last safe location
    /// </summary>
    private void TpBackIfStuck()
    {
        //creates a list of all colliders, that overlap with the attached collider 
        //could be improved with the contact filter
        List<Collider2D> overlapping = new List<Collider2D>();
        _attachedCollider.OverlapCollider((new ContactFilter2D()).NoFilter(), overlapping);

        //the distance between the attached collider and the platform collider
        float distance = Physics2D.Distance(_attachedCollider, _platform).distance;

        //it overlaps with the platform when it is sitting on it - that shall not be checked
        //it should only be teleported back, if the object overlaps too much. that is measured by the distance
        if (overlapping.Contains(_platform) && distance < _amountOfStuckness)
        {
            //we save the current velocity for later
            Vector2 velocityBefore = _rb.velocity;
            //we set the current velocity to zero. otherwise some bugs will happen. (shaking between positions)
            _rb.velocity = Vector2.zero;

            //the position is set the the last saved position (where it didnt overlap with the platform)
            transform.position = _lastSafePosition;
            //then we take the saved speed, take the direction of that and give it the critcal speed (the maximum allwed speed)
            _rb.velocity = velocityBefore.normalized * criticalSpeed;
        }
        else
        {
            //if the collider doesnt overlap with the platform a new safe spot shall be saved
            _lastSafePosition = transform.position;
        }
    }
    /// <summary>
    /// Takes the bounds size of the collider draws some points, to get a rough outline of the collider
    /// </summary>
    /// <param name="collider">the collider that shall be calculated with</param>
    /// <param name="point">the position of the collider</param>
    /// <returns>returns an array of points in world space, that define a rough outline of the given collider</returns>
    private Vector2[] GetRoughColliderPointsForPoint(Collider2D collider, Vector2 point)
    {
        Vector2[] retVal = new Vector2[9];
        Vector2 length = collider.bounds.size / 2;

        retVal[0] = point;
        retVal[1] = point + (length * Vector2.right);
        retVal[2] = point + (length * Vector2.left);
        retVal[3] = point + (length * Vector2.up);
        retVal[4] = point + (length * Vector2.down);

        retVal[5] = point + length;
        retVal[6] = point + length * -1f;
        length.x *= -1f;
        retVal[7] = point + length;
        retVal[8] = point + length * -1f;

        return retVal;
    }
    /// <summary>
    /// Goes through all points and returns if at least one of them is in the collider
    /// </summary>
    /// <param name="collider">the collider that shall be calculated with</param>
    /// <param name="points">the points that shall be checked</param>
    /// <returns>returns if at least one point is overlapping the collider</returns>
    private bool DoesAtLeastCollideWithOnePoint(Collider2D collider, Vector2[] points)
    {
        foreach (Vector2 curr in points)
        {
            if (collider.OverlapPoint(curr))
            {
                return true;
            }
        }
        return false;
    }
    private enum ModeForStoneAntimesh
    {
        TpBackIfStuckInWall,
        SlowDownIfNearWall
    }
}