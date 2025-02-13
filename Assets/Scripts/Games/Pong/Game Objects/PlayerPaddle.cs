using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPaddle : Paddle
{
    private InputManager InputManager;
    private float time;

    public new void Awake()
    {
        base.Awake();
        InputManager = FindFirstObjectByType<InputManager>();
        InputManager.OnStartTouch += TouchStart;
        InputManager.OnEndTouch += TouchEnd;
    }

    private void TouchStart(Vector2 position, float time, bool isFirstTouch)
    {
        if (!hasGameStarted)
            return;
        if (position.y > Screen.height / 2 && PongManager.gameType == GameType.VSLocal && this == PongManager.player1Paddle ||
        position.y < Screen.height / 2 && PongManager.gameType == GameType.VSLocal && this == PongManager.player2Paddle)
            return;
        startPos = position;
        startTime = time;
        allowMovement = true;
    }

    private void Update()
    {
        if (!hasGameStarted)
            rb.position = PongManager.offscreenPosition.position;
        if (allowMovement && PongManager.gameInputType != InputType.Gyro)
        {
            rb.linearVelocity = name == "Blue Player"
                ? paddleSpeed * new Vector2(0, 0) { x = startPos.x > Screen.width / 2 ? 1f : -1f }
                : paddleSpeed * new Vector2(0, 0) { x = startPos.x > Screen.width / 2 ? -1f : 1f };
        }
        else if (PongManager.gameInputType != InputType.Touchscreen)
        {
            rb.linearVelocity = name == "Blue Player"
                ? paddleSpeed * new Vector2(0, 0) { x = Accelerometer.current.acceleration.ReadValue().x > 0 ? 1f : -1f }
                : paddleSpeed * new Vector2(0, 0) { x = Accelerometer.current.acceleration.ReadValue().x < 0 ? -1f : 1f };
            time += Time.deltaTime;
            if (time >= 1)
            {
                powerBar.PowerPercentChange(2, true);
                time = 0;
            }
        }
        else
            rb.linearVelocity = Vector2.zero;
    }
    private void TouchEnd(Vector2 touchPosition, float time, bool isFirstTouch)
    {
        if (!hasGameStarted)
            return;
        if (startPos.y > Screen.height / 2 && PongManager.gameType == GameType.VSLocal && gameObject == PongManager.player1Paddle ||
        startPos.y < Screen.height / 2 && PongManager.gameType == GameType.VSLocal && gameObject == PongManager.player2Paddle)
            return;
        endPos = touchPosition;
        endTime = time;
        allowMovement = false;
        powerBar.PowerPercentChange(2 * (endTime - startTime), true);
    }
}

