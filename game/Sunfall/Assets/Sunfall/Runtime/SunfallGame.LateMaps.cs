using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        static readonly string[][] LateRoomTitles={
            new[]{"거울 정원","기억의 회랑","뒤집힌 예배당","거울지기의 성역"},
            new[]{"밤의 기단","일식의 나선 계단","부서진 종루","월식의 정상"},
            new[]{"빛을 잃은 왕궁","왕의 장례 회랑","검은 태양의 문","꺼진 태양의 왕좌"}
        };
        static readonly string[][][] LateLandmarks={
            new[]{new[]{"거울에 남은 별실","기억을 지키는 악마","기억의 회랑으로"},new[]{"어머니의 기억 조각","마녀를 지운 왕의 기록","뒤집힌 예배당으로"},new[]{"거짓을 비추는 창","마지막 거울 수비대","리세아의 성역으로"},new[]{"진실을 가둔 성역","되찾을 어머니의 이름","일식의 첨탑으로"}},
            new[]{new[]{"별빛 없는 밤","첨탑을 지키는 악마","나선 계단으로"},new[]{"위로 이어진 별실","일식의 파수꾼","부서진 종루로"},new[]{"울리지 않는 종","왕좌 앞 마지막 날개","월식의 정상으로"},new[]{"일식의 장막","어머니에게 닿는 하늘","꺼진 태양의 왕좌로"}},
            new[]{new[]{"영원한 낮을 원했던 왕궁","금빛에 갇힌 악마","장례 회랑으로"},new[]{"왕의 이름 없는 묘비","왕실의 마지막 군대","검은 태양의 문으로"},new[]{"문 너머의 심장 소리","돌아올 길을 남기는 별실","왕과의 결전으로"},new[]{"마지막 별실","태양을 삼킨 왕","어머니와 함께 귀환"}}
        };
        void DrawLateLandmarks()
        {
            float floor=SY(0),roof=SY(12);var accent=LateColor(run.Chapter);
            Box(new Rect(0,roof,1280,floor-roof),new Color(.025f,.02f,.055f,.92f));
            for(int bay=0;bay<8;bay++)
            {
                float x=SX(3+bay*9);if(x<-220||x>1500)continue;
                Box(new Rect(x-22,roof+45,44,floor-roof-45),new Color(.12f,.11f,.17f));
                GothicPolygon(new Color(.25f,.22f,.31f),new Vector2(x-33,roof+48),new Vector2(x,roof),new Vector2(x+33,roof+48));
                GothicLine(new Vector2(x-17,roof+55),new Vector2(x-17,floor),new Color(accent.r,accent.g,accent.b,.28f),3);
                float wx=x+115,wy=SY(6.5f);
                if(run.Chapter==5)
                {
                    // Reflective pointed panes; cracks grow toward the inner sanctum.
                    GothicPolygon(new Color(.32f,.24f,.42f),new Vector2(wx-67,wy+95),new Vector2(wx-67,wy-50),new Vector2(wx,wy-115),new Vector2(wx+67,wy-50),new Vector2(wx+67,wy+95));
                    GothicPolygon(new Color(.1f,.16f,.25f),new Vector2(wx-58,wy+86),new Vector2(wx-58,wy-46),new Vector2(wx,wy-101),new Vector2(wx+58,wy-46),new Vector2(wx+58,wy+86));
                    GothicLine(new Vector2(wx-45,wy+52),new Vector2(wx+37,wy-58),new Color(.68f,.67f,.9f,.35f),5);
                    for(int crack=0;crack<=run.RoomIndex;crack++)GothicLine(new Vector2(wx,wy),new Vector2(wx-45+crack*29,wy-78+crack*34),accent,1);
                }
                else if(run.Chapter==6)
                {
                    GothicDisc(wx,wy,56,new Color(.34f,.45f,.69f));GothicDisc(wx+18,wy-15,52,new Color(.025f,.02f,.055f));
                    for(int star=0;star<8;star++)GothicDiamond(wx-70+star*19,wy-110+(star*37)%76,2,new Color(.53f,.61f,.85f));
                    if(run.RoomIndex==2)DrawChainLinks(new Vector2(wx,roof),new Vector2(wx,wy+110),new Color(.27f,.28f,.4f),22);
                }
                else
                {
                    GothicPolygon(new Color(.25f,.025f,.06f),new Vector2(wx-48,roof+60),new Vector2(wx+48,roof+60),new Vector2(wx+48,wy+90),new Vector2(wx,wy+65),new Vector2(wx-48,wy+90));
                    GothicDisc(wx,wy-35,28,accent);GothicDisc(wx,wy-35,23,new Color(.035f,.02f,.04f));
                }
            }
            if(run.Stage==RunStage.BossArena)
            {
                if(run.Chapter==7)DrawRestoredSun();
                else
                {
                    float x=SX(32),y=SY(7.3f);
                    GothicDisc(x,y,92,new Color(accent.r,accent.g,accent.b,.35f));
                    GothicDisc(x,y,83,new Color(.04f,.025f,.065f));
                    for(int i=0;i<12;i++)
                    {
                        float a=i*Mathf.PI/6+visualClock*.1f;
                        GothicDiamond(x+Mathf.Cos(a)*76,y+Mathf.Sin(a)*76,8,accent);
                    }
                    if(run.BossDefeated)DrawLateSeam(x,y,accent);
                }
            }
        }
        void DrawLateSeam(float x,float y,Color c)
        {
            float progress=Mathf.Clamp01((BossDefeatElapsed-.6f)/1.4f);
            GothicLine(new Vector2(x,y-70),new Vector2(x,y-70+140*progress),c,3);
            for(int i=0;i<12*progress;i++)GothicLine(new Vector2(x-19,y-63+i*11),new Vector2(x+19,y-74+i*11),HudLight,2);
        }
        void DrawRestoredSun()
        {
            float x=SX(32),y=SY(7.5f),restored=run.BossDefeated?Mathf.SmoothStep(0,1,Mathf.Clamp01((BossDefeatElapsed-.5f)/1.8f)):0;
            var gold=new Color(1,.78f,.32f);
            GothicDisc(x,y,91,gold);GothicDisc(x,y,82,Color.Lerp(new Color(.02f,.01f,.03f),new Color(1,.89f,.58f),restored));
            for(int i=0;i<16;i++)
            {
                float angle=i*Mathf.PI/8;Vector2 d=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
                GothicLine(new Vector2(x,y)+d*97,new Vector2(x,y)+d*(108+restored*40),new Color(1,.78f,.32f,.3f+restored*.65f),3);
            }
            for(int side=-1;side<=1;side+=2)DrawChainLinks(new Vector2(x+side*150,y-105),new Vector2(x-side*48,y+80+restored*130),new Color(.6f,.37f,.25f,1-restored),23);
            if(restored>0)
            {
                DrawLateSeam(x,y,gold);
                Box(new Rect(0,0,1280,580),new Color(1,.75f,.37f,restored*.09f));
                if(restored>.95f)Text("돌아온 태양 · 하늘에 남은 금빛 바늘땀",x-260,y+105,520,35,21,HudLight,TextAnchor.MiddleCenter);
            }
        }
    }
}
