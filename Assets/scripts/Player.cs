using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
#region public_readonly
    public int wood {
        get;
        private set;
    }
    public int arrows {
        get;
        private set;
    }
    public int torches {
        get;
        private set;
    }

    public int health{
        get;
        private set;
    }
#endregion

#region public_props
    public float speed;
    public float turnSpeed;
    public float reach;
    public Camera mainCamera;
    public int max_health;
    public LayerMask groundLayer;
    public LayerMask interactionLayer;
#endregion

#region internal
    CharacterController controller;
#endregion

#region Unity_builtins
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        health = max_health;
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
        RaycastHit groundHit;
        if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, groundLayer))
        {
            var lookPoint = new Vector3(groundHit.point.x, transform.position.y, groundHit.point.z);
            
            //transform.LookAt(lookPoint);
            transform.SmoothLookAt(lookPoint, turnSpeed);
        }

        if (Input.GetMouseButtonDown(0)){
            RaycastHit interactionHit;
            if (Physics.Raycast(ray, out interactionHit, Mathf.Infinity, interactionLayer))
            {
                var interaction = interactionHit.collider.tag;
                var position = interactionHit.collider.transform.position;
                var go = interactionHit.collider.gameObject;
                var sqrDist = Vector3.SqrMagnitude(transform.position - position);
                Debug.Log($"Interaction hit! Go: {go.name}, Tag: {interaction}, Dist: {sqrDist}, Reach: {reach}, SqrReach: {reach * reach}, CloseEnough? {sqrDist <= reach * reach}");


                if (sqrDist > reach * reach) goto escape;

                switch(interaction)
                {
                    case "Tree":
                        InteractTree(go);
                        break;
                    case "Villager":
                        InteractVillager(go);
                        break;
                    case "Campfire":
                        InteractCampfire(go);
                        break;
                    default:
                        break;
                }
            }
        }

        escape:
            return;
    }

    void InteractCampfire(GameObject fire)
    {
        if (wood >= 15){
            wood -= 15;
            torches++;
        }
    }

    void InteractTree(GameObject tree)
    {
        Destroy(tree);
        wood += 10;
    }

    void InteractVillager(GameObject villager)
    {
        if (wood >= 1){
            wood--;
            arrows++;
        }
    }
#endregion
}
