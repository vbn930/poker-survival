using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float hp = 100f;

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
        // 경험치(칩) 드랍 로직 추가 예정
        Destroy(gameObject);
    }
}
