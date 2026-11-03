using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        Texture2D chainBossSheet;
        float nextChainBossRetry;
        static readonly HeroFrame[] ChainBossFrames={
            // Measured 1774x887 sheet. Each row has a different transparent
            // bottom margin; explicit foot baselines prevent floating on attacks.
            new HeroFrame(4,4,435.5f,435.5f,221.75f,421),new HeroFrame(447.5f,4,435.5f,435.5f,665.25f,421),
            new HeroFrame(891,4,435.5f,435.5f,1108.75f,421),new HeroFrame(1334.5f,4,435.5f,435.5f,1552.25f,421),
            new HeroFrame(4,447.5f,435.5f,435.5f,221.75f,828),new HeroFrame(447.5f,447.5f,435.5f,435.5f,665.25f,828),
            new HeroFrame(891,447.5f,435.5f,435.5f,1108.75f,828),new HeroFrame(1334.5f,447.5f,435.5f,435.5f,1552.25f,828)
        };
        void LoadChainBossArt()
        {
            chainBossSheet=Resources.Load<Texture2D>("Demons/Stage2MorgaHuskV2");
            if(chainBossSheet!=null){chainBossSheet.filterMode=FilterMode.Point;chainBossSheet.wrapMode=TextureWrapMode.Clamp;}
            nextChainBossRetry=Time.realtimeSinceStartup+1;
        }
        void DrawChainWarden(Enemy boss)
        {
            if(boss.Hp<=0&&boss.DeathTimer<=0)return;
            if(chainBossSheet==null&&Time.realtimeSinceStartup>=nextChainBossRetry)LoadChainBossArt();
            var p=RenderPosition(boss,boss.X,boss.Y);float x=SX(p.x),y=SY(p.y);
            if(x<-230||x>1510)return;
            DrawChainBossTelegraph(boss,x,y);
            float reveal=run.BossIntroducing?Mathf.SmoothStep(0,1,Mathf.Clamp01((BossIntroElapsed-MorgaChainBreakTime)/.75f)):1;
            if(run.BossIntroducing)y-=(1-reveal)*55;
            float alpha=(boss.Hp<=0?Mathf.Clamp01(1-BossDefeatElapsed/1.4f):1)*reveal;
            int pose=boss.PhaseTransition>0?7:boss.Windup>0?(boss.Pattern==BossPattern.ChainSlam?4:boss.Pattern==BossPattern.ChainBind?6:7):
                boss.AttackPose>0?(boss.Pattern==BossPattern.ChainSlam?5:boss.Pattern==BossPattern.ChainBind?6:7):boss.Walking?2+(int)(boss.AnimationTime*6)%2:(int)(boss.AnimationTime*2)%2;
            if(run.BossIntroducing)pose=BossIntroElapsed<2.05f?7:0;
            Texture2D texture=chainBossSheet;HeroFrame frame=ChainBossFrames[pose];Vector2 size=new Vector2(1774,887);float scale=.40f;
            // Keep an existing demon visible while Unity imports the new sprite sheet.
            if(texture==null&&demonSheets!=null&&demonSheets[2]!=null)
            {texture=demonSheets[2];frame=DemonFrames[2][0];size=new Vector2(1254,1254);scale=.30f;}
            if(texture==null)return;
            var matrix=GUI.matrix;var color=GUI.color;var pivot=new Vector3(x,y,0);
            GUI.matrix=matrix*Matrix4x4.Translate(pivot)*Matrix4x4.Scale(new Vector3(boss.Facing,1,1))*Matrix4x4.Translate(-pivot);
            GUI.color=boss.Flash>0?new Color(1,.55f,.35f,alpha):boss.PhaseTwo?new Color(1,.68f,.62f,alpha):new Color(1,1,1,alpha);
            DrawCharacterPose(texture,frame,size,scale,x,y);
            GUI.matrix=matrix;GUI.color=color;
            DrawMorgaRestraints();
        }
        void DrawChainBossTelegraph(Enemy boss,float x,float y)
        {
            if(boss.Hp<=0)return;
            Color hot=boss.PhaseTwo?new Color(1,.2f,.3f):new Color(1,.67f,.25f);
            if(boss.PhaseTransition>0)
            {
                Text("2페이즈 · 왕실의 속박",x-180,y-179,360,34,22,hot,TextAnchor.MiddleCenter);
                for(int i=0;i<8;i++){float a=i*Mathf.PI/4;GothicDiamond(x+Mathf.Cos(a)*70,y-68+Mathf.Sin(a)*60,5,hot);}
                return;
            }
            bool warning=boss.Windup>0,active=boss.AttackPose>0;
            if(!warning&&!active)return;
            float amount=warning?1-Mathf.Clamp01(boss.Windup/boss.WindupDuration):boss.AttackPose/boss.AttackReleaseDuration;
            string label;
            if(boss.Pattern==BossPattern.ChainSlam)
            {
                float left=SX(boss.AimX-boss.ChainSlamHalfWidth),width=boss.ChainSlamHalfWidth*68,ground=SY(0);
                Box(new Rect(left,ground-41,width,41),new Color(hot.r,hot.g,hot.b,warning?.14f:.32f*amount));
                Box(new Rect(left,ground-4,width,4),new Color(hot.r,hot.g,hot.b,.7f));
                if(warning)Box(new Rect(left,ground-7,width*amount,3),hot);
                else
                {
                    DrawChainLinks(new Vector2(x+boss.Facing*25,y-75),new Vector2(SX(boss.AimX),ground-12),hot,12);
                    GothicDiamond(SX(boss.AimX),ground-16,17,hot);
                }
                label="사슬 내려치기 · 점프 / 발판";
            }
            else if(boss.Pattern==BossPattern.ChainBind)
            {
                for(int i=0;i<boss.ChainBindCount;i++)DrawChainColumn(boss.ChainBindX(i),.6f,warning,amount,hot);
                label="구속 사슬 · 표시 구역 밖으로";
            }
            else label=boss.PhaseTwo?"왕실 사슬 낙하 · 7갈래":"왕실 사슬 낙하 · 5갈래";
            if(warning)Text(label,x-200,y-162,400,28,18,hot,TextAnchor.MiddleCenter);
        }
        void DrawChainColumn(float worldX,float halfWidth,bool warning,float progress,Color color)
        {
            float x=SX(worldX),top=SY(RoyalChain.Height),bottom=SY(0),width=halfWidth*68;
            if(x+width<0||x-width>1280)return;
            Box(new Rect(x-width*.5f,top,width,bottom-top),new Color(color.r,color.g,color.b,warning?.08f:.24f));
            if(warning)
            {
                Box(new Rect(x-width*.5f,bottom-6,width,6),color);
                Box(new Rect(x-width*.5f,bottom-10,width*Mathf.Clamp01(progress),3),color);
                for(float y=top;y<bottom;y+=22){Box(new Rect(x-width*.5f,y,2,8),color);Box(new Rect(x+width*.5f-2,y,2,8),color);}
            }
            else
            {
                DrawChainLinks(new Vector2(x,top),new Vector2(x,bottom),color,25);
                GothicDiamond(x,bottom-10,12,color);
            }
        }
        void DrawChainLinks(Vector2 from,Vector2 to,Color color,int count)
        {
            GothicLine(from,to,new Color(.04f,.035f,.05f,color.a),6);
            for(int i=0;i<=count;i++)
            {
                Vector2 p=Vector2.Lerp(from,to,i/(float)count);
                GothicDiamond(p.x,p.y,5,color);GothicDiamond(p.x,p.y,2,new Color(.06f,.04f,.07f,color.a));
            }
        }
        void DrawRoyalChainHazards()
        {
            foreach(var chain in run.RoyalChains)
                DrawChainColumn(chain.X,RoyalChain.HalfWidth,chain.Delay>0,chain.Delay>0?1-chain.Delay/1.2f:1,new Color(1,.32f,.25f));
            if(run.SnareTimer>0)
            {
                Vector2 feet=new Vector2(SX(run.X),SY(run.Y));
                DrawChainLinks(feet+new Vector2(-18,-18),feet+new Vector2(18,-5),new Color(1,.45f,.2f),5);
                Text("구속 · 구르기로 해제",feet.x-110,feet.y-142,220,30,17,HudLight,TextAnchor.MiddleCenter);
            }
        }
        void DrawRoyalBindings()
        {
            float breakProgress=run.BossDefeated?Mathf.Clamp01((BossDefeatElapsed-.55f)/1.3f):0;
            float center=SX(32),top=SY(7);
            GothicPolygon(HudGold,new Vector2(center-42,top),new Vector2(center-40,top-35),new Vector2(center-18,top-15),new Vector2(center,top-48),new Vector2(center+18,top-15),new Vector2(center+40,top-35),new Vector2(center+42,top));
            GothicDiamond(center,top+23,16,new Color(.53f,.16f,.3f));
            for(int side=-1;side<=1;side+=2)
            {
                Vector2 from=new Vector2(SX(32+side*9),SY(12)),to=new Vector2(SX(32+side*1.3f),SY(2.7f));
                for(int i=0;i<=30;i++)
                {
                    bool fragment=i>5&&i<25;float alpha=fragment?1-breakProgress:1;
                    if(alpha<=0)continue;
                    Vector2 p=Vector2.Lerp(from,to,i/30f);
                    if(fragment)p+=new Vector2(side*breakProgress*breakProgress*90,breakProgress*180);
                    GothicDiamond(p.x,p.y,9,new Color(.61f,.43f,.24f,alpha));
                    GothicDiamond(p.x,p.y,5,new Color(.065f,.045f,.08f,alpha));
                }
            }
            if(run.BossDefeated&&breakProgress>=1)Text("왕실의 사슬이 끊어졌습니다",center-200,top+58,400,30,20,HudLight,TextAnchor.MiddleCenter);
        }
    }
}
