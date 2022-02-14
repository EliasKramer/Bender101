using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MetaDataScript : MonoBehaviour
{
    private Collider2D _platform;
    void Awake()
    {
        _platform = GameObject.FindGameObjectWithTag("Obstacle").GetComponent<CompositeCollider2D>();
        if(_platform == null)
        {
            Debug.LogError("Could not find the platform in the Scene");
        }
    }
    public Collider2D Platform
    {
        get
        {
            return _platform;
        }
    }
}
