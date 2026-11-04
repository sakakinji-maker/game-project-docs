using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        Texture2D solarBossSheet;
        float nextSolarBossRetry;
        static readonly HeroFrame[] SolarBossFrames={
            new HeroFrame(0,0,444,444,249,404),new HeroFrame(444,0,443,444,690,404),
            new HeroFrame(887,0,443,444,1135,404),new HeroFrame(1330,0,444,444,1577,405),
            new HeroFrame(0,444,444,443,255,846),new HeroFrame(444,444,490,443,690,846),
            new HeroFrame(960,444,370,443,1126,846),new HeroFrame(1330,444,444,443,1572,846)
        };
        void LoadSolarBossArt()
        {
            solarBossSheet=Resources.Load<Texture2D>("Demons/Stage3AurelEclipseV2");
            if(solarBossSheet!=null){solarBossSheet.filterMode=FilterMode.Point;solarBossSheet.wrapMode=TextureWrapMode.Clamp;}
            nextSolarBossRetry=Time.realtimeSinceStartup+1;
        }
        void DrawSolarRegent(Enemy boss)
        {
            if(boss.Hp<=0&&boss.DeathTimer<=0)return;
            if(solarBossSheet==null&&Time.realtimeSinceStartup>=nextSolarBossRetry)LoadSolarBossArt();
            // Eclipse warnings cover the whole room even if the boss is off camera.
            DrawSolarTelegraph(boss);
            var p=RenderPosition(boss,boss.X,boss.Y);float x=SX(p.x),y=SY(p.y);
            if(x<-230||x>1510)return;
            float reveal=run.BossIntroducing?Mathf.SmoothStep(0,1,Mathf.Clamp01((BossIntroElapsed-.6f)/.8f)):1;
            float alpha=(boss.Hp<=0?Mathf.Clamp01(1-BossDefeatElapsed/1.4f):1)*reveal;
            int pose=boss.PhaseTransition>0?7:boss.Windup>0?(boss.Pattern==BossPattern.SolarSweep?4:boss.Pattern==BossPattern.SolarLances?6:7):
                boss.AttackPose>0?(boss.Pattern==BossPattern.SolarSweep?5:boss.Pattern==BossPattern.SolarLances?6:7):boss.Walking?2+(int)(boss.AnimationTime*6)%2:(int)(boss.AnimationTime*2)%2;
            var texture=solarBossSheet;var frame=SolarBossFrames[pose];float scale=.42f;
            if(texture==null)return;
            var matrix=GUI.matrix;var color=GUI.color;var pivot=new Vector3(x,y,0);
            GUI.matrix=matrix*Matrix4x4.Translate(pivot)*Matrix4x4.Scale(new Vector3(boss.Facing,1,1))*Matrix4x4.Translate(-pivot);
            GUI.color=boss.Flash>0?new Color(1,.5f,.3f,alpha):boss.PhaseTwo?new Color(1,.74f,.56f,alpha):new Color(1,1,1,alpha);
            DrawCharacterPose(texture,frame,new Vector2(1774,887),scale,x,y);
            GUI.matrix=matrix;GUI.color=color;
        }
        void DrawSolarTelegraph(Enemy boss)
        {
            if(boss.Hp<=0)return;
            Color gold=boss.PhaseTwo?new Color(1,.38f,.19f):new Color(1,.76f,.3f);
            if(boss.PhaseTransition>0)
            {
                Text("2페이즈 · 빼앗긴 태양",380,218,520,36,25,gold,TextAnchor.MiddleCenter);return;
            }
            bool warning=boss.Windup>0,active=boss.AttackPose>0;if(!warning&&!active)return;
            float progress=warning?1-Mathf.Clamp01(boss.Windup/boss.WindupDuration):boss.AttackPose/boss.AttackReleaseDuration;
            float ground=SY(0);string label;
            if(boss.Pattern==BossPattern.SolarSweep)
            {
                float left=SX(boss.X-boss.SolarSweepReach),width=boss.SolarSweepReach*68;
                if(warning)
                {
                    DrawBossRune(SX(boss.X),ground,width,progress,gold);
                    Box(new Rect(left,ground-5,width,3),new Color(gold.r,gold.g,gold.b,.55f));
                }
                else DrawBossGroundBurst(left,width,ground,41,progress,gold);
                label="태양 베기 · 점프 / 발판으로";
            }
            else if(boss.Pattern==BossPattern.SolarLances)
            {
                for(int i=0;i<boss.SolarLanceCount;i++)
                {
                    float x=SX(boss.SolarLanceX(i)),top=SY(9),width=.7f*68;
                    if(warning)DrawBossColumnWarning(x,top,ground,width,progress,gold);
                    else
                    {
                        DrawBossPillar(x,top,ground,width,progress,gold,i*1.7f);
                        DrawBossImpact(new Vector2(x,ground-5),width*1.7f,progress,gold);
                    }
                }
                label="왕실 광창 · 표시된 기둥 사이로";
            }
            else
            {
                float left=SX(boss.AimX-boss.EclipseSafeRadius),right=SX(boss.AimX+boss.EclipseSafeRadius),top=SY(10);
                float a=Mathf.Clamp(left,0,1280),b=Mathf.Clamp(right,0,1280);
                Color danger=new Color(.85f,.13f,.18f,warning?.13f:.45f*progress),safe=new Color(.52f,1,.8f);
                if(warning){Box(new Rect(0,top,a,ground-top),danger);Box(new Rect(b,top,1280-b,ground-top),danger);}
                else {DrawBossCurtain(0,a,top,ground,progress,gold);DrawBossCurtain(b,1280,top,ground,progress,gold);}
                Box(new Rect(left,top,right-left,ground-top),new Color(safe.r,safe.g,safe.b,.1f));
                Box(new Rect(left,top,3,ground-top),safe);Box(new Rect(right-3,top,3,ground-top),safe);
                Box(new Rect(left,ground-6,right-left,6),safe);
                Text("안전",left,ground-46,right-left,30,20,safe,TextAnchor.MiddleCenter);
                // Point to the safe band when it falls beyond the current camera view.
                if(right<0||left>1280)Text(right<0?"← 안전지대":"안전지대 →",right<0?25:1015,260,240,38,24,safe,TextAnchor.MiddleCenter);
                label="흑일 심판 · 초록빛 안전지대 안으로";
            }
            if(warning)Text(label,340,218,600,34,23,gold,TextAnchor.MiddleCenter);
        }
        void DrawSolarSeal()
        {
            if(run.Chapter!=3||run.Stage!=RunStage.BossArena)return;
            // Pin the seal animation to the astrolabe in the panorama rather
            // than to world-space stone blocks or the moving boss.
            float x=CathedralBackdropWidth*.5f-CathedralBackdropOffset;
            float y=CathedralBackdropTop+230f*(CathedralBackdropWidth/2172)+cameraY*8;
            float opening=run.BossDefeated?Mathf.Clamp01((BossDefeatElapsed-.55f)/1.3f):0;
            if(opening<1)
            {
                EnsurePresentationArt();
                for(int side=-1;side<=1;side+=2)
                    DrawAnimatedChain(new Vector2(x+side*145,y-95),
                        new Vector2(x-side*110,y+90+opening*70),1-opening,3,side*.3f,false);
            }
            if(opening>0)
            {
                for(int i=0;i<11;i++)
                {
                    float py=y-60+i*12;
                    GothicLine(new Vector2(x-17,py+6),new Vector2(x+17,py-6),new Color(1,.87f,.52f,opening),2);
                }
                Text("봉인의 매듭이 풀렸습니다",x-200,y+100,400,32,21,HudLight,TextAnchor.MiddleCenter);
            }
        }
    }
}
