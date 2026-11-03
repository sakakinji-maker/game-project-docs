using UnityEngine;
using UnityEngine.InputSystem;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        const string GameTitle="SUNFALL";
        const string GameSubtitle="별을 꿰매는 마녀";
        static readonly Rect TitleStartRect=new Rect(440,552,400,54);
        static readonly Rect TitleSettingsRect=new Rect(440,610,400,42);
        Texture2D titleBackdrop,titleWordmark;
        float nextTitleArtRetry;
        int titleSelection;

        void LoadTitleArt()
        {
            titleBackdrop=Resources.Load<Texture2D>("Title/ChainedSunBackdropV1");
            titleWordmark=Resources.Load<Texture2D>("Title/SunfallWordmarkV1");
            foreach(var texture in new[]{titleBackdrop,titleWordmark})
                if(texture!=null){texture.filterMode=FilterMode.Bilinear;texture.wrapMode=TextureWrapMode.Clamp;}
            nextTitleArtRetry=Time.realtimeSinceStartup+1;
        }
        bool UpdateTitleInput(Keyboard keyboard)
        {
            if(!title||settings||paused||StoryActive||Time.unscaledTime<blockedUntil)return false;
            if(Mouse.current!=null&&Mouse.current.delta.ReadValue().sqrMagnitude>0)
            {
                var point=Point(Mouse.current.position.ReadValue());
                if(TitleStartRect.Contains(point))titleSelection=0;
                else if(TitleSettingsRect.Contains(point))titleSelection=1;
            }
            if(keyboard==null)return false;
            if(keyboard.upArrowKey.wasPressedThisFrame||keyboard.downArrowKey.wasPressedThisFrame)
            {titleSelection=1-titleSelection;return true;}
            if(keyboard.enterKey.wasPressedThisFrame||keyboard.numpadEnterKey.wasPressedThisFrame||keyboard.spaceKey.wasPressedThisFrame)
            {ActivateTitleMenu(titleSelection);return true;}
            return false;
        }
        void ActivateTitleMenu(int selection)
        {
            if(!title||settings||StoryActive)return;
            titleSelection=selection;
            if(selection==0)BeginOpeningStory();
            else {settings=true;notice="";Block();}
        }
        void DrawTitle()
        {
            if((titleBackdrop==null||titleWordmark==null)&&Time.realtimeSinceStartup>=nextTitleArtRetry)LoadTitleArt();
            DrawTitleBackdrop();
            if(titleWordmark!=null)
            {
                var color=GUI.color;GUI.color=Color.white;
                GUI.DrawTexture(new Rect(240,310,800,200),titleWordmark,ScaleMode.ScaleToFit,true);
                GUI.color=color;
            }
            else
            {
                Text(GameTitle,242,338,800,133,100,new Color(0,0,0,.8f),TextAnchor.MiddleCenter);
                Text(GameTitle,240,336,800,133,100,new Color(.97f,.9f,.72f),TextAnchor.MiddleCenter);
            }
            Text(GameSubtitle,340,478,600,43,29,new Color(.95f,.85f,.66f),TextAnchor.MiddleCenter);
            GothicLine(new Vector2(426,500),new Vector2(488,500),new Color(.76f,.58f,.3f,.65f),1);
            GothicLine(new Vector2(792,500),new Vector2(854,500),new Color(.76f,.58f,.3f,.65f),1);
            Text("어머니가 돌아올 길을, 찢어진 하늘에 꿰매다.",320,520,640,26,16,new Color(.75f,.77f,.79f),TextAnchor.MiddleCenter);
            DrawTitleMenuButton(TitleStartRect,OpeningStorySeen?"마을로 가기":"이야기 시작",0);
            DrawTitleMenuButton(TitleSettingsRect,"환경 설정",1);
            string hint=titleSelection==1?"조작 · 소리 설정":OpeningStorySeen?"별실을 따라, 다시 마을로":"일곱 봉인 너머, 당신에게 닿을 때까지";
            Text(hint,320,658,640,22,14,new Color(.65f,.64f,.59f),TextAnchor.MiddleCenter);
            Text(TouchControls?"메뉴를 터치해 선택하세요":"↑ ↓ 선택     Enter / Space 결정",320,690,640,20,12,new Color(.48f,.52f,.58f),TextAnchor.MiddleCenter);
            DrawDeveloperTitleStatus();
        }
        void DrawTitleMenuButton(Rect rect,string label,int index)
        {
            bool selected=titleSelection==index;
            Color ink=selected?new Color(1,.91f,.71f):new Color(.64f,.67f,.71f);
            if(selected)
            {
                Color rule=new Color(.78f,.6f,.32f,.7f);
                GothicLine(new Vector2(rect.x+24,rect.center.y),new Vector2(rect.x+77,rect.center.y),rule,1);
                GothicLine(new Vector2(rect.xMax-77,rect.center.y),new Vector2(rect.xMax-24,rect.center.y),rule,1);
                GothicDiamond(rect.x+87,rect.center.y,3,ink);
                GothicDiamond(rect.xMax-87,rect.center.y,3,ink);
            }
            Text(label,rect.x+1,rect.y+2,rect.width,rect.height,index==0?27:22,new Color(0,0,0,.9f),TextAnchor.MiddleCenter);
            Text(label,rect.x,rect.y,rect.width,rect.height,index==0?27:22,ink,TextAnchor.MiddleCenter);
            hits.Add(new Hit(rect,()=>ActivateTitleMenu(index)));
        }
        void DrawTitleBackdrop()
        {
            var color=GUI.color;
            GUI.color=Color.white;
            if(titleBackdrop!=null)
                GUI.DrawTexture(new Rect(0,0,1280,720),titleBackdrop,ScaleMode.ScaleAndCrop,false);
            else
                Box(new Rect(0,0,1280,720),new Color(.018f,.025f,.042f));
            // The artwork stays still: no camera shake, zoom, or parallax.
            // A lower veil separates the wordmark/menu from cathedral detail.
            for(int i=0;i<32;i++)
            {
                float t=(i+.5f)/32;
                Box(new Rect(0,280+i*13.75f,1280,13.75f),new Color(.008f,.012f,.024f,Mathf.SmoothStep(0,.58f,t)));
            }
            for(int i=0;i<8;i++)
                Box(new Rect(0,i*10,1280,10),new Color(.008f,.012f,.024f,.25f*(1-i/8f)));
            // Sparse, slow thread motes at the outer edges never move the art.
            float time=Time.unscaledTime;
            for(int i=0;i<18;i++)
            {
                float x=i%2==0?55+(i*71)%220:1005+(i*53)%220;
                float y=700-Mathf.Repeat(i*53+time*(1.5f+i%3),550);
                float alpha=.10f+.16f*(.5f+.5f*Mathf.Sin(time*.45f+i));
                GothicDiamond(x,y,i%4==0?1.5f:1,new Color(.96f,.76f,.43f,alpha));
            }
            GUI.color=color;
        }
    }
}
