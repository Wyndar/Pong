using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ResetBall());
    }

    // Update is called once per frame
    public IEnumerator ResetBall()
    {
        rb.velocity= Vector3.zero;
        transform.position = Vector3.zero;
        damage = 1;
        speed = 3;
        yield return new WaitForSeconds(2f);
        Launch();
        yield break;
    }
    private void Launch()
    {
        float x = Random.Range(0, 2) == 0 ? -1 : 1,
            y = Random.Range(0, 2) == 0 ? -1 : 1;
        rb.velocity = new Vector2(speed * x, speed * y);
    }
}
