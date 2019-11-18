using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StainDissipator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z - Time.deltaTime * 0.03f
        );

        if(transform.position.z <= 0)
            Destroy(gameObject);
        

    }
}
