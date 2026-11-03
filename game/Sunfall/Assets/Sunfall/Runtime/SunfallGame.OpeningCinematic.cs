using UnityEngine;
using UnityEngine.InputSystem;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        readonly Texture2D[] openingArt=new Texture2D[4];
        static readonly string[] OpeningArtNames={"BoundSunV1","MothersHomeV1","SeveredHandsV1","GoldenVowV1"};
        static readonly string[] OpeningShotNames={"태양의 봉인","빛을 잃은 세계","돌아갈 집","나를 딸이라 불러준 사람","떨어진 손끝","깨어난 별실","살아 있는 심장","등불을 지키는 이","하늘을 꿰매는 맹세"};
        float openingSceneTime,openingTransitionTime;
        bool openingTransition,openingFinish,openingSubtitleRevealed;
        bool OpeningCinematicActive=>ReferenceEquals(storyLines,OpeningStory);
        float OpeningShotDuration=>Mathf.Max(7f,OpeningStory[storyPage].Words.Length/7.5f+3f);
        int OpeningVisibleCharacters=>openingSubtitleRevealed?OpeningStory[storyPage].Words.Length:
            Mathf.Clamp(Mathf.FloorToInt((openingSceneTime-.8f)*22),0,OpeningStory[storyPage].Words.Length);
        void LoadOpeningArt()
        {
            for(int i=0;i<openingArt.Length;i++)openingArt[i]=Resources.Load<Texture2D>("Story/Cinematic/"+OpeningArtNames[i]);
        }
        void UpdateOpeningCinematic(Keyboard keyboard)
        {
            if(paused||settings)return;
            float dt=Mathf.Min(Time.unscaledDeltaTime,.1f);
            if(openingTransition)
            {
                openingTransitionTime+=dt;
                if(openingTransitionTime>=.75f)
                {
                    if(openingFinish){EndStory();return;}
                    storyPage++;openingSceneTime=openingTransitionTime=0;
                    openingTransition=false;openingSubtitleRevealed=false;Block();
                }
                return;
            }
            openingSceneTime+=dt;
            if(Time.unscaledTime>=blockedUntil&&keyboard!=null&&
                (keyboard.spaceKey.wasPressedThisFrame||keyboard.enterKey.wasPressedThisFrame||keyboard.numpadEnterKey.wasPressedThisFrame))
            {AdvanceOpeningCinematic();return;}
            if(openingSceneTime>=OpeningShotDuration)TransitionOpening(false);
        }
        void AdvanceOpeningCinematic()
        {
            if(!OpeningCinematicActive||paused||settings||openingTransition)return;
            if(OpeningVisibleCharacters<OpeningStory[storyPage].Words.Length)
            {openingSubtitleRevealed=true;Block();return;}
            TransitionOpening(false);
        }
        void TransitionOpening(bool skip)
        {
            if(!OpeningCinematicActive||paused||settings)return;
            // Skip may override an in-progress shot transition, but never advances twice.
            if(openingTransition){if(skip){openingFinish=true;RememberStory(OpeningStory);}return;}
            openingFinish=skip||storyPage==OpeningStory.Length-1;
            if(openingFinish)RememberStory(OpeningStory);
            openingTransition=true;openingTransitionTime=0;Block();
        }
        Texture2D OpeningShotTexture()
        {
            if(storyPage==7)return villageBackdrop;
            int index=storyPage<=1?0:storyPage<=3?1:storyPage<=5?2:3;
            return openingArt[index]!=null?openingArt[index]:villageBackdrop;
        }
        void DrawOpeningCinematic()
        {
            hits.Clear();Box(new Rect(0,0,1280,720),Color.black);
            float progress=Mathf.Clamp01(openingSceneTime/OpeningShotDuration);
            float ease=Mathf.SmoothStep(0,1,progress);
            // Lock the illustration to the same screen rectangle in every GUI event.
            // Only local effects animate; there is no camera pan, zoom or group transform.
            var art=OpeningShotTexture();
            if(art!=null){GUI.color=Color.white;GUI.DrawTexture(new Rect(0,0,1280,720),art,ScaleMode.ScaleAndCrop,false);}
            DrawOpeningAtmosphere(ease);
            // Wide scene art fills the screen; only a soft subtitle gradient sits over it.
            for(int i=0;i<22;i++)Box(new Rect(0,500+i*10,1280,10),new Color(.008f,.012f,.022f,Mathf.Lerp(0,.94f,i/21f)));
            Box(new Rect(0,0,1280,64),new Color(.008f,.012f,.022f,.76f));
            Box(new Rect(0,674,1280,46),new Color(.008f,.012f,.022f,.9f));
            var line=OpeningStory[storyPage];
            Text(line.Voice==StoryVoice.Narrator?"":StoryVoiceName(line.Voice),115,521,1050,31,20,HudLight);
            string words=line.Words.Substring(0,OpeningVisibleCharacters);
            Text(words,116,563,1048,104,25,Color.black);
            Text(words,115,561,1050,104,25,new Color(.96f,.95f,.9f));
            if(storyPage==8)
            {
                float a=Mathf.Clamp01((openingSceneTime-1.5f)/2f);
                Text(GameTitle,78,180,510,95,68,new Color(.96f,.86f,.65f,a));
                Text(GameSubtitle,83,272,515,56,30,new Color(.96f,.86f,.65f,a));
            }
            float fade=openingTransition?Mathf.Clamp01(openingTransitionTime/.75f):1-Mathf.Clamp01(openingSceneTime/.9f);
            if(fade>0)Box(new Rect(0,0,1280,720),new Color(0,0,0,fade));
            Text("서장 · 손끝에 남은 별실",45,20,460,31,20,HudLight);
            Text(OpeningShotNames[storyPage],490,23,360,27,16,new Color(.75f,.78f,.8f),TextAnchor.MiddleRight);
            EquipmentButton("일시정지",new Rect(875,15,135,38),()=>{paused=true;Block();});
            EquipmentButton("서장 건너뛰기",new Rect(1030,15,205,38),()=>TransitionOpening(true));
            for(int i=0;i<OpeningStory.Length;i++)
                Box(new Rect(46+i*20,691,13,3),i<=storyPage?HudLight:HudIron);
            Text((storyPage+1)+" / "+OpeningStory.Length+"  · 자동 재생",245,680,235,30,15,new Color(.78f,.8f,.78f));
            Text("Space / Enter 자막·다음   ·   Esc 일시정지",490,680,520,30,14,new Color(.7f,.73f,.75f),TextAnchor.MiddleCenter);
            EquipmentButton(OpeningVisibleCharacters<line.Words.Length?"자막 펼치기":storyPage==8?"마을로":"다음 장면",new Rect(1055,677,180,34),AdvanceOpeningCinematic);
        }
        void DrawOpeningAtmosphere(float progress)
        {
            float t=openingSceneTime;
            bool home=storyPage==2||storyPage==3;
            bool rift=storyPage==4||storyPage==5;
            Color mote=home?new Color(1,.68f,.3f,.3f):rift?new Color(1,.22f,.25f,.45f):new Color(.91f,.77f,.45f,.35f);
            if(storyPage<=1)
            {
                // The bound sun slowly loses its light, with ash moving at several depths.
                Box(new Rect(0,0,1280,720),new Color(.012f,.018f,.045f,storyPage==0?.07f*progress:.22f+.3f*progress));
                float sway=Mathf.Sin(t*.6f)*5;
                DrawChainLinks(new Vector2(-50,90+sway),new Vector2(290,270+sway),new Color(.08f,.085f,.1f,.8f),16);
                DrawChainLinks(new Vector2(1310,100-sway),new Vector2(1110,355-sway),new Color(.1f,.105f,.13f,.7f),14);
            }
            if(home)
            {
                float warm=.025f+.012f*Mathf.Sin(t*1.8f)+.008f*Mathf.Sin(t*3.1f);
                Box(new Rect(0,0,1280,720),new Color(1,.48f,.1f,warm));
            }
            if(rift)
            {
                // A steady breathing glow avoids flashes while reinforcing the moving tear.
                Box(new Rect(0,0,1280,720),new Color(.65f,.025f,.065f,.035f+.025f*Mathf.Sin(t*.9f)));
                float pull=Mathf.SmoothStep(0,15,progress);
                DrawChainLinks(new Vector2(-30,30),new Vector2(310-pull,200-pull),new Color(.08f,.035f,.05f,.8f),18);
                if(storyPage==5)DrawOpeningThread(new Vector2(660,332),new Vector2(615,295),new Vector2(575,269),Mathf.Clamp01((t-1)/3),t);
            }
            if(storyPage==6||storyPage==8)
                DrawOpeningThread(new Vector2(699,267),new Vector2(380,225),new Vector2(130,360),Mathf.Clamp01(t/4),t);
            for(int i=0;i<44;i++)
            {
                float speed=5+(i%5)*3;
                float x=Mathf.Repeat(i*137+t*(rift?-speed*.7f:speed*.18f),1360)-40;
                float y=710-Mathf.Repeat(i*73+t*speed,730);
                if(home&&x>800)continue;
                float a=(.45f+.55f*Mathf.Sin((i+t*.4f)*.7f))*mote.a;
                GothicDiamond(x,y,i%7==0?2:1,new Color(mote.r,mote.g,mote.b,Mathf.Max(.04f,a)));
            }
        }
        void DrawOpeningThread(Vector2 start,Vector2 bend,Vector2 end,float reveal,float time)
        {
            Vector2 previous=start;
            for(int i=1;i<=48;i++)
            {
                float p=i/48f;if(p>reveal)break;
                Vector2 point=(1-p)*(1-p)*start+2*(1-p)*p*bend+p*p*end;
                point.y+=Mathf.Sin(p*18-time*1.6f)*6*Mathf.Sin(p*Mathf.PI);
                GothicLine(previous,point,new Color(1,.65f,.24f,.14f),5);
                GothicLine(previous,point,new Color(1,.85f,.48f,.88f),1);
                if(i%8==0)GothicDiamond(point.x,point.y,1.8f,new Color(1,.95f,.73f,.7f));
                previous=point;
            }
            if(reveal>0)GothicDiamond(previous.x,previous.y,3,new Color(1,.95f,.78f,.8f));
        }
    }
}
