using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private GameManager GameManager;
    [SerializeField] bool isPlayer;

    private void Start()
    {
        GameManager=FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
            GameManager.ScoreChanged(isPlayer, true, 0);
    }
}
