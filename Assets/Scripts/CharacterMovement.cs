using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    Rigidbody2D body;
    float xVel;
    public float speed;

    public int jumps = 1;

    int numJumps;

    public float footDistance = 0;

    public float jumpStrength;

    public Camera mainCamera;

    RaycastHit2D hit;
    Animator animator;

    public ParticleSystem smoke;

    //public GameObject holdingArm;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        xVel = 0;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector2 vel = body.velocity;

        hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), Mathf.Infinity, ~(1 << 10));

        if(Input.GetKey(KeyCode.A)){
            xVel = -speed;
        }
        if(Input.GetKey(KeyCode.D)){
            xVel = speed;
        }
        if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)){
            xVel = 0;
        }
        
        vel.x += (xVel - vel.x) / 10f;
        body.velocity = vel;
        
        if(hit.distance < footDistance){
            animator.SetFloat("LandingTimer", animator.GetFloat("LandingTimer") + Time.deltaTime * 2f);
            numJumps = jumps;
        }
        else{
            animator.SetFloat("LandingTimer", 0);
        }

        animator.SetBool("Ground", hit.distance <= footDistance);
        animator.SetFloat("Speed", Mathf.Abs(body.velocity.x) / speed);
        animator.SetFloat("vSpeed", (-body.velocity.y + 1)/2f / 10f);
        GetComponent<SpriteRenderer>().flipX = body.velocity.x < 0;

        if(Input.GetKeyDown(KeyCode.W) && numJumps > 0){
            Debug.Log("Jump!");
            if(hit.distance <= footDistance){
                animator.SetTrigger("Jump");
                animator.ResetTrigger("DoubleJump");
            }
            else{
                animator.ResetTrigger("Jump");
                animator.SetTrigger("DoubleJump");
            }
        }
        
    }

    void OnDrawGizmosSelected(){

        Gizmos.color = Color.red;

        hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), Mathf.Infinity, ~(1 << 10));

        Gizmos.DrawLine(transform.position, hit.point);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + new Vector3(-.5f, -footDistance, 0), transform.position + new Vector3(.5f, -footDistance, 0));
        
    }

    public void Jump(){
        Vector2 vel = body.velocity;
        vel.y = jumpStrength;
        body.velocity = vel;
        numJumps --;
        if(hit.distance < footDistance)
            smoke.Play();
    }
}
