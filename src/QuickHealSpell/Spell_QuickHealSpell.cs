using ThunderRoad;
using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace QuickHealSpell
{
    public class Spell_QuickHealSpell : SpellCastCharge
    {
        private string projectileId;
        private string projectileDamagerId;
        private string projectileEffectId;

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
	}
}