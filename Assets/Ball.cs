using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public int damage;
    public string lastHitObjectTag = "";
    public int sameTagBounceCount;

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
        int x = Random.Range(0, 2) == 0 ? -1 : 1,
            y = Random.Range(0, 2) == 0 ? -1 : 1;
        rb.velocity = new(speed * x, speed * y);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
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
