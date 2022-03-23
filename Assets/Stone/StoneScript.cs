using System.Collections.Generic;
using UnityEngine;
using Utilities2D;
using System.Threading;
public class StoneScript : MonoBehaviour
{
    //the attached rigidbody of the stone
    private Rigidbody2D _rb;

    [SerializeField]
    private PhysicsMaterial2D _defaultMaterial;
    [SerializeField]
    private PhysicsMaterial2D _noFrictionMaterial;

    private bool _noFrictionNextUpdate = false;

    //at that velocity or higher, it can happen, that the stone gets stuck in a wall or in another stone
    private float criticalSpeed = 5f; //is only a rough value -> could be higher (it is not tested)
    private Delay frictionToNormalDelay = new Delay(1, false);
    //private bool shallExplode = false;
    //private float explosionSpeed = 7f;
    //private Delay explosionDelay;
    void Start()
    {
        //explosionDelay = new Delay(1f,false);
        //getting the attached rigidbody 
        _rb = GetComponent<Rigidbody2D>();
        /*
        this.gameObject.AddComponent<ColliderLineRenderer2D>();
        
        ColliderLineRenderer2D renderer = gameObject.GetComponent<ColliderLineRenderer2D>();
        renderer.color = Color.red;
        renderer.customColor = true;
        renderer.color = Color.yellow;
        renderer.lineWidth = 0.1f;
        renderer.drawEdgeCollider = true;*
        PhysicsShapeGroup2D shapeGroup = new PhysicsShapeGroup2D();
        int amount = GetComponent<PolygonCollider2D>().GetShapes(shapeGroup);

        string temp = "";
        List<Vector2> vertecies = new List<Vector2>();
        List<PhysicsShape2D> shapes = new List<PhysicsShape2D>();
        shapeGroup.GetShapeData(shapes, vertecies);
        foreach (Vector2 curr in vertecies)
        {
            
            temp +=  curr+ ", ";
        }

        Debug.Log($"{amount}stk shapegroup:\n{temp}");*/
    }
    void FixedUpdate()
    {
        //if the velocity gets over the critical speed it should enable the more performance intensive mode,
        //that doesnt let any stone get stuck in walls or stuck in other stones



        _rb.collisionDetectionMode = (_rb.velocity.magnitude > criticalSpeed) ?
            CollisionDetectionMode2D.Continuous :
            CollisionDetectionMode2D.Discrete;


        if (_noFrictionNextUpdate)
        {
            _rb.sharedMaterial = _noFrictionMaterial;
            _noFrictionNextUpdate = false;
        }
        /*if (frictionToNormalDelay.ActionDurationInMs > 5000)
        {
            _rb.sharedMaterial = _defaultMaterial;
        }*/



        //ExplodeIfTooFast();

    }
    /*private void ExplodeIfTooFast()
    {
        if (shallExplode)// && explosionDelay.ActionDurationInMs > Time.deltaTime)
        {
            shallExplode = false;
            Vector2D thisPos = new Vector2D(this.transform.position);
            Slicer2D.Slicing.ExplodeByPointAll(thisPos);
            explosionDelay.StopAction();
        }
    }
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle" && _rb != null && _rb.velocity.magnitude > criticalSpeed)
        {
            shallExplode = true;
            explosionDelay.StartAction();
        }
    }*/
    public void SetNoFriction()
    {
        _noFrictionNextUpdate = true;
        frictionToNormalDelay.StartAction();
    }
}