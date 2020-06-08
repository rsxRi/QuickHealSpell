using System;
using ThunderRoad;
using UnityEngine;

namespace QuickHealSpell
{
    public class Spell_QuickHealSpell : SpellCastCharge
    {
        // General
        public float baseHeal = 15f;
        public float healChargePercent = 50f;
        public float exponentGrowth = 1.3f;

        public string spellHealType = "crush"; // [crush, smash, constant]
        private static SpellHealType spellHealTypeInternal = SpellHealType.Crush;

        // Crush
        public float gripThreshold = 0.7f; // [0, 1]

        // Constant
        public float constantBaseHeal = 5f;
        public float constantExchangeRateConsumption = 1f;

        // Smash
        public float smashDistance = 0.01f;

        enum SpellHealType
        {
            Crush,
            Smash,
            Constant
        }

        public new SpellCastProjectile Clone()
        {
            return base.MemberwiseClone() as SpellCastProjectile;
        }

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            imbueEnabled = false;
            VerifyType();
        }

        private void VerifyType()
        {
            foreach (SpellHealType healType in Enum.GetValues(typeof(SpellHealType)))
            {
                if (healType.ToString().ToLower().Contains(spellHealType.ToLower()))
                {
                    spellHealTypeInternal = healType;
                    return;
                }
            }
            Debug.LogError("No valid type has been assigned to QuickHealSpell.");
        }

        public override void UpdateCaster()
        {
            base.UpdateCaster();

            if (this.currentCharge < (healChargePercent / 100f)) return;

            foreach (Side hand in Enum.GetValues(typeof(Side)))
            {
                if (PlayerControl.GetHand(hand).gripPressed &&
                    PlayerControl.GetHand(hand).GetAverageCurlNoThumb() > gripThreshold &&
                    spellHealTypeInternal == SpellHealType.Crush)
                {
                    HealSelf(true);
                }
                if (spellHealTypeInternal == SpellHealType.Constant)
                {
                    HealSelf(true);
                }
                if (spellHealTypeInternal == SpellHealType.Smash)
                {
                    var spell = spellCaster.magicSource.position;
                    var chest = Creature.player.animator.GetBoneTransform(HumanBodyBones.Chest).position;

                    var dist = Vector3.Distance(spell, chest);
                    var dir = spell - chest;

                    if (dist < 0.35f && Vector3.Dot(PlayerControl.GetHand(hand).GetHandVelocity(), dir) > 0.5f)
                        HealSelf(true);
                }
            }

        }

        public override void Fire(bool active)
        {
            base.Fire(active);

            if (active) return;
            base.spellCaster.isFiring = false;
            base.spellCaster.grabbedFire = false;
            base.spellCaster.telekinesis.TryRelease(false);
        }

        private void HealSelf(bool active)
        {
            if (!active ||
                this.currentCharge < (healChargePercent / 100f) ||
                this.spellCaster.mana.currentMana <= 0 ||
                this.spellCaster.isFiring == false)
                return;

            if (spellHealTypeInternal == SpellHealType.Crush ||
                spellHealTypeInternal == SpellHealType.Smash)
            {
                HealSelfInstant();
            }
            else if (spellHealTypeInternal == SpellHealType.Constant)
            {
                HealSelfConstant();
            }

        }

        private void HealSelfInstant()
        {
            Fire(false);
            Creature.player.health.Heal(Mathf.Pow(this.currentCharge, exponentGrowth) * baseHeal, Creature.player);
            currentCharge = 0;
        }

        private void HealSelfConstant()
        {
            Creature.player.health.Heal(Time.deltaTime * baseHeal / constantBaseHeal, Creature.player);
            this.spellCaster.mana.currentMana -= Time.deltaTime * baseHeal / constantBaseHeal * constantExchangeRateConsumption; // mana consumption 1:x1 with health
            if (this.spellCaster.mana.currentMana <= 0 || Creature.player.health.currentHealth >= Creature.player.health.maxHealth)
            {
                Fire(false);

            }
        }
    }
}
