using UnityEngine;

public class EnemyBulletFire : MonoBehaviour
{
    private GameObject player;
    public float force;
    private Rigidbody2D rb;
    private float timer;

    void Start()
    {
        // rb = GetComponent<Rigidbody2D>();
        // player = GameObject.FindGameObjectWithTag("Player");


        // Vector3 direction = player.transform.position - transform.position;
        // rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;

        // float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 6)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Platform"))
        {
            Destroy(gameObject);
        }

    }
}
