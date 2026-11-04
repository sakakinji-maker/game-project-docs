using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        const float SkySewStart=.55f,SkySewEnd=1.95f;
        const int SkyStitchCount=24;
        // Continuous, deterministic curves: camera movement and OnGUI repaint
        // never reroll the scar or jump between angular control points.
        readonly struct SewnScarPath
        {
            readonly Vector2 from,to;
            readonly float bend;
            public SewnScarPath(Vector2 from,Vector2 to,float bend)
            {this.from=from;this.to=to;this.bend=bend;}
            public Vector2 Point(float t)
            {
                t=Mathf.Clamp01(t);
                var delta=to-from;
                var normal=new Vector2(-delta.y,delta.x).normalized;
                float envelope=Mathf.Sin(t*Mathf.PI);
                float irregularity=(Mathf.Sin(t*19+.4f)*1.8f+Mathf.Sin(t*37)*.65f)*envelope;
                return Vector2.Lerp(from,to,t)+normal*(4*t*(1-t)*bend+irregularity);
            }
            public Vector2 Normal(float t)
            {
                var tangent=Point(t+.002f)-Point(t-.002f);
                return new Vector2(-tangent.y,tangent.x).normalized;
            }
        }
        SewnScarPath FirstSkyScar
        {
            get
            {
                // Match the sanctuary panorama's parallax, not the dying boss.
                var origin=new Vector2(1035-cameraX*((2070f-1280)/(RunModel.WorldWidth-32)),254+cameraY*8);
                return new SewnScarPath(origin+new Vector2(-174,-4),origin+new Vector2(172,2),-12);
            }
        }
        float SkySeamProgress=>run.BossDefeated?Mathf.Clamp01((BossDefeatElapsed-SkySewStart)/(SkySewEnd-SkySewStart)):0;
        Vector2 SkyTearPoint(float t)=>FirstSkyScar.Point(t);
        float SkyTearWidth(float t,float progress)
        {
            float closed=progress<=0?0:Mathf.SmoothStep(0,1,Mathf.Clamp01((progress-t)*SkyStitchCount+1));
            return Mathf.Pow(Mathf.Sin(t*Mathf.PI),.8f)*(12+2*Mathf.Sin(t*23))*(1-closed);
        }
        void DrawSkyRift()
        {
            if(run.Chapter!=1||run.Stage!=RunStage.BossArena)return;
            float progress=SkySeamProgress;
            var path=FirstSkyScar;
            // A tapered violet fissure closes locally behind the needle.
            if(progress<1)
            {
                for(int i=0;i<64;i++)
                {
                    float a=i/64f,b=(i+1)/64f;
                    Vector2 pa=path.Point(a),pb=path.Point(b),na=path.Normal(a),nb=path.Normal(b);
                    float wa=SkyTearWidth(a,progress),wb=SkyTearWidth(b,progress);
                    if(wa+wb<.2f)continue;
                    float pulse=.92f+.08f*Mathf.Sin(visualClock*1.2f+i*.2f);
                    var upperA=pa+na*wa;var upperB=pb+nb*wb;
                    var lowerA=pa-na*wa;var lowerB=pb-nb*wb;
                    GothicLine(upperA,upperB,new Color(.38f,.28f,.55f,.07f),7);
                    GothicLine(lowerA,lowerB,new Color(.38f,.28f,.55f,.07f),7);
                    GothicPolygon(new Color(.018f,.016f,.04f,.92f),upperA,upperB,lowerB,lowerA);
                    GothicLine(upperA,upperB,new Color(.56f,.39f,.63f,.4f*pulse),.9f);
                    GothicLine(lowerA,lowerB,new Color(.38f,.37f,.56f,.32f*pulse),.8f);
                }
            }
            if(progress<=0)return;
            var gold=new Color(.93f,.72f,.37f);
            float settle=Mathf.Clamp01((BossDefeatElapsed-SkySewEnd)/.7f);
            DrawSewnScar(path,progress,SkyStitchCount,gold,settle);
            if(run.BossDissolving)
            {
                DrawRisingStarThread(progress);
                DrawScarAfterglow(path,BossDefeatElapsed-SkySewEnd,gold);
            }
        }
        void DrawSewnScar(SewnScarPath path,float progress,int stitches,Color gold,float settle,float threadScale=1)
        {
            progress=Mathf.Clamp01(progress);
            if(progress<=0)return;
            // The closed wound is a dim crease, with broken gold caught along
            // its edges. No broad gold bar or opaque outline around each stitch.
            for(int i=0;i<64;i++)
            {
                float a=i/64f,b=Mathf.Min((i+1)/64f,progress);
                if(b<=a)break;
                float taper=Mathf.Clamp01(Mathf.Sin((a+b)*.5f*Mathf.PI)*5);
                var pa=path.Point(a);var pb=path.Point(b);
                GothicLine(pa,pb,new Color(.018f,.024f,.038f,.5f*taper),2.1f*threadScale);
                GothicLine(pa,pb,new Color(gold.r,gold.g,gold.b,.035f*taper*(1-settle*.65f)),5*threadScale);
                float fleck=.28f+.16f*Mathf.Sin(i*1.7f);
                GothicLine(pa,pb,new Color(gold.r,gold.g,gold.b,fleck*taper),.65f*threadScale);
            }
            for(int i=0;i<stitches;i++)
            {
                float sewn=Mathf.Clamp01(progress*stitches-i);
                if(sewn<=0)break;
                float t=(i+.5f)/stitches;
                var center=path.Point(t);var normal=path.Normal(t);
                var tangent=new Vector2(normal.y,-normal.x);
                float taper=Mathf.Lerp(.55f,1,Mathf.Sin(t*Mathf.PI));
                float span=(3.6f+.9f*Mathf.Sin(i*2.4f))*taper*threadScale;
                float tension=Mathf.SmoothStep(0,1,sewn);
                float gap=span+(1-tension)*6;
                // One curved, slanted stitch, rather than a pair of crossed rods.
                var from=center-normal*gap-tangent*2.3f;
                var to=center+normal*gap+tangent*2.3f;
                var bend=center+tangent*(2.2f+(1-tension)*6);
                var tint=new Color(gold.r,gold.g,gold.b,(.68f+.14f*Mathf.Sin(i*1.9f))*taper);
                DrawSilkCurve(from,bend,to,sewn,tint,1-settle,threadScale);
                if(sewn<1)
                {
                    Vector2 needle=SilkPoint(from,bend,to,sewn);
                    DrawScarGlint(needle,1,gold);
                }
            }
        }
        static Vector2 SilkPoint(Vector2 from,Vector2 bend,Vector2 to,float t)
        {return (1-t)*(1-t)*from+2*(1-t)*t*bend+t*t*to;}
        void DrawSilkCurve(Vector2 from,Vector2 bend,Vector2 to,float amount,Color tint,float glow,float threadScale=1)
        {
            var previous=from;
            for(int i=1;i<=6;i++)
            {
                float t=Mathf.Min(i/6f,amount);
                var next=SilkPoint(from,bend,to,t);
                if(glow>.01f)GothicLine(previous,next,new Color(tint.r,tint.g,tint.b,tint.a*.075f*glow),3*threadScale);
                GothicLine(previous,next,tint,.85f*threadScale);
                if(i==2||i==3)GothicLine(previous,next,new Color(1,.92f,.69f,tint.a*.6f),.45f*threadScale);
                previous=next;
                if(t>=amount)break;
            }
        }
        void DrawScarGlint(Vector2 point,float alpha,Color gold)
        {
            // Small needle light; only the active sewing head blooms.
            GothicDisc(point.x,point.y,5,new Color(gold.r,gold.g,gold.b,alpha*.04f));
            GothicDisc(point.x,point.y,3,new Color(gold.r,gold.g,gold.b,alpha*.1f));
            GothicLine(point-Vector2.right*2,point+Vector2.right*2,new Color(1,.92f,.7f,alpha*.75f),.7f);
            GothicLine(point-Vector2.up*1.5f,point+Vector2.up*1.5f,new Color(1,.96f,.83f,alpha*.8f),.65f);
        }
        void DrawScarAfterglow(SewnScarPath path,float age,Color gold)
        {
            if(age<0||age>=.8f)return;
            float alpha=Mathf.Sin(age/.8f*Mathf.PI)*.45f;
            for(int i=0;i<9;i++)
            {
                var point=path.Point((i+.5f)/9)+new Vector2(Mathf.Sin(i*2.7f)*age*7,-age*(8+i%3*3));
                DrawScarGlint(point,alpha,gold);
            }
        }
        void DrawRisingStarThread(float progress)
        {
            float alpha=Mathf.Clamp01((1-progress)*4);
            if(alpha<=0)return;
            Vector2 from=new Vector2(SX(run.X)+run.Facing*17,SY(run.Y)-38),to=SkyTearPoint(progress);
            Vector2 bend=(from+to)*.5f+new Vector2(-50,-75),previous=from;
            for(int i=1;i<=32;i++)
            {
                Vector2 next=SilkPoint(from,bend,to,i/32f);
                GothicLine(previous,next,new Color(1,.76f,.4f,alpha*.035f),4);
                GothicLine(previous,next,new Color(1,.84f,.53f,alpha*.6f),.8f);
                previous=next;
            }
        }
    }
}
