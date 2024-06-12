using UnityEngine;
using Unity.Netcode;

public class Paddle : NetworkBehaviour
{
    public Vector2 resetPosition;
    public GameManager GameManager;
    public float paddleSpeed;
    public Rigidbody2D rb;
    public Vector2 startPos;
    public Vector2 endPos;
    public float startTime;
    public float endTime;
    public bool allowMovement;
    
    public void Awake()
    {
        GameManager = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        rb.position = resetPosition;
    }
}
