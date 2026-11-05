using System.Collections.Generic;
using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        // Every actor has 24 authored poses. One body scale per actor, never a
        // per-frame silhouette fit: raised weapons must not shrink the body.
        sealed class EnemyMotionClip
        {
            public readonly string Name;
            public readonly float Scale, RootX, RootY, CycleRate;
            public Texture2D Texture;
            public float RetryAt;
            public EnemyMotionClip(string name,float scale,float rootX,float rootY,float cycleRate)
            {Name=name;Scale=scale;RootX=rootX;RootY=rootY;CycleRate=cycleRate;}
            public bool Load()
            {
                if(Texture!=null)return true;
                if(Time.realtimeSinceStartup<RetryAt)return false;
                RetryAt=Time.realtimeSinceStartup+1;
                Texture=Resources.Load<Texture2D>("Demons/Motion/"+Name);
                if(Texture==null)return false;
                Texture.filterMode=FilterMode.Point;Texture.wrapMode=TextureWrapMode.Clamp;
                return true;
            }
        }
        sealed class EnemyMotionState
        {
            public float Clock, Cycle;
            public int LastPose;
        }
        readonly Dictionary<Enemy,EnemyMotionState> enemyMotionStates=new Dictionary<Enemy,EnemyMotionState>();
        readonly EnemyMotionClip[] enemyMotionClips={
            new EnemyMotionClip("MeleeMotionV3",.378f,150,240,1.45f),
            new EnemyMotionClip("ArcherMotionV3",.378f,150,240,1.45f),
            new EnemyMotionClip("HeavyMotionV3",.475f,150,237,1.1f),
            new EnemyMotionClip("Stage1MotionV3",.65f,140,240,1.2f),
            new EnemyMotionClip("Stage2MotionV3",.77f,132,246,.95f),
            new EnemyMotionClip("Stage3MotionV3",.71f,145,246,1f),
            new EnemyMotionClip("Stage4MotionV3",.75f,140,240,1f),
            new EnemyMotionClip("Stage5MotionV3",.64f,140,245,1f),
            new EnemyMotionClip("Stage6MotionV3",.96f,140,235,1.15f),
            new EnemyMotionClip("Stage7MotionV3",.84f,140,240,.95f)
        };

        // Measured contact lines (not overall sprite heights). Only ground
        // actors use these; flight retains its authored vertical wing/body arc.
        static readonly float[][] EnemyContactLines={
            new float[]{239,239,239,239,238,238,238,238,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240},
            new float[]{240,240,240,240,241,240,239,242,238,237,237,237,237,237,237,238,239,239,239,239,239,239,239,239},
            new float[]{237,236,236,237,237,236,234,237,235,235,235,235,235,234,235,235,234,234,234,234,234,235,235,234},
            null,
            new float[]{246,246,246,246,245,245,245,244,244,244,244,244,242,242,242,242,242,242,243,243,243,243,243,243},
            new float[]{246,245,245,245,243,243,244,244,242,242,242,241,242,242,242,243,244,244,244,244,244,244,244,244},
            new float[]{234,233,232,233,232,232,231,232,233,231,231,231,231,231,231,232,237,237,237,237,238,238,238,238},
            new float[]{245,244,244,245,244,244,244,244,239,239,239,239,238,238,238,238,241,242,242,241,240,240,240,240},
            null,
            new float[]{238,238,237,236,236,234,237,237,238,238,238,239,239,238,239,239,238,240,241,241,241,240,241,238}
        };
        static readonly int[] HammerAnticipation={0,8,9,10,11};
        static readonly float[] GuardianHipRoots={150,158,158,161,150,158,157,160,157,154,157,132,132,132,153,164,154,154,154,154,154,154,154,154};

        int EnemyMotionIndex(Enemy e)=>!e.Boss?(int)e.Kind:e.LateChapter>=4?e.LateChapter+2:e.SolarRegent?5:e.ChainWarden?4:3;
        bool EnemyPrimaryMotion(Enemy e)
        {
            if(!e.Boss)return true;
            if(e.LateChapter>=4)return !e.LateColumns&&!e.LateSafeBand&&!e.LateVolley;
            if(e.SolarRegent)return e.Pattern==BossPattern.SolarSweep;
            if(e.ChainWarden)return e.Pattern==BossPattern.ChainSlam;
            return e.Pattern==BossPattern.Dash;
        }

        int EnemyMotionPose(Enemy e,int index,EnemyMotionClip clip)
        {
            if(!enemyMotionStates.TryGetValue(e,out var state))
            {state=new EnemyMotionState{Clock=e.AnimationTime};enemyMotionStates.Add(e,state);}
            float dt=Mathf.Clamp(e.AnimationTime-state.Clock,0,.1f);state.Clock=e.AnimationTime;
            if(e.Hp<=0)return state.LastPose;
            bool flying=index==3||index==8;
            if(e.Walking||flying)state.Cycle=Mathf.Repeat(state.Cycle+dt*clip.CycleRate,1);
            else
            {
                // Finish the current step instead of snapping to a global
                // animation-clock phase whenever the AI starts or stops walking.
                if(state.Cycle>0)
                    state.Cycle=Mathf.Repeat(Mathf.MoveTowards(state.Cycle,1,dt*clip.CycleRate*1.5f),1);
            }
            int pose=Mathf.Min(7,(int)(state.Cycle*8));
            bool primary=EnemyPrimaryMotion(e);
            int windup=primary?8:16,release=primary?12:20;
            if(e.PhaseTransition>0)
            {
                float duration=e.LateChapter>=4||e.ChainWarden?1.2f:e.SolarRegent?1.1f:1;
                pose=TimedPose(e.PhaseTransition,duration,16,8);
            }
            else if(e.Windup>0)
            {
                // The melee/axe and scythe atlases have a fifth apex pose.
                bool apex=primary&&(index==0||index==2||index==8||index==9);
                pose=TimedPose(e.Windup,e.WindupDuration,windup,apex?5:4);
                if(primary&&index==6)pose=HammerAnticipation[TimedPose(e.Windup,e.WindupDuration,0,5)];
                if(primary&&index==5)pose=TimedPose(e.Windup,e.WindupDuration,8,3);
            }
            else if(e.DashRemaining>0)pose=index==8?13:TimedPose(e.DashRemaining,e.DashDuration,12,2);
            else if(e.AttackPose>0)
            {
                bool apex=primary&&(index==0||index==2||index==8||index==9);
                pose=TimedPose(e.AttackPose,e.AttackReleaseDuration,release+(apex?1:0),apex?3:4);
                if(primary&&index==5)pose=TimedPose(e.AttackPose,e.AttackReleaseDuration,11,5);
                if(primary&&index==9)
                {
                    pose=TimedPose(e.AttackPose,e.AttackReleaseDuration,13,4);
                    if(pose==16)pose=23;
                }
                if(index==3&&e.Pattern==BossPattern.Dash)
                    pose=TimedPose(e.AttackPose,e.AttackReleaseDuration,14,2);
                state.Cycle=0;
            }
            state.LastPose=pose;return pose;
        }

        bool SelectEnemyMotion(Enemy enemy,ref Texture2D texture,ref HeroFrame frame,ref Vector2 size,ref float scale,out int motionPose)
        {
            motionPose=0;
            int index=EnemyMotionIndex(enemy);var clip=enemyMotionClips[index];
            // Preserve the king's authored seated-to-standing introduction.
            if(index==9&&run.BossIntroducing&&BossIntroElapsed<1.55f)return false;
            if(!clip.Load())return false;
            motionPose=EnemyMotionPose(enemy,index,clip);
            float cellX=(motionPose%4)*256,cellY=(motionPose/4)*256;
            float contactY=EnemyContactLines[index]==null?clip.RootY:EnemyContactLines[index][motionPose];
            float rootX=index==3?GuardianHipRoots[motionPose]:clip.RootX;
            frame=new HeroFrame(cellX,cellY,256,256,cellX+rootX,cellY+contactY);
            texture=clip.Texture;size=new Vector2(1024,1536);scale=clip.Scale;
            return true;
        }

        // Separate props follow the new authored head/chest coordinates, not
        // the old 8-frame atlas. Values are local to each 256px cell.
        static readonly Vector2[] LysseaMotionFaces={
            new Vector2(145,74),new Vector2(145,74),new Vector2(145,74),new Vector2(145,74),
            new Vector2(145,74),new Vector2(145,74),new Vector2(145,74),new Vector2(145,74),
            new Vector2(145,80),new Vector2(151,80),new Vector2(151,80),new Vector2(158,80),
            new Vector2(162,90),new Vector2(158,90),new Vector2(147,74),new Vector2(147,74),
            new Vector2(145,74),new Vector2(145,74),new Vector2(145,74),new Vector2(145,74),
            new Vector2(145,74),new Vector2(145,74),new Vector2(145,74),new Vector2(145,74)
        };
        void DrawEnemyMotionProps(Enemy boss,int pose,HeroFrame frame,float scale,float x,float y)
        {
            if(boss.LateChapter==5)
            {
                EnsureMirrorArt();
                var center=new Vector2(x,y)+(LysseaMotionFaces[pose]-frame.Feet)*scale;
                DrawLysseaMaskPart(LysseaMaskIndex(boss.Pattern),center,27*scale,GUI.color,0);
            }
            if(boss.LateChapter==7&&boss.PhaseTwo)
            {
                var chest=new Vector2(x,y)+(new Vector2(140,115)-frame.Feet)*scale;
                DrawRoyalSunPart(2,chest,new Vector2(34,47),GUI.color.a*(.45f+.08f*Mathf.Sin(visualClock*3)));
            }
        }
    }
}
