using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMetaballSingle : MonoBehaviour
{

    public GameObject stainPrefab;
    public RandomSplashPlayer splash;

    int resilience = 0;
    float timer = -5;


    // Start is called before the first frame update
    void Start()
    {
        splash = GameObject.Find("SplashAudio").GetComponent<RandomSplashPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 0){
            Destroy(gameObject);
        }
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
        splash.PlayRandomSplash(transform.localScale.x / 5f);

        if(transform.localScale.x > 1f){
            GameObject newMeta = Instantiate(gameObject);
            newMeta.transform.localScale = new Vector3(newMeta.transform.localScale.x * 0.25f, newMeta.transform.localScale.y * 0.25f, newMeta.transform.localScale.z);

            newMeta.GetComponent<NewMetaballSingle>().resilience = 1;
            Vector2 reflection = direction - 2 * Vector2.Dot(direction, normal) * normal;
            newMeta.GetComponent<Rigidbody2D>().velocity = reflection * 0.1f;

        }

        Destroy(gameObject);    
    }
}
