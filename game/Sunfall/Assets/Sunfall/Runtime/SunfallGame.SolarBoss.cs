using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        Texture2D solarBossSheet;
        float nextSolarBossRetry;
        static readonly HeroFrame[] SolarBossFrames={
            new HeroFrame(0,0,444,443,255,407),new HeroFrame(444,0,443,443,692,407),
            new HeroFrame(887,0,444,443,1123,407),new HeroFrame(1331,0,443,443,1565,407),
            new HeroFrame(0,443,444,444,282,810),new HeroFrame(444,443,443,444,690,810),
            new HeroFrame(887,443,444,444,1138,810),new HeroFrame(1331,443,443,444,1574,810)
        };
        void LoadSolarBossArt()
        {
            solarBossSheet=Resources.Load<Texture2D>("Demons/Stage3SolarRegentV1");
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
            if(texture==null){texture=chainBossSheet;frame=ChainBossFrames[pose];scale=.36f;}
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
                Box(new Rect(left,ground-41,width,41),new Color(gold.r,gold.g,gold.b,warning?.12f:.45f*progress));
                Box(new Rect(left,ground-5,width,5),gold);
                if(warning)Box(new Rect(left,ground-9,width*progress,3),gold);
                else for(int side=-1;side<=1;side+=2)
                {
                    float x=SX(boss.X)+side*(1-progress)*boss.SolarSweepReach*34;
                    GothicPolygon(gold,new Vector2(x-20,ground),new Vector2(x,ground-39),new Vector2(x+20,ground));
                }
                label="태양 베기 · 점프 / 발판으로";
            }
            else if(boss.Pattern==BossPattern.SolarLances)
            {
                for(int i=0;i<boss.SolarLanceCount;i++)
                {
                    float x=SX(boss.SolarLanceX(i)),top=SY(9),width=.7f*68;
                    Box(new Rect(x-width/2,top,width,ground-top),new Color(gold.r,gold.g,gold.b,warning?.09f:.35f*progress));
                    Box(new Rect(x-width/2,ground-6,width,6),gold);
                    if(warning)
                    {
                        for(float y=top;y<ground;y+=24){Box(new Rect(x-width/2,y,2,9),gold);Box(new Rect(x+width/2-2,y,2,9),gold);}
                        Box(new Rect(x-width/2,ground-10,width*progress,3),gold);
                    }
                    else
                    {
                        GothicLine(new Vector2(x,top),new Vector2(x,ground-20),gold,5);
                        GothicPolygon(gold,new Vector2(x-17,ground-29),new Vector2(x,ground),new Vector2(x+17,ground-29));
                    }
                }
                label="왕실 광창 · 표시된 기둥 사이로";
            }
            else
            {
                float left=SX(boss.AimX-boss.EclipseSafeRadius),right=SX(boss.AimX+boss.EclipseSafeRadius),top=SY(10);
                float a=Mathf.Clamp(left,0,1280),b=Mathf.Clamp(right,0,1280);
                Color danger=new Color(.85f,.13f,.18f,warning?.13f:.45f*progress),safe=new Color(.52f,1,.8f);
                Box(new Rect(0,top,a,ground-top),danger);Box(new Rect(b,top,1280-b,ground-top),danger);
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
            float x=SX(32),y=SY(7),opening=run.BossDefeated?Mathf.Clamp01((BossDefeatElapsed-.55f)/1.3f):0;
            GothicDisc(x,y,85,new Color(.48f+opening*.35f,.33f+opening*.3f,.16f));
            GothicDisc(x,y,73,new Color(.04f+opening*.65f,.03f+opening*.41f,.065f+opening*.12f));
            for(int side=-1;side<=1;side+=2)
            {
                var color=new Color(.56f,.42f,.25f,1-opening);
                DrawChainLinks(new Vector2(x+side*(125+opening*60),y-90),new Vector2(x-side*70,y+80+opening*100),color,18);
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
