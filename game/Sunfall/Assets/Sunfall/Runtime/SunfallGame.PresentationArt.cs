using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        Texture2D playerSpellSheet,equipmentSheet,flameAnimationSheet;
        float nextPresentationArtRetry;
        void LoadPresentationArt()
        {
            playerSpellSheet=Resources.Load<Texture2D>("Effects/PlayerSpellsV2");
            equipmentSheet=Resources.Load<Texture2D>("Equipment/GothicEquipmentV1");
            flameAnimationSheet=Resources.Load<Texture2D>("Effects/FlameLoopV1");
            foreach(var texture in new[]{playerSpellSheet,equipmentSheet,flameAnimationSheet})
                if(texture!=null){texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;}
            nextPresentationArtRetry=Time.realtimeSinceStartup+1;
        }
        void EnsurePresentationArt()
        {
            if((playerSpellSheet==null||equipmentSheet==null||flameAnimationSheet==null)&&Time.realtimeSinceStartup>=nextPresentationArtRetry)LoadPresentationArt();
        }
        // Normalized cell UVs survive Unity's platform texture resizing. Insets
        // isolate faint edge glow from neighbouring cells without touching files.
        void DrawAtlasCell(Texture2D texture,Rect rect,int cell,int columns,int rows)
        {
            const float inset=.012f;
            var uv=new Rect((cell%columns+inset)/columns,1-(cell/columns+1-inset)/rows,
                (1-2*inset)/columns,(1-2*inset)/rows);
            DrawArtwork(texture,rect,uv);
        }
        void DrawSpellEffect(SpellKind spell,Vector2 center,Vector2 size,float alpha=1,float angle=0,int facing=1)
        {
            DrawAnimatedSpell(spell,center,size,alpha,angle,facing);
        }
        void DrawSpellEffectCell(int cell,Vector2 center,Vector2 size,float alpha=1,float angle=0,int facing=1)
        {
            EnsurePresentationArt();if(playerSpellSheet==null)return;
            var color=GUI.color;
            using(ArtworkAt(center,angle,facing))
            {
                try
                {
                    GUI.color=new Color(1,1,1,Mathf.Clamp01(alpha));
                    DrawAtlasCell(playerSpellSheet,new Rect(-size*.5f,size),cell,4,4);
                }
                finally {GUI.color=color;}
            }
        }
        bool DrawEquipmentImage(EquipmentId id,Vector2 center,Color tint)
        {
            if(!EquipmentCatalog.Valid(id))return false;
            EnsurePresentationArt();if(equipmentSheet==null)return false;
            int cell=id==EquipmentId.AshSword?0:id==EquipmentId.FurnaceHeart?1:id==EquipmentId.SanctumSeal?2:id==EquipmentId.PilgrimCoat?3:4;
            var color=GUI.color;
            GUI.color=tint.r<.5f?new Color(.42f,.42f,.46f):Color.white;
            DrawAtlasCell(equipmentSheet,new Rect(center.x-28,center.y-28,56,56),cell,3,2);
            GUI.color=color;return true;
        }
        void DrawBossStoryPortrait(Rect rect,StoryExpression expression)
        {
            int chapter=run==null?1:Mathf.Clamp(run.Chapter,1,7);
            if(TryDrawBossExpression(rect,chapter,expression))return;
            Texture2D texture;HeroFrame frame;Vector2 sourceSize;
            if(chapter==1)
            {
                if(stage1BossSheet==null&&Time.realtimeSinceStartup>=nextBossArtRetry)LoadBossArt();
                texture=stage1BossSheet;frame=BossFrames[0];sourceSize=new Vector2(1254,1254);
            }
            else if(chapter==2)
            {
                if(chainBossSheet==null&&Time.realtimeSinceStartup>=nextChainBossRetry)LoadChainBossArt();
                texture=chainBossSheet;frame=ChainBossFrames[0];sourceSize=new Vector2(1774,887);
            }
            else if(chapter==3)
            {
                if(solarBossSheet==null&&Time.realtimeSinceStartup>=nextSolarBossRetry)LoadSolarBossArt();
                texture=solarBossSheet;frame=SolarBossFrames[0];sourceSize=new Vector2(1774,887);
            }
            else
            {
                int index=chapter-4;
                if(lateBossSheets[index]==null&&Time.realtimeSinceStartup>=nextLateBossRetry)LoadLateBossArt();
                texture=lateBossSheets[index];frame=LateBossFrames[index][0];sourceSize=new Vector2(1774,887);
            }
            GothicPlaque(rect,12,HudIron);
            if(texture==null){Text("초상화 불러오는 중",rect.x,rect.center.y-20,rect.width,40,16,HudLight,TextAnchor.MiddleCenter);return;}
            float scale=Mathf.Min((rect.width-16)/frame.Source.width,(rect.height-24)/frame.Source.height);
            var origin=rect.center-new Vector2(frame.Source.width,frame.Source.height)*scale*.5f;
            var color=GUI.color;GUI.color=Color.white;
            DrawHeroRegion(texture,sourceSize,frame.Source,origin,scale);
            if(chapter==5)
            {
                EnsureMirrorArt();
                int mask=expression==StoryExpression.Angry?1:expression==StoryExpression.Pained||expression==StoryExpression.Resigned?2:0;
                DrawLysseaMaskPart(mask,origin+(LysseaFaceCenters[0]-frame.Source.position)*scale,49*scale,Color.white);
            }
            GUI.color=color;
        }
    }
}
