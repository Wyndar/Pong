using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class Ball : NetworkBehaviour
{
    public float speed;
    public int damage;
    private Rigidbody2D rb;
    private string lastHitObjectTag = "";
    private int sameTagBounceCount;

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            enabled = false;
            return;
        }
    }
    private void OnEnable() => rb = GetComponent<Rigidbody2D>();
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
        int x = Random.Range(0, 2) == 0 ? -1 : 1,
            y = Random.Range(0, 2) == 0 ? -1 : 1;
        rb.velocity = new(speed * x, speed * y);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (rb.velocity.sqrMagnitude < new Vector2(speed, speed).sqrMagnitude)
            rb.velocity += new Vector2(0.5f, 0.5f);
        if (lastHitObjectTag == "")
        {
            lastHitObjectTag = collision.tag;
            return;
        }
        if (collision.CompareTag("Wall") && lastHitObjectTag == ("Wall"))
            sameTagBounceCount++;
        if (sameTagBounceCount > 5)
            Launch();
    }
}