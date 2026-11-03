using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        sealed class VillageNpc
        {
            public readonly string Name,ShortName,Resource;
            public readonly float X,Scale;
            public readonly HeroFrame[] Frames;
            public Texture2D Texture;
            public int Facing=1;
            public VillageNpc(string name,string shortName,string resource,float x,float scale,HeroFrame[] frames)
            {Name=name;ShortName=shortName;Resource=resource;X=x;Scale=scale;Frames=frames;}
        }
        // All four poses of each resident use the same anatomical scale.
        // Bounds are measured on the original 1254px PNG, not assumed grid cells.
        readonly VillageNpc[] villageNpcs={
            new VillageNpc("대장장이","대장장이","BlacksmithV1",220,.138f,new[]{
                new HeroFrame(195,53,327,565,350,613),
                new HeroFrame(744,53,321,565,900,613),
                new HeroFrame(194,659,423,560,345,1214),
                new HeroFrame(747,662,328,557,900,1214)
            }),
            new VillageNpc("떠돌이 상인","상인","MerchantV1",780,.127f,new[]{
                new HeroFrame(196,54,289,561,355,610),
                new HeroFrame(766,54,294,561,925,610),
                new HeroFrame(154,659,414,546,365,1200),
                new HeroFrame(755,659,326,546,925,1200)
            }),
            new VillageNpc("아린 · 등불지기","아린","ArinV1",GuideX,.133f,new[]{
                new HeroFrame(256,67,231,535,365,597),
                new HeroFrame(807,67,227,535,915,597),
                new HeroFrame(248,680,304,536,375,1211),
                new HeroFrame(806,680,230,536,915,1211)
            })
        };
        static readonly Vector2 VillageNpcSourceSize=new Vector2(1254,1254);
        int selectedVillageNpc=2;
        float nextVillageArtRetry;
        readonly StoryLine[] smithDialogue={
            new StoryLine(StoryVoice.Smith,"그 검을 다시 꺼냈구나. 네 가문의 재단검… 네 친어머니에게 손질법을 배웠지. 사람을 베기 전에, 찢어진 하늘을 다듬던 날이야.",StoryExpression.Sad),
            new StoryLine(StoryVoice.Hero,"저를 길러주신 어머니가 균열에 끌려갔어요. 이 검으로 그분에게 가는 길을 열 수 있을까요?",StoryExpression.Worried),
            new StoryLine(StoryVoice.Smith,"악마에게 엉겨 붙은 실은 힘으로 당기면 끊어진다. 검으로 매듭을 가르고, 네 마법으로 틈을 꿰매거라. 네 친어머니도 늘 그렇게 말씀하셨지.",StoryExpression.Determined),
            new StoryLine(StoryVoice.Smith,"길에서 모은 골드를 가져오면 장비를 +5까지 벼려주마. 첫 강화는 60골드, 이후 단계마다 60골드씩 더 들지. 내가 맡은 장비는 실패 없이 돌려주겠네.",StoryExpression.Warm),
            new StoryLine(StoryVoice.Hero,"부탁드릴게요. 어머니를 데리고 돌아올 때까지, 이 검을 놓지 않을 거예요.",StoryExpression.Determined)
        };
        readonly StoryLine[] merchantDialogue={
            new StoryLine(StoryVoice.Merchant,"네 어머니는 내가 빈손으로 돌아와도 수프 한 그릇을 내주셨지. 그분이 사라진 집을 보니… 물건을 늘어놓을 손이 멈추더군.",StoryExpression.Sad),
            new StoryLine(StoryVoice.Hero,"숲 너머를 다녀오셨죠. 균열에 끌려간 사람을 본 적 있나요?",StoryExpression.Neutral),
            new StoryLine(StoryVoice.Merchant,"성소 위에서 사람 목소리를 들었네. 붉은 날개가 지나갈 때마다 잠잠해졌지. 더 안쪽 성채에는 왕실 문장이 박힌 사슬이 하늘로 뻗어 있었고.",StoryExpression.Worried),
            new StoryLine(StoryVoice.Merchant,"여기 장비들은 폐허에서 건진 것들이야. 별실에 반응하는 유물도 있지. 악마를 쓰러뜨려 모은 골드로 사고, I를 눌러 장착하게. 같은 물건을 두 번 살 필요는 없어.",StoryExpression.Warm),
            new StoryLine(StoryVoice.Hero,"성소에서 어머니의 흔적부터 찾겠어요. 돌아오면, 같이 그 수프를 먹어요.",StoryExpression.Warm)
        };
        StoryLine[] VillageDialogue=>selectedVillageNpc==0?smithDialogue:selectedVillageNpc==1?merchantDialogue:guideDialogue;

        void LoadVillageNpcArt()
        {
            foreach(var npc in villageNpcs)
            {
                npc.Texture=Resources.Load<Texture2D>("Village/"+npc.Resource);
                if(npc.Texture==null)continue;
                npc.Texture.filterMode=FilterMode.Point;npc.Texture.wrapMode=TextureWrapMode.Clamp;
            }
            nextVillageArtRetry=Time.realtimeSinceStartup+1;
        }
        void EnsureVillageNpcArt(VillageNpc npc)
        {
            if(npc.Texture==null&&Time.realtimeSinceStartup>=nextVillageArtRetry)LoadVillageNpcArt();
        }
        void DrawVillageNpc(int index)
        {
            var npc=villageNpcs[index];EnsureVillageNpcArt(npc);
            bool speaking=dialoguePage>=0&&selectedVillageNpc==index&&VillageDialogue[Mathf.Min(dialoguePage,VillageDialogue.Length-1)].Voice!=StoryVoice.Hero;
            int pose=speaking?(index==2?3:2+(int)(visualClock*1.5f)%2):(int)(visualClock*1.5f+index*.4f)%2;
            // Keep the feet planted and avoid rapid flips while the player overlaps.
            if(!paused&&!settings&&Mathf.Abs(villageX-npc.X)>25)npc.Facing=villageX<npc.X?-1:1;
            if(npc.Texture==null)
            {
                Box(new Rect(npc.X-12,476,24,64),new Color(.46f,.36f,.27f));
                Box(new Rect(npc.X-9,466,18,18),new Color(.85f,.7f,.55f));
            }
            else
            {
                var matrix=GUI.matrix;var color=GUI.color;var pivot=new Vector3(npc.X,540,0);
                GUI.matrix=matrix*Matrix4x4.Translate(pivot)*Matrix4x4.Scale(new Vector3(npc.Facing,1,1))*Matrix4x4.Translate(-pivot);
                GUI.color=Color.white;
                DrawCharacterPose(npc.Texture,npc.Frames[pose],VillageNpcSourceSize,npc.Scale,npc.X,540);
                GUI.matrix=matrix;GUI.color=color;
            }
        }
        void DrawVillageNpcNameplate(int index)
        {
            var npc=villageNpcs[index];
            bool available=CanVillageInteract&&dialoguePage<0&&Mathf.Abs(villageX-npc.X)<=145;
            bool selected=available&&index==NearestVillageNpc();
            Color ink=selected?new Color(1,.81f,.46f):new Color(.85f,.82f,.73f);
            // Text with a dark outline reads over windows without covering the scenery.
            // Keep one fixed height above the tallest pose so the label never bobs.
            DrawNameplateText(npc.Name,npc.X,406,240,28,19,ink);
            Color rule=selected?new Color(.9f,.63f,.29f):new Color(.5f,.47f,.38f,.8f);
            Box(new Rect(npc.X-39,436,32,1),rule);
            Box(new Rect(npc.X+7,436,32,1),rule);
            Box(new Rect(npc.X-2,434,4,4),rule);
            if(selected)
                DrawNameplateText((TouchControls?"터치 · ":"Space · ")+(index==0?"강화":index==1?"장비 구매":"대화"),npc.X,440,170,19,13,new Color(1,.9f,.66f));
            if(available)
                hits.Add(new Hit(new Rect(npc.X-110,402,220,58),()=>{
                    if(village&&!paused&&!settings&&dialoguePage<0&&Mathf.Abs(villageX-npc.X)<=145)
                        BeginVillageDialogue(index);
                }));
        }
        void DrawNameplateText(string label,float center,float y,float width,float height,int size,Color color)
        {
            var shadow=new Color(.015f,.025f,.035f,.95f);
            for(int dx=-1;dx<=1;dx+=2)
                for(int dy=-1;dy<=1;dy+=2)
                    Text(label,center-width*.5f+dx,y+dy,width,height,size,shadow,TextAnchor.MiddleCenter);
            Text(label,center-width*.5f,y,width,height,size,color,TextAnchor.MiddleCenter);
        }
        void DrawVillageNpcPortrait(Rect rect,int index)
        {
            var npc=villageNpcs[index];EnsureVillageNpcArt(npc);
            Box(rect,new Color(.09f,.13f,.19f));
            if(npc.Texture!=null)
            {
                // Reuse the greeting sprite as a full-body dialogue illustration.
                // This does not change the scale or pose of the character in the world.
                var frame=npc.Frames[2];
                float scale=Mathf.Min((rect.width-18)/frame.Source.width,(rect.height-42)/frame.Source.height);
                var origin=new Vector2(rect.center.x-frame.Source.width*scale*.5f,rect.y+8+(rect.height-42-frame.Source.height*scale)*.5f);
                var color=GUI.color;GUI.color=Color.white;
                DrawHeroRegion(npc.Texture,VillageNpcSourceSize,frame.Source,origin,scale);
                GUI.color=color;
            }
            Text(npc.ShortName,rect.x,rect.yMax-30,rect.width,28,17,new Color(.92f,.73f,.65f),TextAnchor.MiddleCenter);
        }
        void DrawVillageWorkplaces()
        {
            // Forge, shelter and merchant stall are painted into the generated scenery.
            if(villageBackdrop!=null)return;
            Color amber=new Color(.94f,.72f,.37f);
            Text("대장간",130,276,175,32,20,amber,TextAnchor.MiddleCenter);
            Text("등불 쉼터",410,276,175,32,20,amber,TextAnchor.MiddleCenter);
            Text("상인의 가판",690,276,175,32,20,amber,TextAnchor.MiddleCenter);
            // Static props stay separate from NPC silhouettes and their pivots.
            Box(new Rect(130,518,52,8),new Color(.43f,.47f,.48f));
            Box(new Rect(146,526,24,14),new Color(.25f,.29f,.3f));
            Box(new Rect(271,509,34,31),new Color(.24f,.19f,.16f));
            Box(new Rect(275,513,26,9),new Color(.88f,.35f,.12f));
            Box(new Rect(700,520,45,20),new Color(.37f,.25f,.16f));
            Box(new Rect(704,513,14,7),new Color(.67f,.53f,.32f));
            Box(new Rect(723,508,16,12),new Color(.5f,.3f,.23f));
        }
    }
}
