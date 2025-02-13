using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private PongManager PongManager;
    [SerializeField] public bool isPlayer1;

    private void Start()
    {
        PongManager=FindFirstObjectByType<PongManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
            PongManager.ScoreChanged(isPlayer1, true, 0);
    }
}
