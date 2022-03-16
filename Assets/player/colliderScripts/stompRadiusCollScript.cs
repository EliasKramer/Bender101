using UnityEngine;

public class stompRadiusCollScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D givenCollider)
    {
        if (givenCollider.tag == "Stone")
        {
            GetComponentInParent<BendingScript>().addCollidedStone(givenCollider.gameObject);
        }
        if(givenCollider.tag == "ActionField")
        {
            Debug.Log("Entered field of action");
        }
    }
    private void OnTriggerExit2D(Collider2D givenCollider)
    {
        if (givenCollider.tag == "Stone")
        {
            GetComponentInParent<BendingScript>().remCollidedStone(givenCollider.gameObject);
        }
    }
}