using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        // V3 poses were calibrated to the crouched silhouette's height, making
        // their anatomy larger than the idle/walk atlas. Apply the same body
        // correction to grounded actions, including reused combo seam poses.
        const float HeroAttackBodyScale=.8f;
        // AirSlashV3 has larger head/torso proportions even after the grounded
        // correction. Match anatomy to HeroAtlas, not its tucked-leg bounds or
        // sword arc. Use one fixed scale through anticipation and recovery.
        const float HeroAirAttackBodyScale=.7f;
        const float HeroAirPelvisHeight=41f;
        bool SwordArtReady
        {
            get {foreach(var clip in heroAttackClips)if(clip.Texture==null)return false;return true;}
        }
        void LoadSwordArt(){foreach(var clip in heroAttackClips)clip.Load();}
        bool SelectSwordArt(out Texture2D texture,out HeroFrame frame,out Vector2 sourceSize,out float scale)
        {
            texture=null;frame=default;sourceSize=Vector2.zero;scale=0;
            if(!SwordArtReady||run==null||run.Rolling)return false;
            int action,pose;
            if(run.Casting||run.MagicRecoveryTimer>0)
            {
                action=4;
                pose=run.Casting?TimedPose(run.CastTimer,RunModel.MagicCastDuration,0,3):
                    TimedPose(run.MagicRecoveryTimer,RunModel.MagicRecoveryDuration,3,5);
            }
            else
            {
                bool airRecovery=run.AirSwordRecovery;
                if(!run.SwordActive&&!airRecovery)return false;
                action=run.AirAttack?3:Mathf.Clamp(run.AttackStage-1,0,2);
                pose=airRecovery?7:run.SwordPreparing?
                    TimedPose(run.SwordWindupRemaining,run.SwordWindupDuration,0,3):
                    TimedPose(run.AttackTimer,run.SwordReleaseDuration,3,5);
                // Exact previous recovery on the combo seam, followed by new inbetweens.
                if(action>0&&action<3&&pose==0){action--;pose=7;}
            }
            var clip=heroAttackClips[action];
            texture=clip.Texture;frame=clip.Frames[pose];sourceSize=clip.Size;
            scale=clip.Scale*(action==3?HeroAirAttackBodyScale:HeroAttackBodyScale);
            if(action==3)
            {
                // Feet is a virtual base for airborne art. Preserve the pelvis
                // height in canvas pixels when changing the sprite scale.
                frame.Feet.y+=HeroAirPelvisHeight/scale-HeroAirPelvisHeight/clip.Scale;
            }
            return true;
        }
    }
}

