using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        readonly Texture2D[] eclipseRooms=new Texture2D[4];
        static readonly string[] EclipseRoomResources={
            "World/Stage6MoonBridgeHDV2","World/Stage6SpiralGalleryHDV2",
            "World/Stage6BrokenBelfryHDV2","World/Stage6EclipseRotundaHDV2"
        };
        static readonly int[] NoctisWingCycle={2,0,1,3,1,0};
        Texture2D ravenEffects;
        float nextEclipseArtRetry;
        void LoadEclipseArt()
        {
            for(int i=0;i<eclipseRooms.Length;i++)
            {
                eclipseRooms[i]=Resources.Load<Texture2D>(EclipseRoomResources[i]);
                if(eclipseRooms[i]!=null){eclipseRooms[i].filterMode=FilterMode.Point;eclipseRooms[i].wrapMode=TextureWrapMode.Clamp;}
            }
            ravenEffects=Resources.Load<Texture2D>("Effects/Stage6RavenEclipseV2");
            if(ravenEffects!=null){ravenEffects.filterMode=FilterMode.Point;ravenEffects.wrapMode=TextureWrapMode.Clamp;}
            nextEclipseArtRetry=Time.realtimeSinceStartup+1;
        }
        void EnsureEclipseArt()
        {
            bool missing=ravenEffects==null;
            foreach(var room in eclipseRooms)missing|=room==null;
            if(missing&&Time.realtimeSinceStartup>=nextEclipseArtRetry)LoadEclipseArt();
        }
        void DrawEclipseBackdrop()
        {
            EnsureEclipseArt();var texture=eclipseRooms[Mathf.Clamp(run.RoomIndex,0,3)];
            Box(new Rect(0,0,1280,600),new Color(.016f,.035f,.05f));
            if(texture==null)return;
            const float width=2070;
            float offset=Mathf.Clamp01(cameraX/(RunModel.WorldWidth-32))*(width-1280);
            var color=GUI.color;GUI.color=Color.white;
            GUI.DrawTexture(new Rect(-offset,-30+cameraY*8,width,width*texture.height/texture.width),texture,ScaleMode.StretchToFill,false);
            GUI.color=color;
        }
        void DrawEclipseLandmarks()
        {
            EnsureEclipseArt();
            if(run.Stage==RunStage.BossArena)DrawEclipseAtmosphere();
            // Sparse drifting feathers stay behind actors and never obscure platforms.
            for(int i=0;i<12;i++)
            {
                float x=SX(Mathf.Repeat(i*6.7f+visualClock*.35f,68)-2);
                float y=SY(3+(i*17%70)*.1f)+Mathf.Sin(visualClock*.8f+i)*13;
                DrawRavenPart(0,new Vector2(x,y),new Vector2(17,17),.3f,25+Mathf.Sin(visualClock+i)*22);
            }
            if(run.Stage!=RunStage.BossArena)return;
            // Lock the eclipse to the high vault's parallax, so jumping does
            // not pull the moon into the HUD or away from its surrounding halo.
            float offset=Mathf.Clamp01(cameraX/(RunModel.WorldWidth-32))*(2070-1280);
            Vector2 center=new Vector2(1035-offset,186+cameraY*8);
            float dissolve=run.BossDefeated?Mathf.Clamp01((BossDefeatElapsed-.3f)/1.6f):0;
            DrawRavenPart(3,center,Vector2.one*470,(1-dissolve)*.25f,visualClock*4);
            DrawRavenPart(2,center,Vector2.one*310,1-dissolve);
            DrawRavenPart(3,center,Vector2.one*350,(1-dissolve)*.5f,-visualClock*6);
            if(run.BossDefeated)DrawLateSeam(center.x,center.y,HudLight);
        }
        void DrawEclipseAtmosphere()
        {
            float fade=run.BossDefeated?1-Mathf.Clamp01((BossDefeatElapsed-.3f)/1.6f):1;
            if(fade<=0)return;
            // Background-only atmosphere spans the viewport. Actors, ledges,
            // attack warnings and the green safe lane are rendered above it.
            for(int band=0;band<24;band++)
            {
                float t=band/23f;
                Box(new Rect(0,band*25,1280,25),new Color(.025f,.018f,.075f,fade*Mathf.Lerp(.26f,.025f,t)));
            }
            float offset=Mathf.Clamp01(cameraX/(RunModel.WorldWidth-32))*(2070-1280);
            for(int i=0;i<7;i++)
            {
                float x=i*340-offset;
                float y=180+Mathf.Sin(visualClock*.16f+i*1.8f)*45+cameraY*8;
                DrawRavenPart(3,new Vector2(x,y),new Vector2(510,370),fade*.1f,visualClock*3+i*53);
            }
        }
        void DrawRavenPart(int cell,Vector2 center,Vector2 size,float alpha=1,float angle=0)
        {
            EnsureEclipseArt();if(ravenEffects==null||alpha<=0)return;
            var color=GUI.color;
            using(ArtworkAt(center,angle))
            {
                try
                {
                    GUI.color=new Color(1,1,1,Mathf.Clamp01(alpha));
                    DrawArtwork(ravenEffects,new Rect(-size*.5f,size),new Rect(cell%2*.5f,1-(cell/2+1)*.5f,.5f,.5f));
                }
                finally {GUI.color=color;}
            }
        }
        int NoctisPose(Enemy boss)
        {
            if(run.BossIntroducing)return NoctisWingCycle[(int)(BossIntroElapsed*8)%NoctisWingCycle.Length];
            if(boss.PhaseTransition>0)return 7;
            if(boss.Windup>0||boss.AttackPose>0||boss.DashRemaining>0)
                return boss.Pattern==BossPattern.NightVeil?7:boss.Pattern==BossPattern.MoonVolley?6:boss.Windup>0?4:5;
            return NoctisWingCycle[(int)(boss.AnimationTime*7)%NoctisWingCycle.Length];
        }
        float NoctisArrivalOffset=>-135*(1-Mathf.SmoothStep(0,1,Mathf.Clamp01((BossIntroElapsed-.4f)/1.45f)));
        void DrawNoctisArrivalEffect()
        {
            float time=BossIntroElapsed;
            Vector2 center=new Vector2(SX(RunModel.BossSpawnX),SY(RunModel.BossSpawnY)-65+NoctisArrivalOffset);
            float fade=Mathf.Clamp01(time/.35f)*Mathf.Clamp01((2.35f-time)/.6f);
            DrawRavenPart(3,center,Vector2.one*(125+time*22),fade*.65f,-time*40);
            for(int i=0;i<20;i++)
            {
                float angle=i*2.399963f+time*.5f;
                float radius=30+time*(22+i%5*7);
                Vector2 pos=center+new Vector2(Mathf.Cos(angle)*radius,Mathf.Sin(angle)*radius*.55f+time*18);
                DrawRavenPart(0,pos,Vector2.one*(17+i%4*4),fade,angle*Mathf.Rad2Deg+time*35);
            }
        }
        void DrawNoctisFeatherOrb(Vector2 point,Vector2 velocity)
        {
            float angle=Mathf.Atan2(velocity.y,velocity.x)*Mathf.Rad2Deg;
            for(int i=2;i>0;i--)
                DrawRavenPart(0,point-velocity.normalized*i*8,Vector2.one*35,.12f/i,angle);
            DrawRavenPart(0,point,Vector2.one*38,1,angle);
        }
        void DrawNoctisDive(Enemy boss,Vector2 center,float remaining)
        {
            // Keep the arc direction stable when the dash reaches its target.
            float angle=boss.Facing>0?25:155;
            float progress=boss.DashRemaining>0?1-Mathf.Clamp01(boss.DashRemaining/Mathf.Max(.01f,boss.DashDuration)):1-Mathf.Clamp01(remaining);
            DrawRavenPart(1,center,new Vector2(118,126),.85f,angle-40+progress*90);
            for(int i=0;i<5;i++)
                DrawRavenPart(0,center+new Vector2(-boss.Facing*(25+i*12),Mathf.Sin(i+visualClock*6)*17),
                    Vector2.one*(26-i*2),.6f-i*.08f,angle);
        }
        void DrawNoctisCurtain(float left,float right,float top,float ground,float remaining)
        {
            left=Mathf.Clamp(left,0,1280);right=Mathf.Clamp(right,0,1280);
            if(right<=left||ground<=top)return;
            // Every wing/wisp is clipped to the damage band, including its glow.
            GUI.BeginGroup(new Rect(left,top,right-left,ground-top));
            try
            {
                float width=right-left,height=ground-top,alpha=Mathf.Clamp01(remaining*5);
                for(float x=0;x<width+90;x+=100)
                    for(float y=35;y<height+75;y+=110)
                        DrawRavenPart(3,new Vector2(x,y),Vector2.one*155,alpha*.46f,visualClock*48+x*.2f-y*.1f);
                for(int i=0;i<24;i++)
                {
                    float x=Mathf.Repeat(i*57+visualClock*(i%2==0?24:-24),Mathf.Max(1,width));
                    float y=Mathf.Repeat(i*41+visualClock*75,height);
                    DrawRavenPart(0,new Vector2(x,y),Vector2.one*(28+i%4*5),alpha*.85f,70+Mathf.Sin(visualClock*2+i)*30);
                }
            }
            finally {GUI.EndGroup();}
        }
    }
}
