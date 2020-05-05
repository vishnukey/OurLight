using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Camp : Singleton<Camp>
{
    public float radius;
    public float healthPerVillager;
    public float healingFactor => healthPerVillager * villagers;
    public int villagers{
        get;
        private set;
    }

    public int baseMaxFuel;
    public float burnTimeout;
    public float villagerFuelContribution;
    public int maxFuel => baseMaxFuel + Mathf.FloorToInt(villagerFuelContribution * villagers);
    public List<Light> lights;

    float timeSinceLastBurn;
    int fuel;
    List<(Light, float)> startIntensity;

    void Start() {
        fuel = maxFuel;
        startIntensity = lights.Select(light => (light, light.intensity)).ToList();
    }

    void Update() {
        timeSinceLastBurn += Time.deltaTime;

        if (timeSinceLastBurn > burnTimeout)
        {
            fuel--;
            timeSinceLastBurn = 0;
        }

        

        startIntensity.ForEach(lightItem => lightItem.Item1.intensity = Mathf.Lerp(0, lightItem.Item2, (float)fuel / maxFuel));

        if (fuel <= 0) {
            lights.ForEach(light => light.intensity = 0);
            Player.Instance.Die("You let the fire dire");
        }

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
