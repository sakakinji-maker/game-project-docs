using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        Texture2D storySkyPortrait;
        float nextStoryArtRetry;
        void LoadStoryArt()
        {
            storySkyPortrait=Resources.Load<Texture2D>("Story/StitchedSkyV1");
            nextStoryArtRetry=Time.realtimeSinceStartup+2;
        }
        void DrawStorySkyPortrait(Rect rect,string caption,Color accent)
        {
            if(storySkyPortrait==null&&Time.realtimeSinceStartup>=nextStoryArtRetry)LoadStoryArt();
            GothicPlaque(rect,12,HudGold);
            GothicPlaque(new Rect(rect.x+2,rect.y+2,rect.width-4,rect.height-4),10,HudInk);
            var picture=new Rect(rect.x+6,rect.y+6,rect.width-12,rect.height-12);
            if(storySkyPortrait!=null)
            {
                var previous=GUI.color;GUI.color=Color.white;
                // Preserve the complete rift, stitches and skyline without stretching or cropping.
                GUI.DrawTexture(picture,storySkyPortrait,ScaleMode.ScaleToFit,false);
                GUI.color=previous;
            }
            // A separate dark caption footer leaves the golden stitches unobscured.
            for(int i=0;i<8;i++)Box(new Rect(picture.x,rect.yMax-67+i*3,picture.width,3),new Color(.02f,.025f,.04f,i/8f*.9f));
            Box(new Rect(picture.x,rect.yMax-43,picture.width,37),new Color(.02f,.025f,.04f,.95f));
            GothicLine(new Vector2(rect.x+23,rect.yMax-43),new Vector2(rect.xMax-23,rect.yMax-43),new Color(.53f,.42f,.25f,.8f),1);
            Text(caption,rect.x+10,rect.yMax-39,rect.width-20,31,15,accent,TextAnchor.MiddleCenter);
            GothicDiamond(rect.center.x,rect.y+3,3,accent);
        }
    }
}
