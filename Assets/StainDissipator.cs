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

        transform.localScale = new Vector3(
            transform.localScale.x - Time.deltaTime,
            transform.localScale.y - Time.deltaTime,
            transform.localScale.z - Time.deltaTime
        );

        if(transform.localScale.x <= 0)
            Destroy(gameObject);
    }
}
