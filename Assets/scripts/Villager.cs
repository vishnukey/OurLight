using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Villager : MonoBehaviour
{
    public enum State {Wandering, Camp, Following}
    public float max_health;
    public new GameObject light;
    public float visibility;

    public State state {
        get;
        private set;
    } = State.Wandering;


    float health;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        health = max_health;
        agent = GetComponent<NavMeshAgent>();
        //agent.enabled = false;
        if (Camp.Instance.InCamp(transform.position))
        {
            state = State.Camp;
            light.SetActive(false);
            Camp.Instance.AddVillager();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(state){
            case State.Wandering:
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, visibility, Vector3.up);
                var containsPlayer = hits.Where(hit => hit.collider.CompareTag("Player")).ToArray().Length > 0;
                if (containsPlayer) state = State.Following;
                break;
            case State.Following:
                //agent.enabled = true;
                agent.SetDestination(Player.Instance.transform.position);
                if (Camp.Instance.InCamp(transform.position)){
                    state = State.Camp;
                    var toPos = Random.insideUnitCircle * (Camp.Instance.radius - 1);
                    agent.SetDestination(new Vector3(toPos.x, 0, toPos.y));
                    Camp.Instance.AddVillager();
                }
                break;
            case State.Camp:
                light.SetActive(false);
                //agent.enabled = false;
                break;
        }

        if (health <= 0) Destroy(gameObject);
        
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, visibility);
    }

    public void TakeDamage(float amnt){
        health -= amnt;
    }

    void OnDestroy()
    {
        if (Camp.Instance != null && state == State.Camp) Camp.Instance.RemoveVillager();
    }
}
