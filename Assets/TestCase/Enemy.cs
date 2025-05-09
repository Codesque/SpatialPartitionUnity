using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    Player player;
    public float Speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindFirstObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position , player.transform.position , Speed * Time.deltaTime);
    }
}
