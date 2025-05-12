using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class PlayerRailGrind : MonoBehaviour
{
    [Header("Rail Detection")]
    //Check for layer
    private bool OnRail;
    public LayerMask railLayer;

    private SplineContainer railSpline;
    private Collider2D railCollider;

    public float lineOffset, lineDistance, railOffset;
    private Vector2 contactPoint;
    private RaycastHit2D hit;

    //For player rotation and movement
    public float GrindSpeed;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private Quaternion originalRotation;
    private Player player;

    public ParticleSystem railSparksParticle;
    private void Start()
    {
        railSpline = null;
        railCollider = null;
        rb = this.GetComponent<Rigidbody2D>();
        player = this.GetComponent<Player>();
        originalRotation = transform.rotation;
    }

    public bool CheckForRail()
    {
        /*Create a RaycastHit from the player that constantly checks if there is a rail layer mask beneath it. If the gameObject we hit has a collider (hit.collider != null). 
          Then the railCollider variable is going to equal the current gameObject's collider that was hit and set OnRail to TRUE, and make thr rigidbody gravity 0 so that the player is not being drag down.
          At the point of contact the players position will be set to the point of contact with an offset on the Y value that can be manipulated in the inspector*/

        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + lineOffset), Vector2.down * lineDistance, Color.cyan);
        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + lineOffset), Vector2.down, lineDistance, railLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + lineOffset), Vector2.right, lineDistance, railLayer);

        if (hit.collider != null)
        {
            railCollider = hit.collider;
            OnRail = true;
            rb.gravityScale = 0f;
            player.JumpSpeed = 10f;
            contactPoint = new Vector2(hit.point.x, hit.point.y + railOffset);
            transform.position = contactPoint;
            PlayerRailMovement();
        }
        else
        {
            /*Set all variables to their original values*/
            player.JumpSpeed = 7f;
            railCollider = null;
            OnRail = false;
            contactPoint = transform.position;
            rb.gravityScale = 1f;
            transform.rotation = originalRotation;
        }
        //Will activate if on rail, deactivate when off rail. Also apply rail particle localscale to the players to change direction its emitting out.
        railSparksParticle.gameObject.SetActive(OnRail);
        railSparksParticle.gameObject.transform.localScale = this.transform.localScale;
        return OnRail;
    }

    private void PlayerRailMovement()
    {
        //While the player is facing right the speed will be positive (+1), if facing left the speed will be negative (-1)
        while (transform.localScale.x == 1)
        {
            rb.linearVelocity = new Vector2(player.MoveSpeed, rb.linearVelocityY);
            break;
        }
        while(transform.localScale.x == -1)
        {
            rb.linearVelocity = new Vector2(-player.MoveSpeed, rb.linearVelocityY);
            break;
        }

        /*While the player is facing right the speed will be positive (+1), if facing left the speed will be negative (-1),
         calculate the move direction for the player and set the angle to rotate along the normal of the rail as the player is moving!*/
        movementDirection = Vector3.Cross(hit.normal, new Vector3(0, 0, 1));
        float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    private void FixedUpdate()
    {
        CheckForRail();
    }

}
