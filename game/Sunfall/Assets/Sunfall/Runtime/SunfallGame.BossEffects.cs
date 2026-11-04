using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        Texture2D bossPillarLoop,bossChainRitual;
        float nextBossEffectRetry;
        void EnsureBossEffectArt()
        {
            if(bossPillarLoop!=null&&bossChainRitual!=null)return;
            if(Time.realtimeSinceStartup<nextBossEffectRetry)return;
            bossPillarLoop=Resources.Load<Texture2D>("Effects/BossPillarLoopV1");
            bossChainRitual=Resources.Load<Texture2D>("Effects/BossChainRitualV1");
            foreach(var texture in new[]{bossPillarLoop,bossChainRitual})
                if(texture!=null){texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;}
            nextBossEffectRetry=Time.realtimeSinceStartup+1;
        }
        // Visuals only: time comes from the paused visual clock / existing attack
        // timers. OnGUI neither advances simulation nor rolls random numbers.
        void DrawBossRitual(int cell,Vector2 center,Vector2 size,Color tint,float angle=0)
        {
            EnsureBossEffectArt();if(bossChainRitual==null||tint.a<=0)return;
            var color=GUI.color;
            using(ArtworkAt(center,angle))
            {
                try {GUI.color=tint;DrawAtlasCell(bossChainRitual,new Rect(-size*.5f,size),cell,2,2);}
                finally {GUI.color=color;}
            }
        }
        void DrawBossRune(float x,float ground,float width,float progress,Color tint)
        {
            tint.a=.35f+.5f*Mathf.Clamp01(progress);
            DrawBossRitual(2,new Vector2(x,ground-3),new Vector2(width*1.2f,24),tint);
        }
        void DrawBossColumnWarning(float x,float top,float ground,float width,float progress,Color tint)
        {
            if(x+width<0||x-width>1280)return;
            DrawBossRune(x,ground,width,progress,tint);
            var edge=new Color(tint.r,tint.g,tint.b,.3f+.4f*progress);
            for(float py=top;py<ground-12;py+=26)
            {
                GothicLine(new Vector2(x-width*.5f,py),new Vector2(x-width*.5f,Mathf.Min(py+9,ground-12)),edge,1);
                GothicLine(new Vector2(x+width*.5f,py),new Vector2(x+width*.5f,Mathf.Min(py+9,ground-12)),edge,1);
            }
        }
        void DrawBossPillar(float x,float top,float ground,float width,float remaining,Color tint,float phase=0)
        {
            EnsureBossEffectArt();if(bossPillarLoop==null||remaining<=0||ground<=top)return;
            int frame=Mathf.FloorToInt(visualClock*18+phase)%8;
            // Measured shared base y484/485, crop away the empty side gutters.
            float baseline=frame<5?484:485;
            var source=new Rect(frame%4*384+96,frame/4*512+4,192,baseline-4);
            float h=ground-top+12,w=width*1.45f;
            var uv=new Rect(source.x/1536,1-source.yMax/1024,source.width/1536,source.height/1024);
            var color=GUI.color;tint.a*=Mathf.Clamp01(remaining*5);GUI.color=tint;
            GUI.DrawTextureWithTexCoords(new Rect(x-w*.5f,ground-h,w,h),bossPillarLoop,uv,true);
            GUI.color=color;
        }
        void DrawBossImpact(Vector2 point,float width,float remaining,Color tint)
        {
            float age=1-Mathf.Clamp01(remaining);
            tint.a*=Mathf.Clamp01(remaining*2);
            DrawBossRitual(3,point,new Vector2(width,width*.48f)*Mathf.Lerp(.65f,1.3f,age),tint,0);
        }
        void DrawBossGroundBurst(float left,float width,float ground,float height,float remaining,Color tint)
        {
            float start=Mathf.Max(left,-60),end=Mathf.Min(left+width,1340);
            for(float x=start+16;x<end;x+=34)
                DrawBossPillar(x,ground-height,ground,30,remaining,tint,(x-left)*.13f);
            DrawBossImpact(new Vector2(left+width*.5f,ground-6),Mathf.Min(width,160),remaining,tint);
        }
        void DrawBossCurtain(float left,float right,float top,float ground,float remaining,Color tint)
        {
            left=Mathf.Clamp(left,0,1280);right=Mathf.Clamp(right,0,1280);
            if(right<=left||ground<=top)return;
            // Clip all flame halos to the hazardous band: no fire in the safe gap.
            GUI.BeginGroup(new Rect(left,top,right-left,ground-top));
            try
            {
                tint.a=.55f;
                for(float x=0;x<right-left+35;x+=45)
                    DrawBossPillar(x,0,ground-top,42,remaining,tint,x*.17f);
            }
            finally {GUI.EndGroup();}
        }
        void DrawBossChain(Vector2 from,Vector2 to,float alpha,float bend,float phase,bool hook)
        {
            EnsureBossEffectArt();if(bossChainRitual==null||alpha<=0)return;
            int count=Mathf.Clamp(Mathf.CeilToInt(Vector2.Distance(from,to)/13),1,64);
            for(int i=0;i<count;i++)
            {
                var a=SpellChainPoint(from,to,i/(float)count,visualClock+phase,bend);
                var b=SpellChainPoint(from,to,(i+1)/(float)count,visualClock+phase,bend);
                float twist=i%2==0?1:.42f+.12f*Mathf.Sin(visualClock*11+i);
                DrawBossMetal(new Rect(90,185,460,250),(a+b)*.5f,new Vector2(Vector2.Distance(a,b)+5,10*twist),
                    Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg,alpha);
            }
            if(hook)
            {
                var tangent=to-SpellChainPoint(from,to,.96f,visualClock+phase,bend);
                DrawBossMetal(new Rect(665,95,555,415),to,new Vector2(38,28),
                    Mathf.Atan2(tangent.y,tangent.x)*Mathf.Rad2Deg,alpha);
            }
        }
        void DrawBossMetal(Rect source,Vector2 center,Vector2 size,float angle,float alpha)
        {
            if(bossChainRitual==null||alpha<=0)return;
            var color=GUI.color;
            using(ArtworkAt(center,angle))
            {
                try
                {
                    GUI.color=new Color(1,1,1,Mathf.Clamp01(alpha));
                    DrawArtwork(bossChainRitual,new Rect(-size*.5f,size),new Rect(source.x/1254,1-source.yMax/1254,source.width/1254,source.height/1254));
                }
                finally {GUI.color=color;}
            }
        }
        void DrawBossMagicOrb(BloodOrb orb,Vector2 point)
        {
            var velocity=new Vector2(orb.VelocityX,-orb.VelocityY);
            float angle=Mathf.Atan2(velocity.y,velocity.x)*Mathf.Rad2Deg;
            int chapter=orb.Owner.LateChapter;
            if(chapter==4){DrawVarkanGearOrb(point,velocity);return;}
            if(chapter==6){DrawNoctisFeatherOrb(point,velocity);return;}
            if(chapter==0)
            {
                EnsurePresentationArt();
                using(ArtworkAt(point,angle))
                {
                    DrawFlameFrame(Vector2.zero,new Vector2(43,32),0,new Color(1,.24f,.4f));
                    DrawFlameFrame(new Vector2(-10,0),new Vector2(35,25),3,new Color(1,.12f,.3f,.32f));
                }
                return;
            }
            var spell=chapter==4?SpellKind.CrownGear:chapter==5?SpellKind.MirrorShards:
                chapter==6?SpellKind.MoonBlades:chapter==7?SpellKind.RoyalLance:SpellKind.EmberWave;
            // Physical projectile radius stays .22; the faint tail is decoration.
            DrawAnimatedSpell(spell,point,chapter==0?new Vector2(43,32):new Vector2(38,30),1,angle,1);
        }
    }
}

