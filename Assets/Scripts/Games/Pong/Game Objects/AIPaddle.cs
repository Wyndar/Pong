using UnityEngine;
public class AIPaddle : Paddle
{
    public float AIMoveThreshold;
    public Ball ball;
    public float chargeAmount = 1f;
    private float time;
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
        time += Time.deltaTime;
        if (time >= 2)
        {
            powerBar.PowerPercentChange(chargeAmount, true);
            time = 0;
        }
    }
}
