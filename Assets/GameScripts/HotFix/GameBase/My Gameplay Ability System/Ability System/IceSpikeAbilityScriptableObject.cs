using System.Collections;
using AbilitySystem;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using UnityEngine;

// 冰锥(1 技能)：读取冰锥层数 N，按朝向扇形发射 N 根冰锥，发射完用 GE 清空层数。
// 层数由普通攻击通过 GE 叠加（见 BasicAttackAbility -> AddIceStack GE）。
[CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Ice Spike")]
public class IceSpikeAbilityScriptableObject : AbstractAbilityScriptableObject
{
    [Header("投射物")]
    [SerializeField] private GameObject projectilePrefab;
    public float ProjectileSpeed = 15f;
    public float ProjectileLifetime = 3f;
    public float IntervalBetweenSpikes = 0.12f;
    public float SpreadAngle = 12f;
    public float HeightOffset = 1f;

    [Header("层数")]
    public AttributeScriptableObject StackAttribute;        // 冰锥层数计数器
    public GameplayEffectScriptableObject ResetStackEffect; // Instant, Override 0
    public int MaxStacks = 5;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new IceSpikeAbilitySpec(this, owner);
        spec.Level = owner.Level;
        spec.CastPoint = owner.GetComponent<CastPointComponent>();
        return spec;
    }

    public class IceSpikeAbilitySpec : AbstractAbilitySpec
    {
        public CastPointComponent CastPoint;

        public IceSpikeAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner)
            : base(ability, owner) { }

        public override void CancelAbility() { }

        public override bool CheckGameplayTags() => true;

        protected override IEnumerator PreActivate() { yield return null; }

        protected override IEnumerator ActivateAbility()
        {
            var ab = (IceSpikeAbilityScriptableObject)Ability;

            // 1) 读层数（Instant GE 改的是 BaseValue，读 BaseValue 最稳，避免同帧 CurrentValue 未刷新）
            int stacks = 0;
            if (ab.StackAttribute != null &&
                Owner.AttributeSystem.GetAttributeValue(ab.StackAttribute, out var sv))
            {
                stacks = Mathf.Clamp(Mathf.RoundToInt(sv.BaseValue), 0, ab.MaxStacks);
            }

            if (stacks <= 0)
            {
                Debug.Log("[IceSpike] 没有冰锥层数，先用普通攻击(F)叠层。");
                yield break;
            }

            // 2) 上冷却（只一次）
            if (Ability.Cooldown != null)
                Owner.ApplyGameplayEffectSpecToSelf(Owner.MakeOutgoingSpec(Ability.Cooldown));
            if (Ability.Cost != null)
                Owner.ApplyGameplayEffectSpecToSelf(Owner.MakeOutgoingSpec(Ability.Cost));

            // 3) 按层数扇形发射
            Vector3 origin = (CastPoint != null ? CastPoint.GetPosition() : Owner.transform.position)
                             + Vector3.up * ab.HeightOffset;
            Vector3 forward = Owner.transform.forward;

            Debug.Log($"[IceSpike] 释放：发射 {stacks} 根冰锥。");
            for (int i = 0; i < stacks; i++)
            {
                float t = stacks == 1 ? 0f : ((float)i / (stacks - 1) - 0.5f);
                float angle = t * ab.SpreadAngle;
                Vector3 dir = (Quaternion.AngleAxis(angle, Vector3.up) * forward).normalized;

                if (ab.projectilePrefab != null)
                {
                    var go = Instantiate(ab.projectilePrefab, origin, Quaternion.LookRotation(dir));
                    var fp = go.GetComponent<ForwardProjectile>();
                    if (fp == null) fp = go.AddComponent<ForwardProjectile>();
                    fp.Direction = dir;
                    fp.Speed = ab.ProjectileSpeed;
                    fp.Lifetime = ab.ProjectileLifetime;
                }

                if (ab.IntervalBetweenSpikes > 0f)
                    yield return new WaitForSeconds(ab.IntervalBetweenSpikes);
            }

            // 4) 清空层数
            if (ab.ResetStackEffect != null)
                Owner.ApplyGameplayEffectSpecToSelf(Owner.MakeOutgoingSpec(ab.ResetStackEffect));
        }
    }
}
