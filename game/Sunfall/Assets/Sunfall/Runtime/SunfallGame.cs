using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Sunfall.Core;
using ETouch=UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Sunfall
{
    public sealed partial class SunfallGame : MonoBehaviour
    {
        struct Hit {public Rect Rect;public Action Action;public Hit(Rect rect,Action action){Rect=rect;Action=action;}}
        readonly List<Hit> hits=new List<Hit>();
        readonly Rect left=new Rect(20,580,80,94),right=new Rect(115,580,80,94),attack=new Rect(1090,580,80,94),jump=new Rect(1178,580,80,94);
        readonly Rect roll=new Rect(320,580,100,94);
        bool previousRoll,queuedRoll;
        RunModel run;Bindings keys;bool title=true,paused,settings,touchPreview,previousJump;
        int capture=-1,captureFrame;float accumulator,blockedUntil;string notice="";
        Font font;GUIStyle text;Texture2D white;AudioSource music;
        float volume=.18f;int nextSeed=12345;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot(){if(FindFirstObjectByType<SunfallGame>()==null)new GameObject("Sunfall Prototype").AddComponent<SunfallGame>();}
        void Awake()
        {
            var cameraObject=new GameObject("Prototype Camera");
            var camera=cameraObject.AddComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;camera.cullingMask=0;
            if(FindFirstObjectByType<AudioListener>()==null)cameraObject.AddComponent<AudioListener>();
            Application.targetFrameRate=Application.isMobilePlatform?60:120;keys=new Bindings();white=Texture2D.whiteTexture;
            LoadEquipment();
            font=Resources.Load<Font>("Korean");
            // The bundled font is a subset. Prefer a full Korean OS font for new dialogue.
            foreach(var installed in Font.GetOSInstalledFontNames())
                if(installed=="Malgun Gothic"||installed=="Apple SD Gothic Neo"||installed=="Noto Sans CJK KR")
                {font=Font.CreateDynamicFontFromOSFont(installed,20);break;}
            if(font==null)font=Font.CreateDynamicFontFromOSFont(new[]{"Malgun Gothic","Apple SD Gothic Neo","Arial"},20);
            volume=Mathf.Clamp01(PlayerPrefs.GetFloat("sunfall.volume",.18f));touchPreview=PlayerPrefs.GetInt("sunfall.touchPreview",0)!=0;
            nextSeed=unchecked((int)DateTime.UtcNow.Ticks);CreateMusic();
        }
        // Awake is not rerun when Unity reloads scripts on an existing instance.
        void OnEnable(){EnhancedTouchSupport.Enable();white=Texture2D.whiteTexture;LoadHeroArt();LoadDemonArt();LoadMapArt();LoadBossArt();LoadVillageNpcArt();LoadVillageBackdrop();LoadSkillIcons();LoadPresentationArt();LoadStoryArt();LoadOpeningArt();LoadBossPortraits();LoadResidentPortraits();LoadTitleArt();LoadStoryProgress();}
        void OnDisable(){EnhancedTouchSupport.Disable();}
        void OnDestroy(){if(music!=null&&music.clip!=null)Destroy(music.clip);}
        void OnApplicationFocus(bool focus){if(!focus)Suspend();}
        void OnApplicationPause(bool value){if(value)Suspend();}
        void Suspend(){if(!title&&(village||(run!=null&&run.Phase==RunPhase.Combat)))paused=true;Block();}
        void Block(){ClearSkillInput();blockedUntil=Time.unscaledTime+.2f;accumulator=0;previousJump=previousRoll=true;queuedJump=queuedDrop=queuedEnter=queuedRoll=false;}
        Rect View
        {
            get {var safe=Screen.safeArea;float s=Mathf.Min(safe.width/1280f,safe.height/720f);return new Rect(safe.x+(safe.width-1280*s)/2,Screen.height-safe.y-safe.height+(safe.height-720*s)/2,1280*s,720*s);}
        }
        Vector2 Point(Vector2 bottomLeft){Rect v=View;return new Vector2((bottomLeft.x-v.x)/ (v.width/1280f),(Screen.height-bottomLeft.y-v.y)/(v.height/720f));}
        bool TouchHeld(Rect rect)
        {
            foreach(var t in ETouch.activeTouches)
                if(t.phase!=UnityEngine.InputSystem.TouchPhase.Ended&&t.phase!=UnityEngine.InputSystem.TouchPhase.Canceled&&rect.Contains(Point(t.screenPosition)))return true;
            return touchPreview&&Mouse.current!=null&&Mouse.current.leftButton.isPressed&&rect.Contains(Point(Mouse.current.position.ReadValue()));
        }
        bool TouchControls=>Application.isMobilePlatform||touchPreview;
        void Click(Vector2 point)
        {
            // Copy the selected action; it may change state and the next layout.
            for(int i=hits.Count-1;i>=0;i--)if(hits[i].Rect.Contains(point)){var action=hits[i].Action;action();return;}
        }
        void Update()
        {
            music.volume=settings?volume:(paused||inventoryOpen||skillWindowOpen||serviceNpc>=0||StoryActive)?volume*.4f:volume;
            var keyboard=Keyboard.current;
            if(DeveloperToolsEnabled&&capture<0&&keyboard!=null&&keyboard.f10Key.wasPressedThisFrame)
            {ResetDeveloperProgress();return;}
            if(UpdateDeveloperChapterInput(keyboard))return;
            if(capture>=0)
            {
                if(Mouse.current!=null&&Mouse.current.leftButton.wasPressedThisFrame)Click(Point(Mouse.current.position.ReadValue()));
                foreach(var touch in ETouch.activeTouches)if(touch.phase==UnityEngine.InputSystem.TouchPhase.Began)Click(Point(touch.screenPosition));
                if(capture<0)return;
                if(keyboard!=null&&Time.frameCount>captureFrame)
                    foreach(var key in keyboard.allKeys)if(key.wasPressedThisFrame)
                    {
                        if(key.keyCode==Key.Escape){capture=-1;notice="키 변경을 취소했어요.";}
                        else if(keys.Set(capture,key.keyCode)){capture=-1;notice="저장했어요. 다음 실행에도 유지돼요.";}
                        else notice="사용 중이거나 전용 기능에 예약된 키예요. 다른 키를 눌러 주세요.";
                        Block();break;
                    }
                return;
            }
            if(keyboard!=null&&keyboard.escapeKey.wasPressedThisFrame)
            {
                if(settings)settings=false;
                else if(skillWindowOpen)skillWindowOpen=false;
                else if(inventoryOpen)inventoryOpen=false;
                else if(serviceNpc>=0)serviceNpc=-1;
                else if(StoryActive)paused=!paused;
                else if(village&&dialoguePage>=0)dialoguePage=-1;
                else if(village)paused=!paused;
                else if(!title&&run!=null&&run.Phase==RunPhase.Combat)paused=!paused;
                Block();return;
            }
            if(keyboard!=null&&keyboard.kKey.wasPressedThisFrame&&CanToggleSkills&&Time.unscaledTime>=blockedUntil)
            {ToggleSkillWindow();return;}
            if(keyboard!=null&&keyboard.iKey.wasPressedThisFrame&&CanToggleInventory&&Time.unscaledTime>=blockedUntil)
            {ToggleInventory();return;}
            if(UpdateSettingsInput(keyboard))return;
            if(UpdateTitleInput(keyboard))return;
            if(Time.unscaledTime>=blockedUntil)
            {
                if(Mouse.current!=null&&Mouse.current.leftButton.wasPressedThisFrame)Click(Point(Mouse.current.position.ReadValue()));
                foreach(var t in ETouch.activeTouches)
                    if(t.phase==UnityEngine.InputSystem.TouchPhase.Began){Click(Point(t.screenPosition));if(Time.unscaledTime<blockedUntil)break;}
            }
            if(StoryActive)
            {
                accumulator=0;
                if(OpeningCinematicActive){UpdateOpeningCinematic(keyboard);return;}
                if(!paused&&!settings&&Time.unscaledTime>=blockedUntil&&keyboard!=null&&keyboard.spaceKey.wasPressedThisFrame)AdvanceStory();
                return;
            }
            if(inventoryOpen||skillWindowOpen||serviceNpc>=0){accumulator=0;return;}
            if(village){UpdateVillage();return;}
            if(title||settings||paused||run==null||run.Phase!=RunPhase.Combat||Time.unscaledTime<blockedUntil){accumulator=0;return;}
            if(run.Cinematic)
            {
                if(TryBeginBossStory())return;
                accumulator+=Mathf.Min(Time.unscaledDeltaTime,.1f);
                while(accumulator>=RunModel.StepSeconds)
                {
                    CaptureWorldMotion();run.Step(0,false,false);accumulator-=RunModel.StepSeconds;
                    if(TryBeginBossStory())break;
                    if(!run.Cinematic){ResetHeroVisuals();Block();break;}
                }
                return;
            }
            if(ShowSkillChoice){accumulator=0;return;}
            ReadSkillInput();
            bool l=keys.Held(0)||(TouchControls&&TouchHeld(left));
            bool r=keys.Held(1)||(TouchControls&&TouchHeld(right));
            bool a=keys.Held(2)||(TouchControls&&TouchHeld(attack));
            bool j=keys.Held(3)||(TouchControls&&TouchHeld(jump));
            bool dodge=keys.Held(4)||(TouchControls&&TouchHeld(roll));
            bool jumpEdge=(j&&!previousJump)||keys.Pressed(3);previousJump=j;
            queuedRoll|=(dodge&&!previousRoll)||keys.Pressed(4);previousRoll=dodge;
            queuedEnter|=keyboard!=null&&keyboard.upArrowKey.wasPressedThisFrame;
            accumulator+=Mathf.Min(Time.unscaledDeltaTime,.1f);
            // Preserve jump edge until a fixed simulation step occurs.
            if(jumpEdge)
            {
                queuedJump=true;
                queuedDrop=keyboard!=null&&keyboard.downArrowKey.isPressed;
            }
            while(accumulator>=RunModel.StepSeconds)
            {
                float previousX=run.X,previousHp=run.Hp;int previousRoom=run.RoomIndex,previousChapter=run.Chapter;bool hadChest=run.ChestOpened;CaptureWorldMotion();
                run.SetCombatView(cameraX+(64-90)/34f,cameraX+(1216-90)/34f,cameraY+(540-565)/34f,cameraY+(540-70)/34f);
                run.Step((r?1:0)-(l?1:0),queuedJump,a,queuedRoll,down:queuedDrop,enter:queuedEnter,skillSlot:QueuedSkillSlot());
                if((!hadChest&&run.ChestOpened)||savedProgressionRevision!=playerProgression.Revision)SaveEquipment();
                if(run.RoomIndex!=previousRoom||run.Chapter!=previousChapter){ResetHeroVisuals();cameraX=cameraY=0;Block();if(run.Chapter==3&&previousChapter!=3)BeginStory("제3장 · 왕도 대성당",CathedralArrivalStory);else if(run.Chapter==4&&previousChapter!=4)BeginStory("제4장 · 왕관의 심장부",CrownHeartArrivalStory);else if(run.Chapter>=5&&run.Chapter!=previousChapter)BeginStory("제"+run.Chapter+"장 · "+RunModel.ChapterName(run.Chapter),LateArrivalStories[run.Chapter-5]);break;}
                if(run.BossDissolving){defeatCameraStart=new Vector2(cameraX,cameraY);ResetHeroVisuals();Block();break;}
                TickHeroVisuals(run.X-previousX,RunModel.StepSeconds,run.Hp<previousHp);
                queuedJump=queuedDrop=queuedEnter=queuedRoll=false;for(int i=0;i<4;i++)queuedSkill[i]=false;accumulator-=RunModel.StepSeconds;
                if(run.Phase!=RunPhase.Combat){Block();break;}
            }
        }
        bool queuedJump,queuedDrop,queuedEnter;
        void StartRun(bool sameSeed=false)
        {
            storyLines=null;bossArrivalStoryChapter=bossDefeatStoryChapter=0;
            if(!sameSeed)nextSeed=unchecked(nextSeed+7919);
            skillWindowOpen=false;selectedSkill=SpellKind.EmberWave;run=new RunModel(nextSeed,playerEquipment,playerProgression);serviceNpc=-1;inventoryOpen=false;title=false;village=false;dialoguePage=-1;paused=false;settings=false;queuedJump=false;ResetHeroVisuals();cameraX=cameraY=0;Block();
        }
        void Box(Rect r,Color c){GUI.color=c;GUI.DrawTexture(r,white);GUI.color=Color.white;}
        void Text(string s,float x,float y,float w,float h,int size=22,Color? color=null,TextAnchor align=TextAnchor.UpperLeft)
        {
            text.fontSize=size;text.normal.textColor=color??new Color(.85f,.9f,.94f);text.alignment=align;
            GUI.Label(new Rect(x,y,w,h),s,text);
        }
        void Button(string s,Rect r,Action action,bool primary=false)
        {
            Box(r,primary?new Color(.78f,.56f,.23f):new Color(.12f,.19f,.25f));
            Text(s,r.x,r.y+3,r.width,r.height-3,22,primary?new Color(.08f,.1f,.12f):(Color?)null,TextAnchor.MiddleCenter);
            hits.Add(new Hit(r,action));
        }
        void OnGUI()
        {
            if(text==null)text=new GUIStyle {font=font,wordWrap=true};
            hits.Clear();var old=GUI.matrix;Rect v=View;
            // One canvas transform, shared with Point(). A physical-pixel group
            // around a scaled logical canvas caused title clipping/offsets.
            GUI.matrix=Matrix4x4.TRS(new Vector3(v.x,v.y,0),Quaternion.identity,new Vector3(v.width/1280,v.height/720,1));
            Box(new Rect(0,0,1280,720),new Color(.025f,.045f,.075f));DrawSky();
            if(OpeningCinematicActive)DrawOpeningCinematic();else if(title)DrawTitle();else if(village)DrawVillage();else{DrawWorld();if(run.BossIntroducing)DrawBossIntroOverlay();else if(run.BossDissolving)DrawBossDefeatOverlay();else DrawHud();if(run.Phase==RunPhase.Dead||run.Phase==RunPhase.Won)DrawResult();}
            if(inventoryOpen)DrawInventory();
            if(skillWindowOpen)DrawSkillWindow();
            if(serviceNpc>=0)DrawVillageService();
            if(StoryActive&&!OpeningCinematicActive)DrawStory();
            if(ShowSkillChoice)DrawBossSkillChoice();
            if(paused&&!settings)DrawPause();if(settings)DrawSettings();
            DrawDeveloperJumpNotice();
            DrawViewportBars(v);GUI.matrix=old;
        }
        void DrawViewportBars(Rect view)
        {
            // Mask overscan after drawing; do not change the canvas clip origin.
            var matrix=GUI.matrix;var color=GUI.color;GUI.matrix=Matrix4x4.identity;GUI.color=Color.black;
            if(view.x>0)GUI.DrawTexture(new Rect(0,0,view.x,Screen.height),white);
            if(view.xMax<Screen.width)GUI.DrawTexture(new Rect(view.xMax,0,Screen.width-view.xMax,Screen.height),white);
            if(view.y>0)GUI.DrawTexture(new Rect(view.x,0,view.width,view.y),white);
            if(view.yMax<Screen.height)GUI.DrawTexture(new Rect(view.x,view.yMax,view.width,Screen.height-view.yMax),white);
            GUI.color=color;GUI.matrix=matrix;
        }
        void DrawSky()
        {
            for(int i=0;i<65;i++)Box(new Rect((i*197)%1280,45+(i*73)%340,i%3==0?2:1,2),new Color(.24f,.33f,.43f));
            for(int i=0;i<16;i++)
            {
                float h=70+(i*47)%120;
                Box(new Rect(i*90,540-h,70,h),new Color(.045f,.08f,.12f));
                Box(new Rect(i*90+18,540-h-15,34,20),new Color(.045f,.08f,.12f));
            }
            // Empty sun: hand-drawn procedural ring, no borrowed artwork.
            for(int i=0;i<80;i++){float a=i*Mathf.PI*2/80;Box(new Rect(1000+Mathf.Cos(a)*65,150+Mathf.Sin(a)*65,3,3),new Color(.22f,.28f,.32f));}
        }
        float SX(float x)=>90+(x-cameraX)*34;
        float SY(float y)=>540-(y-cameraY)*34;
        void DrawWorld()
        {
            GUI.BeginGroup(new Rect(0,0,1280,580));DrawMapTerrain();
            if(run.BossIntroducing)DrawBossArrivalEffect();
            DrawRewardChest();

            foreach(var e in run.Enemies)
            {
                if(e.Boss)DrawStage1Boss(e);else DrawDemon(e);
            }
            if(run.BossDissolving)DrawBossDefeatParticles();
            DrawDemonArrows();DrawBloodOrbs();
            var playerPosition=RenderPosition(run,run.X,run.Y);float px=SX(playerPosition.x),py=SY(playerPosition.y);
            DrawHero(px,py,run.Facing,false);
            DrawPlayerSpellEffects(px,py);
            DrawRoyalChainHazards();
            DrawKillRewards();
            DrawDamageNumbers();
            GUI.EndGroup();
        }
        void DrawHud()
        {
            DrawGothicHudBackdrop();DrawMinimap();DrawBossHud();DrawPlayerVitals();
            Text($"SUNFALL · {run.Chapter}스테이지",35,22,245,35,23,new Color(.88f,.68f,.37f));
            Text(RoomTitle,35,67,260,35,22);
            Text(run.Stage==RunStage.Approach?(run.ExitOpen?NextRoomTitle+" · 문 앞에서 ↑":RunModel.ChapterName(run.Chapter)+" · 적 "+run.RemainingMonsters):run.ChestAvailable&&!run.ChestOpened?"보물상자 앞에서 ↑ · 물약과 장비":run.ExitOpen?"포탈 앞에서 ↑ · "+NextRoomTitle:BossTitle+" 처치",610,28,470,35,17);
            Button("설정 / 정지",new Rect(1095,22,150,48),()=>{paused=true;Block();});
            DrawInventoryEntry();DrawSkillWindowEntry();
            DrawProgressionStatus(610,77,225);
            DrawGothicSkillTray();
            DrawHealingNotice();
            DrawSpellRewardNotice();
        }
        void Overlay(){hits.Clear();Box(new Rect(0,0,1280,720),new Color(.015f,.03f,.05f,.93f));}
        void DrawResult()
        {
            Overlay();bool win=run.Phase==RunPhase.Won;
            Text(win?"함께 돌아온 아침":"빛을 찾아 다시 한번",90,125,1100,80,46,Color.white);
            Text(win?"일곱 봉인을 넘어 양어머니와 함께 돌아왔습니다.\n태양은 다시 떠오르고, 하늘에는 금빛 봉합 자국이 남았습니다.":"어머니에게 닿은 별실은 아직 끊어지지 않았습니다.\n마을에서 정비하고, 다시 성소로 향하세요.",90,245,1100,100,26);
            Text($"처치 {run.Kills}  ·  {run.Chapter}스테이지: {RoomTitle}  ·  시드 {run.Seed}",90,380,1000,45,22);
            Button("새로운 도전",new Rect(90,480,270,65),()=>StartRun(),true);
            Button("같은 시드 재도전",new Rect(380,480,270,65),()=>StartRun(true));
            Button("마을로 귀환",new Rect(670,480,200,65),EnterVillage);
            Button("장비 확인 (I)",new Rect(900,480,270,65),ToggleInventory);
            if(run.BossSpellReward.HasValue)Text("이번 모험에서 획득한 스킬: "+AllRewardSpellLabels+" · 새 모험에서는 다시 보스 보상으로 획득",90,565,1100,95,18);
        }
        void DrawPause()
        {
            Overlay();Text("잠시 쉬어 가기",350,150,650,70,42,Color.white);
            Button("계속하기",new Rect(400,280,480,60),()=>{paused=false;queuedJump=false;Block();},true);
            Button("조작 및 소리 설정",new Rect(400,360,480,60),()=>{settings=true;notice="";Block();});
            Button(village?"처음으로":"이번 도전을 마치고 마을로",new Rect(400,440,480,60),()=>{if(!village){EnterVillage();return;}title=true;storyLines=null;serviceNpc=-1;inventoryOpen=false;village=false;dialoguePage=-1;paused=false;run=null;Block();});
            Text("골드·장비·강화·성장은 저장됩니다.\n귀환 시 현재 도전은 종료되며 다음에는 숲 입구부터 시작합니다.",270,550,760,70,20,null,TextAnchor.MiddleCenter);
        }
        void SetVolume(float v){volume=Mathf.Clamp01(v);PlayerPrefs.SetFloat("sunfall.volume",volume);PlayerPrefs.Save();notice="음량을 "+Mathf.RoundToInt(volume*100)+"%로 저장했습니다.";Block();}
        void CreateMusic()
        {
            // Original placeholder ambient loop; replace after music direction is decided.
            const int rate=22050,seconds=16;float[] data=new float[rate*seconds];
            float[] notes={110f,130.8128f,164.8138f,146.8324f};
            for(int i=0;i<data.Length;i++)
            {
                float t=i/(float)rate,local=t%4f,env=Mathf.Sin(Mathf.PI*local/4f);float hz=notes[Mathf.Min(3,(int)(t/4))];
                data[i]=.16f*env*env*(Mathf.Sin(2*Mathf.PI*hz*t)+.25f*Mathf.Sin(2*Mathf.PI*hz*2*t));
            }
            var clip=AudioClip.Create("Prototype ambient",data.Length,1,rate,false);clip.SetData(data,0);
            music=gameObject.AddComponent<AudioSource>();music.clip=clip;music.loop=true;music.volume=volume;music.Play();
        }
    }
}






