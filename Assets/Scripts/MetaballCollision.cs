using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MetaballCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CircleCollider2D coll = GetComponent<CircleCollider2D>();
        coll.radius = Mathf.Sqrt(transform.position.z) * 0.046f;
    }

    void OnCollisionEnter2D(Collision2D collision){
        Debug.Log(collision.collider.gameObject.tag);
        if(collision.collider.gameObject.tag == "Walls"){
            Destroy(gameObject);
        }
    }
}
