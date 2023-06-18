using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    public float speed = 10f; //player movement speed
    public float jumpingPower = 16f; //player jump power/height
    private bool isFacingRight = true;
    private float forwardMomentum=0f;

    private bool isWallSliding; //check if player is clinging to wall
    public float wallSlidingSpeed=1f; //speed of wall slide down

    private bool isWallJumping;
    private float wallJumpingDirection;
    public float wallJumpingTime = 0.2f; //time for which player jumps of a wall
    private float wallJumpingCounter;
    public float wallJumpingDuration = 0.4f; //time after which player stops being in walljump mode
    private Vector2 wallJumpingPower = new Vector2(8f, 16f); //wall jumping power


    public float coyoteTime = 0.2f; //coyote time so that player has some leeway to jump after walking off a ledge 
    private float coyoteTimeCounter;


    private bool doubleJump;

    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 50f; //dash power
    public float dashingTime = 0.2f; //time player spends dashing
    public float dashingCooldown = 0.25f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private TrailRenderer tr;

    private void Update()
    {
        if (isDashing)
        {
            return;
        }
        
        horizontal = Input.GetAxisRaw("Horizontal");

        if (IsGrounded() && !Input.GetButton("Jump") &&!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            doubleJump = false;
            coyoteTimeCounter = coyoteTime; //coyote
        }
        else//coyote
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") )
        {
            
            if (IsGrounded() || coyoteTimeCounter >0f || doubleJump) //replaced is grounded for coyote
            {
                //Debug.Log(forwardMomentum);
                rb.velocity = new Vector2(/*rb.velocity.x +*/ horizontal * speed, jumpingPower);
                Debug.Log(rb.velocity.x);
                doubleJump = !doubleJump;
            }
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        WallSlide();
        WallJump();
        

        if (!isWallJumping)
        {
            Flip();
        }
    }



    private bool IsGrounded() //CHECK IF player is grounded
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer);
    }

    private void Flip() //decides the direction of the player
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash() //dash coroutine to determine how long player dashes
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private bool IsWalled() //isplayer clinging to a wall
    {
        return Physics2D.OverlapCircle(wallCheck.position, 1f, wallLayer);
    }

    private void WallSlide() //isplayer sliding down a wall
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }


    private void WallJump() //player jumping off a wall logic 
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping() //wall jump time over
    {
        isWallJumping = false;
    }
}