using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideColliderScript : MonoBehaviour
{
    [SerializeField]
    Const.CollisionSide _side;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Obstacle" || (_side == Const.CollisionSide.Down && collision.tag == "Stone"))
        {
            GetComponentInParent<playerScript>().CollisionAddByChildCollider(true,_side);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle" || (_side == Const.CollisionSide.Down && collision.tag == "Stone"))
        {
            GetComponentInParent<playerScript>().CollisionRemByChildCollider(false, _side);
        }
    }
}
