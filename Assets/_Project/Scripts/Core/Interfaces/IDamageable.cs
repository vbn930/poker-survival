using UnityEngine;

public interface IDamageable
{
    // 데미지를 입히는 함수 (데미지 양, 밀쳐내는 힘, 넉백 방향)
    void TakeDamage(float damage, float knockbackForce = 0f, UnityEngine.Vector3 knockbackDir = default);
}
