using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Enemy : MonoBehaviour
{
    public enum State {Attacking, Searching}

    public List<Renderer> eyes;
    public float max_health;
    public float health {
        get;
        private set;
    }
    public float strength;
    public float searchRadius;
    public float reach;
    public float attackCooldown;
    public float searchCooldown;

    NavMeshAgent agent;
    List<Material> materials;
    State state = State.Searching;
    Color darkColour = Color.black;
    Color brightColor;
    float timeSinceLastAttack = 0;
    float timeSinceLastSearch = 0;

    Transform target = null;


    const string COLOUR_FIELD = "_EmissionColor";
    

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        materials = eyes.Select(eye => eye.material).ToList();
        brightColor = materials[0].GetColor("_EmissionColor");
        health = max_health;
    }

    // Update is called once per frame
    void Update()
    {
        if(!target) target = null;

        if (timeSinceLastSearch > searchCooldown){
            var hits = Physics.SphereCastAll(transform.position, searchRadius, Vector3.up);
            var potentialTarget = hits
                .Where(hit => hit.collider.CompareTag("Villager"))
                .OrderBy(hit => Vector3.SqrMagnitude(hit.collider.transform.position - transform.position))
                .ToArray();
            if (potentialTarget.Count() > 0) target = potentialTarget[0].collider.transform;
            timeSinceLastSearch = 0;
        }
        timeSinceLastSearch += Time.deltaTime;

        switch(state){
            case State.Searching:
                materials.ForEach(mat => mat.SetColor(COLOUR_FIELD, darkColour));
                if (Vector3.SqrMagnitude(Player.Instance.transform.position - transform.position) <= searchRadius * searchRadius) state = State.Attacking;
                break;
            case State.Attacking:
                materials.ForEach(mat => mat.SetColor(COLOUR_FIELD, brightColor));
                if (target == null){
                    agent.SetDestination(Player.Instance.transform.position);
                    timeSinceLastAttack += Time.deltaTime;
                    var distToPlayer = Vector3.SqrMagnitude(Player.Instance.transform.position - transform.position);
                    if (distToPlayer <= reach * reach && timeSinceLastAttack >= attackCooldown) Attack(null); 
                    if (distToPlayer > searchRadius * searchRadius) state = State.Searching;
                }else {
                    agent.SetDestination(target.position);
                    timeSinceLastAttack += Time.deltaTime;
                    var distToPlayer = Vector3.SqrMagnitude(target.position - transform.position);
                    if (distToPlayer <= reach * reach && timeSinceLastAttack >= attackCooldown) Attack(target); 
                    if (distToPlayer > searchRadius * searchRadius) state = State.Searching;
                }
                break;
        }

        if (health <= 0) {
            Debug.Log($"Dying with {health} left");
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, reach);
    }

    void Attack(Transform target)
    {
        if (target == null){
            Player.Instance.TakeDamage(strength);
        }else{
            var villager = target.GetComponent<Villager>();
            villager?.TakeDamage(strength);
        }

        timeSinceLastAttack = 0;
    }

    public void TakeDamage(float amnt){
        Debug.Log($"Taking {amnt} damage");
        health -= amnt;
    }
}
