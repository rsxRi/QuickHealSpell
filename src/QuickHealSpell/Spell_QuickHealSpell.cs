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

		public new SpellCastProjectile Clone()
		{
			return base.MemberwiseClone() as SpellCastProjectile;
		}

		public override void OnCatalogRefresh()
		{
			base.OnCatalogRefresh();
			imbueEnabled = false;
		}

        public override void UpdateCaster()
        {
			base.UpdateCaster();

			if (this.currentCharge < (healChargePercent/100f)) return;

			foreach (Side hand in Enum.GetValues(typeof(Side))) 
			{
				if (PlayerControl.GetHand(hand).gripPressed && 
					PlayerControl.GetHand(hand).GetAverageCurl() > gripThreshold)
                {
					HealSelf(true);
                }
			}
            
		}

        public override void Fire(bool active)
        {
            base.Fire(active);
        }

		private void HealSelf(bool active)
		{
			if (!active || this.currentCharge < (healChargePercent / 100f)) return;

			Fire(false);
			Creature.player.health.Heal(Mathf.Pow(this.currentCharge, exponentGrowth) * baseHeal, Creature.player);
			
		}	
    }
}
