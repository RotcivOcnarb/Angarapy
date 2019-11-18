using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MetaballCollision : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject stainPrefab;
    public RandomSplashPlayer splash;

    int resilience = 0;
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
        if(collision.collider.gameObject.tag == "Walls"){
            if(resilience > 0){
                resilience --;                
                if(Mathf.Approximately(collision.GetContact(0).normal.y, 1)){
                    DestroyAndStain(collision.relativeVelocity, collision.GetContact(0).normal);
                    return;
                }
                GetComponent<Rigidbody2D>().velocity = collision.relativeVelocity;
                return;
            }
            DestroyAndStain(collision.relativeVelocity, collision.GetContact(0).normal);
        }
    }

    void DestroyAndStain(Vector2 direction, Vector2 normal){
        GameObject stain = Instantiate(stainPrefab, gameObject.transform.position, stainPrefab.transform.rotation);
        stain.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        stain.transform.position = new Vector3(
            stain.transform.position.x,
            stain.transform.position.y,
            stain.transform.position.z * 1.5f
        );

        splash.PlayRandomSplash(transform.position.z * 20);

        if(transform.position.z > 0.02f){
            GameObject newMeta = Instantiate(gameObject);
            newMeta.transform.position = new Vector3(
                newMeta.transform.position.x,
                newMeta.transform.position.y,
                newMeta.transform.position.z / 4f
            );

            newMeta.GetComponent<MetaballCollision>().resilience = 1;
            Vector2 reflection = direction - 2 * Vector2.Dot(direction, normal) * normal;

            newMeta.GetComponent<Rigidbody2D>().velocity = reflection * 0.1f;

        }

        Destroy(gameObject);    
    }
}
