using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneScript : MonoBehaviour
{
    private Collider2D _attachedCollider;
    private Vector2 _lastSafePosition;
    private Collider2D _platform;
    private static ContactFilter2D _filter = new ContactFilter2D();
    private static float _velocityForAntiMesh = 4f;
    private Rigidbody2D _rb;
    private bool _temp;
    void Start()
    {
        _attachedCollider = GetComponent<Collider2D>();
        _lastSafePosition = transform.position;
        _platform = GameObject.Find("MetaData").GetComponent<MetaDataScript>().Platform;
        _filter.NoFilter();
        _rb = GetComponent<Rigidbody2D>();
        _temp = false;
    }
    void FixedUpdate()
    {
        List<Collider2D> overlapping = new List<Collider2D>();
        int sizeOfList = _attachedCollider.OverlapCollider(_filter, overlapping);
        foreach (Collider2D c in overlapping)
        {
            //Debug.Log($"in {this.name} : {c.name} : {c} : {_platform}");
        }
        float distance = Physics2D.Distance(_attachedCollider, _platform).distance;
        //Debug.Log(distance);
        if (overlapping.Contains(_platform) && distance < -0.1f)// && _temp == false)//_platform.OverlapPoint(transform.position))
        {
            //Debug.Log($"is overlapping lsp{_lastSafePosition}");
            _rb.velocity = Vector2.zero;
            _rb.simulated = false;

            transform.position = _lastSafePosition;
            Debug.Log($"setpos:{_lastSafePosition}");

            _rb.simulated = true;
            /*Vector2 vectorForGettingBackToSafePosition = ((Vector2)transform.position - _lastSafePosition)
                *-1f;
            if(vectorForGettingBackToSafePosition.magnitude > _velocityForAntiMesh)
            {
                vectorForGettingBackToSafePosition = vectorForGettingBackToSafePosition.normalized * _velocityForAntiMesh;
            }
            Debug.DrawLine(transform.position,
                (Vector2)transform.position + vectorForGettingBackToSafePosition,
                Color.magenta,
                0.1f);
            transform.Translate(vectorForGettingBackToSafePosition);*/
        }
        else
        {
            //Debug.Log($"not overlapping lsp{_lastSafePosition}");
            _lastSafePosition = transform.position;
            //_temp = false;
        }
    }
}
