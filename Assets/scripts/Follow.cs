using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public Transform target;
    public float speed;

    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, target.position+offset, Time.deltaTime * speed);
    }
}
