using ThunderRoad;
using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace QuickHealSpell
{
    public class Spell_QuickHealSpell : SpellCastCharge
    {
		public float baseHeal = 30f;
		public float healChargePercent = 50f;
		public float exponentGrowth = 1.3f;
        private float gripThreshold = 0.9f; // [0, 1]

        public List<ValueDropdownItem<string>> GetAllDamagerID()
		{
			return Catalog.GetDropdownAllID(Catalog.Category.Damager, "None");
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x0006C5A0 File Offset: 0x0006A7A0
		public List<ValueDropdownItem<string>> GetAllItemID()
		{
			return Catalog.GetDropdownAllID(Catalog.Category.Item, "None");
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x0007456C File Offset: 0x0007276C
		public new SpellCastProjectile Clone()
		{
			return base.MemberwiseClone() as SpellCastProjectile;
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x0007457C File Offset: 0x0007277C
		public override void OnCatalogRefresh()
		{
			base.OnCatalogRefresh();
			imbueEnabled = false;
			
			//if (this.projectileId != null && this.projectileId != "")
			//{
			//	this.projectileData = Catalog.GetData<ItemPhysic>(this.projectileId, true);
			//}
			//if (this.projectileDamagerId != null && this.projectileDamagerId != "")
			//{
			//	this.projectileDamagerData = Catalog.GetData<DamagerData>(this.projectileDamagerId, true);
			//}
			//if (this.projectileEffectId != null && this.projectileEffectId != "")
			//{
			//	this.projectileEffectData = Catalog.GetData<EffectData>(this.projectileEffectId, true);
			//}
		}

        public override void UpdateCaster()
        {
			base.UpdateCaster();

			if (this.currentCharge < (healChargePercent/100f)) return;
            
			// Fix side with for loop
			foreach (Side hand in Enum.GetValues(typeof(Side))) 
			{
				if (PlayerControl.GetHand(hand).gripPressed && PlayerControl.GetHand(hand).GetAverageCurl() > gripThreshold)
                {
					Fire(true);
                }
			}
            
		}

        public override void Fire(bool active)
        {
			HealSelf(active);
            base.Fire(active);
        }

		private void HealSelf(bool active)
		{
			if (active)
			{
				Creature.player.health.Heal(Mathf.Pow(this.currentCharge, exponentGrowth) * baseHeal, Creature.player);
			}
		}	
    }
}