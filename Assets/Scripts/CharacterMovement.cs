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
    float dashTimer = 0;
    bool flipX = false;

    Vector3 initialScale;

    public bool grabbing = false;

    float smallerXDistanceMetaball;
    GameObject smallerXDistanceObj;

    GameObject[] metaballs;

    //public GameObject holdingArm;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        xVel = 0;
        animator = GetComponent<Animator>();
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

        metaballs = GameObject.FindGameObjectsWithTag("Stain");
        smallerXDistanceObj = null;
        smallerXDistanceMetaball = 100000;
        foreach(GameObject mb in metaballs){
            float dx = Mathf.Abs(transform.position.x - mb.transform.position.x);
            float dy = Mathf.Abs(transform.position.y - mb.transform.position.y);

            if(dy < 0.5f && dx < smallerXDistanceMetaball){
                smallerXDistanceMetaball = dx;
                smallerXDistanceObj = mb;
            }
        }
        
        if(!grabbing){
            Vector3 scl = transform.localScale;
            scl += (initialScale - scl) / 5f;
            transform.localScale = scl;

            Vector2 vel = body.velocity;
            bool wasInGround = OnGround();
            hit = Physics2D.Raycast(transform.position - new Vector3(0, footStart), new Vector2(0, -1), Mathf.Infinity, ~(1 << 10));

            if(!wasInGround && OnGround()){
                StartCoroutine(TouchGround(fallingTime));
                flipX = body.velocity.x < 0;
            }

            if(Input.GetKey(KeyCode.A) && !flipX && !inDash){
                flipX = true;
            }
            if(Input.GetKey(KeyCode.D) && flipX && !inDash){
                flipX = false;
            }

            if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) && !inDash){
                flipX = body.velocity.x < 0;
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

            vel.x += (xVel - vel.x) / 4f;
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
                    //body.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                }
                else{
                    //body.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
            else{
                animator.SetFloat("LandingTimer", 0);
                body.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

        }
        else{

            if(!Input.GetKey(KeyCode.Space) || smallerXDistanceObj == null || smallerXDistanceMetaball > 0.5f){
                grabbing = false;
                body.constraints = RigidbodyConstraints2D.FreezeRotation;
                body.gravityScale = 1;
                dashed = false;
                numJumps = jumps+1;
                Jump();
            }
            else{
                Vector2 newVel = new Vector2(0, 0);
                if(Input.GetKey(KeyCode.W)){
                    newVel.y = speed;
                }
                else if(Input.GetKey(KeyCode.S)){
                    newVel.y = -speed;
                }
                else{
                    newVel.y = 0;
                }

                body.velocity = newVel;
                animator.SetFloat("SpeedYGrab", Mathf.Abs(body.velocity.y) / speed);
            }
        }

        Debug.Log("Obj: " + smallerXDistanceObj);
        Debug.Log("Distance: " + smallerXDistanceMetaball);

        animator.SetBool("Ground", OnGround());
        animator.SetFloat("SpeedX", Mathf.Abs(body.velocity.x) / speed);
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

        if(!grabbing){
            if(Input.GetKeyDown(KeyCode.W) && numJumps > 0){
                

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

        animator.SetBool("Grabbing", grabbing);

        dashTimer -= Time.deltaTime;
        if(dashTimer < 0 && inDash){
            EndDash();
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
        dashTimer = 0.5f;
        Vector2 dash = new Vector2(dashForce, 0);
        Vector2 mouseDir = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        mouseDir.Normalize();
        float deg = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
        dash = Rotate(dash, deg);
        body.velocity = dash;
        Vector3 scl = transform.localScale;
        scl.y *= 0.7f;
        transform.localScale = scl;
        flipX = body.velocity.x < 0;

        if(!flipX){
            transform.rotation = Quaternion.Euler(0, 0, deg);
        }
        else{
            transform.rotation = Quaternion.Euler(0, 0, deg + 180);
        }

        inDash = true;
        body.gravityScale = 0f;
        GetComponent<KaduTrail>().speed = 1;
        GetComponent<KaduTrail>().timer = 1f;
    }

    public void EndDash(){
        body.velocity = new Vector2(0, 0);
        inDash = false;
        body.gravityScale = 1f;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        GetComponent<KaduTrail>().speed = 0;
        GetComponent<KaduTrail>().timer = 1f;
    }

    IEnumerator TouchGround(float fallingTime){
        yield return new WaitForSeconds(0.1f);
        Vector3 scl = transform.localScale;
        scl.x *= 1.3f;
        transform.localScale = scl;
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
        if(OnGround()){
            smoke.Play();
            Vector3 scl = transform.localScale;
            scl.y *= 1.3f;
            transform.localScale = scl;
        }
    }
    void OnCollisionStay2D(Collision2D collision){
        if(collision.collider.tag == "Walls" && !grabbing){ //Agarra numa parede
            if(Mathf.Abs(collision.contacts[0].normal.x) > 0.9f && Mathf.Abs(collision.contacts[0].normal.y) < 0.1f
                && Input.GetKey(KeyCode.Space)){
                if(smallerXDistanceObj != null && smallerXDistanceMetaball < 0.5f){
                    grabbing = true;
                    body.gravityScale = 0;
                    dashed = false;
                    numJumps = jumps;
                    body.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
                }
            }
        }
    }
}
