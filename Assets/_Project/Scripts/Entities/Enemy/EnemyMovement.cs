using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3f;

    private Transform target; // 플레이어
    private SpriteRenderer spriteRenderer; // 방향 전환용

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // 매번 플레이어를 찾음 (플레이어가 죽었다 살아날 수도 있으니)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }

    void Update()
    {
        if (target == null) return;

        // 1. 방향 구하기 (플레이어 - 내 위치)
        Vector3 direction = (target.position - transform.position).normalized;

        // 2. 이동 (단순 좌표 이동)
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 3. 스프라이트 좌우 반전 (플레이어가 왼쪽에 있으면 뒤집기)
        if (direction.x < 0)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }
}
