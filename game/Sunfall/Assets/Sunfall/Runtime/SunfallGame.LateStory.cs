namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        static readonly StoryLine[][] LateArrivalStories={
            new[]{new StoryLine(StoryVoice.Narrator,"심장의 명령이 멎자 거울로 된 길이 열렸다. 왕은 사람의 몸뿐 아니라 기억까지 매듭에 묶어 두었다."),new StoryLine(StoryVoice.Mother,"네 얼굴은 기억나는데… 너를 부르던 이름이 멀어지는구나."),new StoryLine(StoryVoice.Hero,"제가 기억해요. 처음 문을 열어 주신 날도, 저를 딸이라고 부르신 목소리도. 이번에는 제가 찾아드릴게요.",StoryExpression.Sad)},
            new[]{new StoryLine(StoryVoice.Narrator,"거울에서 돌아온 기억이 별실을 환히 밝혔다. 그러나 왕좌로 가는 하늘은 일식의 날개에 가려져 있었다."),new StoryLine(StoryVoice.Mother,"이제 기억나. 네가 돌아올 때마다 등불을 켜 두었지. 이번에는 네 빛이 엄마를 부르는구나."),new StoryLine(StoryVoice.Hero,"이 밤을 넘으면 왕에게 닿아요. 함께 돌아갈 길도 남겨 둘게요.",StoryExpression.Determined)},
            new[]{new StoryLine(StoryVoice.Narrator,"첨탑 끝의 장막이 찢어졌다. 그 너머에는 태양을 삼켜 악마가 된 왕과, 그의 마지막 매듭에 묶인 어머니가 있었다."),new StoryLine(StoryVoice.Mother,"왔구나… 엄마 때문에 네가 여기 남으면 안 돼."),new StoryLine(StoryVoice.Hero,"아무도 대신 남지 않을 거예요. 이 실은 사람을 묶는 사슬이 아니라, 돌아갈 길을 꿰매는 실이니까요.",StoryExpression.Determined)}
        };
        static readonly StoryLine[][] LateBossArrivalStories={
            new[]{new StoryLine(StoryVoice.Boss,"왕의 명령은 심장이 멎어도 돌아간다. 나는 바르칸. 이 장치를 멈출 자는 없다."),new StoryLine(StoryVoice.Hero,"어머니의 심장으로 움직이는 장치라면, 여기서 끝내겠어.",StoryExpression.Determined),new StoryLine(StoryVoice.Boss,"그 여자의 몸을 풀어도 기억은 거울에 남는다. 너는 얼마나 더 잃을 셈이지?",StoryExpression.Scorn),new StoryLine(StoryVoice.Hero,"잃는 건 왕의 소유라는 거짓말뿐이야. 그 망치로 날 막아 봐.",StoryExpression.Determined)},
            new[]{new StoryLine(StoryVoice.Boss,"나는 리세아. 웃음도, 분노도, 눈물도 이 탈 안에 가두었지. 네 어머니도 자신이 누구인지 잊으면 편안해질 텐데.",StoryExpression.Scorn),new StoryLine(StoryVoice.Mother,"문 앞에서 떨던 아이… 내가 손을 잡아 주었는데…"),new StoryLine(StoryVoice.Hero,"그 손이 저를 살렸어. 우리의 기억을 왕의 감옥으로 만들지 마.",StoryExpression.Determined),new StoryLine(StoryVoice.Boss,"그렇다면 똑똑히 보아라. 탈이 바뀔 때마다 네 기억도 하나씩 깨질 것이다.",StoryExpression.Angry)},
            new[]{new StoryLine(StoryVoice.Boss,"이 종은 새벽을 알리지 않는다. 나는 녹티스. 내 날개 아래에서는 태양도 길을 잃지."),new StoryLine(StoryVoice.Hero,"어머니에게 닿은 기억도, 하늘을 꿰맨 빛도 되찾았어. 네 날개로 가릴 수 없어.",StoryExpression.Determined),new StoryLine(StoryVoice.Boss,"이 낫을 넘어 왕에게 닿더라도, 빛을 돌려주려면 누군가 매듭을 대신해야 한다. 네 어머니처럼.",StoryExpression.Scorn),new StoryLine(StoryVoice.Hero,"그건 왕이 만든 규칙이지. 나는 새로 꿰매겠어.",StoryExpression.Determined)},
            new[]{new StoryLine(StoryVoice.Boss,"가시 왕좌 앞에 섰구나. 내가 솔베인이다. 이 검에 묶인 네 어미 하나로 왕국의 낮을 지킬 수 있다.",StoryExpression.Scorn),new StoryLine(StoryVoice.Hero,"밖에는 밤밖에 없어. 당신이 움켜쥔 건 왕국이 아니라 어머니와 찢어진 하늘이야.",StoryExpression.Determined),new StoryLine(StoryVoice.Mother,"왕의 손을 베더라도 실은 놓지 마. 그 틈으로 사람들이 다시 떨어질 거야."),new StoryLine(StoryVoice.Hero,"알아요. 왕관의 매듭은 끊고, 하늘의 틈은 제 실로 이어 붙일게요. 우리 둘 다 돌아가요.",StoryExpression.Determined),new StoryLine(StoryVoice.Boss,"태양 없는 왕은 왕이 아니다. 마지막 마녀여, 네 빛까지 내 것이 될 것이다!",StoryExpression.Angry)}
        };
        static readonly StoryLine[][] LateBossDefeatStories={
            new[]{new StoryLine(StoryVoice.Boss,"톱니가… 멈춘다. 왕의 명령은… 거울 너머에…",StoryExpression.Pained),new StoryLine(StoryVoice.Narrator,"심장 장치가 붉은 가루로 부서졌다. 어머니를 옥죄던 고리가 느슨해졌지만, 실 끝의 목소리는 낯설게 흔들렸다."),new StoryLine(StoryVoice.Mother,"내가… 누구를 기다리고 있었더라…?"),new StoryLine(StoryVoice.Hero,"괜찮아요. 저를 잊으셔도 제가 어머니를 기억해요. 거울 속에 남은 기억까지 찾아올게요.",StoryExpression.Warm)},
            new[]{new StoryLine(StoryVoice.Boss,"내 탈을 깨고도… 흩어진 기억을 다시 잇다니…",StoryExpression.Shocked),new StoryLine(StoryVoice.Narrator,"부서진 거울마다 작은 식탁과 따뜻한 등불이 비쳤다. 나는 흩어진 기억을 별실로 이어 어머니에게 돌려드렸다."),new StoryLine(StoryVoice.Mother,"돌아오면 문을 두드리라고 했지. 이제야 네가 누군지 또렷하게 보이는구나, 내 딸."),new StoryLine(StoryVoice.Hero,"이번엔 같이 문을 열어요. 왕좌로 가는 길만 남았어요.",StoryExpression.Warm)},
            new[]{new StoryLine(StoryVoice.Boss,"왕의 밤이… 날개 사이로 새어 나간다…",StoryExpression.Resigned),new StoryLine(StoryVoice.Narrator,"검은 깃털이 흩어지며 낫의 붉은빛이 꺼졌다. 무너진 성소 위로 금빛 바늘땀이 남았다. 왕좌로 이어진 별실 끝에서, 어머니의 손이 느껴졌다."),new StoryLine(StoryVoice.Mother,"느껴져. 네 손이 이렇게 따뜻했구나."),new StoryLine(StoryVoice.Hero,"이제 마지막이에요. 다시는 그 손을 놓지 않을게요.",StoryExpression.Warm)},
            new[]{new StoryLine(StoryVoice.Boss,"왕관이… 태양을 놓는다고…? 내 영원한 낮은…",StoryExpression.Shocked),new StoryLine(StoryVoice.Hero,"낮은 누구의 소유도 아니야. 이제 그 손을 놓아.",StoryExpression.Determined),new StoryLine(StoryVoice.Narrator,"왕관이 갈라지고 왕의 붉은 대검이 빛을 잃었다. 나는 어머니를 묶은 매듭만 잘라 내고, 되찾은 별실과 태양빛으로 찢어진 하늘의 양쪽을 이어 붙였다. 사람을 제물로 삼던 왕의 명령이 마침내 끝났다."),new StoryLine(StoryVoice.Mother,"정말… 아무도 여기 남지 않아도 되는 거니?"),new StoryLine(StoryVoice.Hero,"네. 이제 하늘이 스스로 빛을 품을 수 있어요. 어머니, 집에 가요.",StoryExpression.Warm),new StoryLine(StoryVoice.Narrator,"어머니의 손이 내 손에 포개졌다. 금빛 봉합 자국 너머로 태양이 떠올랐다. 상처는 지워지지 않았지만, 그 자리는 더는 누구도 삼키지 않았다."),new StoryLine(StoryVoice.Mother,"오늘은 네가 문을 두드리지 않아도 되겠구나. 함께 돌아왔으니까.")}
        };
        string LateDefeatHeading=>run.Chapter==4?"멎어 버린 왕의 심장":run.Chapter==5?"되찾은 어머니의 이름":run.Chapter==6?"밤을 가르는 마지막 바늘땀":"함께 돌아온 아침";
    }
}
