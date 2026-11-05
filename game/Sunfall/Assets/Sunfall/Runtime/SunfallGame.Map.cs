using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        float cameraX,cameraY,nextMapArtRetry;
        Texture2D arenaBackdrop,forestBackdrop,ruinPlatform;
        static readonly string[] RoomTitles={"숲 입구","무너진 정원","악마 주둔지","공중 보스 성소"};
        static readonly string[] ChapterTwoTitles={"잿빛 성문","쇠사슬 회랑","검은 성채","왕실 사슬의 제단"};
        static readonly string[] ChapterThreeTitles={"대성당 진입로","왕실 회랑","태양 봉인 예배당","태양 봉인의 내전"};
        static readonly string[] ChapterFourTitles={"대성당 뒤편 요새길","역광의 왕실 회랑","심장 봉인 회랑","왕관의 결투장"};
        static readonly string[][] ChapterFourLandmarks={
            new[]{"대성당에서 이어진 다리","왕관의 내부 수비대","왕실 회랑으로"},
            new[]{"거꾸로 흐르는 태양빛","별실이 지나는 회랑","심장 봉인 회랑으로"},
            new[]{"어머니가 버티는 마지막 매듭","왕관의 심장","심장지기의 결투장으로"},
            new[]{"멈추지 않는 왕의 명령","바르칸의 심장 장치","거울 성역으로"}
        };
        static readonly string[][] ChapterThreeLandmarks={
            new[]{"왕도에 남은 별실","대성당의 수비대","회랑으로 가는 문"},
            new[]{"빛을 잃은 유리창","왕실의 긴 회랑","태양 봉인 예배당으로"},
            new[]{"양어머니의 심장 소리","태양을 묶은 왕실 제단","집행관의 내전으로"},
            new[]{"별실이 닿는 마지막 문","아우렐이 지키는 태양 봉인","왕관의 심장부로 가는 포탈"}
        };
        static readonly string[][] ChapterTwoLandmarks={
            new[]{"성소 너머의 잿빛 길","성문의 수비대","회랑으로 가는 문"},
            new[]{"늘어진 쇠사슬","상층 궁수의 매복","성채로 가는 문"},
            new[]{"검은 성채의 외벽","악마의 마지막 방어선","간수의 제단으로"},
            new[]{"왕실의 봉인","양어머니를 붙잡은 사슬","왕도 대성당으로 가는 포탈"}
        };
        static readonly string[][] RoomLandmarks={
            new[]{"불씨 마을의 마지막 등불","꺼진 태양의 길","정원으로 가는 문"},
            new[]{"무너진 아치","궁수가 지키는 정원","주둔지로 가는 문"},
            new[]{"악마의 망루","중갑병의 방어선","성소 앞 · 숨 고르기"},
            new[]{"날개의 제단","붉은 날개의 결투장","2스테이지로 가는 문"}
        };
        string[] ActiveRoomTitles=>run.Chapter>=5?LateRoomTitles[run.Chapter-5]:run.Chapter==4?ChapterFourTitles:run.Chapter==3?ChapterThreeTitles:run.Chapter==2?ChapterTwoTitles:RoomTitles;
        string[][] ActiveLandmarks=>run.Chapter>=5?LateLandmarks[run.Chapter-5]:run.Chapter==4?ChapterFourLandmarks:run.Chapter==3?ChapterThreeLandmarks:run.Chapter==2?ChapterTwoLandmarks:RoomLandmarks;
        string RoomTitle=>ActiveRoomTitles[run.RoomIndex];
        string NextRoomTitle=>run.RoomIndex+1<ActiveRoomTitles.Length?ActiveRoomTitles[run.RoomIndex+1]:run.Chapter==RunModel.FinalChapter?"어머니와 귀환 · 결말":(run.Chapter+1)+"스테이지 · "+RunModel.ChapterName(run.Chapter+1);
        Color RoomAccent=>run.Chapter>=5?LateColor(run.Chapter):run.Chapter==4?new Color(.95f,.58f,.4f):run.Chapter==3?new Color(.86f,.72f,.45f):run.Chapter==2?new Color(.55f,.75f,.67f):run.RoomIndex==0?new Color(.63f,.8f,.68f):run.RoomIndex==1?new Color(.48f,.8f,.83f):new Color(.88f,.48f,.54f);
        void LoadMapArt()
        {
            LoadWorldProps();LoadCryptArt();LoadForestProps();LoadCathedralArt();LoadCrownArt();LoadMirrorArt();LoadEclipseArt();LoadThroneArt();
            arenaBackdrop=Resources.Load<Texture2D>("World/AerialSanctum");
            forestBackdrop=Resources.Load<Texture2D>("World/SunlessForest");
            ruinPlatform=Resources.Load<Texture2D>("World/RuinPlatform");
            foreach(var texture in new[]{arenaBackdrop,forestBackdrop,ruinPlatform})
                if(texture!=null){texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;}
            nextMapArtRetry=Time.realtimeSinceStartup+1;
        }
        void UpdateMapCamera(float dt)
        {
            // Keep movement ahead visible; vertical follow leaves room above each jump.
            var position=RenderPosition(run,run.X,run.Y);
            float targetX=Mathf.Clamp(position.x-14,0,RunModel.WorldWidth-32);
            float targetY=Mathf.Clamp(position.y-3,0,6);
            if(run.BossDissolving)
            {
                float elapsed=BossDefeatElapsed;
                float focusX=Mathf.Clamp(run.BossDefeatX-(640-90)/34f,0,RunModel.WorldWidth-32);
                float seamFocus=run.Chapter>=2?RunModel.BossSpawnX-(640-90)/34f:(1035-640)/((2070f-1280)/(RunModel.WorldWidth-32));
                focusX=Mathf.Lerp(focusX,seamFocus,Mathf.SmoothStep(0,1,Mathf.Clamp01((elapsed-.5f)/.45f)));
                float approach=Mathf.SmoothStep(0,1,Mathf.Clamp01(elapsed/.45f));
                float returning=Mathf.SmoothStep(0,1,Mathf.Clamp01((elapsed-(RunModel.BossDefeatDuration-.6f))/.6f));
                cameraX=Mathf.Lerp(Mathf.Lerp(defeatCameraStart.x,focusX,approach),targetX,returning);
                cameraY=Mathf.Lerp(Mathf.Lerp(defeatCameraStart.y,0,approach),targetY,returning);return;
            }
            if(run.BossIntroducing)
            {
                float elapsed=BossIntroElapsed;
                float focusX=Mathf.Clamp(RunModel.BossSpawnX-(640-90)/34f,0,RunModel.WorldWidth-32);
                float focus=Mathf.SmoothStep(0,1,Mathf.Clamp01(elapsed/.65f));
                focus*=Mathf.SmoothStep(0,1,Mathf.Clamp01((RunModel.BossIntroDuration-elapsed)/.85f));
                cameraX=Mathf.Lerp(targetX,focusX,focus);cameraY=0;
                return;
            }
            float blend=1-Mathf.Exp(-7*dt);
            cameraX=Mathf.Lerp(cameraX,targetX,blend);cameraY=Mathf.Lerp(cameraY,targetY,blend);
        }
        void DrawMapTerrain()
        {
            if((arenaBackdrop==null||forestBackdrop==null||ruinPlatform==null)&&Time.realtimeSinceStartup>=nextMapArtRetry)LoadMapArt();
            if(run.Chapter==2)DrawCryptBackdrop();
            if(run.Chapter==3)DrawCathedralBackdrop();
            if(run.Chapter==4)DrawCrownBackdrop();
            if(run.Chapter==5)DrawMirrorBackdrop();
            if(run.Chapter==6)DrawEclipseBackdrop();
            if(run.Chapter==7)DrawThroneBackdrop();
            var backdrop=run.Chapter>=2&&run.Chapter<=7?null:run.Chapter>=5||run.Stage==RunStage.BossArena?arenaBackdrop:forestBackdrop;
            if(backdrop!=null)
            {
                // A single panorama moves more slowly than the walkable world: no tiling seams.
                float width=2070,offset=(cameraX/(RunModel.WorldWidth-32))*(width-1280);
                GUI.color=run.Chapter==4?new Color(.5f,.32f,.4f):run.Chapter==3?new Color(.57f,.55f,.62f):run.Chapter==2?new Color(.64f,.59f,.79f):run.RoomIndex==2?new Color(.82f,.72f,.83f):run.RoomIndex==1?new Color(.83f,.95f,1):Color.white;
                GUI.DrawTexture(new Rect(-offset,-105+cameraY*8,width,690),backdrop,ScaleMode.StretchToFill,true);
                GUI.color=Color.white;
            }
            if(run.Chapter<3)Box(new Rect(0,130,1280,450),run.Chapter==2?new Color(.015f,.025f,.06f,.05f):run.RoomIndex>=2?new Color(.12f,.015f,.06f,.18f):new Color(.015f,.035f,.055f,.13f));
            DrawSkyRift();
            DrawRoomLandmarks();
            // Solid ground remains continuous. Only visible segments are submitted.
            for(float start=0;start<RunModel.WorldWidth;start+=12)
                DrawTerrainSegment(start,Mathf.Min(12,RunModel.WorldWidth-start),0,true);
            foreach(var platform in run.Platforms)DrawTerrainSegment(platform.X,platform.Width,platform.Y,false);
            for(int i=0;i<3;i++)
            {
                float x=SX(i==0?3:i==1?28:56),y=SY(0);
                if(x<-100||x>1380)continue;
                DrawGothicTorch(x,y,(run.Chapter>=2||run.RoomIndex>=2)&&i>0);
                Text(ActiveLandmarks[run.RoomIndex][i]+" →",x-50,y-117,250,30,15,RoomAccent);
            }
            float gateX=SX(RunModel.ExitX),gateY=SY(0);
            if(gateX>-150&&gateX<1430)
            {
                Color glow=run.ExitOpen?new Color(.43f,.95f,.86f):new Color(.55f,.3f,.4f);
                DrawGothicPortal(gateX,gateY,run.ExitOpen);
                string label=run.ExitOpen?NextRoomTitle+" · ↑":run.Stage==RunStage.Approach?$"몬스터 {run.RemainingMonsters}마리 남음":"봉인된 출구";
                Text(label,gateX-125,gateY-225,250,52,18,glow,TextAnchor.MiddleCenter);
                if(run.CanUseExit)DrawNameplateText("↑ 눌러 이동",gateX,gateY-132,190,30,20,Color.white);
            }
        }
        void DrawTerrainSegment(float worldX,float width,float y,bool ground)
        {
            float x=SX(worldX),top=SY(y),pixels=width*34;
            if(x>1300||x+pixels<-20||top>650||top<110)return;
            if(ground)Box(new Rect(x,top+20,pixels,Mathf.Max(0,600-top)),new Color(.025f,.055f,.075f));
            if(run.Chapter==2)DrawCryptStone(x,top,pixels,ground);
            else if(run.Chapter>=5)DrawRoyalCathedralStone(x,top,pixels,ground);
            else if(run.Chapter==4)DrawCrownStone(x,top,pixels,ground);
            else if(run.Chapter==3)DrawRoyalCathedralStone(x,top,pixels,ground);
            else if(ruinPlatform!=null)
            {
                // The source strip includes short grass above a flat stone surface.
                var uv=new Rect(16f/2172,1-556f/724,2140f/2172,306f/724);
                GUI.color=run.Chapter==3?new Color(.72f,.68f,.61f):run.Chapter==2?new Color(.68f,.62f,.77f):run.RoomIndex>=2?new Color(.85f,.73f,.79f):Color.white;
                GUI.DrawTextureWithTexCoords(new Rect(x,top-10,pixels,ground?83:61),ruinPlatform,uv,true);
                GUI.color=Color.white;
            }
            else Box(new Rect(x,top,pixels,ground?60:35),new Color(.16f,.28f,.3f));
            // Thin collision-edge highlight stays readable through grass and effects.
            // Authored royal stone edges provide the collision outline in late chapters.
        }
        // Decorative silhouettes stay behind terrain and actors; only Platforms collide.
        void DrawRoomLandmarks()
        {
            if(run.Chapter>=5)DrawLateLandmarks();
            else if(run.Chapter==4)DrawCrownHeartLandmarks();
            else if(run.Chapter==3)DrawCathedralLandmarks();
            else if(run.Chapter==2)DrawChapterTwoLandmarks();
            else if(run.RoomIndex==0)
            {
                DrawSunSeal(23,3.4f,38,false);
                for(int i=0;i<7;i++)
                {
                    float x=SX(7+i*7),y=SY(0);
                    if(x<-70||x>1350)continue;
                    DrawForestProp(3,x,y,32+i%3*5);
                    if(i%2==0)DrawWorldLantern(7+i*7,0,false);
                }
            }
            else if(run.RoomIndex==1)
            {
                DrawBrokenArch(18,7.8f);
                DrawBrokenArch(47,8.7f);
                DrawSunSeal(30,3.7f,31,false);
                // Collapsed masonry and dry planters anchor the garden's lower path.
                for(int i=0;i<6;i++)
                {
                    float x=SX(7+i*9),y=SY(0);
                    if(x<-100||x>1380)continue;
                    DrawForestProp(i%3==2?3:4,x,y,i%3==2?36:52);
                }
            }
            else if(run.RoomIndex==2)
            {
                DrawWatchtower(18);DrawWatchtower(47);
                DrawSunSeal(31,4,42,true);
                for(int i=0;i<4;i++)
                {
                    float x=SX(11+i*12),y=SY(0);
                    if(x<-100||x>1380)continue;
                    DrawForestProp(5,x,y,42+i%2*6);
                }
                DrawWorldLantern(57,0,false);
                DrawWorldLantern(59,0,false);
            }
            else
            {
                DrawBrokenArch(32,10.5f);
                DrawSunSeal(32,6.8f,100,true);
                for(int i=0;i<4;i++)
                {
                    float x=SX(14+i*12);
                    if(x>=-20&&x<=1300)
                        DrawAnimatedChain(new Vector2(x,SY(11)),new Vector2(x,SY(11)+108),.65f,1,i,false);
                }
                DrawWorldLantern(7,0,true);DrawWorldLantern(57,0,true);
            }
            // Small drifting embers connect the warm village to the corrupted sanctuary.
            for(int i=0;i<18;i++)
            {
                float x=SX(3+i*3.4f)+Mathf.Sin(visualClock*.4f+i)*7;
                float y=SY(1+(i*7%17)*.35f)-Mathf.Repeat(visualClock*5+i*11,40);
                DrawAnimatedSpark(new Vector2(x,y),3,run.RoomIndex>=2?new Color(.9f,.35f,.45f,.5f):new Color(.85f,.7f,.43f,.5f));
            }
        }
        void DrawChapterTwoLandmarks()
        {
            // The new room artwork contains the arches, pillars and prison walls.
            // Do not cover it with the former procedural towers and flat shapes.
            DrawCryptSeal();
        }
        void DrawBrokenArch(float worldX,float height)
        {
            DrawForestProp(0,SX(worldX),SY(0),height*34);
        }
        void DrawWatchtower(float worldX)
        {
            DrawForestProp(1,SX(worldX),SY(0),7.5f*34);
        }
        void DrawSunSeal(float worldX,float worldY,float radius,bool corrupted)
        {
            var tint=corrupted?new Color(.88f,.57f,.66f,.85f):new Color(.75f,.86f,.85f,.8f);
            DrawForestProp(2,SX(worldX),SY(worldY)+radius,radius*2,tint);
        }
        void DrawWorldLantern(float worldX,float worldY,bool crimson)
        {
            DrawGothicTorch(SX(worldX),SY(worldY),crimson);
        }
        void DrawMinimap()
        {
            const float x=900,y=42,w=342,h=70,verticalScale=5.6f;
            Box(new Rect(x-12,16,w+24,108),new Color(.045f,.085f,.11f,.96f));
            Text("지역 지도",x,21,130,19,12,new Color(.62f,.74f,.74f));
            // Show the camera's visible world span without adding another border.
            float viewStart=Mathf.Clamp01((cameraX-90/34f)/RunModel.WorldWidth);
            float viewEnd=Mathf.Clamp01((cameraX+(1280-90)/34f)/RunModel.WorldWidth);
            Box(new Rect(x+viewStart*w,y,(viewEnd-viewStart)*w,h),new Color(.36f,.57f,.59f,.12f));
            float baseline=y+h-4;
            Box(new Rect(x,baseline,w,2),new Color(.3f,.5f,.48f));
            foreach(var p in run.Platforms)
                Box(new Rect(x+p.X/RunModel.WorldWidth*w,baseline-p.Y*verticalScale,p.Width/RunModel.WorldWidth*w,3),new Color(.39f,.67f,.6f));
            foreach(var enemy in run.Enemies)
                if(enemy.Hp>0)
                {
                    float markerSize=enemy.Boss?7:4;
                    float markerY=Mathf.Max(y,baseline-4-enemy.Y*verticalScale);
                    Box(new Rect(x+enemy.X/RunModel.WorldWidth*w-markerSize*.5f,markerY,markerSize,4),
                        enemy.Boss?new Color(1,.6f,.3f):new Color(.85f,.38f,.46f));
                }
            Box(new Rect(x+RunModel.ExitX/RunModel.WorldWidth*w-2,baseline-10,5,10),run.ExitOpen?Color.cyan:Color.gray);
            var player=RenderPosition(run,run.X,run.Y);
            Box(new Rect(x+player.x/RunModel.WorldWidth*w-3,Mathf.Max(y,baseline-8-player.y*verticalScale),6,8),Color.white);
        }
    }
}

