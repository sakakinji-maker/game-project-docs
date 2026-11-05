using UnityEngine;
using UnityEngine.InputSystem;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        const float GuideX=490f;
        const float VillageExitX=1120f;
        bool CanLeaveVillage=>CanVillageInteract&&dialoguePage<0&&Mathf.Abs(villageX-VillageExitX)<=75;
        bool village;
        float villageX;
        Texture2D villageBackdrop;
        float nextVillageBackdropRetry;
        int villageFacing=1,dialoguePage=-1;
        readonly StoryLine[] guideDialogue={
            new StoryLine(StoryVoice.Arin,"어머니가 네 정체를 알고도 널 지켜주셨던 것처럼, 나도 네 편이야. 네 손에 남은 별실이 아직 따뜻해. 그분은 살아 계셔.",StoryExpression.Warm),
            new StoryLine(StoryVoice.Hero,"어머니의 목소리가 잠깐 들렸어. 하지만 별실을 당겨도 돌아오시지 않아.",StoryExpression.Sad),
            new StoryLine(StoryVoice.Arin,"성소의 붉은 날개가 실을 삼키고 있어. 숲과 무너진 정원, 악마의 주둔지를 지나 성소로 가. 그곳에서 얽힌 실을 풀어야 해.",StoryExpression.Determined),
            new StoryLine(StoryVoice.Arin,"악마가 붙어 있는 동안에는 균열에 접근할 수 없어. 적을 정리하고 열린 문 앞에서 ↑를 눌러. 발판의 궁수와 성소의 붉은 돌진을 조심해.",StoryExpression.Worried),
            new StoryLine(StoryVoice.Arin,"네가 길에서 쌓은 경험은 사라지지 않아. 마을에서 I를 열고 능력치를 배분해. 나는 여기서 등불을 켜고, 돌아오는 사람들을 맞을게.",StoryExpression.Warm),
            new StoryLine(StoryVoice.Hero,"어머니가 돌아오시면, 집 앞 등불부터 보실 거야. 그 불을 부탁할게, 아린.",StoryExpression.Warm)
        };

        void EnterVillage()
        {
            storyLines=null;
            skillWindowOpen=false;title=false;village=true;run=RunModel.CreateVillagePractice(playerEquipment,playerProgression,run);cameraX=cameraY=0;paused=false;settings=false;inventoryOpen=false;serviceNpc=-1;
            villageX=370f;villageFacing=1;dialoguePage=-1;selectedVillageNpc=2;ResetHeroVisuals();LoadVillageNpcArt();LoadVillageBackdrop();Block();
        }

        void LoadVillageBackdrop()
        {
            villageBackdrop=Resources.Load<Texture2D>("World/EmberVillageV1");
            if(villageBackdrop!=null)
            {villageBackdrop.filterMode=FilterMode.Point;villageBackdrop.wrapMode=TextureWrapMode.Clamp;}
            nextVillageBackdropRetry=Time.realtimeSinceStartup+1;
        }

        void DrawVillageBackdrop()
        {
            if(villageBackdrop==null&&Time.realtimeSinceStartup>=nextVillageBackdropRetry)LoadVillageBackdrop();
            if(villageBackdrop==null)return;
            // The image's horizontal lane is at 75% height, matching the shared Y=540 feet.
            // Keep it fixed: parallax would shift buildings away from their resident NPCs.
            GUI.color=Color.white;
            GUI.DrawTexture(new Rect(0,0,1280,720),villageBackdrop,ScaleMode.StretchToFill,false);
            Box(new Rect(0,0,1280,142),new Color(.025f,.045f,.065f,.78f));
            Box(new Rect(0,578,1280,142),new Color(.025f,.04f,.06f,.62f));
        }

        void UpdateVillage()
        {
            if(settings||paused||Time.unscaledTime<blockedUntil){accumulator=0;return;}
            if(Keyboard.current!=null&&Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if(dialoguePage>=0)AdvanceVillageDialogue();
                else
                {
                    int nearest=NearestVillageNpc();
                    if(nearest>=0)BeginVillageDialogue(nearest);
                }
                return;
            }
            if(dialoguePage>=0)return;
            if(Keyboard.current!=null&&Keyboard.current.upArrowKey.wasPressedThisFrame&&CanLeaveVillage)
            {StartRun();return;}
            UpdateVillageActions();
        }

        int NearestVillageNpc()
        {
            if(!CanVillageInteract)return -1;
            int nearest=-1;float distance=145f;
            for(int i=0;i<villageNpcs.Length;i++)
            {
                float candidate=Mathf.Abs(villageX-villageNpcs[i].X);
                if(candidate<=distance){distance=candidate;nearest=i;}
            }
            return nearest;
        }
        void BeginVillageDialogue(int index)
        {
            if(!CanVillageInteract||dialoguePage>=0||index<0||index>=villageNpcs.Length||Mathf.Abs(villageX-villageNpcs[index].X)>145)return;
            if(index<2){serviceNpc=index;selectedVillageNpc=index;dialoguePage=-1;serviceNotice="";serviceExpression=index==0?StoryExpression.Determined:StoryExpression.Neutral;TickHeroVisuals(0,0);Block();return;}
            selectedVillageNpc=index;dialoguePage=0;
            TickHeroVisuals(0,0);Block();
        }
        void AdvanceVillageDialogue()
        {
            dialoguePage=dialoguePage+1<VillageDialogue.Length?dialoguePage+1:-1;Block();
        }

        void DrawVillage()
        {
            DrawVillageBackdrop();
            Color amber=new Color(.94f,.72f,.37f);
            Text("SUNFALL  /  불씨 마을",40,30,700,50,30,amber);
            Text("상인에게 구매 · 대장장이에게 강화 · I 능력치 배분",40,90,560,45,19);
            DrawProgressionStatus(610,90,470);

            // Keep a visible fallback while Unity imports the background resource.
            if(villageBackdrop==null)
            {
                for(int i=0;i<3;i++)
                {
                    float x=130+i*280;
                    Box(new Rect(x,330,175,210),new Color(.1f,.16f,.2f));
                    Box(new Rect(x-14,315,203,22),new Color(.22f,.28f,.3f));
                    Box(new Rect(x+65,450,48,90),new Color(.04f,.08f,.11f));
                    Box(new Rect(x+24,370,36,44),new Color(.7f,.46f,.2f));
                    Box(new Rect(x+22,389,40,5),new Color(.22f,.23f,.21f));
                }
                Box(new Rect(70,540,1140,12),new Color(.28f,.34f,.34f));
                Box(new Rect(70,552,1140,15),new Color(.065f,.11f,.15f));
                for(int i=0;i<18;i++)Box(new Rect(80+i*64,545,48,2),new Color(.45f,.42f,.31f));
            }

            DrawVillageWorkplaces();
            DrawGothicPortal(VillageExitX,540,true,180);
            DrawGothicTorch(1040,540,false,87);DrawGothicTorch(1200,540,false,87);
            for(int npc=0;npc<villageNpcs.Length;npc++)DrawVillageNpc(npc);

            DrawVillageActions();
            DrawNameplateText(CanLeaveVillage?"↑ 숲으로 입장":"숲으로 가는 포탈",VillageExitX,316,240,32,22,amber);

            for(int npc=0;npc<villageNpcs.Length;npc++)DrawVillageNpcNameplate(npc);
            DrawVillageActionBar();

            if(dialoguePage>=0)DrawVillageDialogue();
        }

        void DrawVillageDialogue()
        {
            // Dialogue owns input; clicks cannot reach the departure button behind it.
            hits.Clear();
            Box(new Rect(0,0,1280,720),new Color(0,0,0,.35f));
            Box(new Rect(110,365,1060,305),new Color(.06f,.11f,.16f));
            Box(new Rect(110,365,5,305),new Color(.94f,.72f,.37f));
            var lines=VillageDialogue;
            dialoguePage=Mathf.Clamp(dialoguePage,0,lines.Length-1);
            var line=lines[dialoguePage];
            DrawStoryPortrait(line.Voice,new Rect(132,382,215,267),line.Expression);
            Text(StoryVoiceName(line.Voice),375,390,750,40,28,new Color(.94f,.72f,.37f));
            Text(line.Words,375,449,750,125,23);
            Text($"{dialoguePage+1} / {lines.Length}",375,600,140,40,20);
            Button("닫기 (Esc)",new Rect(650,590,200,55),()=>{dialoguePage=-1;Block();});
            Button(dialoguePage==lines.Length-1?"마치기 (Space)":"다음 (Space)",new Rect(875,590,250,55),AdvanceVillageDialogue,true);
        }
    }
}



