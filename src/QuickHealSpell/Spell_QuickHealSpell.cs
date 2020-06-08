using ThunderRoad;
using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace QuickHealSpell
{
    public class Spell_QuickHealSpell : SpellCastCharge
    {
		public float baseHeal = 15f;
		public float healChargePercent = 50f;
		public float exponentGrowth = 1.3f;
		public float gripThreshold = 0.7f; // [0, 1]
		public string spellHealType = "constant"; // [crush, smash, constant]
		private static SpellHealType spellHealTypeInternal = SpellHealType.Crush;
        public float baseHealConstant = 5f;
		public float constantExchangeRateConsumption = 1f;


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
			foreach(SpellHealType healType in Enum.GetValues(typeof(SpellHealType)))
            {
				if (healType.ToString().ToLower().Contains(spellHealType.ToLower()))
                {
					spellHealTypeInternal = healType;
					Debug.Log("QuickHealSpell set to " + healType.ToString());
					return;
                }
            }
			Debug.LogError("No valid type has been assigned to QuickHealSpell.");
        }

        public override void UpdateCaster()
        {
			base.UpdateCaster();

			if (this.currentCharge < (healChargePercent/100f)) return;

			foreach (Side hand in Enum.GetValues(typeof(Side))) 
			{
				if (PlayerControl.GetHand(hand).gripPressed && 
					PlayerControl.GetHand(hand).GetAverageCurlNoThumb() > gripThreshold &&
					spellHealTypeInternal == SpellHealType.Crush)
                {
					HealSelf(true);
                }
				if(spellHealTypeInternal == SpellHealType.Constant)
                {
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
			if (!active || this.currentCharge < (healChargePercent / 100f) || this.spellCaster.mana.currentMana <= 0 || !this.spellCaster.isFiring) return;

			if (spellHealTypeInternal == SpellHealType.Crush || spellHealTypeInternal == SpellHealType.Smash)
			{
				Fire(false);
				Creature.player.health.Heal(Mathf.Pow(this.currentCharge, exponentGrowth) * baseHeal, Creature.player);
				currentCharge = 0;
			}
			else if(spellHealTypeInternal == SpellHealType.Constant)
			{
				Creature.player.health.Heal(Time.deltaTime * baseHeal / baseHealConstant, Creature.player);
				this.spellCaster.mana.currentMana -= Time.deltaTime * baseHeal / baseHealConstant * constantExchangeRateConsumption; // mana consumption 1:x1 with health
				if(this.spellCaster.mana.currentMana <= 0 || Creature.player.health.currentHealth >= Creature.player.health.maxHealth)
                {
					Fire(false);

                }
			}

		}	
    }
}
