using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        Texture2D forestRuinProps;
        float nextForestPropsRetry;
        // Measured regions, not an assumed equal grid: the generated tall props
        // extend below the nominal first row. Anchors are the visible stone feet.
        static readonly HeroFrame[] ForestPropFrames={
            new HeroFrame(50,0,485,640,294,620),
            new HeroFrame(580,0,405,650,775,631),
            new HeroFrame(1025,130,500,505,1274,620),
            new HeroFrame(20,715,555,265,300,966),
            new HeroFrame(590,685,480,300,830,972),
            new HeroFrame(1100,730,410,265,1305,979)
        };
        static readonly float[] ForestPropHeights={607,615,475,238,281,240};
        void LoadForestProps()
        {
            forestRuinProps=Resources.Load<Texture2D>("World/ForestRuinPropsV1");
            if(forestRuinProps!=null)
            {
                forestRuinProps.filterMode=FilterMode.Point;
                forestRuinProps.wrapMode=TextureWrapMode.Clamp;
            }
            nextForestPropsRetry=Time.realtimeSinceStartup+1;
        }
        void DrawForestProp(int index,float x,float groundY,float height,Color? tint=null)
        {
            if(forestRuinProps==null&&Time.realtimeSinceStartup>=nextForestPropsRetry)LoadForestProps();
            if(forestRuinProps==null)return; // Never restore the old rectangle placeholders.
            var frame=ForestPropFrames[index];float scale=height/ForestPropHeights[index];
            float left=x-frame.Feet.x*scale,top=groundY-frame.Feet.y*scale;
            if(left>1280||left+frame.Source.width*scale<0||top>580||top+frame.Source.height*scale<0)return;
            var previous=GUI.color;GUI.color=tint??Color.white;
            DrawHeroRegion(forestRuinProps,new Vector2(forestRuinProps.width,forestRuinProps.height),
                frame.Source,new Vector2(left,top),scale);
            GUI.color=previous;
        }
    }
}
