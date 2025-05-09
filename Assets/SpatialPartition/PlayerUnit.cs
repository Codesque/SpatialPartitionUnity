using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{

    public float Speed = 10f;

    // Update is called once per frame
    void Update()
    {


        Vector3 movementVec = new Vector3(
                
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"),
            0f
        );

        movementVec.Normalize();
        transform.position = new Vector3(

            transform.position.x + movementVec.x * Speed * Time.deltaTime,
            transform.position.y + movementVec.y * Speed * Time.deltaTime,
            transform.position.z
        );





    }
}
