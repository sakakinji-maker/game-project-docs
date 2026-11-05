using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        void DrawFinalEnding()
        {
            hits.Clear();EnsureThroneArt();
            var saved=GUI.color;GUI.color=Color.white;
            if(throneDawn!=null)GUI.DrawTexture(new Rect(0,0,1280,720),throneDawn,ScaleMode.ScaleAndCrop,false);
            GUI.color=saved;
            Box(new Rect(0,0,1280,720),new Color(.025f,.018f,.035f,.64f));
            Text("SUNFALL · 일곱 봉인의 끝",90,65,780,42,25,HudGold);
            Text("함께 돌아온 아침",86,125,900,85,52,HudLight);
            Text("왕의 마지막 매듭이 풀리고, 태양은 다시 하늘로 돌아갔다.\n금빛 봉합 자국은 남았지만, 그 틈은 더는 누구도 삼키지 않았다.",90,240,790,92,24,Color.white);
            Text("“오늘은 문을 두드리지 않아도 되겠구나.\n  함께 돌아왔으니까.”",90,355,770,90,26,HudLight);
            DrawThronePart(4,new Vector2(1050,345),Vector2.one*310);
            Text("양어머니와 함께, 불씨 마을로",900,505,320,38,20,HudLight,TextAnchor.MiddleCenter);
            Text($"7스테이지 완료 · 처치 {run.Kills} · 장비와 성장은 저장됩니다",90,475,790,40,19,Color.white);
            Button("어머니와 함께 마을로",new Rect(90,560,340,64),EnterVillage,true);
            Button("장비 확인 (I)",new Rect(454,560,260,64),ToggleInventory);
            Button("새로운 모험",new Rect(738,560,270,64),()=>StartRun());
            Text("플레이해 주셔서 감사합니다",90,650,1100,32,18,HudGold,TextAnchor.MiddleCenter);
        }
    }
}
