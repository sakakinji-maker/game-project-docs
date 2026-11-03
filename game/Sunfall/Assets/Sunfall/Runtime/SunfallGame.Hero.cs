using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        [SerializeField] Texture2D heroAtlas,heroPortrait;
        const float HeroAtlasWidth=1024,HeroAtlasHeight=1536;
        float nextHeroArtRetry;
        bool heroArtWarning;
        float heroAnimationTime,heroHurtPose;
        bool heroWalking;

        // Non-combat poses from the original sheet. Sword animations have separate
        // padded sheets and pivots in SunfallGame.SwordArt.cs. Top-left coordinates.
        struct HeroFrame
        {
            public Rect Source;
            public Vector2 Feet;
            public HeroFrame(float x,float y,float width,float height,float footX,float footY)
            {Source=new Rect(x,y,width,height);Feet=new Vector2(footX-x,footY-y);}
        }
        static readonly HeroFrame[] HeroFrames={
            new HeroFrame(90,45,158,225,148,264),
            new HeroFrame(335,45,160,225,395,264),
            new HeroFrame(580,45,165,225,641,264),
            new HeroFrame(830,45,170,225,891,264),
            new HeroFrame(20,305,220,207,140,503),
            new HeroFrame(252,305,214,207,382,503),
            new HeroFrame(510,305,220,207,638,503),
            new HeroFrame(765,305,220,207,895,503),
            new HeroFrame(285,1370,222,148,401,1509),
            new HeroFrame(510,1295,227,228,618,1513),
            new HeroFrame(820,1315,185,208,909,1513)
        };

        void LoadHeroArt()
        {
            heroAtlas=Resources.Load<Texture2D>("Hero/HeroAtlas");
            heroPortrait=Resources.Load<Texture2D>("Hero/HeroPortrait");
            if(heroAtlas!=null){heroAtlas.filterMode=FilterMode.Point;heroAtlas.wrapMode=TextureWrapMode.Clamp;}
            LoadSwordArt();
            nextHeroArtRetry=Time.realtimeSinceStartup+1;
            if(heroAtlas!=null&&SwordArtReady)heroArtWarning=false;
        }
        void EnsureHeroArt()
        {
            if((heroAtlas==null||heroPortrait==null||!SwordArtReady)&&Time.realtimeSinceStartup>=nextHeroArtRetry)LoadHeroArt();
            if((heroAtlas==null||!SwordArtReady)&&!heroArtWarning)
            {
                heroArtWarning=true;
                Debug.LogWarning("Sunfall: Hero textures are not all available. Retrying resource loading and using the available player pose.",this);
            }
        }
        void ResetHeroVisuals(){heroAnimationTime=heroHurtPose=0;heroWalking=false;ResetMotion();}
        void TickHeroVisuals(float deltaX,float dt,bool hurt=false)
        {
            heroAnimationTime+=dt;heroWalking=Mathf.Abs(deltaX)>.001f;
            heroHurtPose=hurt?.15f:Mathf.Max(0,heroHurtPose-dt);
        }
        void DrawHero(float x,float feetY,int facing,bool inVillage)
        {
            EnsureHeroArt();
            int frame=heroWalking?4+(int)(heroAnimationTime*10)%4:(int)(heroAnimationTime*4)%4;
            bool rolling=!inVillage&&run.Rolling;
            if(!inVillage)
            {
                if(!run.Grounded)frame=5;
                if(run.Casting)frame=9;
                if(heroHurtPose>0&&!run.SwordActive)frame=10;
                if(rolling)frame=8;
                if(run.SwordActive||run.AirSwordRecovery||rolling||run.Casting||run.MagicRecoveryTimer>0)facing=run.ActionFacing;
            }
            if(heroAtlas==null)
            {
                // Never make the player invisible while an import is incomplete.
                Box(new Rect(x-13,feetY-42,26,42),new Color(.85f,.87f,.91f));
                Box(new Rect(x-9,feetY-55,18,18),new Color(.55f,.24f,.3f));
                return;
            }
            var item=HeroFrames[frame];
            float scale=.34f;
            Texture2D texture=heroAtlas;
            var sourceSize=new Vector2(HeroAtlasWidth,HeroAtlasHeight);
            if(!inVillage&&SelectSwordArt(out var swordTexture,out var swordFrame,out var swordSize,out var swordScale))
            {texture=swordTexture;item=swordFrame;sourceSize=swordSize;scale=swordScale;}
            var oldMatrix=GUI.matrix;var oldColor=GUI.color;
            // Compose in the logical 1280x720 canvas, before its screen scaling.
            // GUIUtility pivot helpers can mix screen and canvas coordinates.
            var mirror=Matrix4x4.Translate(new Vector3(x,feetY,0))*Matrix4x4.Scale(new Vector3(facing<0?-1:1,1,1))*Matrix4x4.Translate(new Vector3(-x,-feetY,0));
            var rotation=Matrix4x4.identity;
            if(rolling)
            {
                var pivot=new Vector3(x,feetY-22,0);
                rotation=Matrix4x4.Translate(pivot)*Matrix4x4.Rotate(Quaternion.Euler(0,0,(1-run.RollTimer/Core.RunModel.RollDuration)*360))*Matrix4x4.Translate(-pivot);
            }
            GUI.matrix=oldMatrix*mirror*rotation;GUI.color=Color.white;
            if(!inVillage&&run.RollInvulnerability>0)GUI.color=new Color(.65f,1,1,1);
            else if(heroHurtPose>0)GUI.color=new Color(1,.55f,.55f,1);
            DrawCharacterPose(texture,item,sourceSize,scale,x,feetY);
            GUI.color=oldColor;GUI.matrix=oldMatrix;
        }
        void DrawHeroRegion(Texture2D texture,Vector2 sourceSize,Rect source,Vector2 origin,float scale)
        {
            // Source rectangles are authored against the original PNG, not any
            // platform-resized imported texture. Normalized UVs survive resizing.
            var uv=new Rect(source.x/sourceSize.x,1-source.yMax/sourceSize.y,source.width/sourceSize.x,source.height/sourceSize.y);
            var target=new Rect(origin.x,origin.y,source.width*scale,source.height*scale);
            GUI.DrawTextureWithTexCoords(target,texture,uv,true);
        }
        void DrawHeroPortrait(Rect rect,bool speaking)
        {
            EnsureHeroArt();
            Box(rect,new Color(.09f,.13f,.19f));
            if(heroPortrait!=null)
            {
                var oldColor=GUI.color;
                GUI.color=speaking?Color.white:new Color(.8f,.85f,.9f,1);
                GUI.DrawTexture(new Rect(rect.x,rect.y,rect.width,rect.height-30),heroPortrait,ScaleMode.ScaleToFit,true);
                GUI.color=oldColor;
            }
            Text(speaking?"여행자 · 말하는 중":"여행자",rect.x,rect.yMax-30,rect.width,28,17,new Color(.92f,.73f,.65f),TextAnchor.MiddleCenter);
        }
    }
}

