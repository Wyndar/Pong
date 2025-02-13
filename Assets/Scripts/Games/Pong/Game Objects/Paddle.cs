using UnityEngine;
using Unity.Netcode;

public class Paddle : MonoBehaviour
{
    public PongManager PongManager;
    public Vector2 resetPosition;
    public float paddleSpeed { get; private set; }
    public Rigidbody2D rb;
    public Vector2 startPos;
    public Vector2 endPos;
    public float startTime;
    public float endTime;
    public bool allowMovement;
    public bool hasGameStarted;
    public PowerBar powerBar;

    public void Awake()
    {
        PongManager = FindFirstObjectByType<PongManager>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void ResetPosition()
    {
        rb.linearVelocity = Vector2.zero;
        rb.position = resetPosition;
        GetComponent<RectTransform>().localScale = new(1, 0.125f);
        paddleSpeed = 5;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ball"))
            return;
        powerBar.PowerPercentChange(5, true);
    }
    public void ScaleSize(float scale) => GetComponent<RectTransform>().localScale = new(scale, 0.125f);
    public void ChangeSpeed(float speed) => paddleSpeed = speed;
    //bloody americans 
    public void SetColor(Color color) => GetComponent<SpriteRenderer>().color = color;
}
