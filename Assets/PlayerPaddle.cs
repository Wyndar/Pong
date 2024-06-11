using UnityEngine;

public class PlayerPaddle : Paddle
{
    private InputManager InputManager;
    private bool isTouch1;
    new private void Awake()
    {
        base.Awake();
        InputManager = FindObjectOfType<InputManager>();
        InputManager.OnStartTouch += TouchStart;
        InputManager.OnEndTouch += TouchEnd;
    }

    private void TouchStart(Vector2 position, float time, bool isFirstTouch)
    {
        if (position.y > Screen.height / 2 && GameManager.gameType == GameType.VSLocal && gameObject == GameManager.playerPaddle)
            return;
        if (position.y < Screen.height / 2 && GameManager.gameType == GameType.VSLocal && gameObject == GameManager.enemyPaddle)
            return;
        isTouch1 = isFirstTouch;
        startPos = position;
        startTime = time;
        allowMovement = true;
    }

    private void Update()
    {
        if (!IsOwner && GameManager.gameType == GameType.VSOnline)
            return;
        if (allowMovement)
            rb.velocity = (paddleSpeed * new Vector2(0, 0) { x = startPos.x > Screen.width / 2 ? 1f : -1f });
        else
            rb.velocity = Vector2.zero;
    }
    private void TouchEnd(Vector2 touchPosition, float time, bool isFirstTouch)
    {
        if (isTouch1 != isFirstTouch)
            return;
        if (startPos.y > Screen.height / 2 && GameManager.gameType == GameType.VSLocal && gameObject == GameManager.playerPaddle)
            return;
        if (startPos.y < Screen.height / 2 && GameManager.gameType == GameType.VSLocal && gameObject == GameManager.enemyPaddle)
            return;
        endPos = touchPosition;
        endTime = time;
        allowMovement = false;
    }
}

