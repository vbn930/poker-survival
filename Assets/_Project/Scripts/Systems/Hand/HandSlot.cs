using UnityEngine;
using System.Collections.Generic;

public class HandSlot : MonoBehaviour
{
    public int handMax = 5;
    public float handCooldown = 3f;
    public float attackRange = 20f;
    public List<CardData> cards = new List<CardData>();
    public HandResult handResult;
    public GameObject projectilePrefab;
    public LayerMask enemyLayer;    // 적 레이어 필터

    private DeckManager deckManager;
    private float timer = 0f;
    private bool isInitialized = false; // 데이터 받기 전엔 작동 안 함

    // 임시 UI 텍스트
    public string handText;

    public void Setup(GameObject prefab, LayerMask layer)
    {
        projectilePrefab = prefab;
        enemyLayer = layer;
        isInitialized = true; // 이제 작동 시작!
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deckManager = FindFirstObjectByType<DeckManager>();
        if (deckManager == null)
        {
            Debug.LogError("DeckManager not found in the scene!");
        }
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized) return;

        timer += Time.deltaTime;

        if (timer > handCooldown)
        {
            cards = deckManager.DrawCards(handMax);
            handText = string.Join(", ", cards);

            handResult = PokerEvaluator.EvaluateHand(cards);
            handText = handResult.Rank.ToString();
            string hand_result_str = $"Hand Rank: {handResult.Rank}, Winings: {string.Join(", ", handResult.WinningCards)}";
            
            for (int i = 0; i < handResult.WinningCards.Count; i++)
            {
                Transform target = FindClosestEnemy(); // 타겟 찾기

                if (target != null)
                {
                    Attack(target);
                    timer = 0f;
                }
            }
            Debug.Log(hand_result_str);
            timer = 0;
        }
    }

    Transform FindClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = hit.transform;
            }
        }
        return closestEnemy;
    }

    void Attack(Transform target)
    {
        // ★ 변경점: PoolManager 대신 Instantiate 사용
        GameObject obj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // 투사체 세팅 (직진형 스크립트 가져오기)
        StrightProjectile projectile = obj.GetComponent<StrightProjectile>();

        if (projectile != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            projectile.Setup(direction, 100f, 10, 3.0f);
        }

        Debug.Log($"Attacking {target.name} with projectile!");
    }

    // 범위 확인용 기즈모
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
