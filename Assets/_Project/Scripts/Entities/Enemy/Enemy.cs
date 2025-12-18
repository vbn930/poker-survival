using System.Threading;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float hp = 100f;

    private PoolManager poolManager;

    private bool isDead = false;

    private void Awake()
    {
        poolManager = FindFirstObjectByType<PoolManager>();
    }

    void OnEnable()
    {
        isDead = false;
        hp = 100f; // 체력 초기화
    }

    void OnDisable()
    {
        // 필요시 초기화 작업
    }

    // 인터페이스 구현
    public void TakeDamage(float damage, float knockbackForce, Vector3 knockbackDir)
    {
        hp -= damage;
        Debug.Log($"{name} 피격! 남은 체력: {hp}");

        // 피격 이펙트, 사운드 재생 등을 여기서 처리

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        // 경험치(칩) 드랍 로직 추가 예정
        poolManager.ReleaseObject("Enemy", gameObject);
    }
}
