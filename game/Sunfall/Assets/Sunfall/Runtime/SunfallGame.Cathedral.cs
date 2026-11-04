using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        readonly Texture2D[] cathedralRooms=new Texture2D[4];
        static readonly string[] CathedralRoomResources={
            "World/Stage3ApproachHDV2","World/Stage3CloisterHDV2",
            "World/Stage3ChapelHDV2","World/Stage3SanctumHDV2"
        };
        Texture2D cathedralStone;
        float nextCathedralRetry;
        const float CathedralBackdropWidth=2070,CathedralBackdropTop=-30;
        float CathedralBackdropOffset=>Mathf.Clamp01(cameraX/(Sunfall.Core.RunModel.WorldWidth-32))*(CathedralBackdropWidth-1280);
        void LoadCathedralArt()
        {
            for(int i=0;i<cathedralRooms.Length;i++)
            {
                cathedralRooms[i]=Resources.Load<Texture2D>(CathedralRoomResources[i]);
                if(cathedralRooms[i]!=null){cathedralRooms[i].filterMode=FilterMode.Point;cathedralRooms[i].wrapMode=TextureWrapMode.Clamp;}
            }
            cathedralStone=Resources.Load<Texture2D>("World/Stage3CathedralStoneV2");
            if(cathedralStone!=null){cathedralStone.filterMode=FilterMode.Point;cathedralStone.wrapMode=TextureWrapMode.Clamp;}
            nextCathedralRetry=Time.realtimeSinceStartup+1;
        }
        void EnsureCathedralArt()
        {
            bool missing=cathedralStone==null;
            foreach(var room in cathedralRooms)missing|=room==null;
            if(missing&&Time.realtimeSinceStartup>=nextCathedralRetry)LoadCathedralArt();
        }
        void DrawCathedralBackdrop()
        {
            EnsureCathedralArt();
            Box(new Rect(0,0,1280,600),new Color(.018f,.026f,.045f));
            var room=cathedralRooms[Mathf.Clamp(run.RoomIndex,0,3)];
            if(room==null)return;
            var color=GUI.color;GUI.color=Color.white;
            // Each room owns a full 2172x724 panorama. Match stage one's scale,
            // retain the source palette and leave the nave above the collision floor.
            GUI.DrawTexture(new Rect(-CathedralBackdropOffset,CathedralBackdropTop+cameraY*8,
                CathedralBackdropWidth,CathedralBackdropWidth*room.height/room.width),room,ScaleMode.StretchToFill,false);
            GUI.color=color;
        }
        void DrawCathedralLandmarks()
        {
            // Architecture now belongs to the painted panorama, with no opaque
            // procedural columns or windows obscuring its detailed masonry.
            if(run.RoomIndex==3)DrawSolarSeal();
        }
        void DrawRoyalCathedralStone(float x,float top,float width,bool ground)
        {
            EnsureCathedralArt();if(cathedralStone==null)return;
            // Measured source crop; the walkable lip is y292 in the original.
            // Preserve stone proportions for narrow platforms and partial tiles.
            const float sourceX=24,sourceY=282,sourceWidth=2122,sourceHeight=260;
            float height=ground?83:61,scale=height/sourceHeight,tileWidth=sourceWidth*scale;
            var color=GUI.color;GUI.color=Color.white;
            for(float offset=0;offset<width;offset+=tileWidth)
            {
                float piece=Mathf.Min(tileWidth,width-offset);
                var uv=new Rect(sourceX/2172,1-(sourceY+sourceHeight)/724,
                    sourceWidth/2172*piece/tileWidth,sourceHeight/724);
                GUI.DrawTextureWithTexCoords(new Rect(x+offset,top-10*scale,piece,height),cathedralStone,uv,true);
            }
            GUI.color=color;
        }
        void DrawCathedralStone(float x,float top,float width,bool ground)
        {
            float height=ground?70:31;
            Box(new Rect(x,top,width,height),new Color(.17f,.17f,.21f));
            Box(new Rect(x,top+6,width,4),new Color(.32f,.29f,.29f));
            for(int row=0;row<3;row++)
            {
                float y=top+12+row*20;if(y>=top+height)break;
                Box(new Rect(x,y,width,2),new Color(.085f,.085f,.12f));
                for(float joint=x+(row%2)*25;joint<x+width;joint+=50)
                    Box(new Rect(joint,y,2,Mathf.Min(18,top+height-y)),new Color(.09f,.09f,.12f));
            }
        }
    }
}
