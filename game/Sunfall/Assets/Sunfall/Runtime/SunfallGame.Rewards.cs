using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        Vector2 defeatCameraStart;
        Texture2D rewardChest,healingPotion;
        float nextRewardArtRetry;
        float BossDefeatElapsed=>Mathf.Clamp(RunModel.BossDefeatDuration-run.BossDefeatRemaining-(1-MotionAlpha)*RunModel.StepSeconds,0,RunModel.BossDefeatDuration);
        static readonly HeroFrame[] RewardFrames={
            new HeroFrame(40,190,800,610,450,757),
            new HeroFrame(920,60,820,740,1337,757)
        };
        void EnsureRewardArt()
        {
            if((rewardChest!=null&&healingPotion!=null)||Time.realtimeSinceStartup<nextRewardArtRetry)return;
            rewardChest=Resources.Load<Texture2D>("World/RewardChestV1");
            healingPotion=Resources.Load<Texture2D>("World/HealingPotionV1");
            foreach(var texture in new[]{rewardChest,healingPotion})
                if(texture!=null){texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;}
            nextRewardArtRetry=Time.realtimeSinceStartup+1;
        }
        void DrawBossDefeatParticles()
        {
            float t=BossDefeatElapsed,x=SX(run.BossDefeatX),y=SY(run.BossDefeatY)-55;
            for(int i=0;i<72;i++)
            {
                float start=(i%9)*.07f,age=t-start;
                if(age<0||age>2.2f)continue;
                float angle=i*2.39996f;
                float spread=18+age*(24+i%7*5);
                float px=x+Mathf.Cos(angle)*spread,py=y+Mathf.Sin(angle)*spread*.55f-age*24;
                float alpha=Mathf.Clamp01(1-age/2.2f),size=i%4==0?5:3;
                GothicDiamond(px,py,size,new Color(1,i%3==0?.62f:.18f,.35f,alpha));
            }
        }
        void DrawBossDefeatOverlay()
        {
            hits.Clear();float fade=Mathf.Clamp01(BossDefeatElapsed/.25f)*Mathf.Clamp01(run.BossDefeatRemaining/.4f);
            Box(new Rect(0,0,1280,132),new Color(.02f,.012f,.03f,.93f*fade));
            Box(new Rect(0,580,1280,140),new Color(.02f,.012f,.03f,.96f*fade));
            string status=run.Chapter>=4?(run.ChestAvailable?"보물상자 · "+NextRoomTitle:run.ExitOpen?"봉합된 길에 포탈이 열렸습니다":"왕의 매듭이 풀리고 금빛 별실이 남습니다"):run.Chapter==3?(run.ChestAvailable?"집행관의 보물상자 · 심장부로 향하는 포탈 개방":run.ExitOpen?"봉인 너머로 어머니의 목소리가 들립니다":"왕실 문장의 틈을 금빛 별실로 꿰맵니다"):run.Chapter==2?(run.ChestAvailable?"간수의 보물상자 · 왕도 대성당으로 떠날 준비":run.ExitOpen?"왕도로 이어지는 별실을 되찾았습니다":"양어머니를 붙잡은 성채의 사슬이 끊어집니다"):run.ChestAvailable?"봉합 자국을 남기고 · 성채로 떠날 준비":run.ExitOpen?"첫 균열 봉합 완료 · 검은 성채로 향하는 길":SkySeamProgress>=1?"하늘에 금빛 봉합 자국이 남았습니다":"되찾은 별실로 첫 균열을 봉합합니다";
            Text(run.Chapter>=4?LateDefeatHeading:run.Chapter==3?"어머니에게 닿은 바늘땀":run.Chapter==2?"왕실의 사슬 해방":"첫 번째 바늘땀",320,605,640,48,36,new Color(1,.76f,.6f,fade),TextAnchor.MiddleCenter);
            Text(status,260,658,760,30,20,new Color(.9f,.8f,.75f,fade),TextAnchor.MiddleCenter);
            if(BossDefeatElapsed>=1.2f&&run.SkillChoicePending)Text("연출이 끝나면 별실의 짜임 하나를 선택하세요",260,70,760,38,26,new Color(1,.76f,.4f,fade),TextAnchor.MiddleCenter);
            GothicLine(new Vector2(380,590),new Vector2(900,590),new Color(.75f,.25f,.35f,fade),1);
        }
        void DrawRewardChest()
        {
            if(!run.ChestAvailable)return;
            EnsureRewardArt();float x=SX(run.ChestX),y=SY(0);
            if(x<-100||x>1380)return;
            float reveal=Mathf.Clamp01((.7f-run.BossDefeatRemaining)/.4f);
            if(rewardChest!=null)
            {
                var frame=RewardFrames[run.ChestOpened?1:0];var color=GUI.color;
                GUI.color=new Color(1,1,1,reveal);
                DrawHeroRegion(rewardChest,new Vector2(1774,887),frame.Source,new Vector2(x-frame.Feet.x*.1f,y-frame.Feet.y*.1f),.1f);
                GUI.color=color;
            }
            DrawNameplateText(run.ChestOpened?"보상 획득 완료 · I 장비 확인":run.CanOpenChest?"↑ 상자 열기 · 물약 + 장비":"보물상자",x,y-103,340,30,17,HudLight);
        }
        void DrawHealingNotice()
        {
            if(run.HealingNotice<=0)return;
            EnsureRewardArt();var panel=new Rect(380,210,520,116);
            GothicPlaque(panel,12,HudGold);GothicPlaque(new Rect(382,212,516,112),10,HudInk);
            if(healingPotion!=null)GUI.DrawTexture(new Rect(410,214,64,58),healingPotion,ScaleMode.ScaleToFit,true);
            Text(run.LastHealing>0?$"회복 물약 즉시 사용 · HP +{run.LastHealing:0}":"물약 사용 완료 · 체력이 가득합니다",478,222,390,42,20,new Color(.71f,.94f,.78f),TextAnchor.MiddleCenter);
            if(run.LastEquipmentReward!=EquipmentId.None)DrawEquipmentIcon(run.LastEquipmentReward,new Vector2(427,291),HudLight);
            Text(run.LastEquipmentReward!=EquipmentId.None?EquipmentCatalog.Name(run.LastEquipmentReward)+" 획득 · I 장착":"장비 5종 수집 완료",460,276,420,32,18,HudLight,TextAnchor.MiddleCenter);
        }
        string BossSpellLabel=>run.BossSpellReward==SpellKind.AshLance?"잿불 창":run.BossSpellReward==SpellKind.CrimsonNova?"홍염 고리":"";
        static string SpellLabel(SpellKind spell)=>LateSpellCatalog.Contains(spell)?LateSpellCatalog.Name(spell):spell==SpellKind.SunLance?"태양의 창":spell==SpellKind.DawnBurst?"여명의 폭발":spell==SpellKind.ChainPull?"포박 사슬":spell==SpellKind.ChainRain?"사슬 강림":spell==SpellKind.AshLance?"잿불 창":spell==SpellKind.CrimsonNova?"홍염 고리":"불씨 파동";
        string AllRewardSpellLabels=>BossSpellLabel+(run.ChainSpellReward.HasValue?" / "+SpellLabel(run.ChainSpellReward.Value):"")+(run.SolarSpellReward.HasValue?" / "+SpellLabel(run.SolarSpellReward.Value):"")+LateRewardLabels;
        void DrawSpellRewardNotice()
        {
            if(run.SpellRewardNotice<=0||!run.LatestSpellReward.HasValue)return;
            GothicPlaque(new Rect(330,143,620,56),12,HudGold);
            GothicPlaque(new Rect(332,145,616,52),10,HudInk);
            var reward=run.LatestSpellReward.Value;
            string key=SkillUseHint(reward);
            Text($"{SpellLabel(reward)} 획득 · {key} · 이번 모험 동안 유지",340,151,600,40,21,HudLight,TextAnchor.MiddleCenter);
        }
        void DrawCrimsonNova()
        {
            if(run.NovaTimer<=0)return;
            float alpha=Mathf.Clamp01(run.NovaTimer/.45f);
            Vector2 c=new Vector2(SX(run.NovaX),SY(run.NovaY));float radius=RunModel.NovaRadius*34;
            DrawSpellEffect(SpellKind.CrimsonNova,c,Vector2.one*(radius*2.25f),alpha);
        }
    }
}
