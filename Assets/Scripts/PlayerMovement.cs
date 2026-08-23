using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashForce = 20f;
    public float spriteScale = 1.5f;

    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackRange = 1.2f;
    public float attackCooldown = 0.4f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private bool isGrounded;
    private float facingSign = 1f;
    private bool attackHeld;
    private float attackCooldownTimer;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Dash.performed += ctx => Dash();

        // Started/canceled (not performed) so Update can keep attacking every
        // cooldown interval for as long as the button/mouse is held down.
        controls.Player.Attack.started += ctx => attackHeld = true;
        controls.Player.Attack.canceled += ctx => attackHeld = false;
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ApplyScale();
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (moveInput.x != 0f)
        {
            facingSign = Mathf.Sign(moveInput.x);
            ApplyScale();
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
            animator.SetBool("Grounded", isGrounded);
        }

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (attackHeld && attackCooldownTimer <= 0f)
        {
            PerformAttack();
        }
    }

    private void ApplyScale()
    {
        transform.localScale = new Vector3(facingSign * spriteScale, spriteScale, transform.localScale.z);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void Dash()
    {
        rb.linearVelocity = new Vector2(facingSign * dashForce, rb.linearVelocity.y);
    }

    private void PerformAttack()
    {
        attackCooldownTimer = attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Vector2 hitCenter = (Vector2)transform.position + new Vector2(facingSign * attackRange * 0.5f, 0f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitCenter, attackRange * 0.5f);

        foreach (var hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 hitCenter = (Vector2)transform.position + new Vector2(facingSign * attackRange * 0.5f, 0f);
        Gizmos.DrawWireSphere(hitCenter, attackRange * 0.5f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
