using UnityEngine;
using UnityEngine.InputSystem;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        bool CanVillageInteract=>village&&run!=null&&run.Grounded&&!run.Casting&&run.MagicRecoveryTimer<=0&&!paused&&!settings&&!StoryActive&&!inventoryOpen&&!skillWindowOpen&&serviceNpc<0;
        void UpdateVillageActions()
        {
            ReadSkillInput();
            bool l=keys.Held(0)||(TouchControls&&TouchHeld(left));
            bool r=keys.Held(1)||(TouchControls&&TouchHeld(right));
            bool j=keys.Held(3)||(TouchControls&&TouchHeld(jump));
            queuedJump|=(j&&!previousJump)||keys.Pressed(3);previousJump=j;
            accumulator+=Mathf.Min(Time.unscaledDeltaTime,.1f);
            while(accumulator>=RunModel.StepSeconds)
            {
                float previousX=run.X;CaptureWorldMotion();
                run.Step((r?1:0)-(l?1:0),queuedJump,false,skillSlot:QueuedSkillSlot());
                TickHeroVisuals(run.X-previousX,RunModel.StepSeconds);
                queuedJump=false;for(int i=0;i<4;i++)queuedSkill[i]=false;
                accumulator-=RunModel.StepSeconds;
            }
            villageX=SX(run.X);villageFacing=run.Facing;
        }
        void DrawVillageActions()
        {
            GUI.BeginGroup(new Rect(0,0,1280,578));
            var position=RenderPosition(run,run.X,run.Y);
            float x=SX(position.x),y=SY(position.y);
            DrawHero(x,y,run.Facing,false);
            DrawPlayerSpellEffects(x,y);
            GUI.EndGroup();
        }
        void DrawPlayerSpellEffects(float x,float y)
        {
            if(run.Casting)
            {
                float size=34+(1-Mathf.Clamp01(run.CastTimer/RunModel.MagicCastDuration))*20;
                DrawCastAnimation(new Vector2(x+run.ActionFacing*25,y-30),size);
            }
            foreach(var wave in run.Waves)
            {
                var position=RenderPosition(wave,wave.X,wave.Y);
                var kind=wave.Solar?SpellKind.SunLance:wave.Lance?SpellKind.AshLance:SpellKind.EmberWave;
                DrawSpellEffect(kind,new Vector2(SX(position.x),SY(position.y)),
                    wave.Solar||wave.Lance?new Vector2(125,64):new Vector2(76,60),facing:wave.Facing);
            }
            DrawCrimsonNova();DrawPlayerChainSpell();DrawDawnBurst();DrawLateMagic();
        }
        void DrawVillageActionBar()
        {
            DrawEquippedSkillSlots();
            DrawGothicSkill(jump,"점프",keys.Label(3),3,0,1,!run.Grounded);
            if(TouchControls){DrawGothicDirection(left,"←");DrawGothicDirection(right,"→");}
            else
                Text($"{keys.Label(0)} / {keys.Label(1)} 이동 · {keys.Label(3)} 점프\nSpace 대화 · K 스킬창\n↑ 문 앞 출발 · Esc 정지",24,588,220,92,14);
            if(CanLeaveVillage)
                EquipmentButton("↑ 숲으로 입장",new Rect(250,682,190,30),()=>{if(CanLeaveVillage)StartRun();});
            else if(TouchControls)
            {
                int npc=NearestVillageNpc();
                if(npc>=0)EquipmentButton("대화 / 거래",new Rect(250,682,190,30),()=>BeginVillageDialogue(npc));
            }
        }
    }
}
