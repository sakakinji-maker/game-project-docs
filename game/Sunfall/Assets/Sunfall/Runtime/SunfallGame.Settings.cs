using UnityEngine;
using UnityEngine.InputSystem;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        int settingsTab;
        static readonly Color SettingsMuted=new Color(.59f,.64f,.69f);
        static readonly Color SettingsPaper=new Color(.93f,.89f,.8f);
        static readonly string[] SettingsTabs={"조작","소리","화면 조작 버튼"};

        void SelectSettingsTab(int tab)
        {
            settingsTab=Mathf.Clamp(tab,0,2);notice="";Block();
        }
        bool UpdateSettingsInput(Keyboard keyboard)
        {
            if(!settings||capture>=0||keyboard==null||Time.unscaledTime<blockedUntil)return false;
            if(keyboard.tabKey.wasPressedThisFrame)
            {
                int direction=keyboard.leftShiftKey.isPressed||keyboard.rightShiftKey.isPressed?-1:1;
                SelectSettingsTab((settingsTab+direction+3)%3);return true;
            }
            if(settingsTab==1)
            {
                if(keyboard.leftArrowKey.wasPressedThisFrame){SetVolume(volume-.05f);return true;}
                if(keyboard.rightArrowKey.wasPressedThisFrame){SetVolume(volume+.05f);return true;}
            }
            return false;
        }
        void SettingsButton(string label,Rect rect,System.Action action,bool active=false)
        {
            bool hover=Mouse.current!=null&&rect.Contains(Point(Mouse.current.position.ReadValue()));
            Color border=active?HudLight:hover?HudGold:HudIron;
            GothicPlaque(rect,7,border);
            GothicPlaque(new Rect(rect.x+1,rect.y+1,rect.width-2,rect.height-2),6,
                active?new Color(.15f,.12f,.08f):hover?new Color(.085f,.095f,.12f):new Color(.035f,.045f,.064f));
            Text(label,rect.x+8,rect.y,rect.width-16,rect.height,18,action==null?SettingsMuted:active?HudLight:SettingsPaper,TextAnchor.MiddleCenter);
            if(action!=null)hits.Add(new Hit(rect,action));
        }
        void SettingsHeading(string titleText,string description)
        {
            Text(titleText,354,151,790,40,29,SettingsPaper);
            Text(description,354,194,790,27,15,SettingsMuted);
        }
        void DrawSettings()
        {
            hits.Clear();
            Box(new Rect(0,0,1280,720),new Color(.006f,.01f,.02f,.78f));
            GothicPlaque(new Rect(52,34,1176,652),18,HudGold);
            GothicPlaque(new Rect(54,36,1172,648),16,new Color(.022f,.031f,.048f,.98f));
            Text("환경 설정",86,61,650,51,35,SettingsPaper);
            Text(GameTitle+"  /  "+GameSubtitle,760,75,425,30,16,HudGold,TextAnchor.MiddleRight);
            GothicLine(new Vector2(86,126),new Vector2(1194,126),HudGold,1);
            GothicDiamond(640,126,4,HudLight);
            GothicLine(new Vector2(318,154),new Vector2(318,655),new Color(.22f,.23f,.24f),1);
            for(int i=0;i<SettingsTabs.Length;i++)
            {
                int tab=i;
                var rect=new Rect(84,169+i*65,207,52);
                SettingsButton(SettingsTabs[i],rect,()=>SelectSettingsTab(tab),settingsTab==tab);
            }
            Text("나의 여정에 맞게",87,414,202,28,18,SettingsPaper);
            Text("변경한 키와 음량은\n자동으로 저장됩니다.",87,451,202,56,16,SettingsMuted);
            Text("Tab · 항목 전환\nEsc · 닫기",87,529,202,52,15,SettingsMuted);
            if(settingsTab==0)
                SettingsButton("기본 키로 복원",new Rect(84,606,207,42),()=>{keys.Reset();notice="기본 조작 키로 복원했습니다.";Block();});
            else if(settingsTab==1)
                SettingsButton("기본 음량 · 18%",new Rect(84,606,207,42),()=>SetVolume(.18f));
            else
                Text("키보드와 화면 버튼을\n함께 사용할 수 있습니다.",87,603,202,46,14,SettingsMuted);

            if(settingsTab==0)DrawSettingsControls();
            else if(settingsTab==1)DrawSettingsAudio();
            else DrawSettingsTouch();
            GothicLine(new Vector2(354,621),new Vector2(1190,621),new Color(.22f,.23f,.24f),1);
            Text(notice.Length>0?notice:"설정은 변경 즉시 적용됩니다.",354,637,620,32,15,notice.Length>0?HudLight:SettingsMuted,TextAnchor.MiddleLeft);
            SettingsButton("완료 · Esc",new Rect(1000,634,190,38),()=>{settings=false;Block();},true);
            if(capture>=0)DrawSettingsKeyCapture();
        }
        void DrawSettingsControls()
        {
            SettingsHeading("키보드 조작","키를 선택한 뒤 새 키를 누르세요. Ctrl · Alt · Shift는 좌우 모두 동작합니다.");
            for(int i=0;i<keys.Names.Length;i++)
            {
                int action=i;
                float x=i<5?354:782,y=236+(i<5?i:i-5)*67;
                GothicPlaque(new Rect(x,y,408,57),6,new Color(.055f,.068f,.089f));
                Text(keys.Names[i],x+16,y,203,57,19,SettingsPaper,TextAnchor.MiddleLeft);
                SettingsButton(keys.Label(i),new Rect(x+226,y+7,167,43),()=>{
                    capture=action;captureFrame=Time.frameCount;notice="새 키를 누르세요. Esc로 취소할 수 있습니다.";
                });
            }
            Text("고정 키",800,510,365,23,15,HudLight);
            Text("Space 대화   ↑ 문 입장   ↓ + 점프 내려가기\nI 인벤토리   K 스킬창   Esc 일시정지",800,538,372,56,14,SettingsMuted);
            Text("사용 중인 키와 고정 기능 키는 지정할 수 없습니다.",354,583,420,26,14,SettingsMuted);
            if(DeveloperToolsEnabled)Text("DEV · F1~F7 이동 / F8 미구현 / F10 초기화",800,597,380,25,13,SettingsMuted);
        }
        void DrawSettingsAudio()
        {
            SettingsHeading("소리","배경 음악의 음량을 조절합니다. 변경한 음량을 바로 들을 수 있습니다.");
            GothicPlaque(new Rect(354,241,836,250),12,new Color(.049f,.06f,.081f));
            Text("배경 음악",386,267,450,43,24,SettingsPaper);
            Text(Mathf.RoundToInt(volume*100)+"%",1000,260,155,52,36,HudLight,TextAnchor.MiddleRight);
            var track=new Rect(392,345,760,16);
            GothicPlaque(new Rect(track.x-4,track.y-4,track.width+8,24),6,HudGold);
            Box(track,HudInk);
            if(volume>0)Box(new Rect(track.x,track.y,track.width*volume,track.height),new Color(.67f,.49f,.25f));
            GothicDiamond(track.x+track.width*volume,track.center.y,10,HudLight);
            // Large hit strip offers 5% steps for mouse and touch alike.
            for(int i=0;i<=20;i++)
            {
                float value=i/20f;
                float x=track.x+track.width*(i-.5f)/20;
                hits.Add(new Hit(new Rect(Mathf.Max(track.x-12,x),325,i==0||i==20?31:38,56),()=>SetVolume(value)));
            }
            Text("0",386,382,90,24,14,SettingsMuted);
            Text("100",1065,382,90,24,14,SettingsMuted,TextAnchor.MiddleRight);
            SettingsButton("− 5%",new Rect(557,425,150,44),()=>SetVolume(volume-.05f));
            SettingsButton("+ 5%",new Rect(731,425,150,44),()=>SetVolume(volume+.05f));
            SettingsButton("음소거",new Rect(386,425,145,44),()=>SetVolume(0),volume<=0);
            SettingsButton("최대 음량",new Rect(907,425,245,44),()=>SetVolume(1),volume>=1);
            Text("게이지를 눌러 5% 단위로 조절할 수 있습니다.",386,520,760,30,18,SettingsPaper);
            Text("← / → · 음량 조절     0% · 음악 끄기",386,562,760,28,16,SettingsMuted);
        }
        void DrawSettingsTouch()
        {
            SettingsHeading("화면 조작 버튼","이동·공격·점프 버튼을 화면에 표시하여 터치나 마우스로 조작합니다.");
            bool enabled=TouchControls;
            GothicPlaque(new Rect(354,241,836,108),12,new Color(.049f,.06f,.081f));
            Text("화면 버튼 표시",380,258,500,32,23,SettingsPaper);
            Text(Application.isMobilePlatform?"모바일에서는 항상 표시됩니다.":"키보드 조작도 계속 사용할 수 있습니다.",380,301,520,27,16,SettingsMuted);
            SettingsButton(enabled?"켜짐":"꺼짐",new Rect(1012,265,150,52),Application.isMobilePlatform?null:(System.Action)(()=>{
                touchPreview=!touchPreview;
                PlayerPrefs.SetInt("sunfall.touchPreview",touchPreview?1:0);PlayerPrefs.Save();
                notice=touchPreview?"화면 조작 버튼을 켰습니다.":"화면 조작 버튼을 껐습니다.";Block();
            }),enabled);
            Text("버튼 배치 안내",380,379,700,30,20,HudLight);
            GothicPlaque(new Rect(380,424,784,112),9,new Color(.015f,.023f,.038f));
            SettingsPreviewKey("←",400,464,55,enabled);
            SettingsPreviewKey("→",463,464,55,enabled);
            SettingsPreviewKey("구르기",541,464,86,enabled);
            SettingsPreviewKey("스킬 1–4",649,464,159,enabled);
            SettingsPreviewKey("공격",918,464,92,enabled);
            SettingsPreviewKey("점프",1022,464,122,enabled);
            Text("실제 버튼은 마을과 전투 화면에서 표시됩니다.",380,559,784,30,16,SettingsMuted);
        }
        void SettingsPreviewKey(string label,float x,float y,float width,bool enabled)
        {
            GothicPlaque(new Rect(x,y,width,48),6,enabled?HudGold:HudIron);
            GothicPlaque(new Rect(x+1,y+1,width-2,46),5,HudInk);
            Text(label,x,y,width,48,16,enabled?HudLight:SettingsMuted,TextAnchor.MiddleCenter);
        }
        void DrawSettingsKeyCapture()
        {
            // Modal owns all clicks; nothing behind it can reset keys or close settings.
            hits.Clear();
            Box(new Rect(0,0,1280,720),new Color(.003f,.006f,.014f,.85f));
            GothicPlaque(new Rect(350,212,580,296),15,HudGold);
            GothicPlaque(new Rect(352,214,576,292),13,HudInk);
            Text("키 변경",380,235,520,39,28,SettingsPaper,TextAnchor.MiddleCenter);
            Text(keys.Names[capture],380,286,520,37,23,HudLight,TextAnchor.MiddleCenter);
            Text("현재 "+keys.Label(capture)+"  →  새 키를 누르세요",380,335,520,32,19,SettingsPaper,TextAnchor.MiddleCenter);
            Text(notice,380,379,520,57,16,SettingsMuted,TextAnchor.MiddleCenter);
            SettingsButton("취소 · Esc",new Rect(530,449,220,40),()=>{capture=-1;notice="키 변경을 취소했습니다.";Block();});
        }
    }
}

