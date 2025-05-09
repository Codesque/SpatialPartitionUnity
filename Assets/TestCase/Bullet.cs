using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public Vector3 Direction;
    public float Lifetime = 0f;
    public float Speed = 10f;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) {

            for (int i = 0; i < 8; i++) { 
            

                Bullet newBull = Instantiate(this , transform.position , Quaternion.identity);
                newBull.Lifetime =  this.Lifetime + 0.05f; 
                newBull.Speed = Speed;
                newBull.Direction = new Vector3(Mathf.Sin(45 * i), Mathf.Cos(45 * i), 0f); 
                Destroy(collision.gameObject);

            
            
            }


        }
    }




    // Update is called once per frame
    void Update()
    {
        transform.position += Direction * Speed * Time.deltaTime;
        Lifetime += Time.deltaTime; 
        if(Lifetime > 1f && this != null) Destroy(this.gameObject);
    }
}
