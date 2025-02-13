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
    private bool firstUpdate = true;

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            enabled = false;
            return;
        }
    }
    private void Update()
    {
        if (!IsOwner) return;
        PredictBallMovement();
    }

    private void PredictBallMovement()
    {
        if (firstUpdate) return;
        Vector2 predictedPosition = rb.position + rb.linearVelocity * Time.deltaTime;
        rb.MovePosition(predictedPosition);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void CorrectClientPositionRPC(Vector2 serverPosition)
    {
        if (!IsOwner) return;

        if (Vector2.Distance(rb.position, serverPosition) > 0.1f)
            rb.position = Vector2.Lerp(rb.position, serverPosition, 0.1f);
        if (rb.linearVelocity.sqrMagnitude < 2 * (new Vector2(speed, speed).sqrMagnitude))
            rb.linearVelocity += new Vector2(0.5f, 0.5f);
        if (sameTagBounceCount > 5)
            Launch();
        firstUpdate = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsOwner) return;
        if (lastHitObjectTag == "")
        {
            lastHitObjectTag = collision.gameObject.tag;
            return;
        }
        if (collision.gameObject.CompareTag("Paddle"))
            sameTagBounceCount = 0;
        if (collision.gameObject.CompareTag("Wall") && lastHitObjectTag == "Wall")
            sameTagBounceCount++;
        if (IsServer) HandleBounce(collision);
        RequestServerCorrectionServerRPC();
    }

    [ServerRpc]
    private void RequestServerCorrectionServerRPC() => CorrectClientPositionRPC(rb.position);

    private void HandleBounce(Collision2D collision)
    {
        Vector2 newVelocity = Vector2.Reflect(rb.linearVelocity, collision.contacts[0].normal);
        rb.linearVelocity= newVelocity;
    }
    private void OnEnable() => rb = GetComponent<Rigidbody2D>();
    public IEnumerator ResetBall()
    {
        rb.linearVelocity= Vector3.zero;
        transform.position = Vector3.zero;
        damage = 1;
        speed = 3;
        yield return new WaitForSeconds(2f);
        Launch();
        yield break;
    }
    private void Launch()
    {
        //sameTagBounceCount = 0;
        //lastHitObjectTag = "";
        int x = Random.Range(0, 2) == 0 ? -1 : 1,
            y = Random.Range(0, 2) == 0 ? -1 : 1;
        rb.linearVelocity = new(speed * x, speed * y);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (rb.linearVelocity.sqrMagnitude < 2 * (new Vector2(speed, speed).sqrMagnitude))
    //        rb.linearVelocity += new Vector2(0.5f, 0.5f);
    //    if (lastHitObjectTag == "")
    //    {
    //        lastHitObjectTag = collision.gameObject.tag;
    //        return;
    //    }
    //    if (collision.gameObject.CompareTag("Paddle"))
    //        sameTagBounceCount = 0;
    //    if (collision.gameObject.CompareTag("Wall") && lastHitObjectTag == "Wall")
    //        sameTagBounceCount++;
    //    if (sameTagBounceCount > 5)
    //        Launch();
    //}
}
