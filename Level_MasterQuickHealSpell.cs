using BS;
using System;
using UnityEngine;

namespace QuickHealSpell
{
    public class Level_MasterQuickHealSpell : LevelModule
    {
        public override void OnLevelLoaded(LevelDefinition levelDefinition)
        {
            initialized = true;
        }
        public override void Update(LevelDefinition levelDefinition)
        {
            if (!initialized) return;
            // Add code here.
        }
        public override void OnLevelUnloaded(LevelDefinition levelDefinition)
        {
            initialized = false;
        }
    }
}