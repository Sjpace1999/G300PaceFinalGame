using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float runningSpeed;
    public float jumpSpeed;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    private Vector2 lastPosition = new Vector2(0, -1.78f);
    public Transform background;
    private bool canJumpAgain = true;
    public AudioSource hit;
    public float deathPause=0;
    public float instructionsTimer;
    public TextMeshProUGUI instructions;
    public CanvasGroup cg;
    private bool endGame = false;
    // Start is called before the first frame update

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        hit = GetComponent<AudioSource>();
    }

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (endGame)
        {
            cg.alpha += Time.deltaTime / 5;
            if(cg.alpha == 1)
            {
                SceneManager.LoadScene("Level");
            }
        }

        if (instructionsTimer > 5f)
        {
            instructions.enabled = false;
        }
        else
        {
            instructionsTimer += Time.deltaTime;
        }
        background.position = new Vector2(body.transform.position.x, body.transform.position.y + 1f);


        if (deathPause > .5f)
        {
            if (isGrounded())
            {
                canJumpAgain = true;
                //lastPosition = new Vector2(body.transform.position.x, body.transform.position.y);
            }
            if (onWall())
            {
                canJumpAgain = true;
            }

            horizontalInput = Input.GetAxis("Horizontal");


            if (horizontalInput > 0)
            {
                transform.localScale = Vector3.one;
            }
            else if (horizontalInput < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }


            anim.SetBool("run", horizontalInput != 0);
            anim.SetBool("grounded", isGrounded());



            if (wallJumpCooldown > .2f)
            {
                body.velocity = new Vector2(horizontalInput * runningSpeed, body.velocity.y);

                if (onWall() && !isGrounded() && horizontalInput != 0)
                {
                    body.gravityScale = 1;
                    body.velocity = new Vector2(0, -1);
                }
                else
                {
                    body.gravityScale = 7f;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump();
                }
            }
            else
            {
                wallJumpCooldown += Time.deltaTime;
            }
        }
        else
        {
            anim.SetBool("run", false);
            anim.SetBool("grounded", true);
            deathPause += Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (isGrounded()) {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
            anim.SetTrigger("jump");
            
        }
        if (canJumpAgain && !isGrounded() && !onWall())
        {
            canJumpAgain = false;
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            /*if (horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 5, -3);
                transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) *3, 15);
            }
            */
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 15);

            wallJumpCooldown = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == 10)
        {
            deathPause = 0;
            hit.PlayOneShot(hit.clip);
            body.transform.position = lastPosition;
            body.velocity = new Vector2(0, 0);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="Respawn")
        {
            deathPause = 0;
            /*while (deathPause < 5f)
            {
                deathPause += Time.deltaTime;
            }*/
            hit.PlayOneShot(hit.clip);
            body.transform.position = lastPosition;
            body.velocity = new Vector2(0, 0);
        }
        else if(collision.tag == "Checkpoint")
        {
            lastPosition = new Vector2(collision.transform.position.x, collision.transform.position.y+.9f);
        }
        else if (collision.tag == "Finish")
        {
            endGame = true;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center,boxCollider.bounds.size,0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
        
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}
