using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Chunk : MonoBehaviour
{
    
    public List<SpawnData> spawnData;
    public float spawnDensity;
    [Range(0, 1)] public float spawnChance;

    float planeCorrectFactor => 5f;

    Vector3 topRight => 
        transform.position + 
        transform.localScale.z * planeCorrectFactor * transform.forward + 
        transform.localScale.x * planeCorrectFactor * transform.right;
    Vector3 bottomLeft => 
        transform.position - 
        transform.localScale.z * planeCorrectFactor * transform.forward - 
        transform.localScale.x * planeCorrectFactor * transform.right;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"{gameObject.name} Scale: {transform.localScale}, Top Right: {topRight}, Bottom Left: {bottomLeft}");
        GenerateItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateItems()
    {
        //var max = spawnData.Aggregate(0f, (acc, data) => acc + data.weight);
        var data = new List<SpawnData>();
        var max = 0f;
        //var min = spawnData[0].weight;
        foreach (var d in spawnData){
            max += d.weight;
            data.Add(new SpawnData{
                item = d.item,
                weight = max,
            });
        }
        for (float x = bottomLeft.x; x < topRight.x; x += spawnDensity){
            for (float z = bottomLeft.z; z < topRight.z; z += spawnDensity){
                var pos = new Vector3(x, 0, z);
                var willSpawn = Random.Range(0, 1f) < spawnChance;
                if (willSpawn)
                {
                    var which = Random.Range(0, max);
                    var item = data.Where(d => d.weight >= which).First();
                    var randomXZ = Random.insideUnitCircle * (spawnDensity / 2);
                    var spawnPos = 
                        new Vector3(x, 0, z) +
                        new Vector3(spawnDensity / 2, 0, spawnDensity / 2) +
                        new Vector3(randomXZ.x, 0, randomXZ.y);
                    var g = Instantiate(item.item, spawnPos, Quaternion.identity);
                    g.transform.parent = transform;

                }
            }
        }
    }

    [System.Serializable]
    public struct SpawnData{
        public float weight;
        public GameObject item;
    }
}
