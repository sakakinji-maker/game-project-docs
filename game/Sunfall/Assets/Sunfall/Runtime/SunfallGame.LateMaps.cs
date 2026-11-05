using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        static readonly string[][] LateRoomTitles={
            new[]{"거울 정원","기억의 회랑","뒤집힌 예배당","거울지기의 성역"},
            new[]{"밤의 기단","일식의 나선 계단","부서진 종루","일식의 원형 성소"},
            new[]{"빛을 잃은 왕궁","왕의 장례 회랑","검은 태양의 문","꺼진 태양의 왕좌"}
        };
        static readonly string[][][] LateLandmarks={
            new[]{new[]{"거울에 남은 별실","기억을 지키는 악마","기억의 회랑으로"},new[]{"어머니의 기억 조각","마녀를 지운 왕의 기록","뒤집힌 예배당으로"},new[]{"거짓을 비추는 창","마지막 거울 수비대","리세아의 성역으로"},new[]{"진실을 가둔 성역","되찾을 어머니의 이름","일식의 첨탑으로"}},
            new[]{new[]{"별빛 없는 밤","첨탑을 지키는 악마","나선 계단으로"},new[]{"위로 이어진 별실","일식의 파수꾼","부서진 종루로"},new[]{"울리지 않는 종","왕좌 앞 마지막 날개","일식의 원형 성소로"},new[]{"일식의 장막","어머니에게 닿는 하늘","꺼진 태양의 왕좌로"}},
            new[]{new[]{"영원한 낮을 원했던 왕궁","금빛에 갇힌 악마","장례 회랑으로"},new[]{"왕의 이름 없는 묘비","왕실의 마지막 군대","검은 태양의 문으로"},new[]{"문 너머의 심장 소리","돌아올 길을 남기는 별실","왕과의 결전으로"},new[]{"마지막 별실","태양을 삼킨 왕","어머니와 함께 귀환"}}
        };
        void DrawLateLandmarks()
        {
            if(run.Chapter==5)DrawMirrorLandmarks();
            else if(run.Chapter==6)DrawEclipseLandmarks();
            else if(run.Chapter==7)DrawThroneLandmarks();
        }
        void DrawLateSeam(float x,float y,Color c)
        {
            float progress=Mathf.Clamp01((BossDefeatElapsed-.6f)/1.4f);
            var path=new SewnScarPath(new Vector2(x-2,y-70),new Vector2(x+2,y+70),4);
            var gold=Color.Lerp(new Color(.93f,.72f,.37f),c,.15f);
            DrawSewnScar(path,progress,12,gold,Mathf.Clamp01((BossDefeatElapsed-2)/.7f));
            DrawScarAfterglow(path,BossDefeatElapsed-2,gold);
        }
        void DrawRestorationScar(Vector2 from,Vector2 to,float bend,int stitches,float scale)
        {
            float progress=Mathf.Clamp01((BossDefeatElapsed-.6f)/1.5f);
            var path=new SewnScarPath(from,to,bend);
            var gold=new Color(.96f,.77f,.42f);
            float settle=Mathf.Clamp01((BossDefeatElapsed-2.1f)/.8f);
            DrawSewnScar(path,progress,stitches,gold,settle,scale);
            DrawScarAfterglow(path,BossDefeatElapsed-2.1f,gold);
        }
    }
}
