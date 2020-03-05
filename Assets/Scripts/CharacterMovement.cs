using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    /* Components */
    Rigidbody2D body;
    Animator animator;

    /* External Objects */
    public Camera mainCamera;
    public ParticleSystem smoke;
    public ParticleSystem walkSmoke;
    public RandomSplashPlayer walkSfx;

    /* Movement */
        /* Properties */
        public float speed;
        public int jumps = 1;
        public float dashForce = 5;
        public float footDistance = 0;
        public float jumpStrength;
        /* Locals */
        public float lastWPress = 1000;
        float xVel;
        public int numJumps;
        float fallingTime = 0;
        bool dashed;
        bool inDash = false;
        float dashTimer = 0;
        bool flipX = false;
        float slidingTimer = 0;
        bool sliding;
        float slidingCooldown = 0;
        [HideInInspector]
        public bool grabbing = false;

    RaycastHit2D hit;
    float smallerXDistanceMetaball;
    GameObject smallerXDistanceObj;
    GameObject[] metaballs;
    float walkSfxTimer = 0;
    bool rtDown = false;
    bool dashJoystick;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        xVel = 0;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame

    private void Update()
    {
        walkSfxTimer += Time.deltaTime;
        lastWPress += Time.deltaTime;

        if (!OnGround() && body.velocity.y < 0) {
            fallingTime += Time.deltaTime;
        }
        else {
            fallingTime = 0;
        }

        slidingTimer -= Time.deltaTime;
        slidingCooldown -= Time.deltaTime;

        dashTimer -= Time.deltaTime;
        if (dashTimer < 0 && inDash) {
            EndDash();
        }
        if (Input.GetButtonDown("Jump")) {
            lastWPress = 0;
        }
    }

    void FixedUpdate()
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

        float triggerRT = Input.GetAxisRaw("DashJoy");

        if (!grabbing){
            Vector2 vel = body.velocity;
            bool wasInGround = OnGround();
            hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), Mathf.Infinity, groundLayerMask);

            if(!wasInGround && OnGround()){
                flipX = body.velocity.x < 0;
            }

            float rawX = Input.GetAxisRaw("Horizontal");

            //Direção e Flip de Sprite
            if (rawX < -.12 && !flipX && !inDash && !sliding) {
                flipX = true;
            }
            if(rawX > .12 && flipX && !inDash && !sliding) {
                flipX = false;
            }

            //Movimentação Física
            if (Mathf.Abs(rawX) > .1) {
                xVel = speed * rawX;
                if (!walkSmoke.isPlaying && OnGround())
                    walkSmoke.Play();
            }

            if (Mathf.Abs(rawX) < .1) 
                xVel = 0;

            if(xVel == 0 || !OnGround())
                walkSmoke.Stop();

            vel.x += (xVel - vel.x) / 4f;
            if(!inDash && !sliding)
                body.velocity = vel;
            
            if(OnGround()){
                animator.SetFloat("LandingTimer", animator.GetFloat("LandingTimer") + Time.deltaTime * 2f);
                numJumps = jumps;
                dashed = false;

                if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f) {
                    body.velocity = new Vector2(body.velocity.x - (hit.normal.x * .5f), body.velocity.y);
                }

                if ( xVel == 0) {// Paradinha q evita o player de "deslizar" pelas plataformas com angulo não reto
                    // Porém dá um problema na animação q o player fica flicando
                    //ainda não descobri um jeito de arrumar isso

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

            if(OnGround() && !sliding && !inDash && xVel != 0) {
                if(walkSfxTimer < 0) {
                    walkSfxTimer = 0.5f;
                    walkSfx.PlayRandomSplash(1f);
                }
            }

        }
        else{

            transform.rotation = Quaternion.identity;

            if(triggerRT < 0.1f || smallerXDistanceObj == null || smallerXDistanceMetaball > 0.5f){
                grabbing = false;
                body.constraints = RigidbodyConstraints2D.FreezeRotation;
                body.gravityScale = 1;
                dashed = false;
                numJumps = jumps;
                Jump();
            }
            else{
                Vector2 newVel = new Vector2(0, 0);
                if(Input.GetAxisRaw("Vertical") > 0.1) {
                    newVel.y = speed;
                }
                else if(Input.GetAxisRaw("Vertical") < -0.1) {
                    newVel.y = -speed;
                }
                else{
                    newVel.y = 0;
                }

                body.velocity = newVel;
                animator.SetFloat("SpeedYGrab", Mathf.Abs(body.velocity.y) / speed);
            }
        }

        animator.SetBool("Ground", OnGround());
        animator.SetFloat("SpeedX", Mathf.Abs(body.velocity.x) / speed);
        animator.SetFloat("vSpeed", (-body.velocity.y + 1)/2f / 10f);
        GetComponent<SpriteRenderer>().flipX = flipX;

        

        if(triggerRT > 0.5f) {
            if (!rtDown) {
                rtDown = true;
                //aqui evento de Down
                if (!OnGround() && !dashed) {
                    animator.SetTrigger("Dash");
                    dashed = true;
                    dashJoystick = true;
                }
            }
        }
        else {
            rtDown = false;
        }

        if (!OnGround() && Input.GetButtonDown("DashKey") && !dashed) {
            animator.SetTrigger("Dash");
            dashed = true;
            dashJoystick = false;
        }

        //if (OnGround() && !grabbing && Input.GetMouseButtonDown(1) && !sliding && slidingCooldown < 0) {
        //    slidingTimer = .7f;
        //    Vector2 dash = new Vector2(dashForce * (flipX ? -1 : 1), 0);
        //    sliding = true;
        //    body.velocity = dash;
        //    //body.gravityScale = 0f;
        //}

        if (slidingTimer <= 0 && sliding) {
            body.velocity = new Vector2(0, body.velocity.y);
            sliding = false;
            body.gravityScale = 1f;
            slidingCooldown = 0.5f;
        }

        animator.SetBool("Sliding", sliding);

        if(!grabbing){
            if(lastWPress < 0.1f && numJumps > 0){ //PULO
                if(OnGround()){
                    dashed = false;
                    animator.SetTrigger("Jump");
                    animator.ResetTrigger("DoubleJump");
                }
                else{
                    animator.ResetTrigger("Jump");
                    animator.SetTrigger("DoubleJump");
                }
                Jump();
                lastWPress = 10000;
            }
        }
        animator.SetBool("Grabbing", grabbing);
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
        Debug.Log("Dashed with joystick? " + dashJoystick);
        if (dashJoystick) {
            mouseDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        mouseDir.Normalize();

        float deg = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
        dash = Rotate(dash, deg);
        body.velocity = dash;
        body.drag = 5f;
        //Vector3 scl = initialScale;
        //scl.y *= 0.7f;
        //transform.localScale = scl;
        flipX = body.velocity.x < 0;

        if(!flipX){
            transform.rotation = Quaternion.Euler(0, 0, deg);
        }
        else{
            transform.rotation = Quaternion.Euler(0, 0, deg + 180);
        }

        inDash = true;
        body.gravityScale = 0f;
        GetComponent<TrailRenderer>().emitting = true;
    }

    public void EndDash(){
        if (inDash) {
            body.velocity = new Vector2(0, 0);
            inDash = false;
            body.drag = 0f;
            body.gravityScale = 1f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            GetComponent<TrailRenderer>().emitting = false;
        }
    }

    public bool OnGround(){
        return hit.distance < footDistance;
    }

    public LayerMask groundLayerMask;

    void OnDrawGizmosSelected(){

        Gizmos.color = Color.red;

        hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), Mathf.Infinity, groundLayerMask);

        Gizmos.DrawLine(transform.position, hit.point);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + new Vector3(-.5f, -footDistance, 0), transform.position + new Vector3(.5f, -footDistance, 0));
        
    }

    public void Jump(){
        Vector2 vel = body.velocity;
        vel.y = jumpStrength;
        body.velocity = vel;
        numJumps --;
        smoke.Play();
        slidingTimer = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    void OnCollisionStay2D(Collision2D collision){
        float triggerRT = Input.GetAxisRaw("DashJoy");
        if (collision.collider.tag == "Walls" && !grabbing){ //Agarra numa parede
            if(Mathf.Abs(collision.contacts[0].normal.x) > 0.9f && Mathf.Abs(collision.contacts[0].normal.y) < 0.1f
                && (Input.GetMouseButton(1) || triggerRT > 0.5f)){
                if(smallerXDistanceObj != null && smallerXDistanceMetaball < 0.5f){
                    grabbing = true;
                    body.gravityScale = 0;
                    dashed = false;
                    numJumps = jumps;
                    body.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
                }
            }
            if (inDash && !grabbing) {
                animator.SetTrigger("DashInterruption");
            }
            EndDash();
        }
    }
}
