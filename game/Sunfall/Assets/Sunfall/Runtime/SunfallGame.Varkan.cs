using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        Texture2D varkanHammerSheet;
        static readonly HeroFrame[] VarkanHammerFrames={
            new HeroFrame(0,0,444,444,244,392),new HeroFrame(444,0,443,444,694,393),
            new HeroFrame(887,0,443,444,1138,393),new HeroFrame(1330,0,444,444,1543,392),
            new HeroFrame(0,444,444,443,224,820),new HeroFrame(444,444,443,443,666,820),
            new HeroFrame(887,444,443,443,1106,820),new HeroFrame(1330,444,444,443,1556,821)
        };
        bool SelectVarkanHammer(Enemy boss,out HeroFrame frame)
        {
            frame=VarkanHammerFrames[0];
            if(varkanHammerSheet==null||boss.Hp<=0||boss.PhaseTransition>0||boss.Pattern!=BossPattern.HammerSweep)return false;
            int pose;
            if(boss.Windup>0)
            {
                float t=1-Mathf.Clamp01(boss.Windup/boss.WindupDuration);
                pose=t<.18f?0:t<.43f?1:t<.82f?2:3;
            }
            else if(boss.AttackPose>0)
            {
                // Core applies the hit when windup ends; contact is frame 4.
                float t=1-Mathf.Clamp01(boss.AttackPose/boss.AttackReleaseDuration);
                pose=t<.22f?4:t<.48f?5:t<.76f?6:7;
            }
            else return false;
            frame=VarkanHammerFrames[pose];return true;
        }
        void DrawVarkanHammerImpact(Enemy boss,float left,float width,float ground,float remaining,Color tint)
        {
            // The hammer impact sends a ground shock across the existing warned
            // reach. Particle travel is decorative and never delays damage.
            float age=1-Mathf.Clamp01(remaining);
            float impactX=SX(boss.X+boss.Facing*2);
            DrawBossImpact(new Vector2(impactX,ground-6),130,remaining,new Color(.85f,.78f,.62f));
            float visibleLeft=Mathf.Max(-60,left),visibleRight=Mathf.Min(1340,left+width);
            for(float x=visibleLeft+20;x<visibleRight;x+=42)
            {
                float alpha=remaining*(.25f+.5f*Mathf.Sin(age*Mathf.PI));
                DrawBossRitual(3,new Vector2(x,ground-4-age*9),new Vector2(54,20+age*15),
                    new Color(tint.r,tint.g,tint.b,alpha));
            }
        }
        void DrawVarkanGearOrb(Vector2 point,Vector2 velocity)
        {
            DrawCrownPart(2,point,29,Color.white,visualClock*300);
            EnsurePresentationArt();
            var direction=velocity.normalized;
            for(int i=1;i<=3;i++)
                DrawAnimatedSpark(point-direction*i*7,7-i,new Color(1,.45f,.18f,.4f/i));
        }
    }
}

