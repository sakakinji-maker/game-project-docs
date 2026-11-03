using System;
using System.Collections.Generic;

namespace Sunfall.Core
{
    public sealed partial class RunModel
    {
        public const int SkillSlotCount=4;
        readonly SpellKind?[] equippedSkills={SpellKind.EmberWave,null,null,null};
        public IEnumerable<SpellKind> LearnedSkills
        {
            get{foreach(SpellKind spell in Enum.GetValues(typeof(SpellKind)))if(HasSpell(spell))yield return spell;}
        }
        public bool CanChangeSkills=>Phase==RunPhase.Combat&&Hp>0&&!Cinematic&&!SkillChoicePending&&!Casting&&!Rolling&&AttackTimer<=0;
        public SpellKind? EquippedSkill(int slot)=>slot>=0&&slot<SkillSlotCount?equippedSkills[slot]:null;
        public int EquippedSlot(SpellKind spell)
        {for(int i=0;i<SkillSlotCount;i++)if(equippedSkills[i]==spell)return i;return -1;}
        public bool IsSpellEquipped(SpellKind spell)=>HasSpell(spell)&&EquippedSlot(spell)>=0;
        public bool EquipSkill(int slot,SpellKind spell)
        {
            if(!CanChangeSkills||slot<0||slot>=SkillSlotCount||!HasSpell(spell))return false;
            int previous=EquippedSlot(spell);if(previous==slot)return true;
            // Moving an equipped spell swaps slots. Its cooldown belongs to the spell.
            if(previous>=0)equippedSkills[previous]=equippedSkills[slot];
            equippedSkills[slot]=spell;return true;
        }
        public bool UnequipSkill(int slot)
        {
            if(!CanChangeSkills||slot<0||slot>=SkillSlotCount||!equippedSkills[slot].HasValue)return false;
            equippedSkills[slot]=null;return true;
        }
        void AutoEquipNewSkill(SpellKind spell)
        {
            if(IsSpellEquipped(spell))return;
            for(int i=0;i<SkillSlotCount;i++)if(!equippedSkills[i].HasValue){equippedSkills[i]=spell;return;}
        }
        public static float BaseSkillReuse(SpellKind spell)
        {
            switch(spell)
            {
                case SpellKind.EmberWave:return MagicReuse;
                case SpellKind.AshLance:return LanceReuse;
                case SpellKind.CrimsonNova:return NovaReuse;
                case SpellKind.ChainPull:return 6;
                case SpellKind.ChainRain:return 9;
                case SpellKind.SunLance:return 8;
                case SpellKind.DawnBurst:return 10;
                default:return LateSpellCatalog.Contains(spell)?LateSpellCatalog.Reuse(spell):0;
            }
        }
        public float SkillReuse(SpellKind spell)=>BaseSkillReuse(spell)*Equipment.MagicReuseMultiplier;
        public float SkillCooldown(SpellKind spell)
        {
            switch(spell)
            {
                case SpellKind.EmberWave:return MagicCooldown;
                case SpellKind.AshLance:return LanceCooldown;
                case SpellKind.CrimsonNova:return NovaCooldown;
                case SpellKind.ChainPull:case SpellKind.ChainRain:return ChainCooldown;
                case SpellKind.SunLance:case SpellKind.DawnBurst:return SolarCooldown;
                default:return LateSpellCatalog.Contains(spell)?LateCooldown(LateSpellCatalog.Slot(spell)):0;
            }
        }
    }
}
