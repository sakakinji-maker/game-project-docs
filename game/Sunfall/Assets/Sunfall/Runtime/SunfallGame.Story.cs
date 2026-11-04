using UnityEngine;
using UnityEngine.InputSystem;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        enum StoryVoice { Narrator, Hero, Mother, Arin, Smith, Merchant, Boss, Villager }
        enum StoryExpression { Neutral, Scorn, Angry, Shocked, Pained, Resigned, Warm, Worried, Determined, Sad }
        readonly struct StoryLine
        {
            public readonly StoryVoice Voice;
            public readonly string Words;
            public readonly StoryExpression Expression;
            public StoryLine(StoryVoice voice,string words,StoryExpression expression=StoryExpression.Neutral)
            {Voice=voice;Words=words;Expression=expression;}
        }
        static readonly StoryLine[] OpeningStory={
            new StoryLine(StoryVoice.Narrator,"하늘은 세계를 감싼 장막이었고, 태양은 그 중심에 꿰매진 빛이었다. 왕은 영원한 낮을 원했다. 대성당에 태양을 묶은 날, 하늘이 찢어졌다."),
            new StoryLine(StoryVoice.Narrator,"태양은 틈 너머로 추락했다. 왕실은 하늘을 수선하던 마녀들에게 죄를 씌웠다. 나는 가문을 잃고, 불씨 마을로 숨어들었다."),
            new StoryLine(StoryVoice.Mother,"네가 어디서 왔는지는 말하지 않아도 된단다. 이 집에서는 굶지 않아도 돼. 돌아오면, 언제든 문을 두드리렴."),
            new StoryLine(StoryVoice.Narrator,"나를 딸이라 불러준 사람. 피 한 방울 섞이지 않았지만, 그분은 내 어머니였다. 마을 위로 붉은 균열이 열린 밤에도, 어머니는 먼저 내 손을 잡았다."),
            new StoryLine(StoryVoice.Mother,"이쪽으로 와! 하늘을 보지 말고, 엄마 손을…!"),
            new StoryLine(StoryVoice.Narrator,"손끝이 떨어졌다. 어머니가 어둠 속으로 끌려가자, 감춰왔던 별실이 내 손에서 터져 나왔다. 실 끝에, 아직 살아 있는 심장 소리가 닿았다."),
            new StoryLine(StoryVoice.Hero,"살아 계셔. 이 실 끝에 어머니가 있어. 다시는 쓰지 않겠다고 했는데… 이번에는 놓지 않을 거야.",StoryExpression.Determined),
            new StoryLine(StoryVoice.Arin,"네 별실에 닿은 흔적이 숲 너머 성소로 이어져. 내가 등불을 지킬게. 떠나기 전에 대장장이에게 재단검을 보여줘.",StoryExpression.Warm),
            new StoryLine(StoryVoice.Hero,"어머니, 기다려 주세요. 당신이 돌아올 수 있도록, 제가 하늘을 꿰맬게요.",StoryExpression.Determined)
        };
        static readonly StoryLine[] BossArrivalStory={
            new StoryLine(StoryVoice.Boss,"그 실… 아직 살아남은 봉합의 마녀가 있었구나. 이 균열까지 닫으러 온 것이냐?"),
            new StoryLine(StoryVoice.Hero,"여기로 끌려온 사람을 찾고 있어. 나를 키워준 분이야. 그분의 흔적이 네 안에서 느껴져.",StoryExpression.Worried),
            new StoryLine(StoryVoice.Boss,"내가 삼킨 별실에는 수많은 목소리가 얽혀 있다. 네 어미도 그중 하나겠지. 하지만 틈을 닫으면 우리는 이 세계에 머물 수 없어.",StoryExpression.Scorn),
            new StoryLine(StoryVoice.Hero,"그렇다면 네가 붙잡고 있는 실부터 되찾겠어. 어머니가 돌아올 길을 열어야 하니까.",StoryExpression.Determined),
            new StoryLine(StoryVoice.Boss,"와라, 마지막 마녀. 이번에는 네 손가락부터 끊어주마.",StoryExpression.Angry)
        };
        static readonly StoryLine[] BossDefeatStory={
            new StoryLine(StoryVoice.Boss,"왕의 사슬은… 아직 태양을 놓지 않았다. 이 틈 하나를 꿰맨다고… 네 어미가 돌아올 것 같으냐…",StoryExpression.Pained),
            new StoryLine(StoryVoice.Hero,"한 곳으로 부족하면, 전부 꿰매겠어. 이제 그 실을 놓아.",StoryExpression.Determined),
            new StoryLine(StoryVoice.Narrator,"붉은 날개에서 별실이 풀려 나왔다. 나는 재단검으로 얽힌 끝을 가르고, 벌어진 하늘에 첫 바늘땀을 놓았다. 실 속에 새로운 마법의 짜임이 남았다."),
            new StoryLine(StoryVoice.Villager,"그분이 우릴 붙잡아 줬어요. 당신 이름을 부르면서… 하지만 검은 사슬이 그분만 더 깊은 곳으로 끌고 갔어요."),
            new StoryLine(StoryVoice.Hero,"어머니는 아직 저 너머에 계셔. 사슬이 어디로 이어지는지 말해줘.",StoryExpression.Worried),
            new StoryLine(StoryVoice.Villager,"검은 성채예요. 왕실의 문장이 새겨진 사슬이 그곳에서 하늘까지 뻗어 있었어요."),
            new StoryLine(StoryVoice.Hero,"마을로 돌아가 아린에게 전해줘. 첫 균열은 닫았다고. 나는 성채로 갈게. 이번에는 그 사슬을 끊겠어.",StoryExpression.Determined)
        };
        static readonly StoryLine[] ChainBossArrivalStory={
            new StoryLine(StoryVoice.Boss,"이 껍질은 감옥이 아니다. 왕의 사슬을 품은 성소지. 나는 모르가. 네가 끊으려는 사슬의 간수다."),
            new StoryLine(StoryVoice.Hero,"그 사슬에 어머니의 별실이 감겨 있어. 어디로 끌고 간 거야?",StoryExpression.Worried),
            new StoryLine(StoryVoice.Boss,"네가 그 여자를 붙잡으려 던진 실이 길을 열었다. 왕의 사슬은 이제 그 실을 따라 마지막 마녀의 힘까지 거둬들일 것이다.",StoryExpression.Scorn),
            new StoryLine(StoryVoice.Hero,"어머니를 구하려던 실을… 왕이 태양을 붙잡는 데 쓰고 있다고?",StoryExpression.Shocked),
            new StoryLine(StoryVoice.Boss,"하늘도, 태양도, 네 어미도 왕의 것이다. 이 성채의 닻이 버티는 한 누구도 사슬에서 벗어나지 못한다.",StoryExpression.Angry),
            new StoryLine(StoryVoice.Hero,"어머니는 누구의 소유도 아니야. 네가 지키는 그 닻부터 잘라주겠어.",StoryExpression.Determined)
        };
        static readonly StoryLine[] ChainBossDefeatStory={
            new StoryLine(StoryVoice.Boss,"닻이… 끊어진다고…? 어리석은 마녀. 여기서 끊은 건 사슬 하나에 불과하다…",StoryExpression.Shocked),
            new StoryLine(StoryVoice.Hero,"어머니는 어디에 있어? 대답해.",StoryExpression.Determined),
            new StoryLine(StoryVoice.Boss,"왕도의 대성당… 태양을 묶은 제단에… 네가 다가갈수록 왕은 더 깊이…",StoryExpression.Resigned),
            new StoryLine(StoryVoice.Narrator,"재단검이 왕실 문장을 가르자 성채를 관통하던 사슬이 부서졌다. 짓눌렸던 별실이 손목을 감쌌다. 실 끝에서, 어머니의 심장이 다시 또렷하게 뛰었다."),
            new StoryLine(StoryVoice.Hero,"느껴져요. 어머니, 아직 제 손을 잡고 계시죠? 이번에는 제가 그곳으로 갈게요.",StoryExpression.Warm),
            new StoryLine(StoryVoice.Narrator,"성채의 닻은 무너졌지만, 하늘에는 여전히 더 큰 사슬이 남아 있었다. 열린 포탈 너머로 왕도의 첨탑이 보였다. 이제 어머니의 별실을 따라 대성당으로 향할 차례다.")
        };
        static readonly StoryLine[] CathedralArrivalStory={
            new StoryLine(StoryVoice.Narrator,"포탈 너머, 태양을 잃은 왕도가 모습을 드러냈다. 대성당의 유리창은 빛을 잊었지만, 손목의 별실은 그 안을 향해 팽팽해졌다."),
            new StoryLine(StoryVoice.Hero,"어머니, 가까워졌어요. 성소에서 되찾은 마법도, 간수에게서 풀어낸 사슬도 이제 제 힘이에요.",StoryExpression.Determined),
            new StoryLine(StoryVoice.Narrator,"진입로와 왕실 회랑에는 아직 악마들이 남아 있었다. 태양을 묶은 예배당의 봉인에 닿으려면, 먼저 그 수비대를 지나야 한다.")
        };
        static readonly StoryLine[] SolarBossArrivalStory={
            new StoryLine(StoryVoice.Boss,"나는 아우렐. 태양 봉인을 집행하는 왕실의 악마다. 사슬을 끊은 마녀여, 이 문 너머는 왕의 빛이다."),
            new StoryLine(StoryVoice.Hero,"어머니의 심장이 이 안에서 뛰고 있어. 그분에게 무슨 짓을 한 거야?",StoryExpression.Worried),
            new StoryLine(StoryVoice.Boss,"네 별실을 붙잡은 그 여자가 봉인의 마지막 매듭이 되었다. 빛이 새어 나갈 때마다 자기 몸으로 틈을 막더군. 다른 이들이 다칠까 두려웠겠지.",StoryExpression.Scorn),
            new StoryLine(StoryVoice.Mother,"들리니…? 엄마는 여기 있어. 하지만 매듭을 함부로 끊으면… 하늘이 다시…"),
            new StoryLine(StoryVoice.Boss,"그 여자를 꺼내면 봉인이 무너진다. 왕께서는 이미 대가를 정하셨다. 한 사람으로 태양을 붙잡을 수 있다면 충분하지.",StoryExpression.Angry),
            new StoryLine(StoryVoice.Hero,"어머니를 대가로 부르지 마. 사슬을 베고 찢어진 곳은 내가 꿰매겠어. 어머니도, 하늘도 포기하지 않아.",StoryExpression.Determined)
        };
        static readonly StoryLine[] SolarBossDefeatStory={
            new StoryLine(StoryVoice.Boss,"봉인을… 베지 않고 다시 꿰맨다고…? 왕의 문장 없이 빛을 견딜 수 있을 리가…",StoryExpression.Shocked),
            new StoryLine(StoryVoice.Hero,"당신들이 태양을 가두느라 찢어 놓은 거야. 어머니가 버티게 두지 않겠어.",StoryExpression.Determined),
            new StoryLine(StoryVoice.Narrator,"집행관의 관이 부서지자 봉인 바깥의 사슬이 풀렸다. 나는 어머니에게 이어진 실을 남겨두고, 왕실 문장이 박힌 틈을 금빛 바늘땀으로 대신했다."),
            new StoryLine(StoryVoice.Mother,"그 따뜻한 실… 네 손이구나. 정말 여기까지 왔구나, 내 딸…"),
            new StoryLine(StoryVoice.Hero,"이제 제 목소리가 들리죠? 조금만 더 버텨 주세요. 안쪽 매듭까지 풀어서 함께 돌아갈게요.",StoryExpression.Warm),
            new StoryLine(StoryVoice.Boss,"왕관의 심장부에… 마지막 명령이 남아 있다. 그 여자의 손을 놓는 순간… 왕이 알게 될 것이다…",StoryExpression.Pained),
            new StoryLine(StoryVoice.Narrator,"태양은 아직 완전히 돌아오지 않았다. 그러나 봉인에 생긴 길 너머에서 어머니의 목소리가 또렷해졌다. 구출을 향한 문 하나가 열렸다.")
        };
        static readonly StoryLine[] CrownHeartArrivalStory={
            new StoryLine(StoryVoice.Narrator,"아우렐의 봉인을 넘어 대성당 뒤편으로 나왔다. 안개 위의 다리는 왕실 요새로 이어졌다. 저 회랑 너머, 왕이 태양을 붙잡아 둔 심장이 아직 뛰고 있었다."),
            new StoryLine(StoryVoice.Mother,"빛이… 조금 따뜻해졌어. 네가 길을 열었구나. 하지만 이 안에는 아직 나를 묶은 매듭이 남아 있단다."),
            new StoryLine(StoryVoice.Hero,"이제 태양의 빛도 제 손에 있어요. 어머니에게 닿는 실을 따라갈게요. 혼자 버티게 두지 않겠어요.",StoryExpression.Determined),
            new StoryLine(StoryVoice.Narrator,"왕실 요새의 푸른 회랑 끝에서 쇠문이 울렸다. 심장 장치 아래의 결투장. 검은 망토의 심장지기가 망치를 끌며 기다리고 있었다.")
        };
        StoryLine[] storyLines;
        int storyPage;
        string storyHeading="";
        int bossArrivalStoryChapter,bossDefeatStoryChapter;
        bool StoryActive=>storyLines!=null;
        void BeginOpeningStory()
        {
            EnterVillage();BeginStory("서장 · 손끝에 남은 별실",OpeningStory);
            openingSceneTime=openingTransitionTime=0;openingTransition=false;openingFinish=false;openingSubtitleRevealed=false;
        }
        void BeginStory(string heading,StoryLine[] lines)
        {
            if(HasSeenStory(lines))return;
            storyHeading=heading;storyLines=lines;storyPage=0;
            skillWindowOpen=false;inventoryOpen=false;serviceNpc=-1;dialoguePage=-1;CaptureWorldMotion();Block();
        }
        void EndStory(){RememberStory(storyLines);storyLines=null;storyPage=0;CaptureWorldMotion();Block();}
        void AdvanceStory()
        {
            if(!StoryActive||paused||settings)return;
            if(OpeningCinematicActive){AdvanceOpeningCinematic();return;}
            if(storyPage+1<storyLines.Length){storyPage++;Block();}else EndStory();
        }
        bool TryBeginBossStory()
        {
            if(StoryActive||run==null)return false;
            if(run.BossIntroducing&&bossArrivalStoryChapter!=run.Chapter&&RunModel.BossIntroDuration-run.BossIntroRemaining>=(run.Chapter>=5?2.5f:run.Chapter==2?2.35f:1.8f))
            {
                bossArrivalStoryChapter=run.Chapter;if(run.Chapter>=4){BeginStory("제"+run.Chapter+"장 · "+BossTitle,LateBossArrivalStories[run.Chapter-4]);return true;}BeginStory(run.Chapter==3?"제3장 · 태양 봉인의 집행관":run.Chapter==2?"제2장 · 왕실 사슬의 간수":"제1장 · 균열을 지키는 붉은 날개",run.Chapter==3?SolarBossArrivalStory:run.Chapter==2?ChainBossArrivalStory:BossArrivalStory);return true;
            }
            if(run.BossDissolving&&bossDefeatStoryChapter!=run.Chapter&&RunModel.BossDefeatDuration-run.BossDefeatRemaining>=(run.Chapter==7?2.95f:.35f))
            {
                bossDefeatStoryChapter=run.Chapter;if(run.Chapter>=4){BeginStory(LateDefeatHeading,LateBossDefeatStories[run.Chapter-4]);return true;}BeginStory(run.Chapter==3?"제3장 · 어머니에게 닿은 바늘땀":run.Chapter==2?"제2장 · 사슬을 끊는 검":"제1장 · 첫 번째 바늘땀",run.Chapter==3?SolarBossDefeatStory:run.Chapter==2?ChainBossDefeatStory:BossDefeatStory);return true;
            }
            return false;
        }
        string StoryVoiceName(StoryVoice voice)
        {
            switch(voice){case StoryVoice.Hero:return "주인공";case StoryVoice.Mother:return run!=null&&run.Chapter==7&&run.BossDefeated?"양어머니 · 되찾은 손":run!=null&&run.Chapter>=3&&!village?"양어머니 · 봉인 너머의 목소리":"양어머니 · 그날의 기억";case StoryVoice.Arin:return "아린 · 등불지기";case StoryVoice.Smith:return "대장장이";case StoryVoice.Merchant:return "떠돌이 상인";case StoryVoice.Boss:return BossTitle;case StoryVoice.Villager:return "균열에서 돌아온 주민";default:return "별을 꿰매는 마녀";}
        }
        void DrawStoryPortrait(StoryVoice voice,Rect rect,StoryExpression expression=StoryExpression.Neutral)
        {
            if(TryDrawResidentExpression(rect,voice,expression))return;
            if(voice==StoryVoice.Hero){DrawHeroPortrait(rect,true);return;}
            if(voice==StoryVoice.Arin||voice==StoryVoice.Smith||voice==StoryVoice.Merchant)
            {DrawVillageNpcPortrait(rect,voice==StoryVoice.Arin?2:voice==StoryVoice.Smith?0:1);return;}
            if(voice==StoryVoice.Boss){DrawBossStoryPortrait(rect,expression);return;}
            Color thread=voice==StoryVoice.Boss?new Color(.85f,.24f,.38f):HudLight;
            string caption=voice==StoryVoice.Boss?(run!=null&&run.Chapter>=4?RunModel.LateBossName(run.Chapter):run!=null&&run.Chapter==3?"태양의 집행관 아우렐":run!=null&&run.Chapter==2?"왕실의 간수 모르가":"붉은 날개의 목소리"):voice==StoryVoice.Mother?(run!=null&&run.Chapter==7&&run.BossDefeated?"함께 돌아갈 어머니":run!=null&&run.Chapter>=3&&!village?"봉인 너머의 어머니":"어머니의 기억"):voice==StoryVoice.Narrator?"하늘에 남은 별실":"별실에 남은 목소리";
            DrawStorySkyPortrait(rect,caption,thread);
        }
        void DrawStory()
        {
            hits.Clear();Box(new Rect(0,0,1280,720),new Color(.012f,.015f,.03f,.55f));
            Text(storyHeading,100,48,1080,45,28,HudLight,TextAnchor.MiddleCenter);
            var line=storyLines[storyPage];
            GothicPlaque(new Rect(85,355,1110,330),16,HudGold);
            GothicPlaque(new Rect(88,358,1104,324),14,HudInk);
            DrawStoryPortrait(line.Voice,new Rect(110,377,215,270),line.Expression);
            Text(StoryVoiceName(line.Voice),355,380,805,40,27,HudLight);
            Text(line.Words,355,436,795,139,24,new Color(.9f,.9f,.86f));
            Text($"{storyPage+1} / {storyLines.Length}",355,611,130,32,18);
            EquipmentButton("이 장면 넘기기",new Rect(540,607,230,46),EndStory);
            EquipmentButton(storyPage==storyLines.Length-1?"계속하기 · Space":"다음 · Space",new Rect(800,607,350,46),AdvanceStory);
            Text("Space / 다음 버튼 · Esc 일시정지",355,660,795,20,14,new Color(.55f,.64f,.66f));
        }
    }
}
