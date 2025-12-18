using UnityEditor.EditorTools;
using UnityEngine;

// abstract: 이 클래스는 단독으로 쓸 수 없고 반드시 상속받아 써야 함
public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Base Stats")]
    protected float damage;
    protected int pierceCount;
    protected float lifetime;

    // 초기화 (자식들이 공통으로 씀)
    public virtual void Initialize(float dmg, int pierce, float duration)
    {
        damage = dmg;
        pierceCount = pierce;
        lifetime = duration;
    }

    protected virtual void Update()
    {
        // 수명 체크
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Despawn();
        }
    }

    // 충돌 로직 (공통)
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
            pierceCount--;

            if (pierceCount <= 0)
            {
                Despawn();
            }
        }
    }

    // 사라지는 로직 (풀 반납)
    protected void Despawn()
    {
        Destroy(gameObject);
    }
}
