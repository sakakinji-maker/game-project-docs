using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        // Hot-core centroids in each original 384x512 frame. The lower row's
        // core is drawn higher; a common cell-centre pivot would visibly jump.
        static readonly Vector2[] FlameCorePivots={
            new Vector2(296.5f,306.8f),new Vector2(300.4f,309.5f),new Vector2(304.7f,307.8f),new Vector2(306,309),
            new Vector2(299.4f,275.6f),new Vector2(299.9f,275.1f),new Vector2(303.3f,278.4f),new Vector2(299.2f,275.7f)
        };
        // Animation is evaluated from the paused visual clock. OnGUI may run
        // several times per frame; it never advances particles or consumes RNG.
        void DrawAnimatedSpell(SpellKind kind,Vector2 center,Vector2 size,float alpha,float angle,int facing)
        {
            if(alpha<=0||size.x<=0||size.y<=0)return;
            EnsurePresentationArt();
            float extent=Mathf.Max(size.x,size.y);
            if(center.x+extent<-80||center.x-extent>1360||center.y+extent<-80||center.y-extent>660)return;
            var color=GUI.color;
            using(ArtworkAt(center,angle,facing))
            try
            {
                if(kind==SpellKind.EmberWave)
                {
                    // The flame's contour changes, while its damaging core stays put.
                    DrawFlameFrame(new Vector2(-size.x*.25f,0),size*.85f,3,new Color(1,.55f,.32f,alpha*.28f));
                    DrawFlameFrame(Vector2.zero,size,0,new Color(1,1,1,alpha));
                    DrawSpellStream(size,alpha,kind,9);return;
                }
                bool lance=kind==SpellKind.AshLance||kind==SpellKind.SunLance||kind==SpellKind.RoyalLance;
                if(lance)
                {
                    DrawSpellEffectCell((int)kind,Vector2.zero,size,alpha);
                    if(kind==SpellKind.AshLance)
                    {
                        for(int i=0;i<3;i++)
                            DrawFlameFrame(new Vector2(-size.x*(.1f+i*.18f),Mathf.Sin(visualClock*11+i*2)*3),
                                new Vector2(size.x*.52f,size.y*.65f),i*2,new Color(1,.85f,.6f,alpha*.65f));
                    }
                    DrawSpellStream(size,alpha,kind,12);return;
                }
                bool ring=kind==SpellKind.CrimsonNova||kind==SpellKind.DawnBurst||kind==SpellKind.MirrorWard||
                    kind==SpellKind.NightMantle||kind==SpellKind.SolarCrown;
                if(ring)
                {
                    float turn=kind==SpellKind.SolarCrown?0:visualClock*(kind==SpellKind.MirrorWard?-13:18);
                    DrawSpellEffectCell((int)kind,Vector2.zero,size,alpha,turn);
                    DrawOrbitingSpellParticles(kind,size,alpha);return;
                }
                if(kind==SpellKind.HeartBreaker)
                {
                    DrawSpellEffectCell((int)kind,Vector2.zero,size,alpha);
                    DrawOrbitingSpellParticles(kind,size,alpha);return;
                }
                if(kind==SpellKind.MoonBlades||kind==SpellKind.MirrorShards)
                {
                    for(int i=3;i>0;i--)
                        DrawSpellEffectCell((int)kind,new Vector2(-i*9,Mathf.Sin(visualClock*9+i)*i*1.4f),size*(1-i*.08f),alpha*.10f);
                }
                DrawSpellEffectCell((int)kind,Vector2.zero,size,alpha);
                DrawSpellStream(size,alpha,kind,6);
            }
            finally {GUI.color=color;}
        }
        void DrawFlameFrame(Vector2 center,Vector2 size,float phase,Color tint)
        {
            if(flameAnimationSheet==null)
            {DrawSpellEffectCell(0,center,size,tint.a);return;}
            int frame=Mathf.FloorToInt(visualClock*16+phase)%8;
            var color=GUI.color;GUI.color=tint;
            var pivot=FlameCorePivots[frame];
            // Match DrawAtlasCell's inset in normalized cell coordinates.
            var anchor=new Vector2((pivot.x/384-.012f)/.976f,(pivot.y/512-.012f)/.976f);
            DrawAtlasCell(flameAnimationSheet,new Rect(center-Vector2.Scale(anchor,size),size),frame,4,2);
            GUI.color=color;
        }
        void DrawSpellStream(Vector2 size,float alpha,SpellKind kind,int count)
        {
            var tint=SkillAccent(kind);
            for(int i=0;i<count;i++)
            {
                float age=Mathf.Repeat(visualClock*(1.7f+i*.037f)+i*.618f,1);
                float side=i%2==0?1:-1;
                var point=new Vector2(size.x*(.12f-age*.83f),side*size.y*(.05f+age*.28f)+Mathf.Sin(age*9+i)*3);
                float diameter=(3+(i%3)*2)*(1-age*.7f);
                DrawAnimatedSpark(point,diameter,new Color(tint.r,tint.g,tint.b,alpha*(1-age)*.85f));
            }
        }
        void DrawOrbitingSpellParticles(SpellKind kind,Vector2 size,float alpha)
        {
            var tint=SkillAccent(kind);
            for(int i=0;i<12;i++)
            {
                float phase=i*Mathf.PI/6+visualClock*.55f;
                float age=Mathf.Repeat(visualClock*1.4f+i*.618f,1);
                var direction=new Vector2(Mathf.Cos(phase),Mathf.Sin(phase));
                var point=Vector2.Scale(direction,size)*(.30f+age*.14f);
                if(kind==SpellKind.CrimsonNova)
                {
                    using(ArtworkAt(point,phase*Mathf.Rad2Deg))
                        DrawFlameFrame(Vector2.zero,new Vector2(35,27)*(1-age*.25f),i,new Color(1,.55f,.5f,alpha*(1-age)*.8f));
                }
                else DrawAnimatedSpark(point,6+7*(1-age),new Color(tint.r,tint.g,tint.b,alpha*(1-age)*.8f));
            }
        }
        void DrawAnimatedSpark(Vector2 point,float diameter,Color tint)
        {
            if(playerSpellSheet==null||diameter<=0||tint.a<=0)return;
            var color=GUI.color;GUI.color=tint;
            DrawAtlasCell(playerSpellSheet,new Rect(point.x-diameter*.5f,point.y-diameter*.5f,diameter,diameter),15,4,4);
            GUI.color=color;
        }
        void DrawCastAnimation(Vector2 center,float size)
        {
            EnsurePresentationArt();
            DrawSpellEffectCell(15,center,Vector2.one*size,.85f,visualClock*80);
            for(int i=0;i<5;i++)
            {
                float age=Mathf.Repeat(visualClock*2+i*.2f,1),a=i*Mathf.PI*.4f+visualClock*3;
                var offset=new Vector2(Mathf.Cos(a),Mathf.Sin(a))*size*.7f*(1-age);
                DrawAnimatedSpark(center+offset,7,new Color(1,.8f,.4f,age));
            }
        }
        static Vector2 SpellChainPoint(Vector2 from,Vector2 to,float t,float time,float bend)
        {
            var delta=to-from;var normal=delta.sqrMagnitude>.001f?new Vector2(-delta.y,delta.x).normalized:Vector2.up;
            return Vector2.Lerp(from,to,t)+normal*(Mathf.Sin(t*Mathf.PI)*Mathf.Sin(t*8-time*22)*bend);
        }
        void DrawAnimatedChain(Vector2 from,Vector2 to,float alpha,float bend,float phase,bool hook)
        {
            EnsurePresentationArt();if(playerSpellSheet==null||Vector2.Distance(from,to)<2)return;
            int links=Mathf.Clamp(Mathf.CeilToInt(Vector2.Distance(from,to)/12),1,48);
            float time=visualClock+phase;
            // Separate metal link and hook samples, not an elongated whole spell.
            var linkSource=new Rect(1000,181,34,24);
            for(int i=0;i<links;i++)
            {
                var a=SpellChainPoint(from,to,(float)i/links,time,bend);
                var b=SpellChainPoint(from,to,(float)(i+1)/links,time,bend);
                var delta=b-a;float angle=Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg;
                float twist=i%2==0?1:.55f+.12f*Mathf.Sin(time*9+i);
                DrawChainPiece(linkSource,(a+b)*.5f,new Vector2(delta.magnitude+4,12*twist),angle,alpha);
            }
            if(hook)
            {
                var tangent=to-SpellChainPoint(from,to,.94f,time,bend);
                DrawChainPiece(new Rect(1121,134,95,119),to,new Vector2(29,36),Mathf.Atan2(tangent.y,tangent.x)*Mathf.Rad2Deg,alpha);
            }
        }
        void DrawChainPiece(Rect source,Vector2 center,Vector2 size,float angle,float alpha)
        {
            if(playerSpellSheet==null||alpha<=0)return;
            var color=GUI.color;
            using(ArtworkAt(center,angle))
            {
                try
                {
                    GUI.color=new Color(1,1,1,Mathf.Clamp01(alpha));
                    DrawArtwork(playerSpellSheet,new Rect(-size*.5f,size),new Rect(source.x/1254,1-source.yMax/1254,source.width/1254,source.height/1254));
                }
                finally {GUI.color=color;}
            }
        }
    }
}
