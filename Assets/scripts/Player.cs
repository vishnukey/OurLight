using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : Singleton<Player>
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

    public float health{
        get;
        private set;
    }

    public int arrowCapacity => arrowBaseCapacity + Mathf.FloorToInt(Camp.Instance.villagers * villagerArrowContribution);
#endregion

#region public_props
    public float speed;
    public float turnSpeed;
    public float reach;
    public Camera mainCamera;
    public float max_health;
    public LayerMask groundLayer;
    public LayerMask interactionLayer;

    public GameObject torchPrefab;
    public GameObject arrowPrefab;

    public List<GameObject> lights;

    public int arrowBaseCapacity;
    public float villagerArrowContribution;

    public event System.Action<string> OnDeath;
#endregion

#region internal
    CharacterController controller;
#endregion

#region Unity_builtins
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        health = 2 * (max_health / 3);

        torches = 3;
        wood = 15;
        arrows = 5;
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
        if (Camp.Instance.InCamp(transform.position)) {
            lights.ForEach(light => light.SetActive(false));
            if (health < max_health){
                health = Mathf.Min(health + Camp.Instance.healingFactor * Time.deltaTime, max_health);
            }
        }
        else lights.ForEach(light => light.SetActive(false));
        
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

        if (Input.GetKeyDown("e") && torches > 0)
        {
            torches--;
            Instantiate(torchPrefab, transform.position - transform.forward, Quaternion.identity);
        }

        if (Input.GetMouseButtonDown(1) && arrows > 0)
        {   
            arrows--;
            Instantiate(arrowPrefab, transform.position + transform.forward * 2, transform.rotation);
        }

        if (health <= 0) Die("You succumbed to your wounds");

        escape:
            return;
    }
#endregion

#region Internal_Methods
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
        if (arrows < arrowCapacity && wood >= 1 && Camp.Instance.InCamp(transform.position)){
            wood--;
            arrows++;
        }
    }

    public void Die(string reason)
    {
        OnDeath?.Invoke(reason);
        Time.timeScale = 0;
        Debug.Log($"You died because {reason}");
    }
#endregion

#region Public_Methods
    public void TakeDamage(float amnt)
    {
        health = Mathf.Max(health - amnt, 0);
    }
#endregion
}
