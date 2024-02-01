using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private float _minY;
    private float _offset;
    // Start is called before the first frame update
    void Start()
    {
        _offset = transform.position.y-target.position.y;
        _minY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (target.position.y < _minY)
        {
            _minY=target.position.y;
            transform.position=new Vector3(transform.position.x,_minY+ _offset,transform.position.z);
        }
    }
}
