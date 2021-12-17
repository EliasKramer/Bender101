using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stompRadiusCollScript : bendingScript
{
    private GameObject parent;
    protected override void Start()
    {
        base.Start();
    }
    private void Update()
    {
        Debug.Log("mainobject count = " + currCollisions.Count);
    }
    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D givenCollider)
    {
        if (givenCollider.tag == "Stone")
        {
            Debug.Log($"collision with: {givenCollider.name}");
            currCollisions.Add(givenCollider.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D givenCollider)
    {
        if (givenCollider.tag == "Stone")
        {
            Debug.Log($"collision exit with: {givenCollider.name}");

            currCollisions.Remove(givenCollider.gameObject);
        }
    }
}
/*Now i got another problem. I think it does add the object to a false list.<br>
i did this in the parent script:
```
void Start()
    {
        construc();
    }
    protected void construc()
    {
        currCollisions = new List<GameObject>();
        coll = GetComponent<CircleCollider2D>();
        _lastTimeStompAttack = DateTime.UtcNow;
    }
```
and this in the child script:
`

`    void Start()
    {
        parent = base.gameObject;
        construc();
    }
`*/