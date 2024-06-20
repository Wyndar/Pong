using UnityEngine;
public class AIPaddle : Paddle
{
    public float AIMoveThreshold;
    public Ball ball;
    new private void Awake()
    {
        base.Awake();
        ball = FindObjectOfType<Ball>();
    }
    private void Update()
    {
        if (ball.transform.position.y > AIMoveThreshold)
            rb.velocity = new(paddleSpeed * ball.transform.position.x, 0f);
        else
            rb.velocity = Vector2.zero;    
    }
}
