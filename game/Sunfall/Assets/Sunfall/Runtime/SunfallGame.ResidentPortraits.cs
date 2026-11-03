using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        static readonly string[] ResidentPortraitAssets={"Hero","Blacksmith","Merchant","Arin"};
        readonly Texture2D[] residentPortraitSheets=new Texture2D[4];
        float nextResidentPortraitRetry;
        void LoadResidentPortraits()
        {
            for(int i=0;i<residentPortraitSheets.Length;i++)
            {
                var texture=Resources.Load<Texture2D>("Story/ResidentPortraits/"+ResidentPortraitAssets[i]+"ExpressionsV1");
                residentPortraitSheets[i]=texture;
                if(texture!=null){texture.filterMode=FilterMode.Bilinear;texture.wrapMode=TextureWrapMode.Clamp;}
            }
            nextResidentPortraitRetry=Time.realtimeSinceStartup+1;
        }
        static int ResidentExpressionCell(StoryExpression expression)
        {
            switch(expression)
            {
                case StoryExpression.Warm:return 1;
                case StoryExpression.Worried:return 2;
                case StoryExpression.Determined:case StoryExpression.Angry:return 3;
                case StoryExpression.Shocked:return 4;
                case StoryExpression.Sad:case StoryExpression.Pained:case StoryExpression.Resigned:return 5;
                default:return 0;
            }
        }
        bool TryDrawResidentExpression(Rect rect,StoryVoice voice,StoryExpression expression)
        {
            int index=voice==StoryVoice.Hero?0:voice==StoryVoice.Smith?1:voice==StoryVoice.Merchant?2:voice==StoryVoice.Arin?3:-1;
            if(index<0)return false;
            if(residentPortraitSheets[index]==null&&Time.realtimeSinceStartup>=nextResidentPortraitRetry)LoadResidentPortraits();
            var texture=residentPortraitSheets[index];if(texture==null)return false;
            GothicPlaque(rect,12,HudGold);
            GothicPlaque(new Rect(rect.x+2,rect.y+2,rect.width-4,rect.height-4),10,HudInk);
            float side=Mathf.Min(rect.width-12,rect.height-18);
            var target=new Rect(rect.center.x-side*.5f,rect.center.y-side*.5f,side,side);
            var color=GUI.color;GUI.color=Color.white;
            DrawAtlasCell(texture,target,ResidentExpressionCell(expression),3,2);
            GUI.color=color;
            return true;
        }
    }
}
