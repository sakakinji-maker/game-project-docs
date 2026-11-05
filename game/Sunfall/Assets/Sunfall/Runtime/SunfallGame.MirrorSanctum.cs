using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        const float MirrorBreakTime=1.15f;
        readonly Texture2D[] mirrorRooms=new Texture2D[4];
        static readonly string[] MirrorRoomResources={
            "World/Stage5MirrorGardenHDV2","World/Stage5MemoryHallHDV2",
            "World/Stage5InvertedChapelHDV2","World/Stage5MirrorSanctumHDV2"
        };
        Texture2D mirrorMaskParts;
        float nextMirrorArtRetry;
        // Face centers in the authored body atlas. These follow the actual head,
        // including the leaning teleport poses; neither the actor root nor scale moves.
        static readonly Vector2[] LysseaFaceCenters={
            new Vector2(247,88),new Vector2(695,88),new Vector2(1140,90),new Vector2(1595,92),
            new Vector2(282,582),new Vector2(727,573),new Vector2(1125,528),new Vector2(1563,532)
        };
        static readonly float[] LysseaFaceAngles={0,0,0,0,8,10,0,0};
        void LoadMirrorArt()
        {
            for(int i=0;i<mirrorRooms.Length;i++)
            {
                mirrorRooms[i]=Resources.Load<Texture2D>(MirrorRoomResources[i]);
                if(mirrorRooms[i]!=null){mirrorRooms[i].filterMode=FilterMode.Point;mirrorRooms[i].wrapMode=TextureWrapMode.Clamp;}
            }
            mirrorMaskParts=Resources.Load<Texture2D>("World/Stage5MasksMirrorV2");
            if(mirrorMaskParts!=null){mirrorMaskParts.filterMode=FilterMode.Point;mirrorMaskParts.wrapMode=TextureWrapMode.Clamp;}
            nextMirrorArtRetry=Time.realtimeSinceStartup+1;
        }
        void EnsureMirrorArt()
        {
            bool missing=mirrorMaskParts==null;
            foreach(var room in mirrorRooms)missing|=room==null;
            if(missing&&Time.realtimeSinceStartup>=nextMirrorArtRetry)LoadMirrorArt();
        }
        void DrawMirrorBackdrop()
        {
            EnsureMirrorArt();var texture=mirrorRooms[Mathf.Clamp(run.RoomIndex,0,3)];
            Box(new Rect(0,0,1280,600),new Color(.035f,.018f,.027f));
            if(texture==null)return;
            const float width=2070;
            float offset=Mathf.Clamp01(cameraX/(RunModel.WorldWidth-32))*(width-1280);
            var color=GUI.color;GUI.color=Color.white;
            GUI.DrawTexture(new Rect(-offset,-30+cameraY*8,width,width*texture.height/texture.width),texture,ScaleMode.StretchToFill,false);
            GUI.color=color;
        }
        void DrawMirrorLandmarks()
        {
            EnsureMirrorArt();
            if(run.Stage!=RunStage.BossArena||mirrorMaskParts==null)return;
            float x=SX(RunModel.BossSpawnX),floor=SY(0)+2;
            float time=run.BossIntroducing?BossIntroElapsed:RunModel.BossIntroDuration;
            if(time<MirrorBreakTime)
            {
                DrawStandingMirror(0,x,floor,1);
                DrawStandingMirror(1,x,floor,Mathf.Clamp01((time-.5f)/.55f));
                // An imprisoned face becomes visible immediately before impact.
                DrawLysseaMaskPart(0,new Vector2(x,floor-169),44,
                    new Color(.8f,.72f,.9f,Mathf.Clamp01((time-.45f)/.6f)*.45f));
            }
            else DrawStandingMirror(2,x,floor,1);
            if(run.BossDefeated)DrawMirrorRestoration(x,floor);
        }
        void DrawMirrorRestoration(float x,float floor)
        {
            float elapsed=BossDefeatElapsed-.6f;
            float progress=Mathf.Clamp01(elapsed/1.4f),settle=Mathf.Clamp01((elapsed-1.4f)/.7f);
            if(progress<=0)return;
            var gold=new Color(1,.82f,.46f);
            var top=new Vector2(x,floor-270);var bottom=new Vector2(x,floor-53);
            // Restore the authored reflection instead of translucent scanline
            // polygons, whose row boundaries formed stripes at scaled resolutions.
            DrawStandingMirror(0,x,floor,Mathf.SmoothStep(0,1,progress)*.88f);
            var main=new SewnScarPath(top+new Vector2(-14,25),bottom+new Vector2(14,-8),-10);
            DrawSewnScar(main,progress,18,gold,settle,1.25f);
            for(int side=-1;side<=1;side+=2)
            {
                var join=main.Point(side<0?.33f:.68f);
                var edge=new Vector2(x+side*46,floor-(side<0?208:104));
                DrawSewnScar(new SewnScarPath(join,edge,side*3),
                    Mathf.Clamp01((progress-.3f)/.7f),4,gold,settle,.9f);
            }
            DrawScarAfterglow(main,elapsed-1.4f,gold);
        }
        void DrawStandingMirror(int state,float x,float floor,float alpha)
        {
            if(alpha<=0)return;
            // Align each painted frame by its center and foot baseline, not by
            // the slightly different transparent margins of the three cells.
            float center=state==0?263:state==1?259:251;
            var source=new Rect(state*512+center-150,522,300,490);
            var color=GUI.color;GUI.color=new Color(1,1,1,alpha);
            const float scale=.64f;
            DrawHeroRegion(mirrorMaskParts,new Vector2(1536,1024),source,
                new Vector2(x-150*scale,floor-490*scale),scale);
            GUI.color=color;
        }
        static int LysseaMaskIndex(BossPattern pattern)
        {
            return pattern==BossPattern.MirrorFan?1:pattern==BossPattern.MirrorPrison?2:0;
        }
        void DrawLysseaMask(Enemy boss,int pose,HeroFrame frame,float scale,float x,float y)
        {
            EnsureMirrorArt();
            Vector2 foot=frame.Source.position+frame.Feet;
            Vector2 head=new Vector2(x,y)+(LysseaFaceCenters[pose]-foot)*scale;
            DrawLysseaMaskPart(LysseaMaskIndex(boss.Pattern),head,49*scale,GUI.color,LysseaFaceAngles[pose]);
        }
        void DrawLysseaMaskPart(int mask,Vector2 center,float height,Color tint,float angle=0)
        {
            if(mirrorMaskParts==null||tint.a<=0)return;
            float cx=mask==0?262:mask==1?256:250;
            var source=new Rect(mask*512+cx-180,24,360,466);
            var size=new Vector2(source.width*height/source.height,height);
            var color=GUI.color;
            using(ArtworkAt(center,angle))
            {
                try
                {
                    GUI.color=tint;
                    DrawArtwork(mirrorMaskParts,new Rect(-size*.5f,size),
                        new Rect(source.x/1536,1-source.yMax/1024,source.width/1536,source.height/1024));
                }
                finally {GUI.color=color;}
            }
        }
        void DrawLysseaArrivalEffect()
        {
            float time=BossIntroElapsed-MirrorBreakTime;
            if(time<0||time>1.3f)return;
            Vector2 origin=new Vector2(SX(RunModel.BossSpawnX),SY(0)-163);
            float fade=Mathf.Clamp01(1-time/1.3f);
            // Deterministic outward trajectories: no random redraw flicker or camera shake.
            for(int i=0;i<22;i++)
            {
                float angle=i*2.399963f;
                float speed=75+(i*43)%150;
                Vector2 direction=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
                Vector2 pos=origin+direction*(18+speed*time)+new Vector2(0,90*time*time);
                float size=12+i%5*4;
                DrawSpellEffect(SpellKind.MirrorShards,pos,new Vector2(size,size*1.35f),fade,
                    i*37+time*(i%2==0?180:-150));
            }
            if(time<.35f)DrawSpellEffect(SpellKind.MirrorWard,origin,Vector2.one*(50+time*310),
                Mathf.Clamp01(1-time/.35f)*.75f,time*55);
        }
    }
}
