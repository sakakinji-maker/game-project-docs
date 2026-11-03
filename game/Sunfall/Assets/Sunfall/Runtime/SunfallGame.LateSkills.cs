using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        bool ShowSkillChoice=>!title&&!village&&run!=null&&run.CanChooseBossSkill&&!StoryActive;
        void ChooseLateSkill(int option)
        {
            if(!ShowSkillChoice||paused||settings)return;
            bool chosen=option<0?run.ChooseRandomBossSkill():run.ChooseBossSkill(option);
            if(chosen&&run.LatestSpellReward.HasValue&&!run.IsSpellEquipped(run.LatestSpellReward.Value))
            {selectedSkill=run.LatestSpellReward.Value;skillWindowOpen=true;skillWindowNotice="새 스킬을 배웠습니다. 교체할 칸을 고르거나 기존 구성을 유지하세요.";}
            CaptureWorldMotion();Block();
        }
        void DrawBossSkillChoice()
        {
            Overlay();
            Text("별실에 새길 힘을 선택하세요",130,75,1020,60,36,HudLight,TextAnchor.MiddleCenter);
            Text(BossTitle+"의 두 가지 짜임",130,145,1020,40,23,Color.white,TextAnchor.MiddleCenter);
            for(int i=0;i<2;i++)
            {
                int option=i;var spell=BossSkillCatalog.Option(run.PendingSkillChapter,i);float x=145+i*510;
                GothicPlaque(new Rect(x,220,480,325),18,HudGold);GothicPlaque(new Rect(x+3,223,474,319),15,HudInk);
                DrawSkillIcon(new Rect(x+204,233,72,72),spell);
                Text(SpellLabel(spell),x+20,310,440,42,30,HudLight,TextAnchor.MiddleCenter);
                Text(BossSkillCatalog.Description(spell),x+25,371,430,85,22,Color.white,TextAnchor.MiddleCenter);
                EquipmentButton("이 스킬 습득",new Rect(x+65,475,350,48),()=>ChooseLateSkill(option));
            }
            EquipmentButton("운명에 맡기기 · 무작위 1개",new Rect(420,571,440,50),()=>ChooseLateSkill(-1));
            Text("획득한 스킬은 유지됩니다. 빈 칸에는 자동 장착되며, 가득 차면 스킬창에서 교체합니다.",125,645,1030,40,20,new Color(.7f,.75f,.77f),TextAnchor.MiddleCenter);
        }
        // HUD fallback only, used while the separate skill icon textures import.
        void DrawLateSpellSymbol(Vector2 c,int slot,Color color)
        {
            if(slot==0)
            {
                GothicDisc(c.x,c.y,14,color);GothicDisc(c.x,c.y,8,HudInk);
                for(int i=0;i<8;i++){float a=i*Mathf.PI/4;var d=new Vector2(Mathf.Cos(a),Mathf.Sin(a));GothicLine(c+d*12,c+d*22,color,4);}
            }
            else if(slot==1){GothicDiamond(c.x,c.y,20,color);GothicDiamond(c.x,c.y,14,HudInk);GothicLine(c+new Vector2(-6,9),c+new Vector2(7,-9),color,2);}
            else if(slot==2){GothicDisc(c.x,c.y,19,color);GothicDisc(c.x+9,c.y-6,16,HudInk);}
            else{DrawGothicSkillSymbol(c,7,color);GothicLine(c+new Vector2(-18,-15),c+new Vector2(0,-26),color,3);GothicLine(c+new Vector2(0,-26),c+new Vector2(18,-15),color,3);}
        }
        void DrawLateMagic()
        {
            foreach(var shot in run.LateShots)
            {
                var p=RenderPosition(shot,shot.X,shot.Y);float x=SX(p.x),y=SY(p.y);
                if(x<-50||x>1330)continue;
                float angle=Mathf.Atan2(-shot.VY,shot.VX)*Mathf.Rad2Deg;
                if(shot.Kind==SpellKind.CrownGear)angle=visualClock*240;
                DrawSpellEffect(shot.Kind,new Vector2(x,y),shot.Kind==SpellKind.MirrorShards?new Vector2(72,44):new Vector2(70,70),angle:angle);
            }
            foreach(var effect in run.LateEffects)
            {
                float x=SX(effect.X),y=SY(effect.Y),alpha=Mathf.Clamp01(effect.Remaining/.5f);
                if(effect.Kind==SpellKind.HeartBreaker||effect.Kind==SpellKind.RoyalLance)
                {
                    float width=(effect.Kind==SpellKind.HeartBreaker?5:18)*34;
                    DrawSpellEffect(effect.Kind,new Vector2(x+effect.Facing*width*.5f,y-12),new Vector2(width*1.12f,140),alpha,facing:effect.Facing);
                }
                else if(effect.Kind==SpellKind.SolarCrown||effect.Kind==SpellKind.NightMantle)
                {
                    float radius=(effect.Kind==SpellKind.SolarCrown?5:3.5f)*34;
                    DrawSpellEffect(effect.Kind,new Vector2(x,y),Vector2.one*(radius*2.25f),alpha);
                    if(effect.Kind==SpellKind.SolarCrown)
                    {
                        float pulse=1-Mathf.Clamp01(effect.PulseDelay/.6f);
                        DrawSpellEffect(SpellKind.DawnBurst,new Vector2(x,y),Vector2.one*(radius*2.25f*pulse),(1-pulse)*.55f);
                    }
                }
            }
            var player=RenderPosition(run,run.X,run.Y);var center=new Vector2(SX(player.x),SY(player.y+1));
            if(run.MirrorShieldTime>0&&run.MirrorShield>0)
            {
                DrawSpellEffect(SpellKind.MirrorWard,center,Vector2.one*112,.85f);
                Text("보호막 "+Mathf.CeilToInt(run.MirrorShield),center.x-65,center.y-74,130,24,16,new Color(.8f,.7f,1),TextAnchor.MiddleCenter);
            }
            if(run.NightMantleTime>0)
            {
                DrawSpellEffect(SpellKind.NightMantle,center,Vector2.one*124,.75f);
                Text("피해 감소 "+run.NightMantleTime.ToString("0.0"),center.x-75,center.y+48,150,24,16,new Color(.6f,.8f,1),TextAnchor.MiddleCenter);
            }
        }
        string LateRewardLabels
        {
            get{string result="";for(int i=0;i<4;i++)if(run.LateReward(i).HasValue)result+=" / "+SpellLabel(run.LateReward(i).Value);return result;}
        }
    }
}
