using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camp : Singleton<Camp>
{
    public float radius;
    public float healthPerVillager;
    public float healingFactor => healthPerVillager * villagers;
    public int villagers{
        get;
        private set;
    }

    public bool InCamp(Vector3 position)
    {
        return Vector3.SqrMagnitude(position - transform.position) <= radius * radius;
    }

    public void AddVillager()
    {
        villagers++;
    }

    public void RemoveVillager()
    {
        villagers--;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    
}
