using UnityEngine;
using UnityEngine.InputSystem;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        static bool DeveloperToolsEnabled
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                return true;
#else
                return false;
#endif
            }
        }
        float developerResetNoticeUntil;
        float developerJumpNoticeUntil;
        string developerJumpNotice="";
        bool UpdateDeveloperChapterInput(Keyboard keyboard)
        {
            if(!DeveloperToolsEnabled||capture>=0||keyboard==null)return false;
            for(int chapter=1;chapter<=8;chapter++)
            {
                if(!keyboard[(Key)((int)Key.F1+chapter-1)].wasPressedThisFrame)continue;
                if(chapter>RunModel.FinalChapter)
                {
                    developerJumpNotice=$"DEV · {chapter}스테이지 미구현 · 현재 {RunModel.FinalChapter}스테이지까지 이동 가능";
                    developerJumpNoticeUntil=Time.unscaledTime+5;
                    return true;
                }
                JumpToDeveloperChapter(chapter);return true;
            }
            return false;
        }
        void JumpToDeveloperChapter(int chapter)
        {
            if(!DeveloperToolsEnabled||chapter<1||chapter>RunModel.FinalChapter)return;
            // Do not mark an interrupted story as watched or grant skipped boss loot.
            run=RunModel.CreateDeveloperChapter(chapter,run==null?nextSeed:run.Seed,playerEquipment,playerProgression,run);
            storyLines=null;storyPage=0;storyHeading="";
            title=village=paused=settings=inventoryOpen=skillWindowOpen=growthTab=false;
            serviceNpc=-1;dialoguePage=-1;capture=-1;
            notice=serviceNotice=skillWindowNotice=equipmentNotice="";
            bossArrivalStoryChapter=bossDefeatStoryChapter=0;
            openingSceneTime=openingTransitionTime=0;
            openingTransition=openingFinish=openingSubtitleRevealed=false;
            cameraX=cameraY=0;defeatCameraStart=Vector2.zero;
            hits.Clear();ResetHeroVisuals();Block();
            developerJumpNotice=$"DEV · {chapter}스테이지 첫 방 · 체력/쿨타임 회복 · 장비/성장/획득 스킬 유지";
            developerJumpNoticeUntil=Time.unscaledTime+5;
        }
        void DrawDeveloperJumpNotice()
        {
            if(!DeveloperToolsEnabled||Time.unscaledTime>=developerJumpNoticeUntil)return;
            GothicPlaque(new Rect(265,129,750,34),8,HudInk);
            Text(developerJumpNotice,275,132,730,28,15,HudLight,TextAnchor.MiddleCenter);
        }

        void ResetDeveloperProgress()
        {
            if(!DeveloperToolsEnabled)return;
            developerJumpNoticeUntil=0;
            // Detach the old journey before replacing its shared progression objects.
            run=null;storyLines=null;storyPage=0;storyHeading="";
            title=true;village=false;paused=settings=inventoryOpen=skillWindowOpen=growthTab=false;
            capture=-1;captureFrame=0;serviceNpc=-1;dialoguePage=-1;selectedVillageNpc=2;
            notice=serviceNotice=skillWindowNotice="";
            selectedSkill=SpellKind.EmberWave;titleSelection=0;
            bossArrivalStoryChapter=bossDefeatStoryChapter=0;
            openingSceneTime=openingTransitionTime=0;
            openingTransition=openingFinish=openingSubtitleRevealed=false;
            villageX=370;villageFacing=1;cameraX=cameraY=0;nextSeed=12345;
            hits.Clear();ResetHeroVisuals();Block();

            // Write only the game's progression keys; retain audio and control settings.
            // Saving the defaults also prevents old progress returning on the next launch.
            playerEquipment=new EquipmentInventory();
            playerProgression=new PlayerProgression();
            ResetStoryProgress();
            SaveEquipment();
            developerResetNoticeUntil=Time.unscaledTime+7;
        }
        void DrawDeveloperTitleStatus()
        {
            if(!DeveloperToolsEnabled)return;
            Text("DEV · F1~F7 스테이지 이동 · F8 미구현 · F10 초기화",26,18,600,23,12,new Color(.67f,.62f,.51f));
            if(Time.unscaledTime<developerResetNoticeUntil)
                Text("초기화 완료 · Lv.1 / 0 G / 장비·시청 기록 초기화",320,28,640,24,14,new Color(.65f,.92f,.78f),TextAnchor.MiddleCenter);
        }
    }
}
