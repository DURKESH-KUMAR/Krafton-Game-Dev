using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float Jumpforce,MoveForce;
    private bool fallingDown;
    float xinp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        xinp = Input.acceleration.x;
    }
    private void FixedUpdate()
    {
        if (rb.linearVelocityY <= 0)
        {
            fallingDown = true;
        }
        rb.AddForce(new Vector2(xinp, 0)*MoveForce*Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform")&&fallingDown)   // the force should be applied only when the ball is falling and touching the platform
        {
            rb.AddForce(Vector2.up * Jumpforce, ForceMode2D.Impulse);
            fallingDown = false;
        }
    }
}
