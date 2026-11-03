using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        float BossIntroElapsed=>Mathf.Clamp(RunModel.BossIntroDuration-run.BossIntroRemaining-(1-MotionAlpha)*RunModel.StepSeconds,0,RunModel.BossIntroDuration);
        void DrawBossArrivalEffect()
        {
            if(run.Chapter==2){DrawMorgaArrivalEffect();return;}
            float time=BossIntroElapsed;
            float envelope=Mathf.Clamp01(time/.4f)*Mathf.Clamp01((RunModel.BossIntroDuration-time-.45f)/.65f);
            float x=SX(RunModel.BossSpawnX),y=SY(0)-5;
            Color crimson=run.Chapter>=2?new Color(.95f,.65f,.24f,envelope*.8f):new Color(.94f,.17f,.36f,envelope*.8f);
            float radius=Mathf.Lerp(18,85,Mathf.SmoothStep(0,1,Mathf.Clamp01(time/.9f)));
            for(int ring=0;ring<2;ring++)
            {
                float r=radius-ring*13;
                for(int i=0;i<48;i++)
                {
                    float a=i*Mathf.PI/24,b=(i+1)*Mathf.PI/24;
                    GothicLine(new Vector2(x+Mathf.Cos(a)*r,y+Mathf.Sin(a)*r*.27f),
                        new Vector2(x+Mathf.Cos(b)*r,y+Mathf.Sin(b)*r*.27f),crimson,2);
                }
            }
            for(int i=0;i<12;i++)
            {
                float angle=i*Mathf.PI/6+time*.35f;
                GothicDiamond(x+Mathf.Cos(angle)*radius,y+Mathf.Sin(angle)*radius*.27f,3,crimson);
                float rise=Mathf.Repeat(time*58+i*17,155);
                Box(new Rect(x+Mathf.Sin(i*3.1f+time)*35,y-rise,2,5),new Color(1,.5f,.6f,envelope*(1-rise/155)));
            }
            // A thin crimson column reveals the boss without changing the gameplay root.
            float beam=envelope*(1-Mathf.Clamp01((time-1.4f)/.8f));
            for(int i=0;i<9;i++)
                Box(new Rect(x-27+i*6,y-190,6,190),new Color(.94f,.12f,.32f,beam*(1-Mathf.Abs(i-4)/5f)*.16f));
        }
        void DrawBossIntroOverlay()
        {
            // Own input like a dialogue; pause/settings are drawn afterward and remain usable.
            hits.Clear();
            float fade=Mathf.Clamp01(BossIntroElapsed/.3f)*Mathf.Clamp01((RunModel.BossIntroDuration-BossIntroElapsed)/.6f);
            Box(new Rect(0,0,1280,132),new Color(.012f,.014f,.025f,fade*.97f));
            Box(new Rect(0,580,1280,140),new Color(.012f,.014f,.025f,fade*.97f));
            Color red=new Color(.77f,.26f,.38f,fade),gold=new Color(.94f,.76f,.5f,fade);
            GothicLine(new Vector2(280,590),new Vector2(600,590),red,1);
            GothicLine(new Vector2(680,590),new Vector2(1000,590),red,1);
            GothicDiamond(640,590,6,gold);
            Text(run.Chapter>=4?"제"+run.Chapter+"장 · "+RoomTitle:run.Chapter==3?"제3장 · 태양 봉인의 내전":run.Chapter==2?"제2장 · 왕실 사슬의 제단":"제1장 · 공중 성소",340,70,600,35,20,gold,TextAnchor.MiddleCenter);
            Text(BossTitle,280,607,720,48,36,new Color(1,.81f,.84f,fade),TextAnchor.MiddleCenter);
            Text(run.Chapter>=4?"어머니에게 이어진 별실을 지키는 마지막 수문장":run.Chapter==3?"어머니를 봉인의 매듭으로 삼은 왕실의 악마":run.Chapter==2?"양어머니의 별실을 붙잡은 왕실의 집행자":"어머니의 별실을 삼킨 균열의 간수",340,660,600,28,18,new Color(.68f,.65f,.7f,fade),TextAnchor.MiddleCenter);
        }
    }
}
