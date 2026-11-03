using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        string BossTitle=>run!=null&&run.Chapter>=4?RunModel.LateBossName(run.Chapter):run!=null&&run.Chapter==3?"태양 봉인의 집행관 · 아우렐":run!=null&&run.Chapter==2?"왕실 사슬의 간수 · 모르가":"붉은 날개의 수호자";
        Texture2D stage1BossSheet;
        float nextBossArtRetry;
        bool bossArtWarning;
        // Padded bounds against the original 1254px PNG. Root is aligned to the
        // pelvis horizontally and a shared virtual body base vertically, not the
        // moving feet or wing tips of an airborne pose.
        static readonly HeroFrame[] BossFrames={
            new HeroFrame(22,70,563,567,397,587),
            new HeroFrame(664,152,561,498,1070,625),
            new HeroFrame(32,701,618,514,397,1200),
            new HeroFrame(705,777,522,393,1020,1200)
        };
        static readonly float[] BossScales={.22f,.22f,.22f,.22f};
        void LoadBossArt()
        {
            LoadLateBossArt();LoadSolarBossArt();LoadChainBossArt();
            foreach(var clip in bossAttackClips)clip.Load();
            stage1BossSheet=Resources.Load<Texture2D>("Demons/Stage1BossFlight");
            if(stage1BossSheet!=null)
            {
                stage1BossSheet.filterMode=FilterMode.Point;stage1BossSheet.wrapMode=TextureWrapMode.Clamp;
                bossArtWarning=false;
            }
            nextBossArtRetry=Time.realtimeSinceStartup+1;
        }
        void DrawStage1Boss(Enemy boss)
        {
            if(boss.LateChapter>0){DrawLateBoss(boss);return;}
            if(boss.SolarRegent){DrawSolarRegent(boss);return;}
            if(boss.ChainWarden){DrawChainWarden(boss);return;}
            if((boss.Hp<=0&&boss.DeathTimer<=0)||SX(boss.X)<-180||SX(boss.X)>1460)return;
            bool missingAttack=bossAttackClips[0].Texture==null||bossAttackClips[1].Texture==null||bossAttackClips[2].Texture==null;
            if((stage1BossSheet==null||missingAttack)&&Time.realtimeSinceStartup>=nextBossArtRetry)LoadBossArt();
            var position=RenderPosition(boss,boss.X,boss.Y);float x=SX(position.x),feetY=SY(position.y);
            float reveal=run.BossIntroducing?Mathf.SmoothStep(0,1,Mathf.Clamp01((BossIntroElapsed-.7f)/.9f)):1;
            if(run.BossIntroducing)feetY+=(1-reveal)*26;
            Color crimson=new Color(.95f,.16f,.32f);
            DrawBossTelegraph(boss,x,feetY);
            if(boss.Hp>0&&boss.AttackPose>0&&boss.Pattern==BossPattern.GroundWave)
            {
                // The whole marked area hits on release; ripples are its visual decay.
                float remaining=boss.AttackPose/RunModel.BossReleaseDuration;
                float start=SX(boss.X-boss.AttackReach),width=boss.AttackReach*68;
                float groundY=SY(0);
                Box(new Rect(start,groundY-13,width,13),new Color(.95f,.12f,.3f,remaining*.65f));
                for(int side=-1;side<=1;side+=2)
                {
                    float waveX=x+side*(1-remaining)*boss.AttackReach*34;
                    Box(new Rect(waveX-9,groundY-24,18,24),new Color(1,.3f,.45f,remaining));
                    Box(new Rect(waveX-3,groundY-19,6,19),new Color(1,.8f,.85f,remaining));
                }
            }
            int pose=boss.PhaseTransition>0?2:boss.DashRemaining>0?3:boss.Windup>0||boss.AttackPose>0?2:(int)(boss.AnimationTime*4)%2;
            if(run.BossIntroducing&&BossIntroElapsed<1.8f)pose=2;
            float alpha=(boss.Hp<=0?Mathf.Clamp01(boss.DeathTimer/.6f):1)*reveal;
            if(boss.Hp<=0&&run.BossDefeated){alpha=Mathf.Clamp01(1-BossDefeatElapsed/1.4f);feetY-=BossDefeatElapsed*12;}
            if(stage1BossSheet==null)
            {
                if(!bossArtWarning){bossArtWarning=true;Debug.LogWarning("Sunfall: Stage1Boss texture unavailable; retrying with a visible fallback.",this);}
                Box(new Rect(x-22,feetY-100,44,100),new Color(.7f,.2f,.35f,alpha));
                return;
            }
            var item=BossFrames[pose];float scale=BossScales[pose];
            var texture=stage1BossSheet;var size=new Vector2(1254,1254);
            bool attacking=SelectBossAttack(boss,out var attackClip,out var attackPose);
            if(attacking){texture=attackClip.Texture;item=attackClip.Frames[attackPose];scale=attackClip.Scale;size=attackClip.Size;}
            var oldMatrix=GUI.matrix;var oldColor=GUI.color;var pivot=new Vector3(x,feetY,0);
            GUI.matrix=oldMatrix*Matrix4x4.Translate(pivot)*Matrix4x4.Scale(new Vector3(boss.Facing,1,1))*Matrix4x4.Translate(-pivot);
            GUI.color=boss.Hp<=0?new Color(1,.24f,.4f,alpha):boss.Flash>0?new Color(1,.5f,.6f,alpha):new Color(1,boss.PhaseTwo?.82f:1,boss.PhaseTwo?.88f:1,alpha);
            DrawCharacterPose(texture,item,size,scale,x,feetY);
            GUI.color=oldColor;GUI.matrix=oldMatrix;
        }
        void DrawBossTelegraph(Enemy boss,float x,float feetY)
        {
            if(boss.Hp<=0)return;
            Color crimson=new Color(.95f,.16f,.32f);
            if(boss.PhaseTransition>0)
            {
                float radius=35+(1-boss.PhaseTransition)*45;
                for(int i=0;i<32;i++)
                {
                    float angle=i*Mathf.PI/16;
                    Box(new Rect(x+Mathf.Cos(angle)*radius-2,feetY-50+Mathf.Sin(angle)*radius-2,4,4),crimson);
                }
                Text("2페이즈 · 붉은 각성",x-130,feetY-165,260,32,21,new Color(1,.65f,.73f),TextAnchor.MiddleCenter);
                return;
            }
            if(boss.DashRemaining>0)
            {
                for(int i=1;i<=4;i++)Box(new Rect(x-boss.Facing*i*18-12,feetY-78+i*6,24,44),new Color(.95f,.1f,.27f,.32f/i));
            }
            if(boss.Windup<=0)return;
            float progress=1-Mathf.Clamp01(boss.Windup/boss.WindupDuration);
            string label;
            if(boss.Pattern==BossPattern.GroundWave)
            {
                float start=SX(boss.X-boss.AttackReach),width=boss.AttackReach*68;
                float groundY=SY(0);
                Box(new Rect(start,groundY-6,width,6),new Color(.85f,.1f,.25f,.32f));
                Box(new Rect(start,groundY-3,width*progress,3),crimson);
                Box(new Rect(start,groundY-18,3,18),crimson);Box(new Rect(start+width-3,groundY-18,3,18),crimson);
                label="지면 파동 · 점프 / 발판!";
            }
            else if(boss.Pattern==BossPattern.Dash)
            {
                for(int i=0;i<=18;i++)
                {
                    float t=i/18f,px=SX(Mathf.Lerp(boss.X,boss.DashEndX,t)),py=SY(Mathf.Lerp(boss.Y,boss.DashEndY,t));
                    Box(new Rect(px-22,py-56,44,56),new Color(1,.28f,.12f,.035f));
                    Box(new Rect(px-2,py-28,4,4),new Color(1,.6f,.3f));
                }
                Box(new Rect(SX(boss.DashEndX)-22,SY(boss.DashEndY)-3,44,3),new Color(1,.48f,.22f));
                label="비행 돌진 · 경로 회피!";
            }
            else
            {
                float originX=boss.X+boss.Facing*.8f,originY=boss.Y+1.4f;
                float angle=Mathf.Atan2(boss.AimY-originY,boss.AimX-originX);
                int spread=boss.PhaseTwo?1:0;
                for(int ray=-spread;ray<=spread;ray++)
                    for(int point=1;point<=20;point++)
                    {
                        float distance=point*.6f,a=angle+ray*.21f;
                        Box(new Rect(SX(originX+Mathf.Cos(a)*distance)-1,SY(originY+Mathf.Sin(a)*distance)-1,3,3),new Color(1,.35f,.65f,.6f));
                    }
                label=boss.PhaseTwo?"붉은 마법탄 · 3갈래":"붉은 마법탄 · 조준 고정";
            }
            Box(new Rect(x-28,feetY-117,56*progress,3),crimson);
            Text(label,x-150,feetY-150,300,30,18,new Color(1,.73f,.76f),TextAnchor.MiddleCenter);
        }
        void DrawBloodOrbs()
        {
            foreach(var orb in run.BloodOrbs)
            {
                var position=RenderPosition(orb,orb.X,orb.Y);float x=SX(position.x),y=SY(position.y);
                if(x<-30||x>1310||y<120||y>600)continue;
                var direction=new Vector2(orb.VelocityX,-orb.VelocityY).normalized;
                Color tint=orb.Owner.LateChapter>0?LateColor(orb.Owner.LateChapter):new Color(.95f,.12f,.32f);
                for(int i=3;i>0;i--)Box(new Rect(x-direction.x*i*6-4,y-direction.y*i*6-4,8,8),new Color(tint.r,tint.g,tint.b,.3f/i));
                GothicDiamond(x,y,9,tint);
                GothicDiamond(x,y,4,new Color(1,.94f,.9f));
            }
        }
        void DrawBossHud()
        {
            var boss=run.Enemies.Find(enemy=>enemy.Boss);
            if(boss==null||run.BossIntroducing||run.Phase!=RunPhase.Combat)return;
            GothicPlaque(new Rect(302,134,676,66),14,new Color(.21f,.13f,.18f,.96f));
            GothicPlaque(new Rect(305,136,670,61),12,new Color(.035f,.025f,.05f,.96f));
            Text(boss.Hp>0?BossTitle+" · "+(boss.PhaseTransition>0?"각성 중":boss.PhaseTwo?"2페이즈":"1페이즈"):run.Chapter>=4?"봉인 해방 · 포탈 앞에서 ↑ · "+NextRoomTitle:run.Chapter==3?"태양 봉인 해방 · 포탈 앞에서 ↑ · 4스테이지":run.Chapter==2?"왕실의 사슬 해방 · 포탈 앞에서 ↑ · 3스테이지":"수호자 처치 · 포탈 앞에서 ↑ · 2스테이지",315,139,520,26,16,new Color(1,.73f,.77f),TextAnchor.MiddleCenter);
            DrawGothicMeter(new Rect(330,173,620,14),boss.Hp/boss.MaxHp,boss.PhaseTwo?new Color(.97f,.22f,.36f):new Color(.69f,.13f,.28f));
            GothicDiamond(640,180,4,new Color(1,.8f,.8f,.85f));
            Text($"{boss.Hp:0} / {boss.MaxHp:0}",835,142,110,24,14,Color.white,TextAnchor.MiddleRight);
        }
    }
}


