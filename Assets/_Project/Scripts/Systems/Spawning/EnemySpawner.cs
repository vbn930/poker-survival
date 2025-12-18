using UnityEditor.EditorTools;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    public float spawnInterval = 1.0f; // 생성 간격 (초)
    public float spawnRadius = 12f;    // 플레이어로부터 거리 (화면보다 커야 함)
    public GameObject enemyPrefab;     // 생성할 적 프리팹

    private float timer;
    private Transform playerTransform;

    void Start()
    {
        // 플레이어 미리 찾아두기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        timer += Time.deltaTime;

        // 시간이 되면 생성
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // 1. 랜덤한 각도의 위치 계산 (단위 벡터 * 반지름)
        // Random.insideUnitCircle은 원 안쪽 랜덤이므로 .normalized로 테두리로 보냄
        Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;

        // 2. 플레이어 위치를 기준으로 더함
        Vector3 spawnPos = playerTransform.position + new Vector3(randomPoint.x, randomPoint.y, 0);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
