using System;
using System.Collections.Generic;

namespace Sunfall.Core
{
    public enum RunPhase { Combat, Won, Dead }
    public enum RunStage { Approach, BossArena }
    public enum EnemyKind { Melee, Archer, Heavy }
    public enum BossPattern { GroundWave, Dash, BloodBolts, ChainSlam, ChainBind, ChainStorm, SolarSweep, SolarLances, Eclipse, HammerSweep, HeartLances, GearVolley, MirrorStep, MirrorFan, MirrorPrison, CrescentDive, MoonVolley, NightVeil, RoyalCleave, Sunfall, LastEclipse }
    public enum SpellKind { EmberWave, AshLance, CrimsonNova, ChainPull, ChainRain, SunLance, DawnBurst, HeartBreaker, CrownGear, MirrorShards, MirrorWard, MoonBlades, NightMantle, RoyalLance, SolarCrown }
    public enum EquipmentSlot { Weapon, Armor, Boots, Relic }
    public enum EquipmentId { None=-1, AshSword, PilgrimCoat, WingBoots, FurnaceHeart, SanctumSeal }
    public enum GrowthStat { Attack, Magic, Health, Defense, Agility }
    public sealed class DamageNumber
    {
        public float X,Y,Amount,Remaining=.75f;
        public bool Sword;
    }
    public sealed class PlayerProgression
    {
        public const int StatCount=5, MaxRank=10, MaxLevel=25, MaxGold=9999999;
        readonly int[] ranks=new int[StatCount];
        public int Gold {get;private set;}
        public int Level {get;private set;}
        public int Experience {get;private set;}
        public int Revision {get;private set;}
        public int NextLevelExperience=>Level>=MaxLevel?0:100+(Level-1)*50;
        public int Points {get {int spent=0;foreach(int rank in ranks)spent+=rank;return Level*2-spent;}}
        public PlayerProgression(int gold=0,int level=1,int experience=0,int[] savedRanks=null)
        {
            Gold=Math.Max(0,Math.Min(MaxGold,gold));Level=Math.Max(1,Math.Min(MaxLevel,level));
            Experience=Level==MaxLevel?0:Math.Max(0,Math.Min(NextLevelExperience-1,experience));
            for(int i=0;i<StatCount;i++)if(savedRanks!=null&&i<savedRanks.Length)ranks[i]=Math.Max(0,Math.Min(Math.Min(MaxRank,Points),savedRanks[i]));
        }
        public int Rank(GrowthStat stat)=>(int)stat>=0&&(int)stat<StatCount?ranks[(int)stat]:0;
        public bool Allocate(GrowthStat stat)
        {
            if((int)stat<0||(int)stat>=StatCount||Points<=0||Rank(stat)>=MaxRank)return false;
            ranks[(int)stat]++;Revision++;return true;
        }
        public static string Name(GrowthStat stat)=>stat==GrowthStat.Attack?"공격력":stat==GrowthStat.Magic?"마력":stat==GrowthStat.Health?"체력":stat==GrowthStat.Defense?"방어력":"민첩";
        public static string Effect(GrowthStat stat)=>stat==GrowthStat.Attack?"검 기본 피해 +2":stat==GrowthStat.Magic?"마법 기본 위력 +2":stat==GrowthStat.Health?"최대 체력 +10":stat==GrowthStat.Defense?"받는 피해 -1 (최소 1)":"이동 속도 +2%";
        public float AttackBonus=>Rank(GrowthStat.Attack)*2;
        public float MagicBonus=>Rank(GrowthStat.Magic)*2;
        public float HealthBonus=>Rank(GrowthStat.Health)*10;
        public float Defense=>Rank(GrowthStat.Defense);
        public float MoveMultiplier=>1+Rank(GrowthStat.Agility)*.02f;
        public bool RewardKill(Enemy enemy)
        {
            if(enemy==null||enemy.Hp>0||enemy.RewardPaid)return false;
            enemy.RewardPaid=true;enemy.RewardTimer=1.6f;
            Gold=(int)Math.Min(MaxGold,(long)Gold+enemy.GoldReward);
            if(Level<MaxLevel)
            {
                Experience+=enemy.ExperienceReward;
                while(Level<MaxLevel&&Experience>=NextLevelExperience){Experience-=NextLevelExperience;Level++;}
                if(Level==MaxLevel)Experience=0;
            }
            Revision++;return true;
        }
        public bool Buy(EquipmentInventory equipment,EquipmentId id)
        {
            if(equipment==null||!EquipmentCatalog.Valid(id)||equipment.Owns(id))return false;
            int cost=EquipmentCatalog.Price(id);if(Gold<cost)return false;
            equipment.Grant(id);Gold-=cost;Revision++;return true;
        }
        public bool Upgrade(EquipmentInventory equipment,EquipmentId id)
        {
            if(equipment==null||!equipment.Owns(id)||equipment.UpgradeLevel(id)>=EquipmentInventory.MaxUpgrade)return false;
            int cost=equipment.UpgradeCost(id);if(Gold<cost)return false;
            if(!equipment.Upgrade(id))return false;
            Gold-=cost;Revision++;return true;
        }
    }
    public static class EquipmentCatalog
    {
        public const int Count=5;
        public static int Price(EquipmentId id)=>id==EquipmentId.AshSword?100:id==EquipmentId.PilgrimCoat?120:id==EquipmentId.WingBoots?90:160;
        public static bool Valid(EquipmentId id)=>(int)id>=0&&(int)id<Count;
        public static EquipmentSlot Slot(EquipmentId id)=>id==EquipmentId.AshSword?EquipmentSlot.Weapon:id==EquipmentId.PilgrimCoat?EquipmentSlot.Armor:id==EquipmentId.WingBoots?EquipmentSlot.Boots:EquipmentSlot.Relic;
        public static string Name(EquipmentId id)
        {
            switch(id){case EquipmentId.AshSword:return "재의 장검";case EquipmentId.PilgrimCoat:return "검은 순례자의 외투";case EquipmentId.WingBoots:return "붉은 날개 장화";case EquipmentId.FurnaceHeart:return "화로의 심장";case EquipmentId.SanctumSeal:return "성소의 인장";default:return "비어 있음";}
        }
        public static string Effect(EquipmentId id)=>EquipmentInventory.EffectAt(id,0);
        public static string SlotName(EquipmentSlot slot)=>slot==EquipmentSlot.Weapon?"무기":slot==EquipmentSlot.Armor?"방어구":slot==EquipmentSlot.Boots?"장화":"유물";
    }
    // Collection and loadout persist between journeys; combat values are derived, never stacked.
    public sealed class EquipmentInventory
    {
        public const int MaxUpgrade=5;
        readonly int[] upgrades=new int[EquipmentCatalog.Count];
        readonly EquipmentId[] equipped={EquipmentId.None,EquipmentId.None,EquipmentId.None,EquipmentId.None};
        public int OwnedMask {get;private set;}
        public EquipmentInventory(int ownedMask=0,int[] savedUpgrades=null)
        {
            OwnedMask=ownedMask&31;
            for(int i=0;i<upgrades.Length;i++)if(Owns((EquipmentId)i)&&savedUpgrades!=null&&i<savedUpgrades.Length)upgrades[i]=Math.Max(0,Math.Min(MaxUpgrade,savedUpgrades[i]));
        }
        public int UpgradeLevel(EquipmentId id)=>EquipmentCatalog.Valid(id)?upgrades[(int)id]:0;
        public int UpgradeCost(EquipmentId id)=>Owns(id)&&UpgradeLevel(id)<MaxUpgrade?60*(UpgradeLevel(id)+1):0;
        internal bool Upgrade(EquipmentId id){if(!Owns(id)||UpgradeLevel(id)>=MaxUpgrade)return false;upgrades[(int)id]++;return true;}
        public string DisplayName(EquipmentId id)=>EquipmentCatalog.Name(id)+(UpgradeLevel(id)>0?" +"+UpgradeLevel(id):"");
        // A single source for shop descriptions, upgrade previews and combat.
        public static int DamageBonusPercent(EquipmentId id,int level)
        {
            level=Math.Max(0,Math.Min(MaxUpgrade,level));
            return id==EquipmentId.AshSword?30+level*8:id==EquipmentId.FurnaceHeart?25+level*7:0;
        }
        public static string EffectAt(EquipmentId id,int level)
        {
            level=Math.Max(0,Math.Min(MaxUpgrade,level));
            switch(id){case EquipmentId.AshSword:return $"검 피해 +{DamageBonusPercent(id,level)}%";case EquipmentId.PilgrimCoat:return $"최대 체력 +{20+level*4}";case EquipmentId.WingBoots:return $"구르기 재사용 -{.1f+level*.01f:0.00}초";case EquipmentId.FurnaceHeart:return $"마법 피해 +{DamageBonusPercent(id,level)}%";case EquipmentId.SanctumSeal:return $"마법 재사용 -{10+level}%";default:return "";}
        }
        public bool Owns(EquipmentId id)=>EquipmentCatalog.Valid(id)&&(OwnedMask&(1<<(int)id))!=0;
        public bool Grant(EquipmentId id){if(!EquipmentCatalog.Valid(id)||Owns(id))return false;OwnedMask|=1<<(int)id;return true;}
        public EquipmentId Equipped(EquipmentSlot slot)=>(int)slot>=0&&(int)slot<4?equipped[(int)slot]:EquipmentId.None;
        public bool Has(EquipmentId id)=>EquipmentCatalog.Valid(id)&&Equipped(EquipmentCatalog.Slot(id))==id;
        public bool Equip(EquipmentId id){if(!Owns(id)||Has(id))return false;equipped[(int)EquipmentCatalog.Slot(id)]=id;return true;}
        public bool Unequip(EquipmentSlot slot){if(Equipped(slot)==EquipmentId.None)return false;equipped[(int)slot]=EquipmentId.None;return true;}
        public float SwordMultiplier=>Has(EquipmentId.AshSword)?1+DamageBonusPercent(EquipmentId.AshSword,UpgradeLevel(EquipmentId.AshSword))/100f:1;
        public float HealthBonus=>Has(EquipmentId.PilgrimCoat)?20+UpgradeLevel(EquipmentId.PilgrimCoat)*4:0;
        public float RollReduction=>Has(EquipmentId.WingBoots)?.1f+UpgradeLevel(EquipmentId.WingBoots)*.01f:0;
        public float MagicMultiplier=>Has(EquipmentId.FurnaceHeart)?1+DamageBonusPercent(EquipmentId.FurnaceHeart,UpgradeLevel(EquipmentId.FurnaceHeart))/100f:1;
        public float MagicReuseMultiplier=>Has(EquipmentId.SanctumSeal)?.9f-UpgradeLevel(EquipmentId.SanctumSeal)*.01f:1;
    }
    public sealed partial class Enemy
    {
        public bool RewardPaid {get;internal set;}
        public float RewardTimer {get;internal set;}
        public int GoldReward=>LateChapter>0?200+LateChapter*90:SolarRegent?350:ChainWarden?250:Boss?150:Kind==EnemyKind.Heavy?30:Kind==EnemyKind.Archer?16:12;
        public int ExperienceReward=>LateChapter>0?150+LateChapter*50:SolarRegent?220:ChainWarden?160:Boss?100:Kind==EnemyKind.Heavy?24:Kind==EnemyKind.Archer?14:10;
        public bool ChainWarden,SolarRegent;
        public float BodyHalfWidth=>LateChapter>0?1.1f:SolarRegent?1.2f:ChainWarden?1.1f:Boss?.85f:Kind==EnemyKind.Heavy?.7f:.46f;
        public float BodyHeight=>LateChapter>0?(LateChapter==7?4.2f:3.8f):SolarRegent?3.9f:ChainWarden?3.6f:Boss?2.65f:Kind==EnemyKind.Heavy?2.2f:1.7f;
        public float ChainSlamHalfWidth=>PhaseTwo?3.2f:2.5f;
        public float SolarSweepReach=>PhaseTwo?10:8;
        public int SolarLanceCount=>PhaseTwo?5:3;
        public float SolarLanceX(int index)=>Math.Max(.7f,Math.Min(RunModel.WorldWidth-.7f,AimX+(index-SolarLanceCount/2)*4));
        public float EclipseSafeRadius=>PhaseTwo?2.4f:3;
        public int ChainBindCount=>PhaseTwo?3:1;
        public float ChainBindX(int index)=>Math.Max(.7f,Math.Min(RunModel.WorldWidth-.7f,AimX+(index-(PhaseTwo?1:0))*3.2f));
        public float X, Y, Hp, MaxHp, Windup, Cooldown, Flash, AttackPose, AnimationTime, DeathTimer;
        public float PullRemaining,PullStartX,PullStartY,PullEndX,PullEndY;
        public float MinX=.5f, MaxX=RunModel.WorldWidth-.5f;
        public int Facing = -1;
        public bool Boss, Walking;
        public EnemyKind Kind;
        public BossPattern Pattern;
        public bool PhaseTwo, DashContact, Flying;
        public int PatternIndex;
        public float PhaseTransition, DashRemaining, DashEndX, DashEndY, AimX, AimY;
        public float DashDuration { get; internal set; }
        public float AttackReleaseDuration=>LateChapter>0?.55f:SolarRegent?.5f:ChainWarden?.45f:Boss?(Pattern==BossPattern.Dash?.2f:RunModel.BossReleaseDuration):Kind==EnemyKind.Heavy?.35f:.22f;
        public float AttackReach=>Boss?8:Kind==EnemyKind.Heavy?3.2f:1.9f;
        public float WindupDuration=>LateChapter>0?LateWindupDuration:SolarRegent?(Pattern==BossPattern.SolarSweep?(PhaseTwo?.65f:.85f):Pattern==BossPattern.SolarLances?(PhaseTwo?.85f:1.05f):(PhaseTwo?1.3f:1.6f)):ChainWarden?(Pattern==BossPattern.ChainSlam?(PhaseTwo?.65f:.8f):Pattern==BossPattern.ChainBind?(PhaseTwo?.85f:1.05f):(PhaseTwo?.95f:1.15f)):Boss?(Pattern==BossPattern.Dash?(PhaseTwo?.5f:.65f):Pattern==BossPattern.BloodBolts?(PhaseTwo?.65f:.9f):(PhaseTwo?.65f:.8f)):Kind==EnemyKind.Archer?.8f:Kind==EnemyKind.Heavy?.95f:.55f;
    }
    public sealed class RoyalChain
    {
        public Enemy Owner;
        public float X,Delay,Active=.28f;
        public bool Hit;
        public const float HalfWidth=.65f,Height=9;
    }
    public sealed class BloodOrb
    {
        public Enemy Owner;
        public float X,Y,VelocityX,VelocityY,Life=3;
        public const float Radius=.22f;
    }
    public sealed class DemonArrow
    {
        public float X, Y, Remaining=20;
        public int Facing;
    }
    public struct Platform
    {
        public float X, Width, Y;
        public Platform(float x, float width, float y) { X=x; Width=width; Y=y; }
    }
    public sealed class EmberWave
    {
        public float X, Y, Remaining=7.5f, Damage, Speed=16;
        public bool Lance,Solar;
        public int Facing;
        internal readonly HashSet<Enemy> Hit=new HashSet<Enemy>();
    }
    // Pure simulation: identical seed + fixed-timestep input yields identical results.
    public sealed partial class RunModel
    {
        public const float StepSeconds=1f/60f;
        public const float WorldWidth=64, ExitX=61;
        public const float BossReleaseDuration=.32f;
        public const float BossSpawnX=WorldWidth*.5f, BossSpawnY=2.2f, BossIntroDuration=3.6f;
        public float BossIntroRemaining { get; private set; }
        public bool BossIntroducing=>Stage==RunStage.BossArena&&BossIntroRemaining>0;
        public const float BossDefeatDuration=3.2f, PotionHealing=40, LanceReuse=6, NovaReuse=8, NovaRadius=3.6f;
        public bool BossDefeated { get; private set; }
        public float BossDefeatRemaining { get; private set; }
        public bool BossDissolving=>BossDefeated&&BossDefeatRemaining>0;
        public bool Cinematic=>BossIntroducing||BossDissolving;
        public float BossDefeatX { get; private set; }
        public float BossDefeatY { get; private set; }
        public float ChestX=>Clamp(BossDefeatX,8,56);
        public bool ChestAvailable=>BossDefeated&&BossDefeatRemaining<=.7f;
        public bool ChestOpened { get; private set; }
        public float HealingNotice { get; private set; }
        public float LastHealing { get; private set; }
        public EquipmentId LastEquipmentReward {get;private set;}=EquipmentId.None;
        public readonly EquipmentInventory Equipment;
        public readonly PlayerProgression Progression;
        public bool CanChangeEquipment=>Phase==RunPhase.Combat&&!Cinematic&&!SkillChoicePending&&Hp>0&&Grounded&&!Rolling&&!Casting&&AttackTimer<=0&&!Enemies.Exists(e=>e.Hp>0);
        public bool Equip(EquipmentId id){if(!CanChangeEquipment||!Equipment.Equip(id))return false;Hp=Math.Min(Hp,MaxHp);return true;}
        public bool Unequip(EquipmentSlot slot){if(!CanChangeEquipment||!Equipment.Unequip(slot))return false;Hp=Math.Min(Hp,MaxHp);return true;}
        public float BaseSwordDamage=>Damage+Progression.AttackBonus;
        public float BaseMagicDamage=>Damage+Progression.MagicBonus;
        public float SwordDamage=>BaseSwordDamage*Equipment.SwordMultiplier;
        public float MagicDamage=>BaseMagicDamage*Equipment.MagicMultiplier;
        public readonly List<DamageNumber> DamageNumbers=new List<DamageNumber>();
        public float MovementSpeed=>(IsVillagePractice?230f/34f:6)*Progression.MoveMultiplier;
        public float EffectiveRollReuse=>RollReuse-Equipment.RollReduction;
        public float EffectiveMagicReuse=>MagicReuse*Equipment.MagicReuseMultiplier;
        public float EffectiveLanceReuse=>LanceReuse*Equipment.MagicReuseMultiplier;
        public float EffectiveNovaReuse=>NovaReuse*Equipment.MagicReuseMultiplier;
        public const float DawnRadius=4.5f;
        public float EffectiveSolarReuse=>(SolarSpellReward==SpellKind.DawnBurst?10:8)*Equipment.MagicReuseMultiplier;
        public float SolarCooldown {get;private set;}
        public float DawnTimer {get;private set;}
        public float DawnX {get;private set;}
        public float DawnY {get;private set;}
        public const float ChainPullRange=8,ChainRainRadius=4,PullDuration=.28f;
        public float EffectiveChainReuse=>(ChainSpellReward==SpellKind.ChainRain?9:6)*Equipment.MagicReuseMultiplier;
        public float ChainCooldown {get;private set;}
        public float ChainEffectTimer {get;private set;}
        public float ChainEffectX {get;private set;}
        public float ChainEffectY {get;private set;}
        public float ChainEndX {get;private set;}
        public Enemy ChainTarget {get;private set;}
        public SpellKind ChainEffectKind {get;private set;}
        public bool CanOpenChest=>Phase==RunPhase.Combat&&!Cinematic&&!SkillChoicePending&&ChestAvailable&&!ChestOpened&&Grounded&&Y<1&&Math.Abs(X-ChestX)<=2&&!Enemies.Exists(e=>e.Hp>0);
        public float LanceCooldown { get; private set; }
        public float NovaCooldown { get; private set; }
        public SpellKind CurrentSpell { get; private set; }
        // Boss skills belong to this journey, never to the permanent growth save.
        public SpellKind? BossSpellReward {get;private set;}
        public SpellKind? ChainSpellReward {get;private set;}
        public SpellKind? SolarSpellReward {get;private set;}
        public SpellKind? LatestSpellReward=>LatestLateReward??SolarSpellReward??ChainSpellReward??BossSpellReward;
        public float SpellRewardNotice {get;private set;}
        public bool HasSpell(SpellKind spell)=>spell==SpellKind.EmberWave||BossSpellReward==spell||ChainSpellReward==spell||SolarSpellReward==spell||HasLateSpell(spell);
        public float NovaTimer { get; private set; }
        public float NovaX { get; private set; }
        public float NovaY { get; private set; }
        public float SnareTimer {get;private set;}
        public readonly List<RoyalChain> RoyalChains=new List<RoyalChain>();
        public readonly int Seed;
        public const int CombatRoomCount=3;
        public int Chapter {get;private set;}=1;
        // Rooms are rebuilt at their exit; health and kills belong to the whole journey.
        public int RoomIndex { get; private set; }
        public RunStage Stage { get; private set; }
        public int RemainingMonsters { get {int count=0;foreach(var e in Enemies)if(!e.Boss&&e.Hp>0)count++;return count;} }
        public bool ExitOpen=>Stage==RunStage.Approach?RemainingMonsters==0:BossDefeated&&BossDefeatRemaining<=1.2f;
        public bool CanUseExit=>!IsVillagePractice&&Phase==RunPhase.Combat&&!Cinematic&&ExitOpen&&!SkillChoicePending&&Grounded&&Y<1&&Math.Abs(X-ExitX)<=1.6f;
        public int Zone=>X<18?0:X<44?1:2;
        public RunPhase Phase { get; private set; }
        public float X=2, Y, VelocityY, Hp=100, Damage=24, AttackPeriod=.4f;
        public float MaxHp=>100+Equipment.HealthBonus+Progression.HealthBonus;
        public int Facing=1, Kills;
        public float HurtTimer, SlashTimer, AttackTimer;
        public bool Grounded=true;
        public const float RollDuration=.35f, RollReuse=.7f, MagicReuse=5f;
        public float RollTimer { get; private set; }
        public float RollCooldown { get; private set; }
        public float RollInvulnerability { get; private set; }
        public float MagicCooldown { get; private set; }
        public float CastTimer { get; private set; }
        public const float MagicCastDuration=.2f, MagicRecoveryDuration=.18f;
        public float MagicRecoveryTimer { get; private set; }
        public float SwordWindupDuration { get; private set; }
        public float SwordReleaseDuration { get; private set; }
        public bool SwordPreparing=>swordPending;
        public float SwordWindupRemaining=>attackWindup;
        public int AttackStage { get; private set; }
        public int ActionFacing { get; private set; }=1;
        public bool AirAttack { get; private set; }
        public float SwordReach=>AttackStage==3&&!AirAttack?3.2f:2.5f;
        public bool Rolling=>RollTimer>0;
        public bool Casting=>CastTimer>0;
        // Rendering follows the same startup / contact / recovery as hit detection.
        public bool SwordActive=>AttackStage>0&&AttackTimer>0;
        public bool AirSwordRecovery=>!Grounded&&AirAttack&&AttackStage>0&&!SwordActive;
        public int SwordAnimationFrame=>swordPending?0:SlashTimer>0?(SlashTimer>slashDuration*.5f?1:2):3;
        float slashDuration;
        public readonly List<EmberWave> Waves=new List<EmberWave>();
        public readonly List<DemonArrow> Arrows=new List<DemonArrow>();
        public readonly List<BloodOrb> BloodOrbs=new List<BloodOrb>();
        float attackWindup, comboWindow;
        int nextCombo;
        bool swordPending;
        public readonly List<Enemy> Enemies=new List<Enemy>();
        public readonly List<Platform> Platforms=new List<Platform>();
        uint random;
        public RunModel(int seed,EquipmentInventory equipment=null,PlayerProgression progression=null) { Equipment=equipment??new EquipmentInventory();Progression=progression??new PlayerProgression();Hp=MaxHp;Seed=seed; random=unchecked((uint)seed)^0x9e3779b9u; if(random==0)random=1; CreateMap(); }
        uint Next() { random^=random<<13; random^=random>>17; random^=random<<5; return random; }
        int Range(int max) { return (int)(Next()%(uint)max); }
        void CreateMap()
        {
            DamageNumbers.Clear();
            ResetLateRoomMagic();
            X=2; Y=0; VelocityY=0; Facing=1; Grounded=true;
            HurtTimer=1; AttackTimer=0; SlashTimer=0; Phase=RunPhase.Combat;
            RollTimer=RollCooldown=RollInvulnerability=MagicCooldown=CastTimer=0;
            MagicRecoveryTimer=SwordWindupDuration=SwordReleaseDuration=0;
            attackWindup=comboWindow=0;nextCombo=0;AttackStage=0;ActionFacing=1;
            swordPending=false;AirAttack=false;Waves.Clear();Arrows.Clear();BloodOrbs.Clear();
            Enemies.Clear(); Platforms.Clear();
            BossIntroRemaining=0;
            RoyalChains.Clear();SnareTimer=0;
            ChainCooldown=ChainEffectTimer=SolarCooldown=DawnTimer=0;ChainTarget=null;
            BossDefeated=ChestOpened=false;BossDefeatRemaining=HealingNotice=LastHealing=0;
            LastEquipmentReward=EquipmentId.None;
            SpellRewardNotice=0;
            LanceCooldown=NovaCooldown=NovaTimer=0;CurrentSpell=SpellKind.EmberWave;
            if(Stage==RunStage.Approach)
            {
                CreateCombatRoom();
                return;
            }
            if(Chapter>=4){CreateLateBossArena();return;}
            if(Chapter==3)
            {
                Platforms.Add(new Platform(8,9,1.6f));Platforms.Add(new Platform(20,7,1.6f));
                Platforms.Add(new Platform(37,7,1.6f));Platforms.Add(new Platform(47,9,1.6f));
                Enemies.Add(new Enemy {X=BossSpawnX,Y=0,Hp=600,MaxHp=600,Boss=true,SolarRegent=true,MinX=8,MaxX=56,Cooldown=1.2f,Pattern=BossPattern.SolarSweep});
                BossIntroRemaining=BossIntroDuration;return;
            }
            if(Chapter==2)
            {
                Platforms.Add(new Platform(9,8,1.6f));Platforms.Add(new Platform(20,7,1.6f));
                Platforms.Add(new Platform(37,7,1.6f));Platforms.Add(new Platform(48,8,1.6f));
                Enemies.Add(new Enemy {X=BossSpawnX,Y=0,Hp=420,MaxHp=420,Boss=true,ChainWarden=true,MinX=8,MaxX=56,Cooldown=1.2f,Pattern=BossPattern.ChainSlam});
                BossIntroRemaining=BossIntroDuration;return;
            }
            // Two mirrored stairways leave the central sky clear for flight telegraphs.
            // Rise is 1.6 per step (jump apex ~2.27); overlapping ends allow safe ascent.
            Platforms.Add(new Platform(7,7,1.6f));
            Platforms.Add(new Platform(12,7,3.2f));
            Platforms.Add(new Platform(17,7,4.8f));
            Platforms.Add(new Platform(40,7,4.8f));
            Platforms.Add(new Platform(45,7,3.2f));
            Platforms.Add(new Platform(50,7,1.6f));
            Enemies.Add(new Enemy {X=BossSpawnX,Y=BossSpawnY,Hp=220,MaxHp=220,Boss=true,Flying=true,MinX=8,MaxX=56,Cooldown=1});
            BossIntroRemaining=BossIntroDuration;
        }
        void CreateCombatRoom()
        {
            if(Chapter>=5){CreateLateCombatRoom();return;}
            if(Chapter==4){CreateChapterFourRoom();return;}
            if(Chapter==3){CreateChapterThreeRoom();return;}
            if(Chapter==2){CreateChapterTwoRoom();return;}
            if(RoomIndex==0)
            {
                // Forest entrance: broad ground encounters and optional low ledges.
                Platforms.Add(new Platform(9,7,1.6f));
                Platforms.Add(new Platform(25,8,1.6f));
                Platforms.Add(new Platform(43,8,1.6f));
                AddEnemy(14,0,EnemyKind.Melee,10,20);
                AddEnemy(29,0,EnemyKind.Melee,24,34);
                AddEnemy(43,0,EnemyKind.Melee,39,48);
                AddEnemy(52,0,EnemyKind.Melee,48,56);
            }
            else if(RoomIndex==1)
            {
                // Ruined garden: both archer nests have overlapping approach steps.
                Platforms.Add(new Platform(8,7,1.6f));
                Platforms.Add(new Platform(13,9,3.2f));
                Platforms.Add(new Platform(27,7,1.6f));
                Platforms.Add(new Platform(37,7,1.6f));
                Platforms.Add(new Platform(42,10,3.2f));
                AddEnemy(12,0,EnemyKind.Melee,8,20);
                AddEnemy(19,3.2f,EnemyKind.Archer,14,21);
                AddEnemy(29,0,EnemyKind.Melee,25,34);
                AddEnemy(40,0,EnemyKind.Melee,36,45);
                AddEnemy(49,3.2f,EnemyKind.Archer,43,51);
                AddEnemy(54,0,EnemyKind.Melee,50,56);
            }
            else
            {
                // Garrison: low bypass ledges flank heavy guards, with raised archers.
                Platforms.Add(new Platform(8,7,1.6f));
                Platforms.Add(new Platform(13,8,3.2f));
                Platforms.Add(new Platform(25,8,1.6f));
                Platforms.Add(new Platform(37,7,1.6f));
                Platforms.Add(new Platform(42,9,3.2f));
                AddEnemy(12,0,EnemyKind.Melee,9,17);
                AddEnemy(19,3.2f,EnemyKind.Archer,14,20);
                AddEnemy(22,0,EnemyKind.Heavy,17,26);
                AddEnemy(30,0,EnemyKind.Melee,27,35);
                AddEnemy(40,0,EnemyKind.Heavy,36,45);
                AddEnemy(48,3.2f,EnemyKind.Archer,43,50);
                AddEnemy(53,0,EnemyKind.Melee,49,55);
                // The last six units are a quiet approach to the sealed sanctum.
            }
        }
        void CreateChapterTwoRoom()
        {
            // Overlapping 1.6-high steps fit the existing 2.27-high jump.
            // Wide ground lanes allow the reward spear or circular spell to reach grouped guards.
            if(RoomIndex==0)
            {
                Platforms.Add(new Platform(8,8,1.6f));Platforms.Add(new Platform(13,8,3.2f));
                Platforms.Add(new Platform(28,9,1.6f));Platforms.Add(new Platform(43,9,1.6f));
                AddEnemy(12,0,EnemyKind.Melee,9,17);AddEnemy(19,3.2f,EnemyKind.Archer,14,20);
                AddEnemy(26,0,EnemyKind.Heavy,22,30);AddEnemy(33,0,EnemyKind.Melee,30,38);
                AddEnemy(46,1.6f,EnemyKind.Archer,44,51);AddEnemy(54,0,EnemyKind.Melee,50,56);
            }
            else if(RoomIndex==1)
            {
                Platforms.Add(new Platform(7,8,1.6f));Platforms.Add(new Platform(12,9,3.2f));
                Platforms.Add(new Platform(25,10,1.6f));Platforms.Add(new Platform(39,8,1.6f));Platforms.Add(new Platform(44,10,3.2f));
                AddEnemy(12,0,EnemyKind.Heavy,9,18);AddEnemy(18,3.2f,EnemyKind.Archer,13,20);
                AddEnemy(27,0,EnemyKind.Melee,23,31);AddEnemy(32,1.6f,EnemyKind.Melee,26,34);
                AddEnemy(41,0,EnemyKind.Heavy,37,45);AddEnemy(50,3.2f,EnemyKind.Archer,45,53);
                AddEnemy(55,0,EnemyKind.Melee,51,56);
            }
            else
            {
                Platforms.Add(new Platform(7,8,1.6f));Platforms.Add(new Platform(12,9,3.2f));
                Platforms.Add(new Platform(26,10,1.6f));Platforms.Add(new Platform(40,8,1.6f));Platforms.Add(new Platform(45,10,3.2f));
                AddEnemy(11,0,EnemyKind.Melee,8,15);AddEnemy(18,0,EnemyKind.Heavy,15,23);
                AddEnemy(18,3.2f,EnemyKind.Archer,13,20);AddEnemy(29,0,EnemyKind.Melee,25,33);
                AddEnemy(33,1.6f,EnemyKind.Archer,27,35);AddEnemy(42,0,EnemyKind.Heavy,38,46);
                AddEnemy(51,3.2f,EnemyKind.Archer,46,54);AddEnemy(55,0,EnemyKind.Melee,51,56);
            }
        }
        void CreateChapterThreeRoom()
        {
            // Cathedral stairs rise in 1.6-unit steps; every raised patrol is supported.
            // Ground groups reward pulling guards together and using the chain area spell.
            if(RoomIndex==0)
            {
                Platforms.Add(new Platform(8,8,1.6f));Platforms.Add(new Platform(13,8,3.2f));
                Platforms.Add(new Platform(27,10,1.6f));Platforms.Add(new Platform(41,8,1.6f));Platforms.Add(new Platform(46,8,3.2f));
                AddEnemy(11,0,EnemyKind.Melee,8,15);AddEnemy(17,0,EnemyKind.Heavy,14,22);
                AddEnemy(18,3.2f,EnemyKind.Archer,14,20);AddEnemy(29,0,EnemyKind.Melee,25,33);
                AddEnemy(35,0,EnemyKind.Melee,32,39);AddEnemy(49,3.2f,EnemyKind.Archer,47,53);
                AddEnemy(54,0,EnemyKind.Heavy,49,56);
            }
            else if(RoomIndex==1)
            {
                Platforms.Add(new Platform(7,8,1.6f));Platforms.Add(new Platform(12,9,3.2f));
                Platforms.Add(new Platform(26,10,1.6f));Platforms.Add(new Platform(40,8,1.6f));Platforms.Add(new Platform(45,10,3.2f));
                AddEnemy(11,0,EnemyKind.Heavy,8,16);AddEnemy(18,3.2f,EnemyKind.Archer,13,20);
                AddEnemy(22,0,EnemyKind.Melee,18,25);AddEnemy(29,0,EnemyKind.Melee,26,33);
                AddEnemy(33,1.6f,EnemyKind.Archer,27,35);AddEnemy(41,0,EnemyKind.Heavy,37,45);
                AddEnemy(51,3.2f,EnemyKind.Archer,46,54);AddEnemy(55,0,EnemyKind.Melee,51,56);
            }
            else
            {
                Platforms.Add(new Platform(7,8,1.6f));Platforms.Add(new Platform(12,8,3.2f));
                Platforms.Add(new Platform(17,8,4.8f));Platforms.Add(new Platform(39,8,4.8f));
                Platforms.Add(new Platform(44,8,3.2f));Platforms.Add(new Platform(49,8,1.6f));
                AddEnemy(11,0,EnemyKind.Heavy,8,16);AddEnemy(17,0,EnemyKind.Melee,14,22);
                AddEnemy(22,4.8f,EnemyKind.Archer,18,24);AddEnemy(27,0,EnemyKind.Melee,24,30);
                AddEnemy(32,0,EnemyKind.Heavy,29,35);AddEnemy(37,0,EnemyKind.Melee,34,41);
                AddEnemy(42,4.8f,EnemyKind.Archer,40,46);AddEnemy(49,0,EnemyKind.Melee,46,53);
                AddEnemy(54,0,EnemyKind.Heavy,51,56);
            }
        }
        void CreateChapterFourRoom()
        {
            if(RoomIndex==0)
            {
                Platforms.Add(new Platform(8,8,1.6f));Platforms.Add(new Platform(13,8,3.2f));
                Platforms.Add(new Platform(27,10,1.6f));Platforms.Add(new Platform(41,8,1.6f));Platforms.Add(new Platform(46,9,3.2f));
                AddEnemy(11,0,EnemyKind.Melee,8,15);AddEnemy(17,0,EnemyKind.Heavy,14,22);
                AddEnemy(19,3.2f,EnemyKind.Archer,14,20);AddEnemy(29,0,EnemyKind.Melee,25,33);
                AddEnemy(34,1.6f,EnemyKind.Archer,28,36);AddEnemy(42,0,EnemyKind.Heavy,38,46);
                AddEnemy(50,3.2f,EnemyKind.Archer,47,54);AddEnemy(55,0,EnemyKind.Melee,51,56);
            }
            else
            {
                // Opposing stairways can be reached with the unchanged single jump.
                Platforms.Add(new Platform(7,8,1.6f));Platforms.Add(new Platform(12,8,3.2f));
                Platforms.Add(new Platform(17,9,4.8f));Platforms.Add(new Platform(38,9,4.8f));
                Platforms.Add(new Platform(43,8,3.2f));Platforms.Add(new Platform(48,9,1.6f));
                if(RoomIndex==1)
                {
                    AddEnemy(11,0,EnemyKind.Melee,8,15);AddEnemy(18,0,EnemyKind.Heavy,15,23);
                    AddEnemy(22,4.8f,EnemyKind.Archer,18,25);AddEnemy(27,0,EnemyKind.Melee,24,31);
                    AddEnemy(33,0,EnemyKind.Melee,30,37);AddEnemy(40,0,EnemyKind.Heavy,37,44);
                    AddEnemy(42,4.8f,EnemyKind.Archer,39,46);AddEnemy(53,1.6f,EnemyKind.Archer,49,56);
                    AddEnemy(55,0,EnemyKind.Melee,51,56);
                }
                else
                {
                    Platforms.Add(new Platform(28,8,1.6f));
                    AddEnemy(11,0,EnemyKind.Heavy,8,15);AddEnemy(17,0,EnemyKind.Melee,14,21);
                    AddEnemy(22,4.8f,EnemyKind.Archer,18,25);AddEnemy(27,0,EnemyKind.Melee,24,30);
                    AddEnemy(32,0,EnemyKind.Heavy,29,35);AddEnemy(33,1.6f,EnemyKind.Archer,29,35);
                    AddEnemy(39,0,EnemyKind.Melee,36,43);AddEnemy(42,4.8f,EnemyKind.Archer,39,46);
                    AddEnemy(50,0,EnemyKind.Heavy,47,54);AddEnemy(55,0,EnemyKind.Melee,52,56);
                }
            }
        }
        void AddEnemy(float x,float y,EnemyKind kind,float min,float max)
        {
            float hp=kind==EnemyKind.Heavy?100:kind==EnemyKind.Archer?36:44;
            hp*=1+(Chapter-1)*.2f;
            Enemies.Add(new Enemy {X=x,Y=y,Kind=kind,Hp=hp,MaxHp=hp,MinX=min,MaxX=max,Cooldown=.8f});
        }
        public void Step(float movement, bool jump, bool attack, bool roll=false, bool magic=false, bool down=false, bool enter=false, bool lance=false, bool nova=false, bool chain=false, bool solar=false, int lateSlot=-1, int skillSlot=-1)
        {
            if(Phase!=RunPhase.Combat)return;
            var requested=EquippedSkill(skillSlot);
            if(requested.HasValue)
            {
                magic|=requested==SpellKind.EmberWave;lance|=requested==SpellKind.AshLance;nova|=requested==SpellKind.CrimsonNova;
                chain|=requested==SpellKind.ChainPull||requested==SpellKind.ChainRain;
                solar|=requested==SpellKind.SunLance||requested==SpellKind.DawnBurst;
                if(LateSpellCatalog.Contains(requested.Value))lateSlot=LateSpellCatalog.Slot(requested.Value);
            }
            magic=magic&&IsSpellEquipped(SpellKind.EmberWave);
            lance=lance&&IsSpellEquipped(SpellKind.AshLance);nova=nova&&IsSpellEquipped(SpellKind.CrimsonNova);
            chain=chain&&ChainSpellReward.HasValue&&IsSpellEquipped(ChainSpellReward.Value);
            solar=solar&&SolarSpellReward.HasValue&&IsSpellEquipped(SolarSpellReward.Value);
            const float dt=StepSeconds;
            for(int i=DamageNumbers.Count-1;i>=0;i--)
            {
                DamageNumbers[i].Remaining-=dt;
                if(DamageNumbers[i].Remaining<=0)DamageNumbers.RemoveAt(i);
            }
            foreach(var enemy in Enemies)enemy.RewardTimer=Math.Max(0,enemy.RewardTimer-dt);
            if(BossIntroducing)
            {
                BossIntroRemaining=Math.Max(0,BossIntroRemaining-dt);
                foreach(var enemy in Enemies)enemy.AnimationTime+=dt;
                return; // Cinematic time advances; movement, combat and cooldowns do not.
            }
            BeginBossDefeatIfNeeded();
            if(BossDissolving)
            {
                BossDefeatRemaining=Math.Max(0,BossDefeatRemaining-dt);
                foreach(var enemy in Enemies)if(enemy.Boss)enemy.DeathTimer=BossDefeatRemaining;
                return;
            }
            if(SkillChoicePending)return;
            HealingNotice=Math.Max(0,HealingNotice-dt);NovaTimer=Math.Max(0,NovaTimer-dt);SnareTimer=Math.Max(0,SnareTimer-dt);
            SpellRewardNotice=Math.Max(0,SpellRewardNotice-dt);
            LanceCooldown=Math.Max(0,LanceCooldown-dt);NovaCooldown=Math.Max(0,NovaCooldown-dt);
            SolarCooldown=Math.Max(0,SolarCooldown-dt);DawnTimer=Math.Max(0,DawnTimer-dt);
            ChainCooldown=Math.Max(0,ChainCooldown-dt);ChainEffectTimer=Math.Max(0,ChainEffectTimer-dt);
            if(ChainEffectTimer<=0)ChainTarget=null;
            HurtTimer=Math.Max(0,HurtTimer-dt); AttackTimer=Math.Max(0,AttackTimer-dt); SlashTimer=Math.Max(0,SlashTimer-dt);
            MagicRecoveryTimer=Math.Max(0,MagicRecoveryTimer-dt);
            RollTimer=Math.Max(0,RollTimer-dt);RollCooldown=Math.Max(0,RollCooldown-dt);
            RollInvulnerability=Math.Max(0,RollInvulnerability-dt);MagicCooldown=Math.Max(0,MagicCooldown-dt);
            if(AttackTimer<=0){comboWindow=Math.Max(0,comboWindow-dt);if(comboWindow<=0)nextCombo=0;}
            movement=Math.Max(-1,Math.Min(1,movement));
            if(roll&&Grounded&&!Rolling&&RollCooldown<=0)
            {
                CancelSword();CastTimer=0;SnareTimer=0;
                Facing=movement!=0?(movement>0?1:-1):Facing;ActionFacing=Facing;
                RollTimer=RollDuration;RollCooldown=EffectiveRollReuse;RollInvulnerability=.2f;
            }
            if(!Rolling&&!Casting&&AttackTimer<=0&&movement!=0)Facing=movement>0?1:-1;
            float speed=Rolling?ActionFacing*(2.3f/RollDuration):movement*(Casting||AttackTimer>0?2.5f*Progression.MoveMultiplier:MovementSpeed);
            if(!Rolling&&SnareTimer>0)speed*=.4f;
            // The sealed gate spans every height: jumping/rolling cannot bypass it.
            float rightBound=IsVillagePractice?VillageMaxX:!ExitOpen?ExitX-1:WorldWidth-.5f;
            X=Math.Max(IsVillagePractice?VillageMinX:.5f,Math.Min(rightBound,X+speed*dt));
            float droppedHeight=float.NaN;
            if(jump&&Grounded&&!Rolling&&!Casting)
            {
                if(down)
                {
                    // Ignore only the supporting height for this crossing, not lower ledges.
                    // Initial downward speed clears the landing tolerance in one fixed step.
                    foreach(var p in Platforms)
                        if(p.Y>0&&Math.Abs(Y-p.Y)<.01f&&X>=p.X-.3f&&X<=p.X+p.Width+.3f)
                        {droppedHeight=p.Y;CancelSword();VelocityY=-2;Grounded=false;break;}
                }
                else {CancelSword();VelocityY=11.5f;Grounded=false;}
            }
            float oldY=Y;
            VelocityY-=28*dt; Y+=VelocityY*dt; Grounded=false;
            if(VelocityY<=0)
            {
                float floor=0;
                foreach(var p in Platforms)
                    if(p.Y!=droppedHeight&&X>=p.X-.3f&&X<=p.X+p.Width+.3f&&oldY>=p.Y-.01f&&Y<=p.Y)floor=Math.Max(floor,p.Y);
                if(Y<=floor){Y=floor; VelocityY=0; Grounded=true;}
            }
            // End the airborne pose on landing (or when a landed strike ends).
            // Otherwise walking off a later ledge resurrects the old slash pose.
            if(Grounded&&AirAttack&&!SwordActive){AirAttack=false;AttackStage=0;}
            if(!Rolling)
            {
                if(Casting)
                {
                    CastTimer=Math.Max(0,CastTimer-dt);
                    if(CastTimer<=0)
                    {
                        ReleaseSpell();AttackTimer=MagicRecoveryTimer=MagicRecoveryDuration;
                    }
                }
                else if(AttackTimer<=0&&((magic&&MagicCooldown<=0)||(lance&&LanceCooldown<=0)||(nova&&NovaCooldown<=0)||(chain&&ChainCooldown<=0)||(solar&&SolarCooldown<=0)||ReadyLateSlot(lateSlot)))
                {
                    CancelSword();ActionFacing=Facing;CastTimer=MagicCastDuration;
                    CurrentSpell=magic&&MagicCooldown<=0?SpellKind.EmberWave:lance&&LanceCooldown<=0?SpellKind.AshLance:nova&&NovaCooldown<=0?SpellKind.CrimsonNova:chain&&ChainCooldown<=0?ChainSpellReward.Value:solar&&SolarCooldown<=0?SolarSpellReward.Value:lateRewards[lateSlot].Value;
                }
                else if(attack&&AttackTimer<=0)
                {
                    AirAttack=!Grounded;AttackStage=AirAttack?1:nextCombo+1;ActionFacing=Facing;
                    float scale=AttackPeriod/.4f;
                    AttackTimer=(AirAttack?.3f:AttackStage==3?.4f:AttackStage==2?.26f:.24f)*scale;
                    attackWindup=SwordWindupDuration=(AttackStage==3?.1f:.06f)*scale;swordPending=true;
                    nextCombo=AirAttack?0:AttackStage%3;comboWindow=.32f;
                }
                if(swordPending)
                {
                    attackWindup-=dt;
                    if(attackWindup<=0){swordPending=false;StrikeSword();}
                }
            }
            UpdateLateMagic(dt);UpdateWaves(dt);
            if(BeginBossDefeatIfNeeded())return;
            foreach(var e in Enemies)
            {
                e.Flash=Math.Max(0,e.Flash-dt);
                e.AnimationTime+=dt;e.Walking=false;
                e.AttackPose=Math.Max(0,e.AttackPose-dt);
                if(e.Hp<=0){e.DeathTimer=Math.Max(0,e.DeathTimer-dt);if(e.Boss)ClearBossAttack(e);continue;}
                if(e.PullRemaining>0&&!e.Boss)
                {
                    e.PullRemaining=Math.Max(0,e.PullRemaining-dt);
                    float t=1-e.PullRemaining/PullDuration;t=t*t*(3-2*t);
                    e.X=e.PullStartX+(e.PullEndX-e.PullStartX)*t;e.Y=e.PullStartY+(e.PullEndY-e.PullStartY)*t;
                    continue;
                }
                if(e.Boss){UpdateBoss(e,dt);continue;}
                // An off-screen archer must not finish a hidden windup. Keep
                // encounter positions; wait until the player can see the enemy.
                if(!InCombatView(e))
                {
                    e.Windup=0;e.Cooldown=Math.Max(e.Cooldown,.35f);continue;
                }
                // Distant encounters wait in their own area instead of crossing the entire map.
                if(e.Windup<=0&&e.AttackPose<=0&&(Math.Abs(e.X-X)>13||Math.Abs(e.Y-Y)>3.5f))continue;
                if(e.Windup>0)
                {
                    e.Windup-=dt;
                    if(e.Windup<=0)
                    {
                        e.AttackPose=e.AttackReleaseDuration;
                        if(!e.Boss&&e.Kind==EnemyKind.Archer)
                            Arrows.Add(new DemonArrow {X=e.X+e.Facing*.9f,Y=e.Y+1.4f,Facing=e.Facing});
                        else
                        {
                            float distance=Math.Abs(e.X-X);
                            float relativeY=Y-e.Y;
                            bool hit=e.Boss?distance<8&&relativeY>=-.5f&&relativeY<.85f:distance<e.AttackReach&&relativeY>=-1.5f&&relativeY<1.3f&&(X-e.X)*e.Facing>=-.5f;
                            if(hit)Hurt(e.Boss?24:e.Kind==EnemyKind.Heavy?20:12);
                        }
                        e.Cooldown=e.Boss?1.2f:e.Kind==EnemyKind.Archer?1.3f:e.Kind==EnemyKind.Heavy?1.1f:.65f;
                    }
                }
                else
                {
                    e.Cooldown-=dt;
                    // Finish the committed strike before turning or moving again.
                    if(e.AttackPose>0)continue;
                    float dx=X-e.X;
                    e.Facing=dx>=0?1:-1;
                    float distance=Math.Abs(dx),step=0;
                    if(!e.Boss&&e.Kind==EnemyKind.Archer)
                    {
                        if(distance<6)step=-e.Facing*2.5f*dt;
                        else if(distance>9)step=e.Facing*2.1f*dt;
                        // Can fire when cornered; never retreat beyond the arena.
                        if(distance<=9&&e.Cooldown<=0){e.Windup=e.WindupDuration;step=0;}
                    }
                    else
                    {
                        float range=e.Boss?7:e.Kind==EnemyKind.Heavy?2.6f:1.4f;
                        if(distance>range)step=e.Facing*(e.Boss?1.7f:e.Kind==EnemyKind.Heavy?1.2f:2.2f)*dt;
                        else if(e.Cooldown<=0)e.Windup=e.WindupDuration;
                    }
                    float nextX=Math.Max(e.MinX,Math.Min(e.MaxX,e.X+step));
                    e.Walking=Math.Abs(nextX-e.X)>.001f;e.X=nextX;
                }
            }
            // Clear the room before resolving leftover hostile projectiles.
            if(Enemies.TrueForAll(e=>e.Hp<=0))Arrows.Clear();
            else UpdateArrows(dt);
            UpdateBloodOrbs(dt);
            UpdateRoyalChains(dt);
            // Death has precedence; rewards never resurrect a lost run.
            if(Hp<=0){BloodOrbs.Clear();RoyalChains.Clear();SnareTimer=0;Phase=RunPhase.Dead;return;}
            if(enter&&CanOpenChest)OpenRewardChest();
            else if(enter&&CanUseExit)
            {
                if(Stage==RunStage.Approach)
                {
                    RoomIndex++;
                    if(RoomIndex==CombatRoomCount)Stage=RunStage.BossArena;
                    CreateMap();
                }
                else
                {
                    // Rebuild only the room. Both boss rewards and current HP stay on this journey.
                    if(Chapter==FinalChapter){Phase=RunPhase.Won;return;}
                    Chapter++;RoomIndex=0;Stage=RunStage.Approach;CreateMap();
                }
            }
        }
        public void Hurt(float amount)
        {
            if(Phase!=RunPhase.Combat||Cinematic||SkillChoicePending||HurtTimer>0||RollInvulnerability>0||amount<=0)return;
            float damage=Math.Max(1,amount-Progression.Defense)*(NightMantleTime>0?.6f:1);
            float absorbed=Math.Min(MirrorShield,damage);MirrorShield-=absorbed;damage-=absorbed;
            Hp=Math.Max(0,Hp-damage);HurtTimer=.8f;
            if(Hp<=0)Phase=RunPhase.Dead;
        }
        void CancelSword()
        {
            AttackTimer=SlashTimer=attackWindup=comboWindow=0;
            MagicRecoveryTimer=SwordWindupDuration=SwordReleaseDuration=0;
            swordPending=false;nextCombo=0;AttackStage=0;AirAttack=false;
        }
        bool BeginBossDefeatIfNeeded()
        {
            if(Stage!=RunStage.BossArena||BossDefeated||Hp<=0)return false;
            var boss=Enemies.Find(e=>e.Boss&&e.Hp<=0);if(boss==null)return false;
            BossDefeated=true;BossDefeatRemaining=BossDefeatDuration;
            if(!BossReward(Chapter).HasValue)PendingSkillChapter=Chapter;
            ResetLateSpellEffects();
            BossDefeatX=boss.X;BossDefeatY=boss.Y;boss.DeathTimer=BossDefeatDuration;
            ClearBossAttack(boss);CancelSword();CastTimer=RollTimer=NovaTimer=DawnTimer=0;
            Waves.Clear();Arrows.Clear();BloodOrbs.Clear();RoyalChains.Clear();SnareTimer=0;ChainEffectTimer=0;ChainTarget=null;return true;
        }
        void OpenRewardChest()
        {
            if(!CanOpenChest)return;
            ChestOpened=true;LastHealing=Math.Min(PotionHealing,Math.Max(0,MaxHp-Hp));
            Hp+=LastHealing;HealingNotice=6;
            var missing=new List<EquipmentId>();
            for(int i=0;i<EquipmentCatalog.Count;i++)if(!Equipment.Owns((EquipmentId)i))missing.Add((EquipmentId)i);
            if(missing.Count>0){LastEquipmentReward=missing[Range(missing.Count)];Equipment.Grant(LastEquipmentReward);}
        }
        void ReleaseSpell()
        {
            if(LateSpellCatalog.Contains(CurrentSpell)){ReleaseLateSpell();return;}
            if(CurrentSpell==SpellKind.SunLance)
            {
                Waves.Add(new EmberWave {X=X,Y=Y+1,Facing=ActionFacing,Damage=MagicDamage*4,Remaining=14,Speed=26,Solar=true});
                SolarCooldown=EffectiveSolarReuse;
            }
            else if(CurrentSpell==SpellKind.DawnBurst)
            {
                DawnX=X;DawnY=Y+1;DawnTimer=.55f;SolarCooldown=EffectiveSolarReuse;
                foreach(var enemy in Enemies)
                {
                    float dx=DawnX-Clamp(DawnX,enemy.X-enemy.BodyHalfWidth,enemy.X+enemy.BodyHalfWidth);
                    float dy=DawnY-Clamp(DawnY,enemy.Y,enemy.Y+enemy.BodyHeight);
                    if(dx*dx+dy*dy<=DawnRadius*DawnRadius)DamageEnemy(enemy,MagicDamage*3.2f);
                }
            }
            else if(CurrentSpell==SpellKind.ChainPull||CurrentSpell==SpellKind.ChainRain)ReleaseChainSpell();
            else if(CurrentSpell==SpellKind.AshLance)
            {
                Waves.Add(new EmberWave {X=X,Y=Y+1,Facing=ActionFacing,Damage=MagicDamage*3.2f,Remaining=12,Speed=22,Lance=true});
                LanceCooldown=EffectiveLanceReuse;
            }
            else if(CurrentSpell==SpellKind.CrimsonNova)
            {
                NovaX=X;NovaY=Y+1;NovaTimer=.45f;NovaCooldown=EffectiveNovaReuse;
                foreach(var enemy in Enemies)
                {
                    float half=enemy.BodyHalfWidth;
                    float height=enemy.BodyHeight;
                    float dx=NovaX-Clamp(NovaX,enemy.X-half,enemy.X+half);
                    float dy=NovaY-Clamp(NovaY,enemy.Y,enemy.Y+height);
                    if(dx*dx+dy*dy<=NovaRadius*NovaRadius)DamageEnemy(enemy,MagicDamage*2);
                }
            }
            else
            {
                Waves.Add(new EmberWave {X=X,Y=Y+.8f,Facing=ActionFacing,Damage=MagicDamage*2.5f});
                MagicCooldown=EffectiveMagicReuse;
            }
        }
        void ReleaseChainSpell()
        {
            ChainCooldown=EffectiveChainReuse;ChainEffectTimer=.45f;ChainEffectKind=CurrentSpell;
            ChainEffectX=X;ChainEffectY=Y;ChainTarget=null;ChainEndX=Clamp(X+ActionFacing*ChainPullRange,.5f,ExitX-1);
            if(CurrentSpell==SpellKind.ChainRain)
            {
                foreach(var enemy in Enemies)
                    if(enemy.Hp>0&&Math.Abs(enemy.X-X)<=ChainRainRadius+enemy.BodyHalfWidth&&enemy.Y<=Y+4&&enemy.Y+enemy.BodyHeight>=Y-.4f)
                        DamageEnemy(enemy,MagicDamage*2.8f);
                return;
            }
            float nearest=float.MaxValue;
            foreach(var enemy in Enemies)
            {
                float ahead=(enemy.X-X)*ActionFacing;
                if(enemy.Hp<=0||ahead<0||ahead>ChainPullRange+enemy.BodyHalfWidth||Y+1.5f<enemy.Y||Y+.5f>enemy.Y+enemy.BodyHeight||ahead>=nearest)continue;
                ChainTarget=enemy;nearest=ahead;
            }
            if(ChainTarget==null)return;
            var target=ChainTarget;ChainEndX=target.X;DamageEnemy(target,MagicDamage*1.5f);
            // Bosses take damage but keep their arena position and committed attacks.
            if(target.Hp<=0||target.Boss)return;
            float endX=Clamp(X+ActionFacing*1.4f,.5f,ExitX-1),floor=0,min=.5f,max=ExitX-1;
            foreach(var p in Platforms)
                if(endX>=p.X+.3f&&endX<=p.X+p.Width-.3f&&p.Y<=Y+.01f&&p.Y>floor)
                {floor=p.Y;min=p.X+.3f;max=p.X+p.Width-.3f;}
            target.PullStartX=target.X;target.PullStartY=target.Y;target.PullEndX=endX;target.PullEndY=floor;target.PullRemaining=PullDuration;
            target.MinX=min;target.MaxX=max;target.Windup=target.AttackPose=0;target.Cooldown=.6f;
        }
        void DamageEnemy(Enemy e,float damage,bool sword=false)
        {
            if(e.Hp<=0||damage<=0)return;
            // Record the resolved strike, including overkill, before reducing HP.
            // World coordinates keep feedback anchored while the camera moves.
            if(DamageNumbers.Count>=64)DamageNumbers.RemoveAt(0);
            DamageNumbers.Add(new DamageNumber {X=e.X,Y=e.Y+e.BodyHeight,Amount=damage,Sword=sword});
            e.Hp=Math.Max(0,e.Hp-damage);e.Flash=.12f;
            if(e.Hp==0){Kills++;e.DeathTimer=e.Boss?.6f:.3f;e.Windup=e.AttackPose=0;Progression.RewardKill(e);}
        }
        void StrikeSword()
        {
            SwordReleaseDuration=AttackTimer;
            SlashTimer=slashDuration=Math.Min(.12f,AttackTimer);
            foreach(var e in Enemies)
                if(e.Hp>0&&(e.X-X)*ActionFacing>=-.4f&&Math.Abs(e.X-X)<SwordReach+((e.LateChapter>0||e.ChainWarden||e.SolarRegent)?.85f:e.Boss?.6f:e.Kind==EnemyKind.Heavy?.25f:0)&&Y-e.Y>=-1.8f&&Y-e.Y<(e.LateChapter>0?e.BodyHeight+.2f:e.SolarRegent?4.1f:e.ChainWarden?3.8f:e.Boss?2.9f:e.Kind==EnemyKind.Heavy?2.2f:1.55f))
                {
                    bool finisher=AttackStage==3&&!AirAttack;
                    DamageEnemy(e,SwordDamage*(finisher?1.6f:1),true);
                    if(finisher&&!e.Boss&&e.Kind!=EnemyKind.Heavy)e.X=Math.Max(e.MinX,Math.Min(e.MaxX,e.X+ActionFacing*.75f));
                }
        }
        void UpdateWaves(float dt)
        {
            for(int i=Waves.Count-1;i>=0;i--)
            {
                var wave=Waves[i];float oldX=wave.X,travel=Math.Min(wave.Remaining,wave.Speed*dt);
                wave.X+=wave.Facing*travel;wave.Remaining-=travel;
                // Sweep the travelled segment so fast projectiles cannot skip enemies.
                foreach(var e in Enemies)
                {
                    float radius=e.BodyHalfWidth;
                    if(e.Hp<=0||wave.Hit.Contains(e)||wave.Y-.35f>e.Y+e.BodyHeight||wave.Y+.35f<e.Y)continue;
                    if(e.X+radius<Math.Min(oldX,wave.X)||e.X-radius>Math.Max(oldX,wave.X))continue;
                    wave.Hit.Add(e);DamageEnemy(e,wave.Damage);
                }
                if(wave.Remaining<=0||wave.X<0||wave.X>WorldWidth)Waves.RemoveAt(i);
            }
        }
        void UpdateArrows(float dt)
        {
            for(int i=Arrows.Count-1;i>=0;i--)
            {
                var arrow=Arrows[i];float oldX=arrow.X,travel=Math.Min(arrow.Remaining,12*dt);
                arrow.X+=arrow.Facing*travel;arrow.Remaining-=travel;
                bool hit=X+.35f>=Math.Min(oldX,arrow.X)&&X-.35f<=Math.Max(oldX,arrow.X)&&
                         arrow.Y>=Y+.15f&&arrow.Y<=Y+1.9f;
                if(hit)Hurt(10);
                if(hit||arrow.Remaining<=0||arrow.X<0||arrow.X>WorldWidth)Arrows.RemoveAt(i);
            }
        }
        void ClearBossAttack(Enemy boss)
        {
            boss.Windup=boss.AttackPose=boss.DashRemaining=boss.PhaseTransition=0;
            BloodOrbs.RemoveAll(orb=>orb.Owner==boss);
            RoyalChains.RemoveAll(chain=>chain.Owner==boss);
        }
        void UpdateBoss(Enemy boss,float dt)
        {
            if(boss.LateChapter>0){UpdateLateBoss(boss,dt);return;}
            if(boss.SolarRegent){UpdateSolarRegent(boss,dt);return;}
            if(boss.ChainWarden){UpdateChainWarden(boss,dt);return;}
            if(!boss.PhaseTwo&&boss.Hp<=boss.MaxHp*.5f)
            {
                ClearBossAttack(boss);boss.PhaseTwo=true;boss.PhaseTransition=1;
                boss.Cooldown=.35f;boss.PatternIndex=1; // Start phase two with a newly telegraphed dash.
                return;
            }
            if(boss.PhaseTransition>0){boss.PhaseTransition=Math.Max(0,boss.PhaseTransition-dt);return;}
            if(boss.DashRemaining>0)
            {
                float oldX=boss.X,oldY=boss.Y,speed=boss.PhaseTwo?20:16;
                float dashDx=boss.DashEndX-oldX,dashDy=boss.DashEndY-oldY;
                float distance=(float)Math.Sqrt(dashDx*dashDx+dashDy*dashDy),ratio=distance>.0001f?Math.Min(1,speed*dt/distance):1;
                boss.X=Clamp(oldX+dashDx*ratio,boss.MinX,boss.MaxX);boss.Y=oldY+dashDy*ratio;
                boss.DashRemaining=Math.Max(0,boss.DashRemaining-dt);
                // Sweep the boss body, so a fast dash cannot skip through the player.
                float enter=0,leave=1;
                if(!boss.DashContact&&ClipAxis(oldX,boss.X-oldX,X-1,X+1,ref enter,ref leave)&&
                   ClipAxis(oldY,boss.Y-oldY,Y-1.5f,Y+1.75f,ref enter,ref leave))
                {boss.DashContact=true;Hurt(18);}
                if(ratio>=1||boss.DashRemaining<=0)
                {boss.DashRemaining=0;boss.AttackPose=.2f;boss.Cooldown=boss.PhaseTwo?.65f:.9f;}
                return;
            }
            if(boss.Windup>0)
            {
                boss.Windup=Math.Max(0,boss.Windup-dt);
                if(boss.Windup>0)return;
                boss.AttackPose=BossReleaseDuration;
                boss.Cooldown=boss.PhaseTwo?.85f:1.2f;
                if(boss.Pattern==BossPattern.GroundWave)
                {
                    if(Math.Abs(boss.X-X)<boss.AttackReach&&Y<.85f)Hurt(24);
                }
                else if(boss.Pattern==BossPattern.Dash)
                {
                    boss.AttackPose=0;boss.DashContact=false;
                    float dashX=boss.DashEndX-boss.X,dashY=boss.DashEndY-boss.Y;
                    boss.DashRemaining=boss.DashDuration=(float)Math.Sqrt(dashX*dashX+dashY*dashY)/(boss.PhaseTwo?20:16);
                }
                else FireBloodOrbs(boss);
                return;
            }
            // Hover during recovery; freeze height during windup so telegraphs stay truthful.
            if(boss.Flying&&Math.Abs(X-boss.X)<23)
            {
                float targetY=Clamp(Y+.65f,1.1f,4.8f)+(float)Math.Sin(boss.AnimationTime*2)*.18f;
                boss.Y+=Clamp(targetY-boss.Y,-1.8f*dt,1.8f*dt);
            }
            boss.Cooldown=Math.Max(0,boss.Cooldown-dt);
            if(boss.AttackPose>0||boss.Cooldown>0)return;
            float dx=X-boss.X;
            if(Math.Abs(dx)>23||Math.Abs(Y-boss.Y)>7)return;
            boss.Facing=dx>=0?1:-1;
            var pattern=(BossPattern)(boss.PatternIndex%3);
            float range=pattern==BossPattern.GroundWave?7:12;
            if(Math.Abs(dx)>range)
            {
                float next=Clamp(boss.X+boss.Facing*(boss.PhaseTwo?2.4f:1.7f)*dt,boss.MinX,boss.MaxX);
                boss.Walking=Math.Abs(next-boss.X)>.001f;boss.X=next;
                if(boss.Walking)return;
                // A player beyond the arena boundary can still be targeted by magic.
                pattern=BossPattern.BloodBolts;
            }
            boss.Pattern=pattern;boss.PatternIndex++;
            boss.AimX=X;boss.AimY=Y+1;
            float dashRange=boss.PhaseTwo?10:7;
            boss.DashEndX=Clamp(Clamp(X+boss.Facing*1.5f,boss.X-dashRange,boss.X+dashRange),boss.MinX,boss.MaxX);
            boss.DashEndY=boss.Flying?Clamp(Y+.15f,.5f,5.2f):boss.Y;
            if(pattern==BossPattern.Dash&&Math.Abs(boss.DashEndX-boss.X)<.5f)boss.Pattern=BossPattern.BloodBolts;
            boss.Windup=boss.WindupDuration;
        }
        void UpdateSolarRegent(Enemy boss,float dt)
        {
            if(!boss.PhaseTwo&&boss.Hp<=boss.MaxHp*.5f)
            {
                ClearBossAttack(boss);boss.PhaseTwo=true;boss.PhaseTransition=1.2f;boss.PatternIndex=2;boss.Cooldown=.5f;return;
            }
            if(boss.PhaseTransition>0){boss.PhaseTransition=Math.Max(0,boss.PhaseTransition-dt);return;}
            if(boss.Windup>0)
            {
                boss.Windup=Math.Max(0,boss.Windup-dt);if(boss.Windup>0)return;
                boss.AttackPose=boss.AttackReleaseDuration;boss.Cooldown=boss.PhaseTwo?1.1f:1.5f;
                if(boss.Pattern==BossPattern.SolarSweep)
                {
                    if(Math.Abs(X-boss.X)<=boss.SolarSweepReach+.35f&&Y<1.2f)Hurt(24);
                }
                else if(boss.Pattern==BossPattern.SolarLances)
                {
                    for(int i=0;i<boss.SolarLanceCount;i++)
                        if(Math.Abs(X-boss.SolarLanceX(i))<=1.05f&&Y<9&&Y+1.9f>0){Hurt(18);break;}
                }
                else if(Math.Abs(X-boss.AimX)>boss.EclipseSafeRadius-.35f)Hurt(28);
                return;
            }
            boss.Cooldown=Math.Max(0,boss.Cooldown-dt);if(boss.AttackPose>0)return;
            float dx=X-boss.X;boss.Facing=dx>=0?1:-1;
            if(Math.Abs(dx)>6)
            {
                float next=Clamp(boss.X+boss.Facing*(boss.PhaseTwo?2.7f:2)*dt,boss.MinX,boss.MaxX);
                boss.Walking=Math.Abs(next-boss.X)>.001f;boss.X=next;
            }
            if(boss.Cooldown>0)return;
            boss.Pattern=(BossPattern)((int)BossPattern.SolarSweep+boss.PatternIndex%3);boss.PatternIndex++;
            if(boss.Pattern==BossPattern.SolarSweep&&Math.Abs(X-boss.X)>boss.SolarSweepReach)boss.Pattern=BossPattern.SolarLances;
            // A safe band appears five units ahead; its position is fixed for the whole warning.
            boss.AimX=boss.Pattern==BossPattern.Eclipse?Clamp(X+boss.Facing*5,5,59):Clamp(X,1,63);
            boss.AimY=Y+1;boss.Windup=boss.WindupDuration;boss.Walking=false;
        }
        void UpdateChainWarden(Enemy boss,float dt)
        {
            if(!boss.PhaseTwo&&boss.Hp<=boss.MaxHp*.5f)
            {
                ClearBossAttack(boss);SnareTimer=0;boss.PhaseTwo=true;boss.PhaseTransition=1.1f;
                boss.Cooldown=.4f;boss.PatternIndex=1;return;
            }
            if(boss.PhaseTransition>0){boss.PhaseTransition=Math.Max(0,boss.PhaseTransition-dt);return;}
            if(boss.Windup>0)
            {
                boss.Windup=Math.Max(0,boss.Windup-dt);if(boss.Windup>0)return;
                boss.AttackPose=boss.AttackReleaseDuration;boss.Cooldown=boss.PhaseTwo?.95f:1.3f;
                if(boss.Pattern==BossPattern.ChainSlam)
                {
                    if(Math.Abs(X-boss.AimX)<=boss.ChainSlamHalfWidth+.35f&&Y<1.2f)Hurt(22);
                }
                else if(boss.Pattern==BossPattern.ChainBind)
                {
                    for(int i=0;i<boss.ChainBindCount;i++)
                        if(Math.Abs(X-boss.ChainBindX(i))<=.6f+.35f&&Y<RoyalChain.Height&&Y+1.9f>0)
                        {
                            float before=Hp;Hurt(14);
                            if(Hp<before&&Hp>0)SnareTimer=.7f;
                            break;
                        }
                }
                else
                {
                    int count=boss.PhaseTwo?7:5;
                    float center=Clamp(boss.AimX,12,52);
                    for(int i=0;i<count;i++)RoyalChains.Add(new RoyalChain {Owner=boss,X=center+(i-count/2)*3.4f,Delay=.45f+i*.12f});
                    boss.Cooldown=boss.PhaseTwo?1.3f:1.6f;
                }
                return;
            }
            boss.Cooldown=Math.Max(0,boss.Cooldown-dt);
            if(boss.AttackPose>0||RoyalChains.Exists(chain=>chain.Owner==boss))return;
            float dx=X-boss.X;boss.Facing=dx>=0?1:-1;
            if(Math.Abs(dx)>5)
            {
                float next=Clamp(boss.X+boss.Facing*(boss.PhaseTwo?2.8f:2.1f)*dt,boss.MinX,boss.MaxX);
                boss.Walking=Math.Abs(next-boss.X)>.001f;boss.X=next;
            }
            if(boss.Cooldown>0)return;
            var pattern=(BossPattern)((int)BossPattern.ChainSlam+boss.PatternIndex%3);
            if(pattern==BossPattern.ChainSlam&&Math.Abs(X-boss.X)>10)return;
            boss.Pattern=pattern;boss.PatternIndex++;boss.Walking=false;
            boss.AimX=Clamp(X,.7f,WorldWidth-.7f);boss.AimY=Y+1;
            boss.Windup=boss.WindupDuration;
        }
        void UpdateRoyalChains(float dt)
        {
            for(int i=RoyalChains.Count-1;i>=0;i--)
            {
                var chain=RoyalChains[i];
                if(chain.Owner.Hp<=0||Phase!=RunPhase.Combat){RoyalChains.RemoveAt(i);continue;}
                if(chain.Delay>0){chain.Delay=Math.Max(0,chain.Delay-dt);if(chain.Delay>0)continue;}
                if(!chain.Hit&&Math.Abs(X-chain.X)<=RoyalChain.HalfWidth+.35f&&Y<RoyalChain.Height&&Y+1.9f>0)
                {chain.Hit=true;Hurt(18);}
                chain.Active=Math.Max(0,chain.Active-dt);if(chain.Active<=0)RoyalChains.RemoveAt(i);
            }
        }
        void FireBloodOrbs(Enemy boss)
        {
            float originX=boss.X+boss.Facing*.8f,originY=boss.Y+1.4f;
            float angle=(float)Math.Atan2(boss.AimY-originY,boss.AimX-originX);
            int spread=boss.PhaseTwo?1:0;float speed=boss.PhaseTwo?11:9;
            for(int i=-spread;i<=spread;i++)
            {
                float direction=angle+i*.21f;
                BloodOrbs.Add(new BloodOrb {Owner=boss,X=originX,Y=originY,VelocityX=(float)Math.Cos(direction)*speed,VelocityY=(float)Math.Sin(direction)*speed});
            }
        }
        void UpdateBloodOrbs(float dt)
        {
            for(int i=BloodOrbs.Count-1;i>=0;i--)
            {
                var orb=BloodOrbs[i];
                if(orb.Owner.Hp<=0||Phase!=RunPhase.Combat){BloodOrbs.RemoveAt(i);continue;}
                float oldX=orb.X,oldY=orb.Y;
                orb.X+=orb.VelocityX*dt;orb.Y+=orb.VelocityY*dt;orb.Life-=dt;
                // Segment versus expanded player rectangle (Minkowski radius).
                float enter=0,leave=1;
                bool hit=ClipAxis(oldX,orb.X-oldX,X-.35f-BloodOrb.Radius,X+.35f+BloodOrb.Radius,ref enter,ref leave)&&
                         ClipAxis(oldY,orb.Y-oldY,Y+.15f-BloodOrb.Radius,Y+1.9f+BloodOrb.Radius,ref enter,ref leave);
                if(hit)Hurt(12);
                if(hit||orb.Life<=0||orb.X<0||orb.X>WorldWidth||orb.Y<0||orb.Y>12)BloodOrbs.RemoveAt(i);
            }
        }
        static bool ClipAxis(float origin,float delta,float min,float max,ref float enter,ref float leave)
        {
            if(Math.Abs(delta)<.000001f)return origin>=min&&origin<=max;
            float first=(min-origin)/delta,last=(max-origin)/delta;
            if(first>last){float swap=first;first=last;last=swap;}
            enter=Math.Max(enter,first);leave=Math.Min(leave,last);return enter<=leave;
        }
        static float Clamp(float value,float min,float max)=>Math.Max(min,Math.Min(max,value));
    }
}
