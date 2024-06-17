using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private PongManager PongManager;
    [SerializeField] bool isPlayer;

    private void Start()
    {
        PongManager=FindObjectOfType<PongManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
            PongManager.ScoreChanged(isPlayer, true, 0);
    }
}
