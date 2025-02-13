using UnityEngine;
public class AIPaddle : Paddle
{
    [SerializeField] private float AIMoveThresholdXAxis;
    [SerializeField] private float AIMoveThresholdYAxis;
    public Ball ball;
    private const float chargeAmount = 2f;
    private float time;
    private RectTransform rectTransform;
    new private void Awake()
    {
        base.Awake();
        ball = FindFirstObjectByType<Ball>();
        rectTransform = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (ball.transform.position.y > AIMoveThresholdYAxis && 
            Mathf.Abs(rectTransform.position.x - ball.transform.position.x) > AIMoveThresholdXAxis)
            rb.linearVelocity = paddleSpeed * new Vector2(0, 0) { x = rectTransform.position.x < ball.transform.position.x ? 1f : -1f };
        else
            rb.linearVelocity = Vector2.zero;
        time += Time.deltaTime;
        if (time >= 1)
        {
            powerBar.PowerPercentChange(chargeAmount, true);
            time = 0;
        }
    }
    public void SetDifficulty()
    { }
}
