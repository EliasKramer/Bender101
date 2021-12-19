using UnityEngine;

public class stompRadiusCollScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D givenCollider)
    {
        if (givenCollider.tag == "Stone")
        {
            GetComponentInParent<bendingScript>().addCollidedStone(givenCollider.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D givenCollider)
    {
        if (givenCollider.tag == "Stone")
        {
            GetComponentInParent<bendingScript>().remCollidedStone(givenCollider.gameObject);
        }
    }
}