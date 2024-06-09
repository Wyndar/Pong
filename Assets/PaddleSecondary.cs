using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PaddleSecondary: MonoBehaviour
{
    [SerializeField] private InputManager InputManager;
    [SerializeField] private float paddleSpeed;
    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 endPos;
    private float startTime;
    private float endTime;
    private bool allowMovement;
    private Vector3 resetPosition;
    private void OnEnable()
    {
        InputManager = FindObjectOfType<InputManager>();
        rb = GetComponent<Rigidbody2D>();
        resetPosition = rb.position;
        InputManager.OnStartSecondaryTouch += TouchStart;
        InputManager.OnEndSecondaryTouch += TouchEnd;
    }
    private void OnDisable()
    {
        InputManager.OnEndTouch -= TouchEnd;
        InputManager.OnStartTouch -= TouchStart;
    }
    private void TouchStart(Vector2 touchPosition , float time)
    {
        //can add trail effects using the params here
        startPos = touchPosition;
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
    private void TouchEnd(Vector2 touchPosition, float time)
    {
        endPos = touchPosition;
        endTime = time;
        allowMovement = false;
    }
    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        rb.position = resetPosition;
    }
}
