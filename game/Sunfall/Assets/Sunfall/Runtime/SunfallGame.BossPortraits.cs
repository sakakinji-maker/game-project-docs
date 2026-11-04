using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        static readonly string[] BossPortraitAssets={
            "RedWingExpressionsV1","MorgaHuskExpressionsV2","AurelEclipseExpressionsV2","VarkanKnightExpressionsV2",
            "LysseaMaskExpressionsV2","NoctisRavenExpressionsV2","SolveinThornExpressionsV2"
        };
        readonly Texture2D[] bossPortraitSheets=new Texture2D[7];
        float nextBossPortraitRetry;
        void LoadBossPortraits()
        {
            for(int i=0;i<bossPortraitSheets.Length;i++)
            {
                var texture=Resources.Load<Texture2D>("Story/BossPortraits/"+BossPortraitAssets[i]);
                bossPortraitSheets[i]=texture;
                if(texture!=null){texture.filterMode=FilterMode.Bilinear;texture.wrapMode=TextureWrapMode.Clamp;}
            }
            nextBossPortraitRetry=Time.realtimeSinceStartup+1;
        }
        bool TryDrawBossExpression(Rect rect,int chapter,StoryExpression expression)
        {
            int index=Mathf.Clamp(chapter-1,0,6);
            if(bossPortraitSheets[index]==null&&Time.realtimeSinceStartup>=nextBossPortraitRetry)LoadBossPortraits();
            var texture=bossPortraitSheets[index];if(texture==null)return false;
            GothicPlaque(rect,12,HudGold);
            GothicPlaque(new Rect(rect.x+2,rect.y+2,rect.width-4,rect.height-4),10,HudInk);
            // One full, square atlas cell per expression; never stretch the face
            // to the taller dialogue panel or blend differently aligned faces.
            float side=Mathf.Min(rect.width-12,rect.height-18);
            var target=new Rect(rect.center.x-side*.5f,rect.center.y-side*.5f,side,side);
            var color=GUI.color;GUI.color=Color.white;
            DrawAtlasCell(texture,target,Mathf.Clamp((int)expression,0,5),3,2);
            GUI.color=color;
            return true;
        }
    }
}
