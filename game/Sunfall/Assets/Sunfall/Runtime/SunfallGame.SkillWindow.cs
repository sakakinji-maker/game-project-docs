using System.Collections.Generic;
using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        bool skillWindowOpen;
        SpellKind selectedSkill=SpellKind.EmberWave;
        string skillWindowNotice="";
        // Include the title, key badge, cooldown bar and readiness label.
        readonly Rect[] skillButtons={new Rect(450,578,110,134),new Rect(590,578,110,134),new Rect(730,578,110,134),new Rect(870,578,110,134)};
        readonly bool[] previousSkill=new bool[4],queuedSkill=new bool[4];
        bool CanToggleSkills=>!title&&!paused&&!settings&&!inventoryOpen&&!StoryActive&&serviceNpc<0&&run!=null&&!run.Cinematic&&!run.SkillChoicePending&&(!village||dialoguePage<0);
        void ClearSkillInput(){for(int i=0;i<4;i++){previousSkill[i]=true;queuedSkill[i]=false;}}
        void ReadSkillInput()
        {
            for(int i=0;i<4;i++)
            {
                bool held=keys.Held(5+i)||(TouchControls&&TouchHeld(skillButtons[i]));
                queuedSkill[i]|=(held&&!previousSkill[i])||keys.Pressed(5+i);previousSkill[i]=held;
            }
        }
        int QueuedSkillSlot(){for(int i=0;i<4;i++)if(queuedSkill[i])return i;return -1;}
        void ToggleSkillWindow()
        {
            if(!CanToggleSkills)return;
            skillWindowOpen=!skillWindowOpen;skillWindowNotice="";
            if(!run.HasSpell(selectedSkill))selectedSkill=SpellKind.EmberWave;
            CaptureWorldMotion();Block();
        }
        void SelectSkill(SpellKind spell){selectedSkill=spell;skillWindowNotice="";Block();}
        void EquipSelectedSkill(int slot)
        {
            if(run.EquipSkill(slot,selectedSkill))skillWindowNotice=SpellLabel(selectedSkill)+" · "+keys.Label(5+slot)+" 칸에 장착했습니다.";
            else skillWindowNotice="공격·시전·구르기가 끝난 뒤 장착할 수 있습니다.";
            Block();
        }
        void RemoveSelectedSkill()
        {
            int slot=run.EquippedSlot(selectedSkill);
            if(run.UnequipSkill(slot))skillWindowNotice="장착을 해제했습니다. 획득한 스킬은 목록에 남습니다.";
            Block();
        }
        string SkillUseHint(SpellKind spell)
        {
            int slot=run.EquippedSlot(spell);return slot>=0?keys.Label(5+slot)+" 사용":"K 스킬창에서 장착";
        }
        static int SkillSymbol(SpellKind spell)=>LateSpellCatalog.Contains(spell)?8+LateSpellCatalog.Slot(spell):spell==SpellKind.EmberWave?1:spell==SpellKind.AshLance?4:spell==SpellKind.CrimsonNova?5:spell==SpellKind.ChainPull||spell==SpellKind.ChainRain?6:7;
        void DrawEquippedSkillSlots()
        {
            for(int i=0;i<4;i++)
            {
                var spell=run.EquippedSkill(i);var rect=skillButtons[i];
                if(spell.HasValue)DrawEquippedSkillIcon(rect,spell.Value,i);
                else
                {
                    GothicDisc(rect.center.x,625,31,HudIron);GothicDisc(rect.center.x,625,28,HudInk);
                    Text("빈 칸",rect.x,608,rect.width,32,19,new Color(.55f,.6f,.64f),TextAnchor.MiddleCenter);
                    Text(keys.Label(5+i),rect.x,659,rect.width,19,14,HudLight,TextAnchor.MiddleCenter);
                    Text("K · 스킬 장착",rect.x,690,rect.width,20,12,new Color(.6f,.65f,.68f),TextAnchor.MiddleCenter);
                }
            }
        }
        void DrawSkillWindow()
        {
            Overlay();GothicPlaque(new Rect(55,30,1170,660),18,HudGold);GothicPlaque(new Rect(58,33,1164,654),16,HudInk);
            Text("별실의 기술 · 장착 스킬 4칸",90,55,820,48,31,HudLight);
            EquipmentButton("닫기 · K / Esc",new Rect(1010,60,175,43),ToggleSkillWindow);
            for(int i=0;i<4;i++)
            {
                DrawSkillWindowSlot(i,run.EquippedSkill(i),95+i*275);
            }
            var learned=new List<SpellKind>(run.LearnedSkills);
            Text("획득한 스킬 · "+learned.Count,95,197,430,32,22,HudLight);
            for(int i=0;i<learned.Count;i++)
            {
                DrawLearnedSkillRow(learned[i],237+i*44);
            }
            GothicPlaque(new Rect(565,205,620,315),15,HudIron);
            DrawSkillIcon(new Rect(588,221,64,64),selectedSkill);
            Text(SpellLabel(selectedSkill),670,219,485,43,31,HudLight);
            int equipped=run.EquippedSlot(selectedSkill);
            Text(equipped>=0?"장착 중 · "+(equipped+1)+"번 슬롯 · "+keys.Label(5+equipped):"미장착 · 아래에서 장착할 칸을 고르세요",670,264,485,25,17,equipped>=0?HudLight:new Color(.7f,.75f,.78f));
            Text(BossSkillCatalog.Description(selectedSkill),590,297,560,98,23,Color.white);
            Text($"기본 쿨타임 {RunModel.BaseSkillReuse(selectedSkill):0.#}초  →  장비 적용 {run.SkillReuse(selectedSkill):0.#}초",590,401,570,34,21,new Color(.75f,.82f,.83f));
            float cooldown=run.SkillCooldown(selectedSkill);
            Text(cooldown>0?$"남은 쿨타임 {Mathf.Ceil(cooldown*10)/10:0.0}초":"남은 쿨타임 0초 · 사용 준비",590,442,570,34,23,cooldown>0?SkillWaiting:SkillReady);
            DrawSkillCooldownBar(new Rect(590,487,565,10),selectedSkill);
            Text("장착할 칸을 선택하세요 · 이미 장착된 스킬은 칸끼리 교환",575,528,610,30,17,HudLight);
            for(int i=0;i<4;i++)
            {
                int slot=i;
                if(equipped==i)GothicPlaque(new Rect(572+i*153,563,149,50),9,HudLight);
                EquipmentButton((equipped==i?"장착 중 · ":(i+1)+"번 · ")+keys.Label(5+i),new Rect(575+i*153,566,143,44),run.CanChangeSkills?()=>EquipSelectedSkill(slot):(System.Action)null);
            }
            EquipmentButton("선택 스킬 해제",new Rect(575,621,215,40),run.CanChangeSkills&&run.EquippedSlot(selectedSkill)>=0?RemoveSelectedSkill:(System.Action)null);
            Text("스킬창에서는 시간이 멈춥니다.\n교체해도 남은 쿨타임은 유지됩니다.",810,618,375,49,16,new Color(.66f,.73f,.76f));
            Text(!run.CanChangeSkills?"공격·시전·구르기 중 또는 모험 종료 후에는 열람만 가능합니다. 창을 닫고 동작을 마치세요.":skillWindowNotice,95,614,445,57,17,new Color(.94f,.75f,.45f));
        }
    }
}
