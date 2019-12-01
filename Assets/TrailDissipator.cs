using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailDissipator : MonoBehaviour
{

    public float life = 1;
    float maxLife = 0;
    // Start is called before the first frame update
    void Start()
    {
        maxLife = life;
    }

    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;
        if(life < 0){
            Destroy(gameObject);
        }

        GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, 1, life / maxLife);
    }
}
