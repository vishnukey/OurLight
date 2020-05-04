using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float speed;
    public Camera mainCamera;

    CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        var direction = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ).normalized;

        controller.Move(direction * speed * Time.deltaTime);
        
        //Debug.Log(Input.mousePosition);
        //transform.LookAt(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y));
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;
        if (Physics.Raycast(ray, out hitinfo))
        {
            var lookPoint = new Vector3(hitinfo.point.x, transform.position.y, hitinfo.point.z);
            
            transform.LookAt(lookPoint);
        }
        
    }
}
