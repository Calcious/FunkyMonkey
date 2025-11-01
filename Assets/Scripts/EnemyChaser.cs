using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("Chase Settings")]
    public float chaseSpeed = 2f;
    public float stopDistance = 1f;

    [Header("Detection")]
    public float detectionRange = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public bool canJump = true;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private bool isGrounded;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the Player has the 'Player' tag.");
        }

        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (playerTransform == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange && distanceToPlayer > stopDistance)
        {
            ChasePlayer();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        if (canJump && isGrounded && playerTransform.position.y > transform.position.y + 1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
