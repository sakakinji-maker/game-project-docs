using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        readonly Texture2D[] lateBossSheets=new Texture2D[4];
        float nextLateBossRetry;
        static readonly string[] LateBossAssets={"Stage4VarkanKnightV2","Stage5LysseaMaskBodyV2","Stage6NoctisRavenV2","Stage7SolveinThornV2"};
        // Hand-aligned roots and padded bounds include weapons crossing nominal grid cells.
        // A single scale per character preserves body size throughout the attack.
        static readonly HeroFrame[][] LateBossFrames={
            new[]{new HeroFrame(0,0,444,444,244,422),new HeroFrame(444,0,443,444,688,422),new HeroFrame(887,0,443,444,1144,422),new HeroFrame(1330,0,444,444,1586,420),
                new HeroFrame(0,444,444,443,244,863),new HeroFrame(444,444,443,443,676,862),new HeroFrame(887,444,443,443,1122,866),new HeroFrame(1330,444,444,443,1594,863)},
            new[]{new HeroFrame(0,0,444,443,247,402),new HeroFrame(444,0,443,443,695,402),new HeroFrame(887,0,444,443,1140,403),new HeroFrame(1331,0,443,443,1595,405),
                new HeroFrame(0,443,444,444,245,832),new HeroFrame(444,443,443,444,690,832),new HeroFrame(887,443,444,444,1125,833),new HeroFrame(1331,443,443,444,1563,833)},
            new[]{new HeroFrame(0,0,444,443,235,390),new HeroFrame(444,0,443,443,679,390),new HeroFrame(887,0,444,443,1125,390),new HeroFrame(1331,0,443,443,1567,390),
                new HeroFrame(0,443,444,444,253,813),new HeroFrame(444,443,443,444,674,813),new HeroFrame(887,443,444,444,1125,814),new HeroFrame(1331,443,443,444,1567,813)},
            new[]{new HeroFrame(0,0,444,443,260,392),new HeroFrame(444,0,443,443,710,392),new HeroFrame(887,0,444,443,1128,392),new HeroFrame(1331,0,443,443,1570,392),
                new HeroFrame(0,443,444,444,246,817),new HeroFrame(444,443,443,444,655,818),new HeroFrame(887,443,444,444,1120,822),new HeroFrame(1331,443,443,444,1570,818)}
        };
        void LoadLateBossArt()
        {
            varkanHammerSheet=Resources.Load<Texture2D>("Demons/Stage4VarkanHammerV2");
            if(varkanHammerSheet!=null){varkanHammerSheet.filterMode=FilterMode.Point;varkanHammerSheet.wrapMode=TextureWrapMode.Clamp;}
            for(int i=0;i<4;i++)
            {
                lateBossSheets[i]=Resources.Load<Texture2D>("Demons/"+LateBossAssets[i]);
                if(lateBossSheets[i]!=null){lateBossSheets[i].filterMode=FilterMode.Point;lateBossSheets[i].wrapMode=TextureWrapMode.Clamp;}
            }
            nextLateBossRetry=Time.realtimeSinceStartup+1;
        }
        Color LateColor(int chapter)=>chapter==4?new Color(1,.48f,.28f):chapter==5?new Color(.76f,.57f,1):chapter==6?new Color(.43f,.7f,1):new Color(1,.77f,.3f);
        void DrawLateBoss(Enemy boss)
        {
            if(boss.Hp<=0&&boss.DeathTimer<=0)return;
            DrawLateTelegraph(boss);
            int index=boss.LateChapter-4;
            if((lateBossSheets[index]==null||(index==0&&varkanHammerSheet==null))&&Time.realtimeSinceStartup>=nextLateBossRetry)LoadLateBossArt();
            var texture=lateBossSheets[index];if(texture==null)return;
            var p=RenderPosition(boss,boss.X,boss.Y);float x=SX(p.x),y=SY(p.y);
            if(x<-260||x>1540)return;
            bool melee=!boss.LateColumns&&!boss.LateSafeBand&&!boss.LateVolley;
            int pose=boss.PhaseTransition>0?7:boss.Windup>0?(melee?4:boss.LateSafeBand?7:6):boss.AttackPose>0||boss.DashRemaining>0?(melee?5:boss.LateSafeBand?7:6):boss.Walking?2+(int)(boss.AnimationTime*6)%2:(int)(boss.AnimationTime*2)%2;
            if(index==1&&boss.Pattern==BossPattern.MirrorPrison&&(boss.Windup>0||boss.AttackPose>0))pose=7;
            if(index==2)
            {
                pose=NoctisPose(boss);
                if(run.BossIntroducing)y+=NoctisArrivalOffset;
            }
            if(index==3)pose=SolveinPose(boss);
            float revealStart=index==1?MirrorBreakTime:index==2?.15f:.6f;
            float reveal=run.BossIntroducing?Mathf.SmoothStep(0,1,Mathf.Clamp01((BossIntroElapsed-revealStart)/.8f)):1;
            if(index==3)reveal=1; // The king is already seated when the room appears.
            float alpha=(boss.Hp<=0?Mathf.Clamp01(1-BossDefeatElapsed/1.4f):1)*reveal;
            var matrix=GUI.matrix;var color=GUI.color;var pivot=new Vector3(x,y,0);
            GUI.matrix=matrix*Matrix4x4.Translate(pivot)*Matrix4x4.Scale(new Vector3(boss.Facing,1,1))*Matrix4x4.Translate(-pivot);
            GUI.color=boss.Flash>0?new Color(1,.45f,.4f,alpha):new Color(1,1,1,alpha);
            if(index==2&&run.BossIntroducing)
            {
                var arrivalTint=Color.Lerp(new Color(.12f,.16f,.24f),Color.white,Mathf.Clamp01((BossIntroElapsed-.35f)/1.15f));
                arrivalTint.a=alpha;GUI.color=arrivalTint;
            }
            var frame=LateBossFrames[index][pose];float scale=index==0?.34f:index==1?.4f:index==2?.5f:boss.LateChapter==7?.46f:.39f;
            if(index==0&&SelectVarkanHammer(boss,out var hammerFrame))
            {
                texture=varkanHammerSheet;frame=hammerFrame;
                // Padding repair reduced source bodies uniformly. Calibrated
                // against the idle helmet-to-heel height, not total weapon bounds.
                scale=.435f;
            }
            var size=new Vector2(1774,887);
            bool extended=SelectEnemyMotion(boss,ref texture,ref frame,ref size,ref scale,out var motionPose);
            DrawCharacterPose(texture,frame,size,scale,x,y);
            if(extended)DrawEnemyMotionProps(boss,motionPose,frame,scale,x,y);
            else
            {
                if(index==1)DrawLysseaMask(boss,pose,frame,scale,x,y);
                if(index==3)DrawSolveinArmorLight(boss,pose,frame,scale,x,y);
            }
            GUI.matrix=matrix;GUI.color=color;
        }
        void DrawLateTelegraph(Enemy boss)
        {
            if(boss.Hp<=0)return;
            var c=LateColor(boss.LateChapter);
            if(boss.PhaseTransition>0){Text(boss.LateChapter==7?"2페이즈 · 왕의 갑옷에서 태양빛이 새어 나옵니다":"2페이즈 · 왕의 명령이 폭주합니다",330,218,620,36,24,c,TextAnchor.MiddleCenter);return;}
            bool warning=boss.Windup>0,active=boss.AttackPose>0||boss.DashRemaining>0;if(!warning&&!active)return;
            float ground=SY(0),top=SY(10),progress=warning?1-boss.Windup/boss.WindupDuration:Mathf.Clamp01(boss.AttackPose/boss.AttackReleaseDuration);
            Color fill=new Color(c.r,c.g,c.b,warning?.14f:.4f);
            string label;
            if(boss.LateColumns)
            {
                for(int i=0;i<boss.LateColumnCount;i++)
                {
                    float x=SX(boss.LateColumnX(i));
                    if(warning)DrawBossColumnWarning(x,top,ground,48,progress,c);
                    else if(boss.LateChapter==5)
                    {
                        // Mirror prison is a cascade of moving glass, not fire.
                        for(int shard=0;shard<6;shard++)
                        {
                            float py=Mathf.Lerp(top,ground-18,shard/5f);
                            float offset=Mathf.Sin(visualClock*9+shard)*6;
                            DrawSpellEffect(SpellKind.MirrorShards,new Vector2(x+offset,py),new Vector2(45,58),Mathf.Clamp01(progress*4),90);
                        }
                        DrawBossImpact(new Vector2(x,ground-5),75,progress,c);
                    }
                    else if(boss.LateChapter==7)DrawRoyalFallingSword(x,top,ground,progress);
                    else
                    {
                        DrawBossPillar(x,top,ground,48,progress,c,i*1.7f);
                        DrawBossImpact(new Vector2(x,ground-5),80,progress,c);
                    }
                }
                label=boss.LateChapter==4?"심장 광주 · 기둥 사이로":boss.LateChapter==5?"우는 탈 · 거울 감옥 · 틈으로 이동":"왕검 낙하 · 칼날 사이로";
            }
            else if(boss.LateSafeBand)
            {
                // Night Veil hits at every height. Its warning and feathers
                // must also cover the upper viewport, including high platforms.
                if(boss.LateChapter==6)top=0;
                float left=SX(boss.AimX-boss.LateSafeRadius),right=SX(boss.AimX+boss.LateSafeRadius);
                if(warning)
                {
                    Box(new Rect(0,top,Mathf.Clamp(left,0,1280),ground-top),fill);
                    Box(new Rect(Mathf.Clamp(right,0,1280),top,1280-Mathf.Clamp(right,0,1280),ground-top),fill);
                }
                else
                {
                    if(boss.LateChapter==6)
                    {
                        DrawNoctisCurtain(0,left,top,ground,progress);
                        DrawNoctisCurtain(right,1280,top,ground,progress);
                    }
                    else if(boss.LateChapter==7)
                    {
                        DrawRoyalEclipseCurtain(0,left,top,ground,progress);
                        DrawRoyalEclipseCurtain(right,1280,top,ground,progress);
                    }
                    else
                    {
                        DrawBossCurtain(0,left,top,ground,progress,c);
                        DrawBossCurtain(right,1280,top,ground,progress,c);
                    }
                }
                var safe=new Color(.43f,1,.72f);
                Box(new Rect(left,top,right-left,ground-top),new Color(.3f,1,.65f,.08f));
                Box(new Rect(left,top,3,ground-top),safe);Box(new Rect(right-3,top,3,ground-top),safe);
                Text("안전",left,ground-43,right-left,30,20,safe,TextAnchor.MiddleCenter);
                if(right<0||left>1280)Text(right<0?"← 안전지대":"안전지대 →",right<0?25:1015,260,240,38,24,safe,TextAnchor.MiddleCenter);
                label=boss.LateChapter==6?"검은 날개의 장막 · 초록빛 안으로":"마지막 일식 · 초록빛 안으로";
            }
            else if(boss.LateVolley)
            {
                Vector2 origin=new Vector2(SX(boss.X+boss.Facing),SY(boss.Y+2));
                float angle=Mathf.Atan2(boss.AimY-boss.Y-2,boss.AimX-boss.X-boss.Facing);
                int spread=boss.PhaseTwo?2:1;
                for(int i=-spread;i<=spread;i++)
                {
                    float a=angle+i*(boss.LateChapter==5?.3f:.2f);
                    var end=origin+new Vector2(Mathf.Cos(a),-Mathf.Sin(a))*440;
                    GothicLine(origin,end,new Color(c.r,c.g,c.b,.3f),2);
                }
                DrawBossRitual(2,origin,Vector2.one*(25+progress*22),c,visualClock*45);
                label=boss.LateChapter==4?"톱니 탄막 · 조준선을 벗어나세요":boss.LateChapter==5?"성난 탈 · 거울 파편 · 부채꼴 탄막":"깃털 칼날 · 높이를 바꾸세요";
            }
            else if(boss.LateDash)
            {
                Vector2 from=new Vector2(SX(boss.X),SY(boss.Y+boss.BodyHeight*.5f)),to=new Vector2(SX(boss.AimX),SY(boss.AimY+boss.BodyHeight*.5f));
                if(warning)
                {
                    GothicLine(from,to,fill,boss.BodyHeight*34);GothicLine(from,to,c,2);
                    DrawBossRitual(2,to,Vector2.one*40,c,visualClock*30);
                }
                else if(boss.LateChapter==6)DrawNoctisDive(boss,from,progress);
                else DrawSpellEffect(SpellKind.MoonBlades,from,new Vector2(100,90),.85f,visualClock*90,boss.Facing);
                label="낫 급강하 · 경로 밖으로";
            }
            else
            {
                float center=boss.Pattern==BossPattern.MirrorStep?boss.AimX:boss.X,height=boss.Pattern==BossPattern.MirrorStep?85:41;
                float left=SX(center-boss.LateReach),width=boss.LateReach*68;
                if(warning)
                {
                    DrawBossRune(SX(center),ground,width,progress,c);
                    Box(new Rect(left,ground-5,width,3),new Color(c.r,c.g,c.b,.55f));
                }
                else if(boss.Pattern==BossPattern.MirrorStep)
                {
                    DrawSpellEffect(SpellKind.MirrorWard,new Vector2(SX(center),ground-height*.5f),new Vector2(width,height*1.4f),Mathf.Clamp01(progress*4));
                    DrawBossImpact(new Vector2(SX(center),ground-5),width,progress,c);
                }
                else if(boss.Pattern==BossPattern.RoyalCleave)DrawRoyalCleave(left,width,ground,progress);
                else if(boss.Pattern==BossPattern.HammerSweep)DrawVarkanHammerImpact(boss,left,width,ground,progress,c);
                else DrawBossGroundBurst(left,width,ground,height,progress,c);
                label=boss.Pattern==BossPattern.MirrorStep?"웃는 탈 · 거울 도약 · 표시된 위치에서 멀리":boss.Pattern==BossPattern.RoyalCleave?"왕의 대검 · 점프 / 발판으로":"망치 휘두르기 · 점프 / 발판으로";
            }
            if(warning)Text(label,300,218,680,34,23,c,TextAnchor.MiddleCenter);
        }
    }
}
