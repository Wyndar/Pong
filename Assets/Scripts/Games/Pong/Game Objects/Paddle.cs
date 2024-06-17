using UnityEngine;
using Unity.Netcode;

public class Paddle : NetworkBehaviour
{
    public PongManager PongManager;
    public Vector2 resetPosition;
    public float paddleSpeed;
    public Rigidbody2D rb;
    public Vector2 startPos;
    public Vector2 endPos;
    public float startTime;
    public float endTime;
    public bool allowMovement;
    
    public void Awake()
    {
        PongManager = FindObjectOfType<PongManager>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        rb.position = resetPosition;
    }
    public void DisableScript()
    {
        if (!IsOwner && PongManager.gameType == GameType.VSOnline)
        {
            enabled = false;
            return;
        }
    }
    //bloody americans 
    public void SetColor(Color color) => GetComponent<SpriteRenderer>().color = color;
}
