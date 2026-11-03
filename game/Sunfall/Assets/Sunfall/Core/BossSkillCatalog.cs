using System;

namespace Sunfall.Core
{
    public static class BossSkillCatalog
    {
        public static SpellKind Option(int chapter,int option)
        {
            if(chapter<1||chapter>RunModel.FinalChapter)throw new ArgumentOutOfRangeException(nameof(chapter));
            if(option<0||option>1)throw new ArgumentOutOfRangeException(nameof(option));
            switch(chapter)
            {
                case 1:return option==0?SpellKind.AshLance:SpellKind.CrimsonNova;
                case 2:return option==0?SpellKind.ChainPull:SpellKind.ChainRain;
                case 3:return option==0?SpellKind.SunLance:SpellKind.DawnBurst;
                default:return LateSpellCatalog.Option(chapter,option);
            }
        }
        public static string Description(SpellKind spell)
        {
            switch(spell)
            {
                case SpellKind.EmberWave:return "전방 7.5칸을 관통하는 불씨 파동.\n마력 ×2.5 피해 · 재사용 5초";
                case SpellKind.AshLance:return "전방 12칸을 관통하는 불꽃 창.\n마력 ×3.2 피해 · 재사용 6초";
                case SpellKind.CrimsonNova:return "주변 반경 3.6칸을 불꽃으로 휩쓸기.\n마력 ×2 피해 · 재사용 8초";
                case SpellKind.ChainPull:return "전방 8칸의 적 하나를 끌어오기.\n마력 ×1.5 · 보스는 피해만 · 재사용 6초";
                case SpellKind.ChainRain:return "주변 좌우 4칸에 사슬 내려치기.\n마력 ×2.8 피해 · 재사용 9초";
                case SpellKind.SunLance:return "전방 14칸을 관통하는 태양의 창.\n마력 ×4 피해 · 재사용 8초";
                case SpellKind.DawnBurst:return "주변 반경 4.5칸에 태양빛 폭발.\n마력 ×3.2 피해 · 재사용 10초";
                default:return LateSpellCatalog.Description(spell);
            }
        }
    }
}
