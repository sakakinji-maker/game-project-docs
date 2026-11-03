using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        EquipmentInventory playerEquipment;
        bool inventoryOpen;
        bool growthTab;
        string equipmentNotice="";
        const string EquipmentSaveKey="sunfall.equipment.v1.";
        bool CanToggleInventory=>!skillWindowOpen&&!title&&!paused&&!settings&&!StoryActive&&serviceNpc<0&&(village?dialoguePage<0:run!=null&&!run.Cinematic&&!run.SkillChoicePending);
        bool CanEditEquipment=>village||run!=null&&run.CanChangeEquipment;

        void LoadEquipment()
        {
            var upgrades=new int[EquipmentCatalog.Count];
            for(int i=0;i<upgrades.Length;i++)upgrades[i]=PlayerPrefs.GetInt(EquipmentSaveKey+"upgrade."+i,0);
            playerEquipment=new EquipmentInventory(PlayerPrefs.GetInt(EquipmentSaveKey+"owned",0),upgrades);
            for(int i=0;i<4;i++)
            {
                var id=(EquipmentId)PlayerPrefs.GetInt(EquipmentSaveKey+i,-1);
                if(EquipmentCatalog.Valid(id)&&(int)EquipmentCatalog.Slot(id)==i)playerEquipment.Equip(id);
            }
            LoadProgression();
        }
        void SaveEquipment()
        {
            PlayerPrefs.SetInt(EquipmentSaveKey+"owned",playerEquipment.OwnedMask);
            for(int i=0;i<4;i++)PlayerPrefs.SetInt(EquipmentSaveKey+i,(int)playerEquipment.Equipped((EquipmentSlot)i));
            for(int i=0;i<EquipmentCatalog.Count;i++)PlayerPrefs.SetInt(EquipmentSaveKey+"upgrade."+i,playerEquipment.UpgradeLevel((EquipmentId)i));
            StoreProgression();
            PlayerPrefs.Save();
        }
        void ToggleInventory()
        {
            if(!CanToggleInventory)return;
            inventoryOpen=!inventoryOpen;equipmentNotice="";Block();
        }
        void ChangeEquipment(EquipmentId id,bool remove)
        {
            if(!inventoryOpen||!CanEditEquipment)return;
            float beforeSword=run.SwordDamage,beforeMagic=run.MagicDamage;
            var slot=EquipmentCatalog.Slot(id);
            bool changed=village?(remove?playerEquipment.Unequip(slot):playerEquipment.Equip(id)):(remove?run.Unequip(slot):run.Equip(id));
            if(changed)
            {
                equipmentNotice=$"검 {beforeSword:0.0} → {run.SwordDamage:0.0} · 마법 {beforeMagic:0.0} → {run.MagicDamage:0.0}";
                SaveEquipment();
            }
            Block();
        }
        void EquipmentButton(string label,Rect r,System.Action action)
        {
            var tint=action==null?new Color(.29f,.31f,.33f):HudGold;
            GothicPlaque(r,8,tint);GothicPlaque(new Rect(r.x+2,r.y+2,r.width-4,r.height-4),7,HudInk);
            Text(label,r.x,r.y,r.width,r.height,17,action==null?new Color(.47f,.51f,.53f):HudLight,TextAnchor.MiddleCenter);
            if(action!=null)hits.Add(new Hit(r,action));
        }
        void DrawInventoryEntry()
        {
            if(!CanToggleInventory)return;
            EquipmentButton("인벤토리 (I)",village?new Rect(1095,78,150,35):new Rect(1095,102,150,22),ToggleInventory);
        }
        void DrawInventory()
        {
            Overlay();
            GothicPlaque(new Rect(55,36,1170,648),20,HudGold);
            GothicPlaque(new Rect(58,39,1164,642),18,HudInk);
            Text(growthTab?"여행자의 성장":"여행자의 장비",93,63,480,50,34,HudLight);
            EquipmentButton(growthTab?"장비 보기":$"능력치 · {playerProgression.Points} P",new Rect(740,65,230,43),()=>{growthTab=!growthTab;Block();});
            EquipmentButton("닫기 · I / Esc",new Rect(1000,65,180,43),ToggleInventory);
            if(growthTab){DrawGrowthContents();return;}
            Text(CanEditEquipment?"장검은 검 피해, 화로의 심장은 마법 피해를 높입니다. 같은 부위의 장비는 교체됩니다.":"열람 중 · 마을 또는 적을 모두 처치한 방에서, 행동을 마친 뒤 변경할 수 있습니다.",95,124,1080,36,18,new Color(.72f,.8f,.79f));
            for(int i=0;i<EquipmentCatalog.Count;i++)
            {
                var id=(EquipmentId)i;float y=180+i*82;
                bool owned=playerEquipment.Owns(id),equipped=playerEquipment.Has(id);
                GothicPlaque(new Rect(92,y,600,72),8,equipped?new Color(.26f,.37f,.34f):HudIron);
                DrawEquipmentIcon(id,new Vector2(126,y+35),owned?HudLight:new Color(.39f,.43f,.46f));
                Text(playerEquipment.DisplayName(id),165,y+7,350,28,21,owned?HudLight:new Color(.55f,.59f,.61f));
                Text(EquipmentCatalog.SlotName(EquipmentCatalog.Slot(id))+" · "+EquipmentInventory.EffectAt(id,playerEquipment.UpgradeLevel(id)),165,y+39,360,26,16);
                EquipmentButton(!owned?"미획득":equipped?"해제":playerEquipment.Equipped(EquipmentCatalog.Slot(id))!=EquipmentId.None?"교체 장착":"장착",
                    new Rect(545,y+16,126,40),owned&&CanEditEquipment?(System.Action)(()=>ChangeEquipment(id,equipped)):null);
            }
            Text("장착 중",735,174,420,30,22,HudLight);
            for(int i=0;i<4;i++)
            {
                var slot=(EquipmentSlot)i;var id=playerEquipment.Equipped(slot);float y=217+i*53;
                GothicLine(new Vector2(735,y+40),new Vector2(1175,y+40),HudIron,1);
                Text(EquipmentCatalog.SlotName(slot),738,y,85,30,18,new Color(.6f,.7f,.7f));
                if(id!=EquipmentId.None)DrawEquipmentIcon(id,new Vector2(846,y+14),HudLight);
                Text(playerEquipment.DisplayName(id),880,y,292,32,19,id==EquipmentId.None?new Color(.4f,.47f,.49f):HudLight);
            }
            Text("적용 능력치",735,445,440,30,22,HudLight);
            Text($"검 피해  {run.SwordDamage:0.0}  (장비 +{run.SwordDamage-run.BaseSwordDamage:0.0})\n"+
                 $"마법 위력  {run.MagicDamage:0.0}  (장비 +{run.MagicDamage-run.BaseMagicDamage:0.0})\n"+
                 $"최대 체력 {run.MaxHp:0} · 방어력 {playerProgression.Defense:0}\n"+
                 $"구르기 재사용 {run.EffectiveRollReuse:0.00}초",
                 738,480,450,100,18);
            Text(equipmentNotice,738,582,450,25,16,HudGold);
            Text($"보유 {playerProgression.Gold:N0} G · 상인 구매 / 대장장이 강화 · 보스 상자에서 미보유 장비 획득",95,610,1080,28,18,HudLight);
            Text("유물은 1개만 장착 · 외투 장착으로 현재 체력이 회복되지는 않습니다.",95,644,1080,24,16,new Color(.59f,.66f,.68f));
        }
        // Shared by inventory, equipped slots, merchant, smith and chest rewards.
        void DrawEquipmentIcon(EquipmentId id,Vector2 c,Color color)
        {
            if(DrawEquipmentImage(id,c,color))return;
            if(id==EquipmentId.AshSword){DrawGothicSkillSymbol(c,2,color);return;}
            if(id==EquipmentId.FurnaceHeart){DrawGothicFlame(c,.8f,color);return;}
            if(id==EquipmentId.SanctumSeal)
            {
                for(int i=0;i<12;i++)
                {
                    float a=i*Mathf.PI/6,b=a+.37f;
                    GothicLine(c+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*19,c+new Vector2(Mathf.Cos(b),Mathf.Sin(b))*19,color,2);
                }
                GothicDiamond(c.x,c.y,10,new Color(.4f,.75f,.7f));return;
            }
            if(id==EquipmentId.PilgrimCoat)
            {
                GothicPolygon(color,c+new Vector2(-7,-20),c+new Vector2(7,-20),c+new Vector2(23,20),c+new Vector2(-23,20));
                GothicLine(c+new Vector2(0,-12),c+new Vector2(0,18),HudInk,3);
                GothicDiamond(c.x,c.y-12,4,HudGold);return;
            }
            GothicPolygon(color,c+new Vector2(-13,-18),c+new Vector2(5,-18),c+new Vector2(5,7),c+new Vector2(20,12),c+new Vector2(20,20),c+new Vector2(-13,20));
            GothicPolygon(new Color(.72f,.25f,.33f),c+new Vector2(-9,-6),c+new Vector2(-24,-17),c+new Vector2(-17,4));
        }
    }
}
