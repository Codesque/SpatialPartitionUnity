using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{


    public Enemy enemyPrefab;
    public int numOfEnemies = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }


    public Vector3 GetRandomLoc() => new Vector3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-20f, 20f), 0f);


    IEnumerator SpawnCoroutine() {

        while (true) {

            Instantiate(enemyPrefab, GetRandomLoc(), Quaternion.identity);
            numOfEnemies++;
            yield return new WaitForSeconds(0.01f);

        
        
        }
    
    
    
    }
}
