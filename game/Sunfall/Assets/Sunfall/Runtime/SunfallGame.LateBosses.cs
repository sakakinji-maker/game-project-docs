using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        readonly Texture2D[] lateBossSheets=new Texture2D[4];
        float nextLateBossRetry;
        static readonly string[] LateBossAssets={"Stage4VarkanV1","Stage5LysseaV1","Stage6NoctisV1","Stage7SolveinV1"};
        // Hand-aligned roots and padded bounds include weapons crossing nominal grid cells.
        // A single scale per character preserves body size throughout the attack.
        static readonly HeroFrame[][] LateBossFrames={
            new[]{new HeroFrame(0,0,444,443,250,390),new HeroFrame(444,0,443,443,690,390),new HeroFrame(887,0,444,443,1124,390),new HeroFrame(1331,0,443,443,1585,390),
                new HeroFrame(0,443,444,444,245,831),new HeroFrame(444,443,474,444,660,831),new HeroFrame(918,443,413,444,1120,831),new HeroFrame(1331,443,443,444,1580,831)},
            new[]{new HeroFrame(0,0,444,443,239,414),new HeroFrame(444,0,443,443,676,414),new HeroFrame(887,0,444,443,1125,414),new HeroFrame(1331,0,443,443,1567,414),
                new HeroFrame(0,443,405,444,236,853),new HeroFrame(405,443,520,444,640,853),new HeroFrame(925,443,406,444,1122,853),new HeroFrame(1331,443,443,444,1574,853)},
            new[]{new HeroFrame(0,0,444,443,246,393),new HeroFrame(444,0,443,443,691,393),new HeroFrame(887,0,444,443,1124,393),new HeroFrame(1331,0,443,443,1575,393),
                new HeroFrame(0,443,430,444,242,848),new HeroFrame(430,443,514,444,660,848),new HeroFrame(944,443,387,444,1133,848),new HeroFrame(1331,443,443,444,1555,848)},
            new[]{new HeroFrame(0,0,444,443,255,400),new HeroFrame(444,0,443,443,695,400),new HeroFrame(887,0,444,443,1138,400),new HeroFrame(1331,0,443,443,1577,400),
                new HeroFrame(0,443,430,444,275,818),new HeroFrame(430,443,510,444,662,818),new HeroFrame(940,443,391,444,1142,825),new HeroFrame(1331,443,443,444,1594,825)}
        };
        void LoadLateBossArt()
        {
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
            if(lateBossSheets[index]==null&&Time.realtimeSinceStartup>=nextLateBossRetry)LoadLateBossArt();
            var texture=lateBossSheets[index];if(texture==null)return;
            var p=RenderPosition(boss,boss.X,boss.Y);float x=SX(p.x),y=SY(p.y);
            if(x<-260||x>1540)return;
            bool melee=!boss.LateColumns&&!boss.LateSafeBand&&!boss.LateVolley;
            int pose=boss.PhaseTransition>0?7:boss.Windup>0?(melee?4:boss.LateSafeBand?7:6):boss.AttackPose>0||boss.DashRemaining>0?(melee?5:boss.LateSafeBand?7:6):boss.Walking?2+(int)(boss.AnimationTime*6)%2:(int)(boss.AnimationTime*2)%2;
            float reveal=run.BossIntroducing?Mathf.SmoothStep(0,1,Mathf.Clamp01((BossIntroElapsed-.6f)/.8f)):1;
            float alpha=(boss.Hp<=0?Mathf.Clamp01(1-BossDefeatElapsed/1.4f):1)*reveal;
            var matrix=GUI.matrix;var color=GUI.color;var pivot=new Vector3(x,y,0);
            GUI.matrix=matrix*Matrix4x4.Translate(pivot)*Matrix4x4.Scale(new Vector3(boss.Facing,1,1))*Matrix4x4.Translate(-pivot);
            GUI.color=boss.Flash>0?new Color(1,.45f,.4f,alpha):new Color(1,1,1,alpha);
            DrawCharacterPose(texture,LateBossFrames[index][pose],new Vector2(1774,887),boss.LateChapter==7?.42f:.39f,x,y);
            GUI.matrix=matrix;GUI.color=color;
        }
        void DrawLateTelegraph(Enemy boss)
        {
            if(boss.Hp<=0)return;
            var c=LateColor(boss.LateChapter);
            if(boss.PhaseTransition>0){Text("2페이즈 · 왕의 명령이 폭주합니다",330,218,620,36,24,c,TextAnchor.MiddleCenter);return;}
            bool warning=boss.Windup>0,active=boss.AttackPose>0||boss.DashRemaining>0;if(!warning&&!active)return;
            float ground=SY(0),top=SY(10),progress=warning?1-boss.Windup/boss.WindupDuration:1;
            Color fill=new Color(c.r,c.g,c.b,warning?.14f:.4f);
            string label;
            if(boss.LateColumns)
            {
                for(int i=0;i<boss.LateColumnCount;i++)
                {
                    float x=SX(boss.LateColumnX(i));
                    Box(new Rect(x-24,top,48,ground-top),fill);Box(new Rect(x-24,ground-6,48,6),c);
                    if(warning)Box(new Rect(x-24,ground-11,48*progress,3),c);
                    else GothicLine(new Vector2(x,top),new Vector2(x,ground),c,6);
                }
                label=boss.LateChapter==4?"심장 광주 · 기둥 사이로":boss.LateChapter==5?"거울 감옥 · 틈으로 이동":"태양 낙하 · 기둥 사이로";
            }
            else if(boss.LateSafeBand)
            {
                float left=SX(boss.AimX-boss.LateSafeRadius),right=SX(boss.AimX+boss.LateSafeRadius);
                Box(new Rect(0,top,Mathf.Clamp(left,0,1280),ground-top),fill);Box(new Rect(Mathf.Clamp(right,0,1280),top,1280-Mathf.Clamp(right,0,1280),ground-top),fill);
                var safe=new Color(.43f,1,.72f);
                Box(new Rect(left,top,right-left,ground-top),new Color(.3f,1,.65f,.08f));
                Box(new Rect(left,top,3,ground-top),safe);Box(new Rect(right-3,top,3,ground-top),safe);
                Text("안전",left,ground-43,right-left,30,20,safe,TextAnchor.MiddleCenter);
                if(right<0||left>1280)Text(right<0?"← 안전지대":"안전지대 →",right<0?25:1015,260,240,38,24,safe,TextAnchor.MiddleCenter);
                label=boss.LateChapter==6?"영원한 밤 · 초록빛 안으로":"마지막 일식 · 초록빛 안으로";
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
                GothicDisc(origin.x,origin.y,8+progress*9,c);
                label=boss.LateChapter==4?"톱니 탄막 · 조준선을 벗어나세요":boss.LateChapter==5?"거울 파편 · 부채꼴 탄막":"초승달 탄막 · 높이를 바꾸세요";
            }
            else if(boss.LateDash)
            {
                Vector2 from=new Vector2(SX(boss.X),SY(boss.Y+boss.BodyHeight*.5f)),to=new Vector2(SX(boss.AimX),SY(boss.AimY+boss.BodyHeight*.5f));
                GothicLine(from,to,fill,boss.BodyHeight*34);GothicLine(from,to,c,3);GothicDiamond(to.x,to.y,18,c);
                label="초승달 급강하 · 경로 밖으로";
            }
            else
            {
                float center=boss.Pattern==BossPattern.MirrorStep?boss.AimX:boss.X,height=boss.Pattern==BossPattern.MirrorStep?85:41;
                float left=SX(center-boss.LateReach),width=boss.LateReach*68;
                Box(new Rect(left,ground-height,width,height),fill);Box(new Rect(left,ground-5,width,5),c);
                if(warning)Box(new Rect(left,ground-10,width*progress,3),c);
                else for(int side=-1;side<=1;side+=2)GothicLine(new Vector2(SX(center),ground-height),new Vector2(SX(center)+side*width/2,ground),c,5);
                label=boss.Pattern==BossPattern.MirrorStep?"거울 도약 · 표시된 위치에서 멀리":boss.Pattern==BossPattern.RoyalCleave?"왕의 대검 · 점프 / 발판으로":"심장 파쇄 · 점프 / 발판으로";
            }
            if(warning)Text(label,300,218,680,34,23,c,TextAnchor.MiddleCenter);
        }
    }
}
