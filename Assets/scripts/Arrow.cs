using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed;
    public float damage;

    public float ttl;

    float timeAlive;

    Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.drag = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += transform.forward * speed * Time.deltaTime;
        body.velocity = transform.forward * speed;


        timeAlive += Time.deltaTime;

        if (timeAlive > ttl) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy"))
        {   
            Debug.Log($"Hitting enemy for {damage} damager");
            other.GetComponent<Enemy>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
