using System.Collections;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

// 普通攻击能力：施加一个“命中效果” GE（这里用来给自己叠加冰锥层数 +1）。
// 与内置 SimpleAbility 的区别：这里对 Cost/Cooldown 做了空值保护，
// 允许普攻不配 Cost/Cooldown（SimpleAbility 在 Cooldown/Cost 为空时会 NullReference）。
[CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Basic Attack")]
public class BasicAttackAbilityScriptableObject : AbstractAbilityScriptableObject
{
    public GameplayEffectScriptableObject OnHitEffect;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new BasicAttackAbilitySpec(this, owner);
        spec.Level = owner.Level;
        return spec;
    }

    public class BasicAttackAbilitySpec : AbstractAbilitySpec
    {
        public BasicAttackAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner)
            : base(ability, owner) { }

        public override void CancelAbility() { }

        public override bool CheckGameplayTags() => true;

        protected override IEnumerator PreActivate() { yield return null; }

        protected override IEnumerator ActivateAbility()
        {
            var ab = (BasicAttackAbilityScriptableObject)Ability;

            if (Ability.Cooldown != null)
                Owner.ApplyGameplayEffectSpecToSelf(Owner.MakeOutgoingSpec(Ability.Cooldown));
            if (Ability.Cost != null)
                Owner.ApplyGameplayEffectSpecToSelf(Owner.MakeOutgoingSpec(Ability.Cost));

            if (ab.OnHitEffect != null)
            {
                Owner.ApplyGameplayEffectSpecToSelf(Owner.MakeOutgoingSpec(ab.OnHitEffect));
                Debug.Log("[BasicAttack] 命中：冰锥层数 +1");
            }

            yield return null;
        }
    }
}
