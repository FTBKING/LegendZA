using UnityEngine;

// 朝固定方向匀速飞行、到寿命后自毁的简易投射物。
// 冰锥技能不依赖锁定目标，按角色朝向直接发射，因此用它而不是需要 Target 的 Projectile。
public class ForwardProjectile : MonoBehaviour
{
    public Vector3 Direction = Vector3.forward;
    public float Speed = 15f;
    public float Lifetime = 3f;

    private float _age;

    private void Update()
    {
        transform.position += Direction * Speed * Time.deltaTime;
        _age += Time.deltaTime;
        if (_age >= Lifetime)
        {
            Destroy(gameObject);
        }
    }
}
