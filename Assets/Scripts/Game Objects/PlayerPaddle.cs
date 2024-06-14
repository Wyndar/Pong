﻿using Unity.Netcode;
using UnityEngine;

public class PlayerPaddle : Paddle
{
    private InputManager InputManager;
    private bool isTouch1;
    public int clientID;

    public override void OnNetworkSpawn()
    {
        GameManager = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        rb.position = GameManager.offscreenPosition.position;
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
        if (GameManager.gameType == GameType.VSOnline)
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
        if (position.y > Screen.height / 2 && GameManager.gameType == GameType.VSLocal && gameObject == GameManager.player1Paddle)
            return;
        if (position.y < Screen.height / 2 && GameManager.gameType == GameType.VSLocal && gameObject == GameManager.player2Paddle)
            return;
        isTouch1 = isFirstTouch;
        startPos = position;
        startTime = time;
        allowMovement = true;
    }

    private void Update()
    {
        if (allowMovement)
            rb.velocity = (paddleSpeed * new Vector2(0, 0) { x = startPos.x > Screen.width / 2 ? 1f : -1f });
        else
            rb.velocity = Vector2.zero;
    }
    private void TouchEnd(Vector2 touchPosition, float time, bool isFirstTouch)
    {
        if (isTouch1 != isFirstTouch)
            return;
        if (startPos.y > Screen.height / 2 && GameManager.gameType == GameType.VSLocal && gameObject == GameManager.player1Paddle)
            return;
        if (startPos.y < Screen.height / 2 && GameManager.gameType == GameType.VSLocal && gameObject == GameManager.player2Paddle)
            return;
        endPos = touchPosition;
        endTime = time;
        allowMovement = false;
    }
}

