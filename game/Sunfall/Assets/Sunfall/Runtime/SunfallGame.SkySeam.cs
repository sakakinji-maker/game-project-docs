using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        // The seam is part of the sanctuary panorama, not attached to the dying boss.
        const float SkySewStart=.55f,SkySewEnd=1.95f;
        const int SkyStitchCount=11;
        static readonly Vector2[] SkyTearPath={
            new Vector2(-174,-4),new Vector2(-132,-22),new Vector2(-83,0),
            new Vector2(-34,-12),new Vector2(15,12),new Vector2(64,-7),
            new Vector2(115,14),new Vector2(172,2)
        };
        float SkySeamProgress=>run.BossDefeated?Mathf.Clamp01((BossDefeatElapsed-SkySewStart)/(SkySewEnd-SkySewStart)):0;
        Vector2 SkyTearPoint(float t)
        {
            float section=Mathf.Clamp01(t)*(SkyTearPath.Length-1);
            int index=Mathf.Min((int)section,SkyTearPath.Length-2);
            // Same horizontal parallax and vertical drift as the 2070px backdrop.
            var origin=new Vector2(1035-cameraX*((2070f-1280)/(RunModel.WorldWidth-32)),254+cameraY*8);
            return origin+Vector2.Lerp(SkyTearPath[index],SkyTearPath[index+1],section-index);
        }
        Vector2 SkyTearNormal(float t)
        {
            Vector2 tangent=SkyTearPoint(Mathf.Min(1,t+.01f))-SkyTearPoint(Mathf.Max(0,t-.01f));
            return new Vector2(-tangent.y,tangent.x).normalized;
        }
        float SkyTearWidth(float t,float progress)
        {
            float closed=Mathf.SmoothStep(0,1,Mathf.Clamp01(progress*SkyStitchCount-t*SkyStitchCount+1));
            if(progress<=0)closed=0;
            return Mathf.Sin(t*Mathf.PI)*(17+4*Mathf.Sin(t*29))*(1-closed);
        }
        void DrawSkyRift()
        {
            if(run.Chapter!=1||run.Stage!=RunStage.BossArena)return;
            float progress=SkySeamProgress;
            // Close the black opening locally behind the advancing needle.
            if(progress<1)
            {
                for(int i=0;i<28;i++)
                {
                    float a=i/28f,b=(i+1)/28f;
                    Vector2 pa=SkyTearPoint(a),pb=SkyTearPoint(b),na=SkyTearNormal(a),nb=SkyTearNormal(b);
                    float wa=SkyTearWidth(a,progress),wb=SkyTearWidth(b,progress);
                    if(wa+wb<.2f)continue;
                    float pulse=.85f+.15f*Mathf.Sin(visualClock*2+i*.7f);
                    var wound=new Color(.85f,.16f,.32f,.56f*pulse);
                    GothicLine(pa+na*wa,pb+nb*wb,new Color(.64f,.08f,.23f,.2f),9);
                    GothicLine(pa-na*wa,pb-nb*wb,new Color(.64f,.08f,.23f,.2f),9);
                    GothicPolygon(new Color(.018f,.008f,.032f,.98f),pa+na*wa,pb+nb*wb,pb-nb*wb,pa-na*wa);
                    GothicLine(pa+na*wa,pb+nb*wb,wound,2);
                    GothicLine(pa-na*wa,pb-nb*wb,wound,2);
                }
            }
            if(progress<=0)return;
            Color gold=new Color(1,.74f,.3f,.95f),core=new Color(1,.95f,.68f);
            // A fine scar remains when the active particle effect and death timer end.
            for(int i=0;i<28;i++)
            {
                float a=i/28f,b=Mathf.Min((i+1)/28f,progress);
                if(b<=a)break;
                GothicLine(SkyTearPoint(a),SkyTearPoint(b),new Color(.8f,.48f,.12f,.15f),7);
                GothicLine(SkyTearPoint(a),SkyTearPoint(b),new Color(.93f,.65f,.24f,.8f),2);
            }
            for(int i=0;i<SkyStitchCount;i++)
            {
                float stitch=Mathf.Clamp01(progress*SkyStitchCount-i);
                if(stitch<=0)break;
                float t=(i+.5f)/SkyStitchCount;
                Vector2 center=SkyTearPoint(t),normal=SkyTearNormal(t),tangent=new Vector2(normal.y,-normal.x);
                float span=Mathf.Lerp(24,9,Mathf.SmoothStep(0,1,stitch));
                Vector2 a=center-normal*span-tangent*7,b=center+normal*span+tangent*7;
                Vector2 c=center+normal*span-tangent*7,d=center-normal*span+tangent*7;
                if(i%2!=0){var swap=a;a=b;b=swap;swap=c;c=d;d=swap;}
                DrawGoldThread(a,b,Mathf.Clamp01(stitch*2),gold,core);
                DrawGoldThread(c,d,Mathf.Clamp01(stitch*2-1),gold,core);
                if(stitch>=1)
                {
                    GothicDiamond(a.x,a.y,2,gold);GothicDiamond(b.x,b.y,2,gold);
                }
                else
                {
                    Vector2 needle=stitch<.5f?Vector2.Lerp(a,b,stitch*2):Vector2.Lerp(c,d,stitch*2-1);
                    GothicDiamond(needle.x,needle.y,6,new Color(1,.7f,.22f,.28f));
                    GothicDiamond(needle.x,needle.y,3,core);
                    for(int spark=0;spark<4;spark++)
                    {
                        float angle=spark*Mathf.PI*.5f+stitch*3;
                        Vector2 p=needle+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*(6+stitch*12);
                        Box(new Rect(p.x,p.y,2,2),new Color(1,.84f,.4f,(1-stitch)*.8f));
                    }
                }
            }
            if(run.BossDissolving)
            {
                DrawRisingStarThread(progress);
                float age=BossDefeatElapsed-SkySewEnd;
                if(age>=0&&age<.65f)
                    for(int i=0;i<16;i++)
                    {
                        Vector2 p=SkyTearPoint((i+.5f)/16f)+new Vector2(Mathf.Sin(i*3)*age*18,-age*(17+i%4*6));
                        GothicDiamond(p.x,p.y,i%3==0?3:2,new Color(1,.88f,.48f,(1-age/.65f)*.75f));
                    }
            }
        }
        void DrawGoldThread(Vector2 from,Vector2 to,float amount,Color gold,Color core)
        {
            if(amount<=0)return;
            var end=Vector2.Lerp(from,to,amount);
            GothicLine(from,end,new Color(.05f,.025f,.015f,.7f),5);
            GothicLine(from,end,new Color(1,.66f,.2f,.15f),8);
            GothicLine(from,end,gold,3);GothicLine(from,end,core,1);
        }
        void DrawRisingStarThread(float progress)
        {
            float alpha=Mathf.Clamp01((1-progress)*4);
            if(alpha<=0)return;
            Vector2 from=new Vector2(SX(run.X)+run.Facing*17,SY(run.Y)-38),to=SkyTearPoint(progress);
            Vector2 bend=(from+to)*.5f+new Vector2(-50,-75),previous=from;
            for(int i=1;i<=24;i++)
            {
                float t=i/24f;
                Vector2 next=(1-t)*(1-t)*from+2*(1-t)*t*bend+t*t*to;
                GothicLine(previous,next,new Color(1,.74f,.32f,alpha*.65f),2);previous=next;
            }
        }
    }
}
