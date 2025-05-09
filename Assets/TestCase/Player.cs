using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float Speed = 10f;
    public Bullet bulletPrefab;
    Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputVec = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f); 
        Vector3 movementVec = inputVec.normalized * Speed * Time.deltaTime;
        transform.position += movementVec;


        if (Input.GetMouseButtonDown(0)) { 
            
            
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            Bullet newBull = Instantiate(bulletPrefab, transform.position, Quaternion.identity); 
            newBull.Direction = (pos - newBull.transform.position).normalized; 




        }






    }
}
