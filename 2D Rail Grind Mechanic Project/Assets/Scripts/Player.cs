using UnityEngine;

public class Player : MonoBehaviour
{
    // movement
    [Header("Movement")]

    public Rigidbody2D physics;
    private Vector2 PlayerInput;
    public float MoveSpeed;
    public float MoveSpeedMultiplier;
    public float Acceleration;
    public float Decceleration;

    [Header("Jump")]
    public bool canJump;
    public bool isJumping;
    public float JumpSpeed;

    [Header("Ground Detection")]
    //ground
    public bool grounded;
    public float linedistance;
    public LayerMask groundLayer;

    [Header("Player Direction Facing")]
    public bool facingRight;

    public void PlayerDirection()
    {
        //Flip Sprite
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void PlayerDirectionalChange()
    {
        //If moving right flip sprite to the right, if moving left flip sprite to the left
        if (physics.linearVelocity.x < -0.01f && !facingRight)
        {
            PlayerDirection();
        }

        if (physics.linearVelocity.x > 0.01f && facingRight)
        {
            PlayerDirection();
        }
    }

    public Vector2 Direction()
    {
        return PlayerInput;
    }

    public bool OnGround()
    {
        grounded = Physics2D.Raycast(transform.position, Vector2.down, linedistance, groundLayer);
        if (grounded)
        {
            canJump = true;
            isJumping = false;
        }
        else
        {
            canJump = false;
        }

        return grounded;
    }


    public void Friction()
    {
        if (PlayerInput.x.Equals(0) & grounded)
        {
            float continuedMovement = PlayerInput.x * MoveSpeed;
            if (Mathf.Abs(physics.linearVelocity.x) > 0)
            {
                continuedMovement -= 0.00001f;
                physics.linearVelocity = new Vector2(continuedMovement, physics.linearVelocity.y);
            }
        }
        else
        {
            physics.linearVelocity = new Vector2(physics.linearVelocity.x, physics.linearVelocity.y);
        }
    }

    public void MovePlayer()
    {
        PlayerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        float PlayerVelocity = PlayerInput.x * MoveSpeed;

        float SpeedDifference = PlayerVelocity - physics.linearVelocity.x;

        float PlayerAcceleration = (Mathf.Abs(PlayerVelocity) > 0.1f) ? Acceleration : Decceleration;

        float movement = Mathf.Pow(Mathf.Abs(SpeedDifference) * Acceleration, MoveSpeedMultiplier) * Mathf.Sign(SpeedDifference);

        PlayerDirectionalChange();

        physics.AddForce(movement * Vector2.right);
    }

    public void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            physics.AddForce(Vector2.up * JumpSpeed, ForceMode2D.Impulse);
            isJumping = true;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
        PlayerJump();
        OnGround();
    }

}