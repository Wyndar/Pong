using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class AIPaddle : MonoBehaviour
{
    public float paddleSpeed;
    public float AIMoveThreshold;
    private Ball ball;
    private Rigidbody2D rb;
    private Vector3 resetPosition;
    private void OnEnable()
    {
        ball = FindObjectOfType<Ball>();
        rb = GetComponent<Rigidbody2D>();
        resetPosition = rb.position;
    }

    private void Update()
    {
        if (ball.transform.position.y > AIMoveThreshold)
            rb.velocity = new(paddleSpeed * ball.transform.position.x, 0f);
        else
            rb.velocity = Vector2.zero;    
    }

    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        rb.position = resetPosition;
    }
}
