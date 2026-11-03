using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        PlayerProgression playerProgression;
        int savedProgressionRevision;
        int serviceNpc=-1;
        string serviceNotice="";
        StoryExpression serviceExpression=StoryExpression.Warm;
        const string ProgressionKey="sunfall.growth.v1.";
        bool CanTrade=>village&&!paused&&!settings&&!inventoryOpen&&serviceNpc>=0&&serviceNpc<2&&Mathf.Abs(villageX-villageNpcs[serviceNpc].X)<=145;
        void LoadProgression()
        {
            var ranks=new int[PlayerProgression.StatCount];
            for(int i=0;i<ranks.Length;i++)ranks[i]=PlayerPrefs.GetInt(ProgressionKey+"rank."+i,0);
            playerProgression=new PlayerProgression(PlayerPrefs.GetInt(ProgressionKey+"gold",0),PlayerPrefs.GetInt(ProgressionKey+"level",1),PlayerPrefs.GetInt(ProgressionKey+"xp",0),ranks);
            savedProgressionRevision=playerProgression.Revision;
        }
        void StoreProgression()
        {
            PlayerPrefs.SetInt(ProgressionKey+"gold",playerProgression.Gold);
            PlayerPrefs.SetInt(ProgressionKey+"level",playerProgression.Level);
            PlayerPrefs.SetInt(ProgressionKey+"xp",playerProgression.Experience);
            for(int i=0;i<PlayerProgression.StatCount;i++)PlayerPrefs.SetInt(ProgressionKey+"rank."+i,playerProgression.Rank((GrowthStat)i));
            savedProgressionRevision=playerProgression.Revision;
        }
        void DrawProgressionStatus(float x,float y,float width)
        {
            if(width<300)
            {
                Text($"{playerProgression.Gold:N0} G · Lv.{playerProgression.Level}",x,y,width,24,17,HudLight);
                Text($"성장 {playerProgression.Points} P · XP {playerProgression.Experience}",x,y+26,width,20,14,new Color(.63f,.73f,.73f));return;
            }
            Text($"{playerProgression.Gold:N0} G   ·   Lv.{playerProgression.Level}   ·   성장 {playerProgression.Points} P",x,y,width,28,19,HudLight);
            Text(playerProgression.Level==PlayerProgression.MaxLevel?"최고 레벨":$"경험치 {playerProgression.Experience} / {playerProgression.NextLevelExperience}",x,y+27,width,20,14,new Color(.63f,.73f,.73f));
        }
        void Trade(EquipmentId id)
        {
            if(!CanTrade)return;
            bool upgraded=serviceNpc==0;
            bool success=upgraded?playerProgression.Upgrade(playerEquipment,id):playerProgression.Buy(playerEquipment,id);
            serviceExpression=success?StoryExpression.Warm:StoryExpression.Worried;
            serviceNotice=success?playerEquipment.DisplayName(id)+(upgraded?" 강화 완료":" 구매 완료 · 인벤토리에서 장착하세요"):"보유 여부와 골드, 강화 단계를 확인하세요.";
            if(success)SaveEquipment();Block();
        }
        void DrawVillageService()
        {
            Overlay();bool smith=serviceNpc==0;
            GothicPlaque(new Rect(55,36,1170,648),20,HudGold);
            GothicPlaque(new Rect(58,39,1164,642),18,HudInk);
            Text(smith?"불씨 대장간":"떠돌이 상인의 장비",93,63,540,50,34,HudLight);
            Text($"보유 {playerProgression.Gold:N0} G",650,72,320,36,24,HudLight,TextAnchor.MiddleRight);
            EquipmentButton("닫기 · Esc",new Rect(1000,65,180,43),()=>{serviceNpc=-1;Block();});
            Text(smith?"보유 장비를 골드로 강화합니다. 성공률 100% · 최대 +5 · 미장착 장비도 강화 가능":"장비 5종을 판매합니다. 이미 가진 장비는 중복 구매할 수 없습니다.",95,124,1080,36,18,new Color(.72f,.8f,.79f));
            DrawStoryPortrait(smith?StoryVoice.Smith:StoryVoice.Merchant,new Rect(95,183,220,320),serviceExpression);
            Text(smith?"재단검을 맡기거라.\n어머니께 돌아갈 길을\n함께 준비하자.":"그분의 집에 다시\n등불이 켜질 때까지,\n나도 힘을 보태겠네.",95,516,220,90,18,HudLight,TextAnchor.MiddleCenter);
            for(int i=0;i<EquipmentCatalog.Count;i++)
            {
                var id=(EquipmentId)i;float y=180+i*82;
                int level=playerEquipment.UpgradeLevel(id),cost=smith?playerEquipment.UpgradeCost(id):EquipmentCatalog.Price(id);
                bool owned=playerEquipment.Owns(id),max=level>=EquipmentInventory.MaxUpgrade;
                bool eligible=smith?owned&&!max:!owned;
                bool affordable=eligible&&playerProgression.Gold>=cost;
                GothicPlaque(new Rect(340,y,840,72),8,HudIron);
                DrawEquipmentIcon(id,new Vector2(374,y+35),owned||!smith?HudLight:new Color(.39f,.43f,.46f));
                Text(playerEquipment.DisplayName(id),411,y+5,520,27,20,HudLight);
                string effect=EquipmentInventory.EffectAt(id,level);
                if(smith&&owned&&!max)effect+=" → "+EquipmentInventory.EffectAt(id,level+1);
                Text(effect,411,y+36,560,30,15);
                string label=smith?(!owned?"미보유":max?"최대 +5":$"강화 {cost} G"):(owned?"보유 중":$"구매 {cost} G");
                if(eligible&&!affordable)label=$"{cost} G · 부족";
                EquipmentButton(label,new Rect(985,y+16,180,40),CanTrade&&affordable?(System.Action)(()=>Trade(id)):null);
            }
            EquipmentButton("이야기 듣기",new Rect(95,620,220,40),()=>{selectedVillageNpc=serviceNpc;serviceNpc=-1;dialoguePage=0;Block();});
            Text(serviceNotice,340,612,610,54,17,HudLight);
            EquipmentButton("인벤토리",new Rect(985,620,195,40),()=>{serviceNpc=-1;growthTab=false;inventoryOpen=true;Block();});
        }
        void SpendGrowthPoint(GrowthStat stat)
        {
            if(!village||!inventoryOpen||!growthTab||paused||settings)return;
            if(playerProgression.Allocate(stat))SaveEquipment();Block();
        }
        void DrawGrowthContents()
        {
            Text(village?"능력치마다 최대 10단계. 포인트를 배분하면 다음 전투부터 적용됩니다.":"성장은 마을에서 배분할 수 있습니다. 현재는 능력치 확인만 가능합니다.",95,124,1080,36,18,new Color(.72f,.8f,.79f));
            for(int i=0;i<PlayerProgression.StatCount;i++)
            {
                var stat=(GrowthStat)i;float y=180+i*82;int rank=playerProgression.Rank(stat);
                GothicPlaque(new Rect(92,y,600,72),8,HudIron);
                GothicDiamond(124,y+36,12,HudGold);
                Text(PlayerProgression.Name(stat)+$"   {rank} / 10",155,y+7,360,28,22,HudLight);
                Text(PlayerProgression.Effect(stat),155,y+39,370,26,17);
                bool can=village&&playerProgression.Points>0&&rank<PlayerProgression.MaxRank;
                EquipmentButton(rank==PlayerProgression.MaxRank?"최대":playerProgression.Points==0?"포인트 없음":"+1 · 1 P",new Rect(545,y+16,126,40),can?(System.Action)(()=>SpendGrowthPoint(stat)):null);
            }
            DrawProgressionStatus(735,180,440);
            Text($"남은 포인트  {playerProgression.Points} P",735,259,440,40,28,HudLight);
            Text($"검 피해  {run.SwordDamage:0.0}\n"+
                 $"마법 기본 위력  {run.MagicDamage:0.0}\n"+
                 $"최대 체력  {run.MaxHp:0}\n"+
                 $"받는 피해 감소  {playerProgression.Defense:0}\n"+
                 $"이동 속도  {6*playerProgression.MoveMultiplier:0.00}",735,323,440,180,23);
            Text("마법 위력에 각 스킬의 배율이 적용됩니다.\n방어력은 검·화살·보스 공격에 모두 적용됩니다.",735,520,440,65,17,new Color(.64f,.72f,.72f));
            Text("처음 2 P 지급 · 몬스터 처치 경험치로 레벨업 시 2 P · 최대 Lv.25",95,610,1080,28,18,HudLight);
            Text("골드·레벨·배분한 능력치·강화는 사망하거나 게임을 다시 켜도 유지됩니다.",95,644,1080,24,16,new Color(.59f,.66f,.68f));
        }
        void DrawKillRewards()
        {
            foreach(var enemy in run.Enemies)
            {
                if(enemy.RewardTimer<=0)continue;
                float age=1.6f-enemy.RewardTimer,x=SX(enemy.X),y=SY(enemy.Y)-94-age*23;
                float alpha=Mathf.Clamp01(enemy.RewardTimer/.4f);
                for(int i=0;i<3;i++)
                {
                    float cx=x+(i-1)*(8+age*16),cy=y-15-Mathf.Sin(Mathf.Min(1,age)*Mathf.PI)*20;
                    GothicDisc(cx,cy,5,new Color(.95f,.67f,.19f,alpha));GothicLine(new Vector2(cx,cy-3),new Vector2(cx,cy+3),new Color(1,.91f,.57f,alpha),1);
                }
                Text($"+{enemy.GoldReward} G  +{enemy.ExperienceReward} XP",x-100,y,200,28,18,new Color(1,.84f,.42f,alpha),TextAnchor.MiddleCenter);
            }
        }
        void DrawDamageNumbers()
        {
            foreach(var hit in run.DamageNumbers)
            {
                float age=.75f-hit.Remaining;
                float x=SX(hit.X)+(hit.Sword?-15:15),y=SY(hit.Y)-22-age*55;
                float alpha=Mathf.Clamp01(hit.Remaining/.2f);
                var color=hit.Sword?new Color(1,.87f,.57f,alpha):new Color(.78f,.7f,1,alpha);
                string amount=hit.Amount.ToString("0.#");
                Text(amount,x-60+1,y+1,120,30,22,new Color(.02f,.02f,.03f,alpha),TextAnchor.MiddleCenter);
                Text(amount,x-60,y,120,30,22,color,TextAnchor.MiddleCenter);
            }
        }
    }
}
