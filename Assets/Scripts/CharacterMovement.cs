using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    Rigidbody2D body;
    float xVel;
    public float speed;
    public float jumpStrength;

    public Camera mainCamera;

    public GameObject holdingArm;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        xVel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vel = body.velocity;


        if(Input.GetKey(KeyCode.A)){
            xVel = -speed;
        }
        if(Input.GetKey(KeyCode.D)){
            xVel = speed;
        }
        if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)){
            xVel = 0;
        }
        
        if(Input.GetKeyDown(KeyCode.W)){
            vel.y = jumpStrength;
            body.velocity = vel;
        }

        vel.x += (xVel - vel.x) / 10f;

        //Gravity;
        //vel += Physics2D.gravity;
        body.velocity = vel;
        

        GetComponent<Animator>().SetBool("Running", Mathf.Abs(body.velocity.x) > 0.1f);
        GetComponent<SpriteRenderer>().flipX = body.velocity.x < 0;

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mouseWorldPosition - holdingArm.transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if(body.velocity.x < 0)
                angle = 180 - angle;
        

        holdingArm.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
