using UnityEngine;

public class StrightProjectile : ProjectileBase
{
    [Header("Movement")]
    public float speed = 10f;
    private Vector3 direction;

    // 부모의 Initialize에 방향(Direction)만 추가로 받음
    public void Setup(Vector3 dir, float dmg, int pierce, float duration)
    {
        base.Initialize(dmg, pierce, duration); // 부모의 초기화 기능 사용
        direction = dir.normalized;

        // 카드 회전 (날아가는 방향 보기)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    // 부모의 Update를 덮어쓰기(Override)해서 이동 로직 추가
    protected override void Update()
    {
        base.Update(); // 부모의 수명 체크 기능 유지

        // 이동 로직
        transform.position += direction * speed * Time.deltaTime;
    }
}
