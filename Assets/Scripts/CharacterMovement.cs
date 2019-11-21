using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    Rigidbody2D body;
    float xVel;
    public float speed;

    public int jumps = 1;

    public float dashForce = 5;

    int numJumps;

    public float footDistance = 0;
    public float footStart = 0;

    float fallingTime = 0;
    public float jumpStrength;

    public Camera mainCamera;

    RaycastHit2D hit;
    Animator animator;

    public ParticleSystem smoke;

    bool dashed;

    bool inDash = false;

    bool flipX = false;

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

        bool wasInGround = OnGround();
        hit = Physics2D.Raycast(transform.position - new Vector3(0, footStart), new Vector2(0, -1), Mathf.Infinity, ~(1 << 10));

        if(!wasInGround && OnGround()){
            Debug.Log("Touched gound after falling for " + fallingTime + " seconds");
            StartCoroutine(TouchGround(fallingTime));

            flipX = body.velocity.x < 0;
        }

        if(Input.GetKeyDown(KeyCode.A)){
            flipX = true;
        }
        if(Input.GetKeyDown(KeyCode.D)){
            flipX = false;
        }

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
        if(!inDash)
            body.velocity = vel;
        
        if(OnGround()){
            animator.SetFloat("LandingTimer", animator.GetFloat("LandingTimer") + Time.deltaTime * 2f);
            numJumps = jumps;
            dashed = false;

            if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f) {
                 body.velocity = new Vector2(body.velocity.x - (hit.normal.x * .5f), body.velocity.y);
            }

            if ( xVel == 0) {
                body.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
            else{
                body.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        else{
            animator.SetFloat("LandingTimer", 0);
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
        }


        animator.SetBool("Ground", OnGround());
        animator.SetFloat("Speed", Mathf.Abs(body.velocity.x) / speed);
        animator.SetFloat("vSpeed", (-body.velocity.y + 1)/2f / 10f);
        GetComponent<SpriteRenderer>().flipX = flipX;

        if(!OnGround() && body.velocity.y < 0){
            fallingTime += Time.deltaTime;
        }
        else{
            fallingTime = 0;
        }

        if(!OnGround() && Input.GetMouseButtonDown(1) && !dashed){
            animator.SetTrigger("Dash");
            dashed = true;
        }


        if(Input.GetKeyDown(KeyCode.W) && numJumps > 0){
            animator.ResetTrigger("TouchGroundStrong");
            animator.ResetTrigger("TouchGroundWeak");
            if(OnGround()){
                animator.SetTrigger("Jump");
                animator.ResetTrigger("DoubleJump");
            }
            else{
                animator.ResetTrigger("Jump");
                animator.SetTrigger("DoubleJump");
            }
        }
    }

    public Vector2 Rotate(Vector2 v, float degrees) {
         float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
         float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
         float tx = v.x;
         float ty = v.y;
         v.x = (cos * tx) - (sin * ty);
         v.y = (sin * tx) + (cos * ty);
         return v;
     }

    public void Dash(){

        Vector2 dash = new Vector2(dashForce, 0);
        Vector2 mouseDir = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        mouseDir.Normalize();
        float deg = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
        dash = Rotate(dash, deg);
        body.velocity = dash;

        flipX = body.velocity.x < 0;

        if(!flipX){
            transform.rotation = Quaternion.Euler(0, 0, deg);
        }
        else{
            transform.rotation = Quaternion.Euler(0, 0, deg + 180);
        }

        inDash = true;
        body.gravityScale = 0f;
    }

    public void EndDash(){
        body.velocity = new Vector2(0, 0);
        inDash = false;
        body.gravityScale = 1f;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

   

    IEnumerator TouchGround(float fallingTime){
        yield return new WaitForSeconds(0.1f);
        if(fallingTime > 0.25f){
            animator.SetTrigger("TouchGroundStrong");
        }
        else{
            animator.SetTrigger("TouchGroundWeak");
        }
    }

    bool OnGround(){
        return hit.distance < footDistance;
    }

    void OnDrawGizmosSelected(){

        Gizmos.color = Color.red;

        hit = Physics2D.Raycast(transform.position - new Vector3(0, footStart), new Vector2(0, -1), Mathf.Infinity, ~(1 << 10));

        Gizmos.DrawLine(transform.position - new Vector3(0, footStart), hit.point);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + new Vector3(-.5f, -footDistance, 0), transform.position + new Vector3(.5f, -footDistance, 0));
        
    }

    public void Jump(){
        Vector2 vel = body.velocity;
        vel.y = jumpStrength;
        body.velocity = vel;
        numJumps --;
        if(OnGround())
            smoke.Play();
    }
}
