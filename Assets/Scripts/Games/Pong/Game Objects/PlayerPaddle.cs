using Unity.Netcode;
using UnityEngine;

public class PlayerPaddle : Paddle
{
    private InputManager InputManager;
    private bool isTouch1;
    public int clientID;
    public override void OnNetworkSpawn()
    {
        PongManager = FindObjectOfType<PongManager>();
        rb = GetComponent<Rigidbody2D>();
        if (!IsOwner)
            return;
        SetPlayerPaddleRpc(NetworkManager.Singleton.LocalClientId);
        InputManager = FindObjectOfType<InputManager>();
        InputManager.OnStartTouch += TouchStart;
        InputManager.OnEndTouch += TouchEnd;
       
    }

    public new void Awake()
    {
        base.Awake();
        if (PongManager.gameType == GameType.VSOnline)
            return;
        InputManager = FindObjectOfType<InputManager>();
        InputManager.OnStartTouch += TouchStart;
        InputManager.OnEndTouch += TouchEnd;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetPlayerPaddleRpc(ulong id)
    {
        clientID = (int)id;
    }

    private void TouchStart(Vector2 position, float time, bool isFirstTouch)
    {
        if (position.y > Screen.height / 2 && PongManager.gameType == GameType.VSLocal && this == PongManager.player1Paddle)
            return;
        if (position.y < Screen.height / 2 && PongManager.gameType == GameType.VSLocal && this == PongManager.player2Paddle)
            return;
        startPos = position;
        startTime = time;
        allowMovement = true;
    }

    private void Update()
    {
        if (!hasGameStarted)
            rb.position = PongManager.offscreenPosition.position;
        if (allowMovement)
        {
            if (PongManager.gameType == GameType.VSOnline && !IsHost)
                rb.velocity = paddleSpeed * new Vector2(0, 0) { x = startPos.x > Screen.width / 2 ? -1f : 1f };
            else
                rb.velocity = paddleSpeed * new Vector2(0, 0) { x = startPos.x > Screen.width / 2 ? 1f : -1f };
        }
        else
            rb.velocity = Vector2.zero;
    }
    private void TouchEnd(Vector2 touchPosition, float time, bool isFirstTouch)
    {
        if (startPos.y > Screen.height / 2 && PongManager.gameType == GameType.VSLocal && gameObject == PongManager.player1Paddle)
            return;
        if (startPos.y < Screen.height / 2 && PongManager.gameType == GameType.VSLocal && gameObject == PongManager.player2Paddle)
            return;
        endPos = touchPosition;
        endTime = time;
        allowMovement = false;
    }
}

