using System.Collections.Generic;
using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        readonly Dictionary<object,Vector2> previousPositions=new Dictionary<object,Vector2>();
        float visualClock;
        float MotionAlpha=>serviceNpc>=0||(village&&dialoguePage>=0)||paused||settings||inventoryOpen||skillWindowOpen||StoryActive||ShowSkillChoice||run==null||run.Phase!=RunPhase.Combat||Time.unscaledTime<blockedUntil?1:Mathf.Clamp01(accumulator/RunModel.StepSeconds);
        Vector2 RenderPosition(object key,float x,float y)
        {
            var current=new Vector2(x,y);
            return previousPositions.TryGetValue(key,out var previous)?Vector2.Lerp(previous,current,MotionAlpha):current;
        }
        void CaptureWorldMotion()
        {
            previousPositions.Clear();if(run==null)return;
            previousPositions[run]=new Vector2(run.X,run.Y);
            foreach(var e in run.Enemies)previousPositions[e]=new Vector2(e.X,e.Y);
            foreach(var wave in run.Waves)previousPositions[wave]=new Vector2(wave.X,wave.Y);
            foreach(var arrow in run.Arrows)previousPositions[arrow]=new Vector2(arrow.X,arrow.Y);
            foreach(var shot in run.LateShots)previousPositions[shot]=new Vector2(shot.X,shot.Y);
            foreach(var orb in run.BloodOrbs)previousPositions[orb]=new Vector2(orb.X,orb.Y);
        }
        void ResetMotion()
        {
            visualClock=0;enemyMotionStates.Clear();CaptureWorldMotion();
        }
        void LateUpdate()
        {
            if(title||(village&&dialoguePage>=0)||paused||settings||inventoryOpen||skillWindowOpen||serviceNpc>=0||StoryActive||ShowSkillChoice||Time.unscaledTime<blockedUntil||(!village&&(run==null||run.Phase!=RunPhase.Combat)))return;
            float dt=Mathf.Min(Time.unscaledDeltaTime,.1f);visualClock+=dt;
            if(!village)UpdateMapCamera(dt);
        }
        void DrawCharacterPose(Texture2D texture,HeroFrame frame,Vector2 size,float scale,float x,float y)
        {
            // Two source-over layers at 50% alpha cover only 75% of an opaque body,
            // causing a brightness pulse at every pose change. Draw one authored
            // frame instead; position interpolation and intermediate frames stay intact.
            // Keep the caller's tint/alpha for intentional damage and death effects.
            DrawHeroRegion(texture,size,frame.Source,new Vector2(x-frame.Feet.x*scale,y-frame.Feet.y*scale),scale);
        }
    }
}
