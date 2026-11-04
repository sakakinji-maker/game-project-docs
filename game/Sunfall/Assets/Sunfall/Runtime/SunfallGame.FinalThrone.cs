using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        readonly Texture2D[] throneRooms=new Texture2D[4];
        static readonly string[] ThroneRoomResources={
            "World/Stage7PalaceApproachHDV2","World/Stage7FuneralGalleryHDV2",
            "World/Stage7SunGateHDV2","World/Stage7ThroneHallHDV2"
        };
        Texture2D throneDawn,throneEnding,royalSunEffects;
        float nextThroneArtRetry;
        static readonly Vector2[] SolveinChestCenters={
            new Vector2(250,247),new Vector2(714,207),new Vector2(1130,178),new Vector2(1600,185),
            new Vector2(258,630),new Vector2(654,668),new Vector2(1118,642),new Vector2(1578,639)
        };
        void LoadThroneArt()
        {
            for(int i=0;i<throneRooms.Length;i++)
            {
                throneRooms[i]=Resources.Load<Texture2D>(ThroneRoomResources[i]);
                if(throneRooms[i]!=null){throneRooms[i].filterMode=FilterMode.Point;throneRooms[i].wrapMode=TextureWrapMode.Clamp;}
            }
            throneDawn=Resources.Load<Texture2D>("World/Stage7ThroneDawnHDV2");
            throneEnding=Resources.Load<Texture2D>("World/Stage7ThroneEndingV2");
            royalSunEffects=Resources.Load<Texture2D>("Effects/Stage7RoyalSunV2");
            foreach(var texture in new[]{throneDawn,throneEnding,royalSunEffects})
                if(texture!=null){texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;}
            nextThroneArtRetry=Time.realtimeSinceStartup+1;
        }
        void EnsureThroneArt()
        {
            bool missing=throneDawn==null||throneEnding==null||royalSunEffects==null;
            foreach(var room in throneRooms)missing|=room==null;
            if(missing&&Time.realtimeSinceStartup>=nextThroneArtRetry)LoadThroneArt();
        }
        float FinalDawn=>run.BossDefeated?Mathf.SmoothStep(0,1,Mathf.Clamp01((BossDefeatElapsed-1.5f)/1.3f)):0;
        void DrawThroneBackdrop()
        {
            EnsureThroneArt();var texture=throneRooms[Mathf.Clamp(run.RoomIndex,0,3)];
            Box(new Rect(0,0,1280,600),new Color(.025f,.02f,.035f));
            if(texture==null)return;
            const float width=2070;
            float offset=Mathf.Clamp01(cameraX/(RunModel.WorldWidth-32))*(width-1280);
            var target=new Rect(-offset,-30+cameraY*8,width,width*texture.height/texture.width);
            var color=GUI.color;GUI.color=Color.white;
            GUI.DrawTexture(target,texture,ScaleMode.StretchToFill,false);
            if(run.Stage==RunStage.BossArena&&throneDawn!=null&&FinalDawn>0)
            {
                GUI.color=new Color(1,1,1,FinalDawn);
                GUI.DrawTexture(target,throneDawn,ScaleMode.StretchToFill,true);
            }
            GUI.color=color;
        }
        void DrawThronePart(int cell,Vector2 center,Vector2 size,float alpha=1,float angle=0)
        {
            EnsureThroneArt();if(throneEnding==null||alpha<=0)return;
            var color=GUI.color;
            using(ArtworkAt(center,angle))
            {
                try
                {
                    GUI.color=new Color(1,1,1,Mathf.Clamp01(alpha));
                    DrawArtwork(throneEnding,new Rect(-size*.5f,size),new Rect(cell%3/3f,1-(cell/3+1)/2f,1/3f,.5f));
                }
                finally {GUI.color=color;}
            }
        }
        void DrawThroneLandmarks()
        {
            if(run.Stage!=RunStage.BossArena)return;
            EnsureThroneArt();
            float x=SX(RunModel.BossSpawnX),floor=SY(0),time=BossDefeatElapsed;
            // Align the actual painted seat (x344), not the atlas-cell center.
            DrawThronePart(0,new Vector2(x+(256-344)*.46f,floor+(256-465)*.46f),Vector2.one*(512*.46f));
            Vector2 sun=new Vector2(x,SY(8.7f));
            float restored=run.BossDefeated?Mathf.SmoothStep(0,1,Mathf.Clamp01((time-1.15f)/1.35f)):0;
            DrawThronePart(1,sun,Vector2.one*190,1-restored);
            DrawThronePart(2,sun,Vector2.one*190,restored);
            float release=run.BossDefeated?Mathf.SmoothStep(0,1,Mathf.Clamp01((time-.65f)/.9f)):0;
            Vector2 mother=new Vector2(x+138,Mathf.Lerp(floor-220,floor-50,release));
            DrawThronePart(3,mother,Vector2.one*120,1-release);
            // The free portrait is slightly taller in source pixels; match her height.
            DrawThronePart(4,mother,Vector2.one*110,release);
            for(int i=0;i<5;i++)
            {
                Vector2 from=sun+new Vector2(-45+i*22,38);
                Vector2 to=mother+new Vector2(-25+i*12,30);
                Vector2 mid=(from+to)*.5f+new Vector2(Mathf.Sin(visualClock+i)*6,16);
                var gold=new Color(1,.72f,.32f,(1-release)*.55f);
                GothicLine(from,mid,gold,1);GothicLine(mid,to,gold,1);
            }
            if(run.BossDefeated)
            {
                DrawFinalSkyStitches(sun,new Color(1,.83f,.42f));
                float crownAlpha=Mathf.Clamp01(time/.18f)*Mathf.Clamp01((1.2f-time)/.55f);
                DrawThronePart(5,new Vector2(SX(run.BossDefeatX),SY(run.BossDefeatY)-140-time*18),
                    Vector2.one*(45+time*22),crownAlpha,time*28);
            }
        }
        void DrawFinalSkyStitches(Vector2 center,Color color)
        {
            float p=Mathf.Clamp01((BossDefeatElapsed-1.15f)/1.35f);
            Vector2 from=center+new Vector2(-150,-30),to=center+new Vector2(150,20);
            var path=new SewnScarPath(from,to,-9);
            DrawSewnScar(path,p,22,color,Mathf.Clamp01((BossDefeatElapsed-2.5f)/.7f));
            DrawScarAfterglow(path,BossDefeatElapsed-2.5f,color);
        }
        int SolveinPose(Enemy boss)
        {
            if(run.BossIntroducing)return BossIntroElapsed<.85f?0:BossIntroElapsed<1.55f?1:2;
            if(boss.PhaseTransition>0)return 7;
            if(boss.Windup>0||boss.AttackPose>0)
                return boss.Pattern==BossPattern.LastEclipse?7:boss.Pattern==BossPattern.Sunfall?6:boss.Windup>0?4:5;
            return boss.Walking?2+(int)(boss.AnimationTime*5)%2:2;
        }
        void DrawSolveinArmorLight(Enemy boss,int pose,HeroFrame frame,float scale,float x,float y)
        {
            if(!boss.PhaseTwo)return;
            Vector2 chest=new Vector2(x,y)+(SolveinChestCenters[pose]-frame.Source.position-frame.Feet)*scale;
            DrawRoyalSunPart(2,chest,new Vector2(34,47),GUI.color.a*(.45f+.08f*Mathf.Sin(visualClock*3)));
        }
        void DrawSolveinArrivalEffect()
        {
            float time=BossIntroElapsed;
            float alpha=Mathf.Clamp01((time-.25f)/.4f)*Mathf.Clamp01((2.4f-time)/.65f);
            Vector2 center=new Vector2(SX(RunModel.BossSpawnX),SY(0)-65);
            DrawRoyalSunPart(3,center,new Vector2(110,38),alpha*.6f,time*25);
            for(int i=0;i<12;i++)
            {
                float rise=Mathf.Repeat(time*60+i*19,180);
                GothicDiamond(center.x+Mathf.Sin(i*2.4f)*37,center.y-rise,2,
                    new Color(1,.53f,.35f,alpha*(1-rise/180)));
            }
        }
        void DrawRoyalSunPart(int cell,Vector2 center,Vector2 size,float alpha=1,float angle=0)
        {
            EnsureThroneArt();if(royalSunEffects==null||alpha<=0)return;
            var color=GUI.color;
            using(ArtworkAt(center,angle))
            {
                try
                {
                    GUI.color=new Color(1,1,1,Mathf.Clamp01(alpha));
                    DrawArtwork(royalSunEffects,new Rect(-size*.5f,size),new Rect(cell%2*.5f,1-(cell/2+1)*.5f,.5f,.5f));
                }
                finally {GUI.color=color;}
            }
        }
        void DrawRoyalFallingSword(float x,float top,float ground,float remaining)
        {
            // Damage is applied at release: the blade already spans the danger
            // column at that instant. Only its glow settles toward the ground.
            float height=ground-top;
            DrawRoyalSunPart(0,new Vector2(x,(top+ground)*.5f),new Vector2(150,height*1.28f),Mathf.Clamp01(remaining*5));
            DrawRoyalSunPart(3,new Vector2(x,ground-6),new Vector2(78,35),Mathf.Clamp01(remaining*4),visualClock*45);
        }
        void DrawRoyalCleave(float left,float width,float ground,float remaining)
        {
            // Width matches the two-sided existing hit area; a low arc leaves
            // the jump/platform escape readable without changing its hitbox.
            DrawRoyalSunPart(1,new Vector2(left+width*.5f,ground-20),new Vector2(width*1.28f,110),Mathf.Clamp01(remaining*5));
        }
        void DrawRoyalEclipseCurtain(float left,float right,float top,float ground,float remaining)
        {
            left=Mathf.Clamp(left,0,1280);right=Mathf.Clamp(right,0,1280);
            if(right<=left||ground<=top)return;
            GUI.BeginGroup(new Rect(left,top,right-left,ground-top));
            try
            {
                float alpha=Mathf.Clamp01(remaining*5);
                for(float x=15;x<right-left+90;x+=110)
                    for(float y=55;y<ground-top+90;y+=125)
                        DrawRoyalSunPart(3,new Vector2(x,y),Vector2.one*180,alpha*.55f,visualClock*26+x*.2f);
            }
            finally {GUI.EndGroup();}
        }
    }
}
