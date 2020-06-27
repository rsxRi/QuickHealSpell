using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;
using static QuickHealSpell.QuickHealSpellUtils;

namespace QuickHealSpell
{
    public class Spell_QuickHealSpell : SpellCastCharge
    {
        // General
        public float baseHeal = 15f;
        public float healChargePercent = 50f;

        public string spellHealType = "smash"; // [crush, smash, constant]
        private static SpellHealType spellHealTypeInternal = SpellHealType.Smash;

        // Crush
        public float gripThreshold = 0.7f; // [0, 1]

        // Smash
        public float smashDistance = 0.37f;
        public float smashVelocity = 0.45f;

        // Constant
        public float constantBaseHeal = 5f;
        public float constantExchangeRateConsumption = 1f;

        private GameObject vfxOrb;
        private bool healSuccess = false;

        private enum SpellHealType
        {
            Crush,
            Smash,
            Constant
        }

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            System.Collections.Generic.List<GameObject> vfxs = QuickHealSpellUtils.LoadResources<GameObject>(new string[] { "healing_orb.prefab", "healing_aoe.prefab" }, "healingsfx");
            healingOrb = vfxs.Where(x => x.name == "healing_orb").First();
            healingAoe = vfxs.Where(x => x.name == "healing_aoe").First();
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

            if (!base.spellCaster.isFiring) this.currentCharge = 0;

            if (this.currentCharge < (healChargePercent / 100f)) return;

            if (PlayerControl.GetHand(spellCaster.bodyHand.side).gripPressed &&
                PlayerControl.GetHand(spellCaster.bodyHand.side).GetAverageCurlNoThumb() > gripThreshold &&
                spellHealTypeInternal == SpellHealType.Crush)
            {
                HealSelf();
            }
            if (spellHealTypeInternal == SpellHealType.Constant)
            {
                HealSelf();
            }
            if (spellHealTypeInternal == SpellHealType.Smash)
            {
                Vector3 spell = spellCaster.magicSource.position;

                Vector3 chest = Creature.player.animator.GetBoneTransform(HumanBodyBones.Chest).position;

                float dist = Vector3.Distance(spell, chest);

                Vector3 dir = chest - spell;

                if (dist < smashDistance && Vector3.Dot(Player.local.transform.rotation * PlayerControl.GetHand(this.spellCaster.bodyHand.side).GetHandVelocity(), dir) > smashVelocity)
                    HealSelf();
            }
        }

        public override void Fire(bool active)
        {
            base.Fire(active);
            if (active)
            {
                vfxOrb = GameObject.Instantiate(healingOrb, spellCaster.magicSource);
                vfxOrb.transform.localPosition = Vector3.zero;
                vfxOrb.transform.localScale = vfxOrb.transform.localScale / 11;
            }

            if (active) return;
            if (healSuccess) Timing.RunCoroutine(LerpVfx(0.2f, vfxOrb, vfxOrb.transform.localScale, vfxOrb.transform.localScale * 2f));
            else Timing.RunCoroutine(LerpVfx(0.2f, vfxOrb, vfxOrb.transform.localScale, Vector3.zero));
            healSuccess = false;
            base.spellCaster.isFiring = false;
            base.spellCaster.grabbedFire = false;
            this.currentCharge = 0;
            base.spellCaster.telekinesis.TryRelease(false);
        }

        private void HealSelf()
        {
            if (this.currentCharge < (healChargePercent / 100f) ||
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
            healSuccess = true;
            Creature.player.health.Heal(baseHeal, Creature.player);
            Fire(false);
        }

        private void HealSelfConstant()
        {
            Creature.player.health.Heal(Time.deltaTime * baseHeal / constantBaseHeal, Creature.player);
            this.spellCaster.mana.currentMana -= Time.deltaTime * constantExchangeRateConsumption * (baseHeal / constantBaseHeal);
            if (this.spellCaster.mana.currentMana <= 0 || Creature.player.health.currentHealth >= Creature.player.health.maxHealth)
                Fire(false);
        }

        public IEnumerator<float> LerpVfx(float seconds, GameObject vfx, Vector3 startScale, Vector3 endScale)
        {
            if (vfx != null)
            {
                float time = 0f;
                vfx.transform.SetParent(null);
                vfx.GetComponent<VisualEffect>().playRate = 4f;
                vfx.GetComponent<VisualEffect>().Stop();
                while (time < 1f)
                {
                    time += Time.fixedDeltaTime / (seconds / 2f);
                    vfx.transform.localScale = Vector3.Lerp(startScale, endScale, time);
                    yield return Time.fixedDeltaTime;
                }
                time = 0f;
                while (time < 1f)
                {
                    time += Time.fixedDeltaTime / (1f - (seconds / 2f));
                    yield return Time.fixedDeltaTime;
                }

                GameObject.Destroy(vfx);
            }
        }
    }
}
