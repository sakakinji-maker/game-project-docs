using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        readonly Texture2D[] cryptRooms=new Texture2D[4];
        static readonly string[] CryptRoomResources={
            "World/Stage2GateHDV3","World/Stage2CloisterHDV3",
            "World/Stage2SealHallHDV3","World/Stage2BossCryptHDV3"
        };
        Texture2D cryptSeal,cryptStone;
        float nextCryptRetry;
        const float MorgaChainBreakTime=1.25f;
        // Actual transparent gutter is x742..750, slightly left of the atlas midpoint.
        static readonly HeroFrame[] CryptSealFrames={
            new HeroFrame(0,0,748,1024,384,978),
            new HeroFrame(748,0,788,1024,1152,978)
        };
        void LoadCryptArt()
        {
            for(int i=0;i<cryptRooms.Length;i++)
            {
                cryptRooms[i]=Resources.Load<Texture2D>(CryptRoomResources[i]);
                if(cryptRooms[i]!=null){cryptRooms[i].filterMode=FilterMode.Point;cryptRooms[i].wrapMode=TextureWrapMode.Clamp;}
            }
            cryptSeal=Resources.Load<Texture2D>("World/Stage2PixelSealV2");
            cryptStone=Resources.Load<Texture2D>("World/RuinPlatform");
            foreach(var texture in new[]{cryptSeal,cryptStone})
                if(texture!=null){texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;}
            nextCryptRetry=Time.realtimeSinceStartup+1;
        }
        void EnsureCryptArt()
        {
            bool missing=cryptSeal==null||cryptStone==null;
            foreach(var room in cryptRooms)missing|=room==null;
            if(missing&&Time.realtimeSinceStartup>=nextCryptRetry)LoadCryptArt();
        }
        void DrawCryptBackdrop()
        {
            EnsureCryptArt();
            Box(new Rect(0,0,1280,580),new Color(.018f,.03f,.065f));
            var room=cryptRooms[Mathf.Clamp(run.RoomIndex,0,3)];
            if(room==null)return;
            // Full 2172x724 source per room, matching stage one's pixel density.
            // The former 1086x362 atlas strip was enlarged to almost twice its size.
            float width=2070,height=width*room.height/room.width;
            float offset=Mathf.Clamp01(cameraX/(RunModel.WorldWidth-32))*Mathf.Max(0,width-1280);
            var color=GUI.color;GUI.color=Color.white;
            GUI.DrawTexture(new Rect(-offset,-105+cameraY*8,width,height),room,ScaleMode.StretchToFill,false);
            GUI.color=color;
        }
        void DrawCryptStone(float x,float top,float width,bool ground)
        {
            EnsureCryptArt();
            // Share stage one's pixel masonry and its grass/collision alignment.
            // Repeat at native aspect ratio instead of stretching stone blocks.
            float height=ground?83:61,tileWidth=height*2140f/306f;
            if(cryptStone==null)return;
            var color=GUI.color;GUI.color=new Color(.78f,.88f,1);
            for(float offset=0;offset<width;offset+=tileWidth)
            {
                float piece=Mathf.Min(tileWidth,width-offset);
                var uv=new Rect(16f/2172,1-556f/724,2140f/2172*piece/tileWidth,306f/724);
                GUI.DrawTextureWithTexCoords(new Rect(x+offset,top-10,piece,height),cryptStone,uv,true);
            }
            GUI.color=color;
        }
        void DrawCryptSeal()
        {
            if(run.Stage!=RunStage.BossArena)return;
            EnsureCryptArt();
            float x=SX(RunModel.BossSpawnX),y=SY(0);
            if(cryptSeal!=null)
            {
                float opened=run.BossIntroducing?Mathf.SmoothStep(0,1,Mathf.Clamp01((BossIntroElapsed-MorgaChainBreakTime)/.55f)):1;
                var color=GUI.color;
                if(opened<1){GUI.color=new Color(1,1,1,1-opened);DrawCryptSealState(0,x,y);}
                GUI.color=new Color(.72f,.82f,1,opened*(run.BossIntroducing?1:.55f));
                DrawCryptSealState(1,x,y);GUI.color=color;
            }
            // The opened shell remains as scenery after the guardian emerges.
            if(run.BossDefeated)
            {
                // Repair the opened shell itself, not empty air above it.
                DrawRestorationScar(new Vector2(x-28,y-194),new Vector2(x+24,y-69),-16,12,1.25f);
            }
        }
        void DrawCryptSealState(int state,float x,float groundY)
        {
            var frame=CryptSealFrames[state];const float scale=280f/1024;
            DrawHeroRegion(cryptSeal,new Vector2(1536,1024),frame.Source,
                new Vector2(x-frame.Feet.x*scale,groundY-frame.Feet.y*scale),scale);
        }
        void DrawMorgaArrivalEffect()
        {
            float elapsed=BossIntroElapsed-MorgaChainBreakTime;
            if(elapsed<0||elapsed>1.1f)return;
            float x=SX(RunModel.BossSpawnX),y=SY(0)-125;
            float progress=elapsed/1.1f,alpha=1-progress;
            for(int i=0;i<18;i++)
            {
                float angle=i*Mathf.PI*2/18;
                var direction=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle)*.7f);
                var point=new Vector2(x,y)+direction*(20+progress*115)+Vector2.up*(progress*progress*55);
                GothicLine(point,point-direction*(5+alpha*10),new Color(.83f,.89f,.65f,alpha*.65f),1);
            }
        }
        void DrawMorgaRestraints()
        {
            if(!run.BossIntroducing)return;
            EnsurePresentationArt();if(playerSpellSheet==null)return;
            float time=BossIntroElapsed;
            float x=SX(RunModel.BossSpawnX),y=SY(0);
            for(int i=0;i<4;i++)
            {
                int side=i%2==0?-1:1;
                var anchor=new Vector2(x+side*(i<2?225:180),y-(i<2?285:45));
                var target=new Vector2(x-side*35,y-(i<2?105:155));
                if(time<MorgaChainBreakTime)
                {
                    float tension=Mathf.Clamp01(time/MorgaChainBreakTime);
                    DrawAnimatedChain(anchor,target,1,Mathf.Lerp(9,.5f,tension),i*.3f,false);
                }
                else
                {
                    float t=Mathf.Clamp01((time-MorgaChainBreakTime)/1.05f);
                    var split=Vector2.Lerp(anchor,target,.66f);
                    var recoil=Vector2.Lerp(split,anchor,t*.55f)+new Vector2(side*t*35,t*t*85);
                    DrawAnimatedChain(anchor,recoil,1-t*.7f,(1-t)*7,i*.3f,false);
                    for(int link=0;link<5;link++)
                    {
                        var point=Vector2.Lerp(split,target,link/4f);
                        point+=new Vector2(side*t*(55+link*17),-t*50+t*t*(160+link*9));
                        DrawChainPiece(new Rect(1000,181,34,24),point,new Vector2(18,12),side*(time*170+link*37),1-t);
                    }
                }
            }
        }
    }
}

