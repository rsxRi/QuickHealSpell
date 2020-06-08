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
		public string spellHealType = "crush"; // [crush, smash, constant]
		private static SpellHealType spellHealTypeInternal = SpellHealType.Crush;

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
			if (!active || this.currentCharge < (healChargePercent / 100f)) return;

			Debug.Log("Healing player " + Mathf.Pow(this.currentCharge, exponentGrowth) * baseHeal);
			Fire(false);
			Creature.player.health.Heal(Mathf.Pow(this.currentCharge, exponentGrowth) * baseHeal, Creature.player);
			currentCharge = 0;
		}	
    }
}
