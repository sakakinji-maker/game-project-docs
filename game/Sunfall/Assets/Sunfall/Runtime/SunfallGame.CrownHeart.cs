using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        readonly Texture2D[] crownRooms=new Texture2D[4];
        static readonly string[] CrownRoomResources={
            "World/Stage4CausewayHDV2","World/Stage4CloisterHDV2",
            "World/Stage4SealGalleryHDV2","World/Stage4RoyalArenaHDV2"
        };
        Texture2D crownHeartParts;
        float nextCrownArtRetry;
        Vector2 CrownBackdropPoint(Vector2 source)
        {
            const float scale=2070f/2172;
            float offset=Mathf.Clamp01(cameraX/(RunModel.WorldWidth-32))*(2070-1280);
            return source*scale+new Vector2(-offset,-30+cameraY*8);
        }
        void LoadCrownArt()
        {
            for(int i=0;i<crownRooms.Length;i++)
            {
                crownRooms[i]=Resources.Load<Texture2D>(CrownRoomResources[i]);
                if(crownRooms[i]!=null){crownRooms[i].filterMode=FilterMode.Point;crownRooms[i].wrapMode=TextureWrapMode.Clamp;}
            }
            crownHeartParts=Resources.Load<Texture2D>("World/Stage4HeartMechanismV2");
            if(crownHeartParts!=null){crownHeartParts.filterMode=FilterMode.Point;crownHeartParts.wrapMode=TextureWrapMode.Clamp;}
            nextCrownArtRetry=Time.realtimeSinceStartup+1;
        }
        void EnsureCrownArt()
        {
            bool missing=crownHeartParts==null;
            foreach(var room in crownRooms)missing|=room==null;
            if(missing&&Time.realtimeSinceStartup>=nextCrownArtRetry)LoadCrownArt();
        }
        void DrawCrownBackdrop()
        {
            EnsureCrownArt();var texture=crownRooms[Mathf.Clamp(run.RoomIndex,0,3)];
            Box(new Rect(0,0,1280,600),new Color(.018f,.025f,.045f));
            if(texture==null)return;
            const float width=2070;
            float offset=Mathf.Clamp01(cameraX/(RunModel.WorldWidth-32))*(width-1280);
            var color=GUI.color;GUI.color=Color.white;
            GUI.DrawTexture(new Rect(-offset,-30+cameraY*8,width,width*texture.height/texture.width),texture,ScaleMode.StretchToFill,false);
            GUI.color=color;
        }
        void DrawCrownPart(int cell,Vector2 center,float size,Color tint,float angle=0)
        {
            EnsureCrownArt();if(crownHeartParts==null||tint.a<=0)return;
            var color=GUI.color;
            using(ArtworkAt(center,angle))
            {
                try
                {
                    GUI.color=tint;
                    DrawArtwork(crownHeartParts,new Rect(-(Vector2.one*size)*.5f,(Vector2.one*size)),new Rect(cell%2*.5f,1-(cell/2+1)*.5f,.5f,.5f));
                }
                finally {GUI.color=color;}
            }
        }
        void DrawCrownHeartLandmarks()
        {
            // The causeway and flower-rib vaults are full-resolution artwork.
            // Only the moving royal device is drawn separately.
            if(run.RoomIndex<2)return;
            // Device and suspension share the panorama's parallax. The upper
            // anchors are the painted stone capitals, not offsets in open air.
            var center=CrownBackdropPoint(new Vector2(1086,run.RoomIndex==3?348:300));
            float x=center.x,y=center.y,size=run.RoomIndex==3?185:135;
            float breaking=run.BossDefeated?Mathf.Clamp01((BossDefeatElapsed-.25f)/1.35f):0;
            float beat=run.BossDefeated?0:Mathf.Pow(Mathf.Max(0,Mathf.Sin(visualClock*3.8f)),5);
            for(int side=-1;side<=1;side+=2)
            {
                var anchor=CrownBackdropPoint(new Vector2(side<0?850:1320,run.RoomIndex==3?76:146));
                DrawBossChain(anchor,new Vector2(x+side*size*.32f,y-size*.26f+breaking*30),
                    1-breaking,.6f,side*.4f,false);
            }
            DrawCrownPart(2,new Vector2(x,y),size*.92f,new Color(.7f,.72f,.8f,1-breaking),
                run.BossDefeated?0:visualClock*7);
            DrawCrownPart(0,new Vector2(x,y),size,new Color(1,1,1,1-breaking));
            DrawCrownPart(3,new Vector2(x,y),size,new Color(.8f,.78f,.76f,breaking));
            DrawCrownPart(1,new Vector2(x,y+2),size*(.66f+beat*.035f),new Color(1,1-beat*.12f,1-beat*.16f,1-breaking));
            if(breaking>0&&breaking<1)
                DrawBossImpact(new Vector2(x,y),size*.85f,1-breaking,new Color(1,.24f,.32f));
            if(run.BossDefeated)DrawLateSeam(x,y,HudLight);
        }
        void DrawCrownStone(float x,float top,float width,bool ground)
        {
            // Shared fine-cut royal masonry matches the adjacent cathedral.
            DrawRoyalCathedralStone(x,top,width,ground);
        }
    }
}

