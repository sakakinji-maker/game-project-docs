using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        Texture2D gothicPortals,gothicTorches;
        float nextWorldPropsRetry;
        // Measured roots use the stone plinth / iron feet, not the surrounding glow.
        // Both states use one scale so opening a portal never changes its size.
        static readonly HeroFrame[] PortalPropFrames={
            new HeroFrame(100,0,640,1010,439,991),
            new HeroFrame(780,0,640,1010,1096,991)
        };
        static readonly HeroFrame[] TorchPropFrames={
            new HeroFrame(160,0,608,1024,513,978),
            new HeroFrame(768,0,608,1024,1047,978)
        };
        void LoadWorldProps()
        {
            gothicPortals=Resources.Load<Texture2D>("World/GothicPortalsV1");
            gothicTorches=Resources.Load<Texture2D>("World/GothicTorchesV1");
            foreach(var texture in new[]{gothicPortals,gothicTorches})
                if(texture!=null){texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;}
            nextWorldPropsRetry=Time.realtimeSinceStartup+1;
        }
        void EnsureWorldProps()
        {
            if((gothicPortals==null||gothicTorches==null)&&Time.realtimeSinceStartup>=nextWorldPropsRetry)LoadWorldProps();
        }
        void DrawGothicPortal(float x,float groundY,bool open,float height=170)
        {
            EnsureWorldProps();
            if(gothicPortals==null)
            {
                Text(open?"↑ 포탈":"봉인된 포탈",x-70,groundY-80,140,40,18,HudLight,TextAnchor.MiddleCenter);
                return;
            }
            DrawWorldProp(gothicPortals,PortalPropFrames[open?1:0],x,groundY,height/991f);
        }
        void DrawGothicTorch(float x,float groundY,bool crimson,float height=82)
        {
            if(x<-65||x>1345)return;
            EnsureWorldProps();
            if(gothicTorches==null)return;
            DrawWorldProp(gothicTorches,TorchPropFrames[crimson?1:0],x,groundY,height/978f);
        }
        void DrawWorldProp(Texture2D texture,HeroFrame frame,float x,float y,float scale)
        {
            var oldColor=GUI.color;GUI.color=Color.white;
            DrawHeroRegion(texture,new Vector2(texture.width,texture.height),frame.Source,
                new Vector2(x-frame.Feet.x*scale,y-frame.Feet.y*scale),scale);
            GUI.color=oldColor;
        }
    }
}
