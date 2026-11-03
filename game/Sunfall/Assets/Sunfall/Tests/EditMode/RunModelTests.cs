using NUnit.Framework;
using Sunfall.Core;
namespace Sunfall.Tests
{
    public partial class RunModelTests
    {
        [TestCase(SpellKind.AshLance)]
        [TestCase(SpellKind.CrimsonNova)]
        public void BossPortalContinuesTheSameJourneyAndRewardSpellWorksInChapterTwo(SpellKind spell)
        {
            var equipment=new EquipmentInventory(31,new[]{2,1,0,1,0});
            equipment.Equip(EquipmentId.AshSword);equipment.Equip(EquipmentId.PilgrimCoat);equipment.Equip(EquipmentId.FurnaceHeart);
            var r=WithBossSpell(spell,equipment);r.Progression.Allocate(GrowthStat.Attack);
            var progression=r.Progression;float sword=r.SwordDamage,magic=r.MagicDamage,maxHp=r.MaxHp;
            int gold=progression.Gold,points=progression.Points,kills=r.Kills;
            r.Hp=57;r.X=RunModel.ExitX;r.Waves.Add(new EmberWave {X=20,Y=1,Facing=1,Damage=1});
            r.Step(0,false,false,enter:true);
            Assert.AreEqual(2,r.Chapter);Assert.AreEqual(0,r.RoomIndex);Assert.AreEqual(RunStage.Approach,r.Stage);Assert.AreEqual(RunPhase.Combat,r.Phase);
            Assert.AreEqual(2,r.X);Assert.AreEqual(0,r.Y);Assert.AreEqual(57,r.Hp);Assert.AreEqual(maxHp,r.MaxHp);
            Assert.AreSame(equipment,r.Equipment);Assert.AreSame(progression,r.Progression);
            Assert.AreEqual(sword,r.SwordDamage);Assert.AreEqual(magic,r.MagicDamage);Assert.AreEqual(gold,r.Progression.Gold);Assert.AreEqual(points,r.Progression.Points);Assert.AreEqual(kills,r.Kills);
            Assert.AreEqual(0,r.Waves.Count);Assert.IsFalse(r.BossDefeated);Assert.IsFalse(r.ChestAvailable);Assert.IsFalse(r.Cinematic);
            Assert.AreEqual(6,r.RemainingMonsters);Assert.IsFalse(r.ExitOpen);Assert.IsTrue(r.HasSpell(spell));
            r.Step(0,false,false,lance:spell==SpellKind.AshLance,nova:spell==SpellKind.CrimsonNova);
            Assert.IsTrue(r.Casting);for(int i=0;i<15;i++)r.Step(0,false,false);
            if(spell==SpellKind.AshLance){Assert.Greater(r.LanceCooldown,0);Assert.IsTrue(r.Waves.Exists(w=>w.Lance));Assert.IsFalse(r.HasSpell(SpellKind.CrimsonNova));}
            else {Assert.Greater(r.NovaCooldown,0);Assert.Greater(r.NovaTimer,0);Assert.IsFalse(r.HasSpell(SpellKind.AshLance));}
            var restart=new RunModel(r.Seed,equipment,progression);Assert.AreEqual(1,restart.Chapter);Assert.IsNull(restart.BossSpellReward);
        }
        [Test] public void ChapterTwoRoomsHaveSupportedEnemiesAndRequireEveryClearAndExplicitPortalPress()
        {
            var r=WithBossSpell(SpellKind.AshLance);r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);
            var reward=r.BossSpellReward;int[] counts={6,7,8};
            for(int room=0;room<3;room++)
            {
                Assert.AreEqual(2,r.Chapter);Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(counts[room],r.RemainingMonsters);
                Assert.IsFalse(r.Enemies.Exists(e=>e.Boss));
                foreach(var platform in r.Platforms){Assert.GreaterOrEqual(platform.X,0);Assert.LessOrEqual(platform.X+platform.Width,RunModel.WorldWidth);}
                foreach(var enemy in r.Enemies)
                {
                    Assert.GreaterOrEqual(enemy.X,8);Assert.LessOrEqual(enemy.X,56);
                    if(enemy.Y>0)Assert.IsTrue(r.Platforms.Exists(p=>p.Y==enemy.Y&&p.X<=enemy.MinX&&p.X+p.Width>=enemy.MaxX));
                }
                r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(RunPhase.Combat,r.Phase);Assert.Less(r.X,RunModel.ExitX);
                foreach(var enemy in r.Enemies)enemy.Hp=0;
                r.X=RunModel.ExitX;r.Step(0,false,false);Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(RunPhase.Combat,r.Phase);
                Assert.IsTrue(r.CanUseExit);r.Step(0,false,false,enter:true);
                Assert.AreEqual(reward,r.BossSpellReward);
                Assert.AreEqual(RunPhase.Combat,r.Phase);
            }
            Assert.AreEqual(RunStage.BossArena,r.Stage);Assert.IsTrue(r.BossIntroducing);
            Assert.AreEqual(1,r.Enemies.Count);Assert.IsTrue(r.Enemies[0].ChainWarden);
        }
        static RunModel EnterChainBoss()
        {
            var r=WithBossSpell(SpellKind.AshLance);r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);
            EnterBoss(r);r.HurtTimer=0;return r;
        }
        [Test] public void ChainWardenBlocksExitAndDefeatPreservesSpellAndClearsHazards()
        {
            var r=EnterChainBoss();var boss=r.Enemies[0];var reward=r.BossSpellReward;
            Assert.AreEqual(420,boss.MaxHp);Assert.AreEqual(32,boss.X);Assert.IsFalse(boss.Flying);
            r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);Assert.AreEqual(RunPhase.Combat,r.Phase);Assert.IsFalse(r.ExitOpen);
            r.RoyalChains.Add(new RoyalChain {Owner=boss,X=r.X,Delay=1});
            boss.Hp=0;r.Step(0,false,false);Assert.IsTrue(r.BossDissolving);Assert.AreEqual(0,r.RoyalChains.Count);
            Assert.AreEqual(reward,r.BossSpellReward);Assert.IsNull(r.ChainSpellReward);Assert.AreEqual(2,r.PendingSkillChapter);
            FinishBossDefeat(r);Assert.IsTrue(r.ExitOpen);Assert.IsTrue(r.ChestAvailable);
            r.X=r.ChestX;r.Hp=40;r.Step(0,false,false,enter:true);
            Assert.IsTrue(r.ChestOpened);Assert.AreEqual(80,r.Hp);Assert.AreNotEqual(EquipmentId.None,r.LastEquipmentReward);
            r.X=RunModel.ExitX;r.Step(0,false,false);Assert.AreEqual(RunPhase.Combat,r.Phase);
            var chainReward=r.ChainSpellReward;r.Step(0,false,false,enter:true);
            Assert.AreEqual(RunPhase.Combat,r.Phase);Assert.AreEqual(3,r.Chapter);
            Assert.AreEqual(reward,r.BossSpellReward);Assert.AreEqual(chainReward,r.ChainSpellReward);
        }
        [TestCase(0,78)] [TestCase(1.6f,100)]
        public void ChainSlamHitsGroundAndAllowsElevatedDodge(float height,float expectedHp)
        {
            var r=EnterChainBoss();var boss=r.Enemies[0];r.X=32;r.Y=height;r.VelocityY=0;
            boss.Pattern=BossPattern.ChainSlam;boss.AimX=32;boss.Windup=RunModel.StepSeconds*.5f;
            r.Step(0,false,false);Assert.AreEqual(expectedHp,r.Hp);
        }
        [Test] public void BindingSlowsMovementAndRollBreaksIt()
        {
            var r=EnterChainBoss();var boss=r.Enemies[0];r.X=32;
            boss.Pattern=BossPattern.ChainBind;boss.AimX=32;boss.Windup=RunModel.StepSeconds*.5f;
            r.Step(0,false,false);Assert.AreEqual(86,r.Hp);Assert.Greater(r.SnareTimer,0);
            float x=r.X;r.Step(1,false,false);
            Assert.That(r.X-x,Is.EqualTo(r.MovementSpeed*.4f*RunModel.StepSeconds).Within(.001f));
            r.Step(1,false,false,roll:true);Assert.IsTrue(r.Rolling);Assert.AreEqual(0,r.SnareTimer);
        }
        [TestCase(false,5)] [TestCase(true,7)]
        public void ChainStormWarnsBeforeDamageAndPhaseTwoAddsColumns(bool phaseTwo,int count)
        {
            var r=EnterChainBoss();var boss=r.Enemies[0];boss.PhaseTwo=phaseTwo;r.X=32;
            boss.Pattern=BossPattern.ChainStorm;boss.AimX=32;boss.Windup=RunModel.StepSeconds*.5f;
            r.Step(0,false,false);Assert.AreEqual(count,r.RoyalChains.Count);Assert.AreEqual(100,r.Hp);
            var chain=r.RoyalChains[0];r.X=chain.X;chain.Delay=RunModel.StepSeconds*.5f;
            r.Step(0,false,false);Assert.AreEqual(82,r.Hp);
            r.HurtTimer=0;r.Step(0,false,false);Assert.AreEqual(82,r.Hp);
        }
        [Test] public void HalfHealthTransitionCancelsChainsAndOnlyOccursOnce()
        {
            var r=EnterChainBoss();var boss=r.Enemies[0];boss.Hp=210;boss.Windup=.5f;
            r.RoyalChains.Add(new RoyalChain {Owner=boss,X=r.X});r.Step(0,false,false);
            Assert.IsTrue(boss.PhaseTwo);Assert.Greater(boss.PhaseTransition,0);Assert.AreEqual(0,boss.Windup);
            Assert.AreEqual(0,r.RoyalChains.Count);Assert.AreEqual(3,boss.ChainBindCount);Assert.AreEqual(100,r.Hp);
            float transition=boss.PhaseTransition;r.Step(0,false,false);Assert.Less(boss.PhaseTransition,transition);
        }
        [Test] public void ChainWardenKillPaysItsRewardOnlyOnce()
        {
            var r=EnterChainBoss();var boss=r.Enemies[0];boss.Hp=1;boss.Cooldown=100;
            r.X=31;r.Facing=1;int gold=r.Progression.Gold;
            r.Step(0,false,true);for(int i=0;i<8;i++)r.Step(0,false,false);
            Assert.IsTrue(boss.RewardPaid);Assert.AreEqual(gold+250,r.Progression.Gold);
            FinishBossDefeat(r);r.Step(0,false,true);Assert.AreEqual(gold+250,r.Progression.Gold);
        }
        static RunModel WithChainSpell(SpellKind spell,SpellKind? firstReward=null)
        {
            for(int seed=1;seed<=64;seed++)
            {
                var r=new RunModel(seed);EnterBoss(r);r.Enemies[0].Hp=0;r.Step(0,false,false);FinishBossDefeat(r);
                r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);EnterBoss(r);
                r.Enemies[0].Hp=0;r.Step(0,false,false);FinishBossDefeat(r);
                if(r.HasSpell(spell)&&(!firstReward.HasValue||r.BossSpellReward==firstReward)){r.Enemies.Clear();r.X=10;r.Y=0;r.HurtTimer=0;return r;}
            }
            Assert.Fail("No seed produced the requested chain reward.");return null;
        }
        [TestCase(SpellKind.AshLance,SpellKind.ChainPull)]
        [TestCase(SpellKind.AshLance,SpellKind.ChainRain)]
        [TestCase(SpellKind.CrimsonNova,SpellKind.ChainPull)]
        [TestCase(SpellKind.CrimsonNova,SpellKind.ChainRain)]
        public void SecondBossPortalPreservesBothRewardsAndProgressAndCastsInCathedral(SpellKind first,SpellKind second)
        {
            var r=WithChainSpell(second,first);r.Equipment.Grant(EquipmentId.PilgrimCoat);r.Equipment.Equip(EquipmentId.PilgrimCoat);
            r.Progression.Allocate(GrowthStat.Magic);var equipment=r.Equipment;var growth=r.Progression;
            float magicDamage=r.MagicDamage,maxHp=r.MaxHp;int gold=growth.Gold,level=growth.Level,xp=growth.Experience,kills=r.Kills;
            r.Step(0,false,false,chain:true);for(int i=0;i<15;i++)r.Step(0,false,false);
            Assert.Greater(r.ChainCooldown,0);
            r.Hp=53;r.X=RunModel.ExitX;r.Y=0;r.VelocityY=0;
            r.Waves.Add(new EmberWave {X=20,Y=1});r.RoyalChains.Add(new RoyalChain {Owner=new Enemy {Hp=100},X=20,Delay=10});
            r.Step(0,false,false,enter:true);
            Assert.AreEqual(3,r.Chapter);Assert.AreEqual(0,r.RoomIndex);Assert.AreEqual(RunStage.Approach,r.Stage);Assert.AreEqual(RunPhase.Combat,r.Phase);
            Assert.AreEqual(2,r.X);Assert.AreEqual(0,r.Y);Assert.AreEqual(53,r.Hp);Assert.AreEqual(maxHp,r.MaxHp);
            Assert.AreSame(equipment,r.Equipment);Assert.AreSame(growth,r.Progression);Assert.AreEqual(magicDamage,r.MagicDamage);
            Assert.AreEqual(gold,growth.Gold);Assert.AreEqual(level,growth.Level);Assert.AreEqual(xp,growth.Experience);Assert.AreEqual(kills,r.Kills);
            Assert.AreEqual(first,r.BossSpellReward);Assert.AreEqual(second,r.ChainSpellReward);Assert.IsTrue(r.HasSpell(SpellKind.EmberWave));
            Assert.AreEqual(0,r.Waves.Count);Assert.AreEqual(0,r.RoyalChains.Count);Assert.AreEqual(0,r.ChainCooldown);Assert.AreEqual(0,r.ChainEffectTimer);
            Assert.IsFalse(r.BossDefeated);Assert.IsFalse(r.Cinematic);Assert.IsFalse(r.ChestAvailable);Assert.IsFalse(r.ExitOpen);Assert.AreEqual(7,r.RemainingMonsters);
            r.Enemies.Clear();r.Step(0,false,false,chain:true);Assert.AreEqual(second,r.CurrentSpell);
            for(int i=0;i<30;i++)r.Step(0,false,false);Assert.Greater(r.ChainCooldown,0);
            r.Step(0,false,false,lance:first==SpellKind.AshLance,nova:first==SpellKind.CrimsonNova);Assert.IsTrue(r.Casting);Assert.AreEqual(first,r.CurrentSpell);
            for(int i=0;i<30;i++)r.Step(0,false,false);
            Assert.Greater(first==SpellKind.AshLance?r.LanceCooldown:r.NovaCooldown,0);
            r.Step(0,false,false,magic:true);Assert.IsTrue(r.Casting);Assert.AreEqual(SpellKind.EmberWave,r.CurrentSpell);
            var restart=new RunModel(r.Seed,equipment,growth);Assert.AreEqual(1,restart.Chapter);Assert.IsNull(restart.BossSpellReward);Assert.IsNull(restart.ChainSpellReward);
        }
        [Test] public void CathedralRoomsRequireEveryClearAndExplicitGroundedExitToSolarBoss()
        {
            var r=WithChainSpell(SpellKind.ChainRain);var first=r.BossSpellReward;var second=r.ChainSpellReward;
            r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);
            for(int room=0;room<3;room++)
            {
                Assert.AreEqual(3,r.Chapter);Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(7+room,r.RemainingMonsters);
                Assert.IsFalse(r.Enemies.Exists(e=>e.Boss));
                foreach(var p in r.Platforms){Assert.GreaterOrEqual(p.X,0);Assert.LessOrEqual(p.X+p.Width,RunModel.WorldWidth);}
                foreach(var e in r.Enemies)
                {
                    if(e.Y>0)Assert.IsTrue(r.Platforms.Exists(p=>p.Y==e.Y&&p.X<=e.MinX&&p.X+p.Width>=e.MaxX));
                    float baseHp=e.Kind==EnemyKind.Heavy?100:e.Kind==EnemyKind.Archer?36:44;
                    Assert.That(e.MaxHp,Is.EqualTo(baseHp*1.4f).Within(.001f));
                }
                r.X=RunModel.ExitX;r.Y=6;r.Step(1,false,false,enter:true);
                Assert.Less(r.X,RunModel.ExitX);Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(RunPhase.Combat,r.Phase);
                foreach(var e in r.Enemies)e.Hp=0;
                r.X=RunModel.ExitX;r.Y=3;r.VelocityY=0;r.Step(0,false,false,enter:true);
                Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(RunPhase.Combat,r.Phase);
                r.Y=0;r.VelocityY=0;r.Step(0,false,false);Assert.IsTrue(r.CanUseExit);Assert.AreEqual(room,r.RoomIndex);
                r.Step(0,false,false,enter:true);
                Assert.AreEqual(first,r.BossSpellReward);Assert.AreEqual(second,r.ChainSpellReward);
                Assert.AreEqual(RunPhase.Combat,r.Phase);
            }
            Assert.AreEqual(3,r.RoomIndex);Assert.AreEqual(RunStage.BossArena,r.Stage);Assert.IsTrue(r.BossIntroducing);
            Assert.IsTrue(r.Enemies[0].SolarRegent);Assert.AreEqual(600,r.Enemies[0].MaxHp);
        }
        static RunModel EnterSolarBoss(SpellKind second=SpellKind.ChainPull,SpellKind? first=null)
        {
            var r=WithChainSpell(second,first);r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);EnterBoss(r);r.HurtTimer=0;return r;
        }
        [TestCase(0,76)] [TestCase(1.6f,100)]
        public void SolarSweepHitsGroundAndAllowsJumpAndPlatformDodge(float height,float expectedHp)
        {
            var r=EnterSolarBoss();var boss=r.Enemies[0];r.X=30;r.Y=height;r.VelocityY=0;
            boss.Pattern=BossPattern.SolarSweep;boss.Windup=RunModel.StepSeconds*.5f;
            r.Step(0,false,false);Assert.AreEqual(expectedHp,r.Hp);
        }
        [TestCase(false,3)] [TestCase(true,5)]
        public void SolarLancesLockTheirTelegraphAndHitOnlyTheirColumns(bool phaseTwo,int count)
        {
            var r=EnterSolarBoss();var boss=r.Enemies[0];boss.PhaseTwo=phaseTwo;boss.PatternIndex=1;boss.Cooldown=0;r.X=30;
            r.Step(0,false,false);Assert.AreEqual(BossPattern.SolarLances,boss.Pattern);Assert.AreEqual(count,boss.SolarLanceCount);
            float aim=boss.AimX;r.Step(1,false,false);Assert.AreEqual(aim,boss.AimX);
            r.X=aim+2;boss.Windup=RunModel.StepSeconds*.5f;r.Step(0,false,false);Assert.AreEqual(100,r.Hp);
            r.X=aim;boss.Windup=RunModel.StepSeconds*.5f;r.Step(0,false,false);Assert.AreEqual(82,r.Hp);
        }
        [TestCase(20,false,100)] [TestCase(24,false,72)] [TestCase(24,true,100)]
        public void EclipseHonorsSafeBandAndRollInvulnerability(float x,bool roll,float expectedHp)
        {
            var r=EnterSolarBoss();var boss=r.Enemies[0];r.X=x;
            boss.Pattern=BossPattern.Eclipse;boss.AimX=20;boss.Windup=RunModel.StepSeconds*.5f;
            r.Step(0,false,false,roll:roll);Assert.AreEqual(expectedHp,r.Hp);
        }
        [Test] public void SolarHalfHealthCancelsAttackAndTransitionsOnlyOnce()
        {
            var r=EnterSolarBoss();var boss=r.Enemies[0];r.X=32;boss.Hp=300;boss.Pattern=BossPattern.SolarSweep;boss.Windup=.001f;
            r.Step(0,false,false);Assert.IsTrue(boss.PhaseTwo);Assert.AreEqual(0,boss.Windup);Assert.AreEqual(100,r.Hp);
            Assert.AreEqual(5,boss.SolarLanceCount);Assert.AreEqual(2.4f,boss.EclipseSafeRadius);
            float time=boss.PhaseTransition;r.Step(0,false,false);Assert.Less(boss.PhaseTransition,time);
        }
        [TestCase(SpellKind.ChainPull)] [TestCase(SpellKind.ChainRain)]
        public void SolarBossDefeatPreservesSkillsPaysOnceAndRequiresPortalAfterCinematic(SpellKind spell)
        {
            var r=EnterSolarBoss(spell);var boss=r.Enemies[0];var first=r.BossSpellReward;var second=r.ChainSpellReward;
            r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);Assert.IsFalse(r.ExitOpen);Assert.AreEqual(RunPhase.Combat,r.Phase);
            r.X=31;r.Facing=1;boss.Hp=1;boss.PhaseTwo=true;boss.Cooldown=100;int gold=r.Progression.Gold;
            r.Step(0,false,true);for(int i=0;i<8;i++)r.Step(0,false,false);
            Assert.IsTrue(r.BossDissolving);Assert.IsFalse(r.ExitOpen);Assert.AreEqual(0,boss.Windup);Assert.IsTrue(boss.RewardPaid);
            Assert.AreEqual(gold+350,r.Progression.Gold);Assert.AreEqual(first,r.BossSpellReward);Assert.AreEqual(second,r.ChainSpellReward);
            Assert.AreEqual(0,r.SpellRewardNotice);Assert.IsNull(r.SolarSpellReward);Assert.AreEqual(3,r.PendingSkillChapter);
            r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);Assert.AreEqual(RunPhase.Combat,r.Phase);
            FinishBossDefeat(r);Assert.IsTrue(r.ExitOpen);Assert.IsTrue(r.ChestAvailable);
            r.X=r.ChestX;r.Hp=40;r.Step(0,false,false,enter:true);Assert.AreEqual(80,r.Hp);Assert.IsTrue(r.ChestOpened);
            Assert.AreNotEqual(EquipmentId.None,r.LastEquipmentReward);Assert.AreEqual(gold+350,r.Progression.Gold);
            r.X=RunModel.ExitX;r.Step(0,false,false);Assert.AreEqual(RunPhase.Combat,r.Phase);
            var solar=r.SolarSpellReward;r.Step(0,false,false,enter:true);Assert.AreEqual(RunPhase.Combat,r.Phase);Assert.AreEqual(4,r.Chapter);
            Assert.AreEqual(first,r.BossSpellReward);Assert.AreEqual(second,r.ChainSpellReward);Assert.AreEqual(solar,r.SolarSpellReward);
        }
        static RunModel FinishThreeBosses(int seed)
        {
            var r=new RunModel(seed);
            for(int chapter=1;chapter<=3;chapter++)
            {
                EnterBoss(r);r.Enemies[0].Hp=0;r.Step(0,false,false);FinishBossDefeat(r);
                if(chapter<3){r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);}
            }
            r.Enemies.Clear();r.X=10;r.Y=0;return r;
        }
        static RunModel WithSolarSpell(SpellKind spell)
        {
            for(int seed=1;seed<=64;seed++)
            {
                var r=FinishThreeBosses(seed);if(r.SolarSpellReward==spell)return r;
            }
            Assert.Fail("No seed produced the requested solar reward.");return null;
        }
        [TestCase(SpellKind.SunLance)] [TestCase(SpellKind.DawnBurst)]
        public void ThirdPortalPreservesEveryRewardAndProgressAndAllSpellsWorkInChapterFour(SpellKind spell)
        {
            var r=WithSolarSpell(spell);var first=r.BossSpellReward;var chain=r.ChainSpellReward;
            r.Equipment.Grant(EquipmentId.PilgrimCoat);r.Equipment.Equip(EquipmentId.PilgrimCoat);r.Progression.Allocate(GrowthStat.Magic);
            var equipment=r.Equipment;var growth=r.Progression;int gold=growth.Gold,level=growth.Level,xp=growth.Experience,kills=r.Kills;
            float damage=r.MagicDamage,maxHp=r.MaxHp;
            r.Step(0,false,false,solar:true);for(int i=0;i<15;i++)r.Step(0,false,false);Assert.Greater(r.SolarCooldown,0);
            r.Waves.Add(new EmberWave {X=20,Y=1});r.RoyalChains.Add(new RoyalChain {Owner=new Enemy {Hp=100},X=20,Delay=10});
            r.X=RunModel.ExitX;r.Y=0;r.VelocityY=0;r.Hp=67;r.Step(0,false,false,enter:true);
            Assert.AreEqual(4,r.Chapter);Assert.AreEqual(0,r.RoomIndex);Assert.AreEqual(RunStage.Approach,r.Stage);Assert.AreEqual(RunPhase.Combat,r.Phase);
            Assert.AreEqual(2,r.X);Assert.AreEqual(0,r.Y);Assert.AreEqual(67,r.Hp);Assert.AreEqual(maxHp,r.MaxHp);Assert.AreEqual(damage,r.MagicDamage);
            Assert.AreSame(equipment,r.Equipment);Assert.AreSame(growth,r.Progression);Assert.AreEqual(gold,growth.Gold);Assert.AreEqual(level,growth.Level);Assert.AreEqual(xp,growth.Experience);Assert.AreEqual(kills,r.Kills);
            Assert.AreEqual(first,r.BossSpellReward);Assert.AreEqual(chain,r.ChainSpellReward);Assert.AreEqual(spell,r.SolarSpellReward);
            Assert.AreEqual(0,r.Waves.Count);Assert.AreEqual(0,r.RoyalChains.Count);Assert.AreEqual(0,r.SolarCooldown);Assert.AreEqual(0,r.DawnTimer);
            Assert.AreEqual(0,r.ChainCooldown);Assert.IsFalse(r.BossDefeated);Assert.IsFalse(r.Cinematic);Assert.IsFalse(r.ChestAvailable);Assert.AreEqual(8,r.RemainingMonsters);
            r.Enemies.Clear();r.Step(0,false,false,solar:true);Assert.IsTrue(r.Casting);Assert.AreEqual(spell,r.CurrentSpell);
            for(int i=0;i<30;i++)r.Step(0,false,false);Assert.Greater(r.SolarCooldown,0);
            r.Step(0,false,false,chain:true);Assert.IsTrue(r.Casting);Assert.AreEqual(chain.Value,r.CurrentSpell);
            for(int i=0;i<30;i++)r.Step(0,false,false);Assert.Greater(r.ChainCooldown,0);
            r.Step(0,false,false,lance:first==SpellKind.AshLance,nova:first==SpellKind.CrimsonNova);Assert.IsTrue(r.Casting);Assert.AreEqual(first.Value,r.CurrentSpell);
            for(int i=0;i<30;i++)r.Step(0,false,false);
            r.Step(0,false,false,magic:true);Assert.IsTrue(r.Casting);Assert.AreEqual(SpellKind.EmberWave,r.CurrentSpell);
            var restart=new RunModel(r.Seed,equipment,growth);Assert.AreEqual(1,restart.Chapter);Assert.IsNull(restart.BossSpellReward);Assert.IsNull(restart.ChainSpellReward);Assert.IsNull(restart.SolarSpellReward);
        }
        [Test] public void CrownHeartRoomsHaveSupportedGuardsAndRequireClearAndGroundedPortalInput()
        {
            var r=WithSolarSpell(SpellKind.DawnBurst);var first=r.BossSpellReward;var chain=r.ChainSpellReward;var solar=r.SolarSpellReward;
            r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);
            for(int room=0;room<3;room++)
            {
                Assert.AreEqual(4,r.Chapter);Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(8+room,r.RemainingMonsters);Assert.IsFalse(r.Enemies.Exists(e=>e.Boss));
                foreach(var p in r.Platforms){Assert.GreaterOrEqual(p.X,0);Assert.LessOrEqual(p.X+p.Width,RunModel.WorldWidth);}
                foreach(var e in r.Enemies)
                {
                    Assert.GreaterOrEqual(e.X,8);Assert.LessOrEqual(e.X,56);
                    if(e.Y>0)Assert.IsTrue(r.Platforms.Exists(p=>p.Y==e.Y&&p.X<=e.MinX&&p.X+p.Width>=e.MaxX));
                    float baseHp=e.Kind==EnemyKind.Heavy?100:e.Kind==EnemyKind.Archer?36:44;
                    Assert.That(e.MaxHp,Is.EqualTo(baseHp*1.6f).Within(.001f));
                }
                r.X=RunModel.ExitX;r.Y=6;r.Step(1,false,false,enter:true);Assert.Less(r.X,RunModel.ExitX);Assert.AreEqual(room,r.RoomIndex);
                foreach(var e in r.Enemies)e.Hp=0;
                r.X=RunModel.ExitX;r.Y=3;r.VelocityY=0;r.Step(0,false,false,enter:true);Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(RunPhase.Combat,r.Phase);
                r.Y=0;r.VelocityY=0;r.Step(0,false,false);Assert.IsTrue(r.CanUseExit);Assert.AreEqual(room,r.RoomIndex);
                r.Step(0,false,false,enter:true);Assert.AreEqual(RunPhase.Combat,r.Phase);
                Assert.AreEqual(first,r.BossSpellReward);Assert.AreEqual(chain,r.ChainSpellReward);Assert.AreEqual(solar,r.SolarSpellReward);
            }
            Assert.AreEqual(3,r.RoomIndex);Assert.AreEqual(RunStage.BossArena,r.Stage);Assert.AreEqual(4,r.Enemies[0].LateChapter);
        }
        [TestCase(SpellKind.SunLance)] [TestCase(SpellKind.DawnBurst)]
        public void ThirdBossRewardPreservesAllPriorSkillsAndIsDeterministicAndNeverRerolled(SpellKind spell)
        {
            var r=WithSolarSpell(spell);var first=r.BossSpellReward;var second=r.ChainSpellReward;
            Assert.IsFalse(r.ChestOpened);Assert.AreEqual(spell,r.LatestSpellReward);
            Assert.AreNotEqual(r.HasSpell(SpellKind.SunLance),r.HasSpell(SpellKind.DawnBurst));
            Assert.IsTrue(r.HasSpell(first.Value));Assert.IsTrue(r.HasSpell(second.Value));Assert.IsTrue(r.HasSpell(SpellKind.EmberWave));
            r.Step(0,false,false,solar:true);Assert.AreEqual(spell,r.CurrentSpell);
            for(int i=0;i<30;i++)r.Step(0,false,false);Assert.Greater(r.SolarCooldown,0);
            Assert.AreEqual(0,r.ChainCooldown);Assert.AreEqual(0,r.LanceCooldown);Assert.AreEqual(0,r.NovaCooldown);
            r.Step(0,false,false,chain:true);Assert.IsTrue(r.Casting);Assert.AreEqual(second.Value,r.CurrentSpell);
            for(int i=0;i<30;i++)r.Step(0,false,false);
            r.Step(0,false,false,lance:first==SpellKind.AshLance,nova:first==SpellKind.CrimsonNova);Assert.IsTrue(r.Casting);Assert.AreEqual(first.Value,r.CurrentSpell);
            for(int i=0;i<30;i++)r.Step(0,false,false);
            r.Step(0,false,false,magic:true);Assert.IsTrue(r.Casting);Assert.AreEqual(SpellKind.EmberWave,r.CurrentSpell);
            for(int i=0;i<30;i++)r.Step(0,false,false);
            r.X=r.ChestX;r.Step(0,false,false,enter:true);Assert.IsTrue(r.ChestOpened);
            Assert.AreEqual(first,r.BossSpellReward);Assert.AreEqual(second,r.ChainSpellReward);Assert.AreEqual(spell,r.SolarSpellReward);
            var replay=FinishThreeBosses(r.Seed);Assert.AreEqual(first,replay.BossSpellReward);Assert.AreEqual(second,replay.ChainSpellReward);Assert.AreEqual(spell,replay.SolarSpellReward);
            var fresh=new RunModel(r.Seed,r.Equipment,r.Progression);Assert.IsNull(fresh.SolarSpellReward);Assert.IsNull(fresh.ChainSpellReward);Assert.IsNull(fresh.BossSpellReward);
        }
        [TestCase(SpellKind.SunLance)] [TestCase(SpellKind.DawnBurst)]
        public void LockedSolarInputDoesNothingAndRollingCancelsStartup(SpellKind spell)
        {
            var locked=new RunModel(4);locked.Enemies.Clear();locked.Step(0,false,false,solar:true);
            Assert.IsFalse(locked.Casting);Assert.AreEqual(0,locked.SolarCooldown);Assert.AreEqual(0,locked.Waves.Count);Assert.AreEqual(0,locked.DawnTimer);
            var r=WithSolarSpell(spell);r.Step(0,false,false,solar:true);r.Step(0,false,false,roll:true);
            for(int i=0;i<30;i++)r.Step(0,false,false);
            Assert.AreEqual(0,r.SolarCooldown);Assert.AreEqual(0,r.Waves.Count);Assert.AreEqual(0,r.DawnTimer);
        }
        [Test] public void SunLancePiercesForwardTargetsOnceAndRespectsHeightAndMaximumRange()
        {
            var r=WithSolarSpell(SpellKind.SunLance);r.Platforms.Clear();
            var a=new Enemy {X=13,Hp=300,MaxHp=300,Cooldown=100,MinX=13,MaxX=13};
            var b=new Enemy {X=18,Hp=300,MaxHp=300,Cooldown=100,MinX=18,MaxX=18};
            var rear=new Enemy {X=8,Hp=300,MaxHp=300,Cooldown=100,MinX=8,MaxX=8};
            var high=new Enemy {X=15,Y=4,Hp=300,MaxHp=300,Cooldown=100,MinX=15,MaxX=15};
            var far=new Enemy {X=27,Hp=300,MaxHp=300,Cooldown=100,MinX=27,MaxX=27};
            r.Enemies.AddRange(new[]{a,b,rear,high,far});r.Step(0,false,false,solar:true);
            for(int i=0;i<70;i++)r.Step(0,false,false,solar:true);
            Assert.AreEqual(204,a.Hp);Assert.AreEqual(204,b.Hp);Assert.AreEqual(300,rear.Hp);Assert.AreEqual(300,high.Hp);Assert.AreEqual(300,far.Hp);
            Assert.AreEqual(0,r.Waves.Count);Assert.Greater(r.SolarCooldown,0);
        }
        [Test] public void DawnBurstHitsNearbyEnemiesOnBothSidesOnlyOnce()
        {
            var r=WithSolarSpell(SpellKind.DawnBurst);r.Platforms.Clear();
            var a=new Enemy {X=7,Hp=300,MaxHp=300,Cooldown=100,MinX=7,MaxX=7};
            var b=new Enemy {X=13,Hp=300,MaxHp=300,Cooldown=100,MinX=13,MaxX=13};
            var high=new Enemy {X=10,Y=8,Hp=300,MaxHp=300,Cooldown=100,MinX=10,MaxX=10};
            var far=new Enemy {X=17,Hp=300,MaxHp=300,Cooldown=100,MinX=17,MaxX=17};
            r.Enemies.AddRange(new[]{a,b,high,far});r.Step(0,false,false,solar:true);
            for(int i=0;i<60;i++)r.Step(0,false,false,solar:true);
            Assert.That(a.Hp,Is.EqualTo(300-24*3.2f).Within(.001f));Assert.AreEqual(a.Hp,b.Hp);Assert.AreEqual(300,high.Hp);Assert.AreEqual(300,far.Hp);
        }
        [TestCase(SpellKind.SunLance,7.2f)] [TestCase(SpellKind.DawnBurst,9f)]
        public void SolarCooldownUsesEquipmentAndStartsOnlyOnRelease(SpellKind spell,float cooldown)
        {
            var r=WithSolarSpell(spell);r.Equipment.Grant(EquipmentId.SanctumSeal);r.Equipment.Equip(EquipmentId.SanctumSeal);
            r.Step(0,false,false,solar:true);Assert.AreEqual(0,r.SolarCooldown);
            while(r.Casting)r.Step(0,false,false);
            Assert.That(r.SolarCooldown,Is.EqualTo(cooldown).Within(.001f));
        }
        [TestCase(SpellKind.AshLance,SpellKind.ChainPull)] [TestCase(SpellKind.AshLance,SpellKind.ChainRain)]
        [TestCase(SpellKind.CrimsonNova,SpellKind.ChainPull)] [TestCase(SpellKind.CrimsonNova,SpellKind.ChainRain)]
        public void BothEarlierRewardSpellsCanDamageSolarBoss(SpellKind first,SpellKind second)
        {
            var r=EnterSolarBoss(second,first);var boss=r.Enemies[0];r.X=30;r.Facing=1;boss.Cooldown=100;
            r.Step(0,false,false,chain:true);for(int i=0;i<30;i++)r.Step(0,false,false);
            Assert.Less(boss.Hp,600);Assert.AreEqual(32,boss.X);float hp=boss.Hp;
            r.Step(0,false,false,lance:first==SpellKind.AshLance,nova:first==SpellKind.CrimsonNova);
            for(int i=0;i<30;i++)r.Step(0,false,false);Assert.Less(boss.Hp,hp);
        }
        [TestCase(SpellKind.ChainPull)] [TestCase(SpellKind.ChainRain)]
        public void SecondBossGrantsOneChainSpellWithoutReplacingFirstReward(SpellKind spell)
        {
            var r=WithChainSpell(spell);var first=r.BossSpellReward;var second=r.ChainSpellReward;
            Assert.IsTrue(first.HasValue);Assert.IsTrue(r.HasSpell(first.Value));Assert.AreEqual(spell,second);
            Assert.AreNotEqual(r.HasSpell(SpellKind.ChainPull),r.HasSpell(SpellKind.ChainRain));Assert.IsFalse(r.ChestOpened);
            for(int i=0;i<60;i++)r.Step(0,false,false);
            Assert.AreEqual(second,r.ChainSpellReward);Assert.AreEqual(first,r.BossSpellReward);
            r.Step(0,false,false,chain:true);Assert.IsTrue(r.Casting);
            for(int i=0;i<30;i++)r.Step(0,false,false);
            Assert.Greater(r.ChainCooldown,0);Assert.AreEqual(0,r.LanceCooldown);Assert.AreEqual(0,r.NovaCooldown);
            r.Step(0,false,false,lance:first==SpellKind.AshLance,nova:first==SpellKind.CrimsonNova);
            Assert.IsTrue(r.Casting);Assert.AreEqual(first.Value,r.CurrentSpell);
            var restart=new RunModel(r.Seed,r.Equipment,r.Progression);
            Assert.IsNull(restart.ChainSpellReward);Assert.IsNull(restart.BossSpellReward);
        }
        [Test] public void LockedChainInputDoesNotCastAndRollCancelsChainStartup()
        {
            var r=new RunModel(2);r.Enemies.Clear();r.Step(0,false,false,chain:true);
            Assert.IsFalse(r.Casting);Assert.AreEqual(0,r.ChainCooldown);Assert.AreEqual(0,r.ChainEffectTimer);
            r=WithChainSpell(SpellKind.ChainPull);r.Step(0,false,false,chain:true);r.Step(0,false,false,roll:true);
            for(int i=0;i<30;i++)r.Step(0,false,false);
            Assert.AreEqual(0,r.ChainCooldown);Assert.AreEqual(0,r.ChainEffectTimer);
        }
        [Test] public void ChainPullChoosesNearestForwardTargetAndPullsSmoothlyToSupportedGround()
        {
            var r=WithChainSpell(SpellKind.ChainPull);r.Platforms.Clear();
            var near=new Enemy {X=15,Y=0,Hp=200,MaxHp=200,Cooldown=100,MinX=15,MaxX=15};
            var far=new Enemy {X=17,Y=0,Hp=200,MaxHp=200,Cooldown=100,MinX=17,MaxX=17};
            var behind=new Enemy {X=8,Y=0,Hp=200,MaxHp=200,Cooldown=100,MinX=8,MaxX=8};
            r.Enemies.Add(far);r.Enemies.Add(behind);r.Enemies.Add(near);
            r.Step(0,false,false,chain:true);while(r.Casting)r.Step(0,false,false);
            Assert.AreSame(near,r.ChainTarget);Assert.AreEqual(164,near.Hp);Assert.AreEqual(200,far.Hp);Assert.AreEqual(200,behind.Hp);
            Assert.Greater(near.X,11.4f);Assert.Less(near.X,15);
            while(near.PullRemaining>0)r.Step(0,false,false);
            Assert.That(near.X,Is.EqualTo(11.4f).Within(.001f));Assert.AreEqual(0,near.Y);
            Assert.LessOrEqual(near.MinX,near.X);Assert.GreaterOrEqual(near.MaxX,near.X);
        }
        [Test] public void ChainRainHitsBothSidesOnceAndRespectsHeightAndRange()
        {
            var r=WithChainSpell(SpellKind.ChainRain);r.Platforms.Clear();
            var left=new Enemy {X=7,Hp=200,MaxHp=200,Cooldown=100,MinX=7,MaxX=7};
            var right=new Enemy {X=13,Hp=200,MaxHp=200,Cooldown=100,MinX=13,MaxX=13};
            var high=new Enemy {X=10,Y=6,Hp=200,MaxHp=200,Cooldown=100,MinX=10,MaxX=10};
            var far=new Enemy {X=17,Hp=200,MaxHp=200,Cooldown=100,MinX=17,MaxX=17};
            r.Enemies.AddRange(new[]{left,right,high,far});r.Step(0,false,false,chain:true);
            for(int i=0;i<45;i++)r.Step(0,false,false,chain:true);
            Assert.That(left.Hp,Is.EqualTo(200-24*2.8f).Within(.001f));Assert.AreEqual(left.Hp,right.Hp);
            Assert.AreEqual(200,high.Hp);Assert.AreEqual(200,far.Hp);Assert.Greater(r.ChainCooldown,0);
        }
        [Test] public void ChainPullDamagesBossWithoutMovingItAndSealReducesCooldown()
        {
            var r=WithChainSpell(SpellKind.ChainPull);r.Equipment.Grant(EquipmentId.SanctumSeal);r.Equipment.Equip(EquipmentId.SanctumSeal);
            var boss=new Enemy {Boss=true,X=15,Hp=300,MaxHp=300,Windup=100,MinX=8,MaxX=56};r.Enemies.Add(boss);
            r.Step(0,false,false,chain:true);while(r.Casting)r.Step(0,false,false);
            Assert.AreEqual(264,boss.Hp);Assert.AreEqual(15,boss.X);Assert.AreEqual(0,boss.PullRemaining);
            Assert.That(r.ChainCooldown,Is.EqualTo(5.4f).Within(.001f));
        }
        [Test] public void ChainPullLandsAnElevatedEnemyOnThePlayersPlatform()
        {
            var r=WithChainSpell(SpellKind.ChainPull);r.Platforms.Clear();r.Platforms.Add(new Platform(8,5,1.6f));
            r.Y=1.6f;r.Grounded=true;r.VelocityY=0;
            var enemy=new Enemy {X=16,Y=1.6f,Hp=200,MaxHp=200,Cooldown=100,MinX=16,MaxX=16};r.Enemies.Add(enemy);
            r.Step(0,false,false,chain:true);while(r.Casting||enemy.PullRemaining>0)r.Step(0,false,false);
            Assert.That(enemy.X,Is.EqualTo(11.4f).Within(.001f));Assert.AreEqual(1.6f,enemy.Y);
            Assert.GreaterOrEqual(enemy.MinX,8);Assert.LessOrEqual(enemy.MaxX,13);
        }
        static RunModel WithBossSpell(SpellKind spell,EquipmentInventory equipment=null)
        {
            for(int seed=1;seed<=64;seed++)
            {
                var r=new RunModel(seed,equipment);EnterBoss(r);r.Enemies[0].Hp=0;
                r.Step(0,false,false);FinishBossDefeat(r);
                if(r.HasSpell(spell)){r.Enemies.Clear();r.X=2;r.Y=0;return r;}
            }
            Assert.Fail("No seed produced the requested boss reward.");return null;
        }
        [Test] public void FreshJourneyCannotCastEitherBossSkillButEmberWaveStillWorks()
        {
            var r=new RunModel(23);r.Enemies.Clear();
            for(int i=0;i<30;i++)r.Step(0,false,false,lance:true,nova:true);
            Assert.IsFalse(r.Casting);Assert.AreEqual(0,r.Waves.Count);Assert.AreEqual(0,r.NovaTimer);
            Assert.AreEqual(0,r.LanceCooldown);Assert.AreEqual(0,r.NovaCooldown);
            Assert.IsFalse(r.HasSpell(SpellKind.AshLance));Assert.IsFalse(r.HasSpell(SpellKind.CrimsonNova));
            r.Step(0,false,false,magic:true,lance:true,nova:true);Assert.IsTrue(r.Casting);
            for(int i=0;i<15;i++)r.Step(0,false,false);
            Assert.Greater(r.MagicCooldown,0);Assert.AreEqual(1,r.Waves.Count);Assert.IsFalse(r.Waves[0].Lance);
        }
        [Test] public void FirstBossOptionalRandomChoiceGrantsExactlyOneSeededSpellWithoutChestAndNeverRerolls()
        {
            var rewards=new System.Collections.Generic.HashSet<SpellKind>();
            for(int seed=1;seed<=16;seed++)
            {
                var r=new RunModel(seed);EnterBoss(r);Assert.IsNull(r.BossSpellReward);
                r.Enemies[0].Hp=0;r.Step(0,false,false);Assert.IsNull(r.BossSpellReward);FinishBossDefeat(r);
                Assert.IsTrue(r.BossSpellReward.HasValue);Assert.AreNotEqual(r.HasSpell(SpellKind.AshLance),r.HasSpell(SpellKind.CrimsonNova));
                var reward=r.BossSpellReward;rewards.Add(reward.Value);FinishBossDefeat(r);
                Assert.IsFalse(r.ChestOpened);Assert.AreEqual(reward,r.BossSpellReward);
                for(int i=0;i<60;i++)r.Step(0,false,false);
                Assert.AreEqual(reward,r.BossSpellReward);
                var replay=new RunModel(seed,r.Equipment,r.Progression);Assert.IsNull(replay.BossSpellReward);
                Assert.IsFalse(replay.HasSpell(SpellKind.AshLance));Assert.IsFalse(replay.HasSpell(SpellKind.CrimsonNova));
                EnterBoss(replay);replay.Enemies[0].Hp=0;replay.Step(0,false,false);Assert.IsNull(replay.BossSpellReward);FinishBossDefeat(replay);Assert.AreEqual(reward,replay.BossSpellReward);
            }
            Assert.AreEqual(2,rewards.Count);
        }
        [Test] public void KillRewardsPayOnceAndLevelUpPreservesExcessExperience()
        {
            var growth=new PlayerProgression(level:1,experience:90);
            var alive=new Enemy {Hp=1,Boss=true};Assert.IsFalse(growth.RewardKill(alive));
            alive.Hp=0;Assert.IsTrue(growth.RewardKill(alive));Assert.IsFalse(growth.RewardKill(alive));
            Assert.AreEqual(150,growth.Gold);Assert.AreEqual(2,growth.Level);Assert.AreEqual(90,growth.Experience);Assert.AreEqual(4,growth.Points);
            var capped=new PlayerProgression(PlayerProgression.MaxGold,25,999);
            capped.RewardKill(new Enemy {Boss=true});Assert.AreEqual(PlayerProgression.MaxGold,capped.Gold);Assert.AreEqual(25,capped.Level);Assert.AreEqual(0,capped.Experience);
        }
        [Test] public void PurchasesAndUpgradesChargeOnlyForValidTransactions()
        {
            var items=new EquipmentInventory();var growth=new PlayerProgression(150);
            Assert.IsFalse(growth.Upgrade(items,EquipmentId.AshSword));
            Assert.IsFalse(growth.Buy(items,(EquipmentId)99));Assert.AreEqual(150,growth.Gold);
            Assert.IsTrue(growth.Buy(items,EquipmentId.AshSword));Assert.AreEqual(50,growth.Gold);
            Assert.IsFalse(growth.Buy(items,EquipmentId.AshSword));Assert.IsFalse(growth.Upgrade(items,EquipmentId.AshSword));
            Assert.AreEqual(50,growth.Gold);Assert.AreEqual(0,items.UpgradeLevel(EquipmentId.AshSword));
            var wealthy=new PlayerProgression(1000);
            for(int level=1;level<=5;level++){Assert.AreEqual(level*60,items.UpgradeCost(EquipmentId.AshSword));Assert.IsTrue(wealthy.Upgrade(items,EquipmentId.AshSword));}
            Assert.AreEqual(100,wealthy.Gold);Assert.IsFalse(wealthy.Upgrade(items,EquipmentId.AshSword));Assert.AreEqual(100,wealthy.Gold);
            Assert.AreEqual(EquipmentId.None,items.Equipped(EquipmentSlot.Weapon));
            items.Equip(EquipmentId.AshSword);Assert.That(items.SwordMultiplier,Is.EqualTo(1.7f).Within(.001f));
        }
        [Test] public void GrowthPointsAreLimitedAndSavedRanksCannotExceedTheEarnedBudget()
        {
            var growth=new PlayerProgression();Assert.AreEqual(2,growth.Points);
            Assert.IsFalse(growth.Allocate((GrowthStat)99));
            Assert.IsTrue(growth.Allocate(GrowthStat.Attack));Assert.IsTrue(growth.Allocate(GrowthStat.Magic));
            Assert.IsFalse(growth.Allocate(GrowthStat.Health));Assert.AreEqual(0,growth.Points);
            var restored=new PlayerProgression(-50,1,900,new[]{99,99,99,99,99});
            Assert.AreEqual(0,restored.Gold);Assert.AreEqual(2,restored.Rank(GrowthStat.Attack));Assert.AreEqual(0,restored.Points);
            var max=new PlayerProgression(level:25,savedRanks:new[]{10,10,10,10,10});Assert.AreEqual(0,max.Points);Assert.IsFalse(max.Allocate(GrowthStat.Attack));
        }
        [Test] public void GrowthAffectsSwordMagicHealthMovementAndIncomingDamageIndependently()
        {
            var growth=new PlayerProgression(level:10,savedRanks:new[]{2,3,4,5,6});
            var r=new RunModel(75,progression:growth);r.Enemies.Clear();
            Assert.AreEqual(28,r.SwordDamage);Assert.AreEqual(30,r.MagicDamage);Assert.AreEqual(140,r.MaxHp);Assert.AreEqual(140,r.Hp);
            Assert.That(r.MovementSpeed,Is.EqualTo(6.72f).Within(.001f));
            r.HurtTimer=0;r.Hurt(12);Assert.AreEqual(133,r.Hp);
            r.HurtTimer=0;r.Hurt(3);Assert.AreEqual(132,r.Hp);
            float x=r.X;r.Step(1,false,false);Assert.That(r.X-x,Is.EqualTo(r.MovementSpeed*RunModel.StepSeconds).Within(.001f));
        }
        [Test] public void RealSwordKillGrantsGoldAndExperienceAndCannotBeFarmedAgain()
        {
            var growth=new PlayerProgression();var r=new RunModel(91,progression:growth);r.Enemies.Clear();
            var target=new Enemy {X=3,Hp=1,MaxHp=1,Cooldown=100};r.Enemies.Add(target);
            r.Step(0,false,true);for(int i=0;i<60;i++)r.Step(0,false,true);
            Assert.AreEqual(1,r.Kills);Assert.AreEqual(12,growth.Gold);Assert.AreEqual(10,growth.Experience);Assert.IsTrue(target.RewardPaid);
        }
        [Test] public void UpgradeSaveRestoresOnlyOwnedItemsAndCapsAllFiveEffects()
        {
            var items=new EquipmentInventory(31,new[]{5,5,5,5,5});
            items.Equip(EquipmentId.AshSword);items.Equip(EquipmentId.PilgrimCoat);items.Equip(EquipmentId.WingBoots);items.Equip(EquipmentId.FurnaceHeart);
            var r=new RunModel(8,items);Assert.AreEqual(140,r.MaxHp);
            Assert.That(r.EffectiveRollReuse,Is.EqualTo(.55f).Within(.001f));Assert.That(r.MagicDamage,Is.EqualTo(24*1.6f).Within(.001f));
            items.Equip(EquipmentId.SanctumSeal);Assert.That(r.EffectiveMagicReuse,Is.EqualTo(4.25f).Within(.001f));
            var invalid=new EquipmentInventory(1,new[]{999,5,5,5,5});Assert.AreEqual(5,invalid.UpgradeLevel(EquipmentId.AshSword));Assert.AreEqual(0,invalid.UpgradeLevel(EquipmentId.PilgrimCoat));
        }
        [Test] public void EquipmentOwnershipAndRelicSlotRejectInvalidOrStackedLoadouts()
        {
            var items=new EquipmentInventory();
            Assert.IsFalse(items.Equip(EquipmentId.AshSword));
            Assert.IsFalse(items.Grant((EquipmentId)99));
            Assert.IsTrue(items.Grant(EquipmentId.FurnaceHeart));
            Assert.IsFalse(items.Grant(EquipmentId.FurnaceHeart));
            items.Grant(EquipmentId.SanctumSeal);items.Equip(EquipmentId.FurnaceHeart);
            Assert.AreEqual(1.25f,items.MagicMultiplier);
            items.Equip(EquipmentId.SanctumSeal);
            Assert.IsFalse(items.Has(EquipmentId.FurnaceHeart));
            Assert.AreEqual(1,items.MagicMultiplier);Assert.AreEqual(.9f,items.MagicReuseMultiplier);
            items.Unequip(EquipmentSlot.Relic);Assert.AreEqual(1,items.MagicReuseMultiplier);
        }
        [Test] public void ArmorCannotHealByRepeatedEquipAndIsBlockedDuringCombat()
        {
            var items=new EquipmentInventory();items.Grant(EquipmentId.PilgrimCoat);
            var r=new RunModel(13,items);r.Hp=70;
            Assert.IsFalse(r.Equip(EquipmentId.PilgrimCoat));
            foreach(var enemy in r.Enemies)enemy.Hp=0;
            Assert.IsTrue(r.Equip(EquipmentId.PilgrimCoat));Assert.AreEqual(120,r.MaxHp);Assert.AreEqual(70,r.Hp);
            Assert.IsFalse(r.Equip(EquipmentId.PilgrimCoat));
            r.Hp=115;r.Unequip(EquipmentSlot.Armor);Assert.AreEqual(100,r.Hp);
            r.Equip(EquipmentId.PilgrimCoat);Assert.AreEqual(100,r.Hp);
            var next=new RunModel(14,items);Assert.AreEqual(120,next.Hp);Assert.AreEqual(120,next.MaxHp);
        }
        [Test] public void SwordAndMagicBonusesApplyToSeparateDamageSources()
        {
            var items=new EquipmentInventory(31);items.Equip(EquipmentId.AshSword);items.Equip(EquipmentId.FurnaceHeart);
            var r=WithBossSpell(SpellKind.AshLance,items);
            var target=new Enemy {X=3,Y=0,Hp=1000,MaxHp=1000,Cooldown=100};r.Enemies.Add(target);
            r.Step(0,false,true);for(int i=0;i<6;i++)r.Step(0,false,false);
            Assert.That(1000-target.Hp,Is.EqualTo(31.2f).Within(.001f));
            for(int i=0;i<30;i++)r.Step(0,false,false);
            r.Step(0,false,false,lance:true);for(int i=0;i<15;i++)r.Step(0,false,false);
            Assert.That(1000-target.Hp,Is.EqualTo(31.2f+24*1.25f*3.2f).Within(.001f));
        }
        [Test] public void EquippedCooldownsAreAppliedWhenActionsRelease()
        {
            var items=new EquipmentInventory(31);items.Equip(EquipmentId.WingBoots);items.Equip(EquipmentId.SanctumSeal);
            var r=WithBossSpell(SpellKind.AshLance,items);
            r.Step(0,false,false,roll:true);Assert.That(r.RollCooldown,Is.EqualTo(.6f).Within(.001f));
            for(int i=0;i<30;i++)r.Step(0,false,false);
            r.Step(0,false,false,magic:true);while(r.Casting)r.Step(0,false,false);
            Assert.That(r.MagicCooldown,Is.EqualTo(4.5f).Within(.001f));
            for(int i=0;i<30;i++)r.Step(0,false,false);
            r.Step(0,false,false,lance:true);while(r.Casting)r.Step(0,false,false);
            Assert.That(r.LanceCooldown,Is.EqualTo(5.4f).Within(.001f));
            r=WithBossSpell(SpellKind.CrimsonNova,items);
            r.Step(0,false,false,nova:true);while(r.Casting)r.Step(0,false,false);
            Assert.That(r.NovaCooldown,Is.EqualTo(7.2f).Within(.001f));
        }
        [Test] public void FiveBossChestsCollectUniqueEquipmentAndNeverAutoEquip()
        {
            var items=new EquipmentInventory();
            for(int journey=0;journey<6;journey++)
            {
                int oldMask=items.OwnedMask;
                var r=new RunModel(28,items);EnterBoss(r);r.Enemies[0].Hp=0;
                r.Step(0,false,false);FinishBossDefeat(r);r.X=r.ChestX;r.Y=0;r.Hp=40;
                r.Step(0,false,false,enter:true);
                Assert.IsTrue(r.ChestOpened);Assert.AreEqual(80,r.Hp);
                if(journey<5){Assert.AreNotEqual(EquipmentId.None,r.LastEquipmentReward);Assert.AreNotEqual(oldMask,items.OwnedMask);}
                else Assert.AreEqual(EquipmentId.None,r.LastEquipmentReward);
                int newMask=items.OwnedMask;r.Step(0,false,false,enter:true);
                Assert.AreEqual(newMask,items.OwnedMask);Assert.AreEqual(80,r.Hp);
                for(int slot=0;slot<4;slot++)Assert.AreEqual(EquipmentId.None,items.Equipped((EquipmentSlot)slot));
            }
            Assert.AreEqual(31,items.OwnedMask);
        }
        [Test] public void CollectionCanBeRestoredWithoutGrantingUnownedItems()
        {
            var saved=new EquipmentInventory(31);saved.Equip(EquipmentId.PilgrimCoat);saved.Equip(EquipmentId.SanctumSeal);
            var loaded=new EquipmentInventory(saved.OwnedMask);
            for(int i=0;i<4;i++)loaded.Equip(saved.Equipped((EquipmentSlot)i));
            var r=new RunModel(7,loaded);Assert.AreEqual(120,r.MaxHp);Assert.AreEqual(120,r.Hp);
            Assert.That(r.EffectiveMagicReuse,Is.EqualTo(4.5f).Within(.001f));
            var invalid=new EquipmentInventory(32);Assert.AreEqual(0,invalid.OwnedMask);Assert.IsFalse(invalid.Equip(EquipmentId.AshSword));
        }
        static void EnterBoss(RunModel run,bool finishIntro=true)
        {
            for(int room=run.RoomIndex;room<RunModel.CombatRoomCount;room++)
            {
                foreach(var enemy in run.Enemies)enemy.Hp=0;
                run.X=RunModel.ExitX;run.Y=0;run.Step(0,false,false,enter:true);
            }
            if(finishIntro)FinishBossIntro(run);
        }
        static void FinishBossIntro(RunModel run)
        {
            for(int i=0;i<240&&run.BossIntroducing;i++)run.Step(0,false,false);
            Assert.IsFalse(run.BossIntroducing);
        }
        static void FinishBossDefeat(RunModel run,bool chooseEarlyReward=true)
        {
            for(int i=0;i<240&&run.BossDissolving;i++)run.Step(0,false,false);
            Assert.IsFalse(run.BossDissolving);
            // Legacy combat fixtures explicitly accept the optional random reward.
            // Choice-specific tests disable this step to inspect the pending state.
            if(chooseEarlyReward&&run.Chapter<=3&&run.CanChooseBossSkill)Assert.IsTrue(run.ChooseRandomBossSkill());
        }
        static RunModel BossFight(out Enemy boss)
        {
            var run=new RunModel(42);EnterBoss(run);boss=run.Enemies.Find(e=>e.Boss);
            foreach(var enemy in run.Enemies)if(!enemy.Boss)enemy.Hp=0;
            boss.X=30;boss.Y=0;boss.Flying=false;
            run.X=27;run.HurtTimer=0;return run;
        }
        [Test] public void HalfHealthTransitionsOnceAndCancelsCommittedAttacks()
        {
            var r=BossFight(out var boss);
            boss.Hp=boss.MaxHp*.5f;boss.Windup=.4f;boss.DashRemaining=.3f;
            r.BloodOrbs.Add(new BloodOrb {Owner=boss,X=22,Y=1});
            r.Step(0,false,false);
            Assert.IsTrue(boss.PhaseTwo);Assert.AreEqual(1,boss.PhaseTransition);
            Assert.AreEqual(0,boss.Windup);Assert.AreEqual(0,boss.DashRemaining);Assert.AreEqual(0,r.BloodOrbs.Count);
            r.Step(0,false,false);Assert.Less(boss.PhaseTransition,1);
            boss.Hp=20;r.Step(0,false,false);Assert.Less(boss.PhaseTransition,.99f);
        }
        [Test] public void BossArrivesAtMapCenterAndIntroBlocksCombatUntilItFinishes()
        {
            var r=new RunModel(26);r.Hp=63;EnterBoss(r,false);
            var boss=r.Enemies.Find(e=>e.Boss);
            Assert.IsTrue(r.BossIntroducing);Assert.AreEqual(RunModel.BossIntroDuration,r.BossIntroRemaining);
            Assert.AreEqual(RunModel.WorldWidth*.5f,boss.X);Assert.AreEqual(RunModel.BossSpawnY,boss.Y);
            float playerX=r.X,bossCooldown=boss.Cooldown;r.HurtTimer=0;
            r.Hurt(100);Assert.AreEqual(63,r.Hp);
            for(int i=0;i<240&&r.BossIntroducing;i++)r.Step(1,true,true,true,true,true,true);
            Assert.IsFalse(r.BossIntroducing);Assert.AreEqual(RunPhase.Combat,r.Phase);
            Assert.AreEqual(playerX,r.X);Assert.AreEqual(0,r.Y);Assert.AreEqual(63,r.Hp);
            Assert.AreEqual(RunModel.BossSpawnX,boss.X);Assert.AreEqual(RunModel.BossSpawnY,boss.Y);
            Assert.AreEqual(boss.MaxHp,boss.Hp);Assert.AreEqual(bossCooldown,boss.Cooldown);
            Assert.AreEqual(0,boss.Windup);Assert.AreEqual(0,r.BloodOrbs.Count);Assert.AreEqual(0,r.Waves.Count);
            Assert.IsFalse(r.Rolling);Assert.IsFalse(r.Casting);Assert.IsFalse(r.SwordActive);
            r.Step(1,false,false);Assert.Greater(r.X,playerX);
            Assert.IsFalse(r.BossIntroducing);
        }
        [Test] public void DashHitsOnceAndStopsAtLockedDestination()
        {
            var r=BossFight(out var boss);r.X=29;
            boss.Pattern=BossPattern.Dash;boss.Facing=-1;boss.DashEndX=23;boss.DashRemaining=.5f;
            for(int i=0;i<35;i++){r.HurtTimer=0;r.Step(0,false,false);}
            Assert.AreEqual(82,r.Hp);Assert.AreEqual(23,boss.X,.001f);Assert.AreEqual(0,boss.DashRemaining);
        }
        [Test] public void RollCanPassThroughDashWithoutDelayedContactDamage()
        {
            var r=BossFight(out var boss);r.X=29;
            boss.Pattern=BossPattern.Dash;boss.Facing=-1;boss.DashEndX=23;boss.DashRemaining=.5f;
            r.Step(-1,false,false,true);
            for(int i=0;i<34;i++)r.Step(0,false,false);
            Assert.IsTrue(boss.DashContact);Assert.AreEqual(100,r.Hp);
        }
        [Test] public void PhaseTwoFiresThreeFixedDirectionOrbsAndDeathClearsThem()
        {
            var r=BossFight(out var boss);r.X=22;
            boss.PhaseTwo=true;boss.Pattern=BossPattern.BloodBolts;boss.Windup=.001f;boss.AimX=22;boss.AimY=1;
            r.Step(0,false,false);Assert.AreEqual(3,r.BloodOrbs.Count);
            float vx=r.BloodOrbs[0].VelocityX,vy=r.BloodOrbs[0].VelocityY;
            r.Step(1,true,false);
            Assert.AreEqual(vx,r.BloodOrbs[0].VelocityX);Assert.AreEqual(vy,r.BloodOrbs[0].VelocityY);
            boss.Hp=0;r.Step(0,false,false);Assert.AreEqual(0,r.BloodOrbs.Count);Assert.IsFalse(r.ExitOpen);
            FinishBossDefeat(r);Assert.IsTrue(r.ExitOpen);
        }
        [Test] public void ClearingEnemiesDoesNotInterruptExploration()
        {
            var r=new RunModel(10);foreach(var e in r.Enemies)e.Hp=0;r.Step(0,false,false);
            Assert.AreEqual(RunPhase.Combat,r.Phase);Assert.IsTrue(r.ExitOpen);
            Assert.AreEqual(24,r.Damage);Assert.AreEqual(100,r.MaxHp);
            r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);
            Assert.AreEqual(RunStage.Approach,r.Stage);Assert.AreEqual(1,r.RoomIndex);
            Assert.AreEqual(RunPhase.Combat,r.Phase);Assert.IsFalse(r.ExitOpen);
        }
        [Test] public void ExitRequiresDefeatedGuardian()
        {
            var r=new RunModel(10);EnterBoss(r);r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);
            Assert.AreEqual(RunPhase.Combat,r.Phase);
            foreach(var e in r.Enemies)if(e.Boss)e.Hp=0;
            r.Step(0,false,false);FinishBossDefeat(r);
            r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);Assert.AreEqual(RunPhase.Combat,r.Phase);Assert.AreEqual(2,r.Chapter);
        }
        [Test] public void FlyingBossTracksPlayerHeightDuringRecovery()
        {
            var r=new RunModel(20);EnterBoss(r);var boss=r.Enemies.Find(e=>e.Boss);
            Assert.IsTrue(boss.Flying);Assert.AreEqual(1,r.Enemies.Count);
            r.X=42;r.Y=3.2f;boss.Cooldown=10;float before=boss.Y;
            r.Step(0,false,false);Assert.Greater(boss.Y,before);Assert.Less(boss.Y,5.2f);
        }
        [Test] public void DeathStopsSimulation()
        {
            var r=new RunModel(10);r.HurtTimer=0;r.Hurt(100);float x=r.X;r.Step(1,true,true);
            Assert.AreEqual(RunPhase.Dead,r.Phase);Assert.AreEqual(x,r.X);
        }
        [Test] public void LivingMonsterBlocksGateAtEveryHeightAndBossIsNotSpawned()
        {
            var r=new RunModel(30);
            Assert.AreEqual(4,r.RemainingMonsters);Assert.IsFalse(r.Enemies.Exists(e=>e.Boss));
            for(int i=0;i<r.Enemies.Count-1;i++)r.Enemies[i].Hp=0;
            r.X=RunModel.ExitX;r.Y=6;r.Step(1,false,false);
            Assert.Less(r.X,RunModel.ExitX);Assert.AreEqual(RunStage.Approach,r.Stage);
            r.X=RunModel.ExitX;r.Y=0;r.Grounded=true;r.Step(1,false,false,true);
            Assert.Less(r.X,RunModel.ExitX);Assert.AreEqual(RunStage.Approach,r.Stage);
        }
        [Test] public void GateTransitionKeepsHealthStatsKillsAndClearsProjectiles()
        {
            var r=new RunModel(31);r.Hp=63;r.Kills=6;
            r.Arrows.Add(new DemonArrow {X=50,Y=1,Facing=1});
            EnterBoss(r);
            Assert.AreEqual(RunStage.BossArena,r.Stage);Assert.AreEqual(63,r.Hp);Assert.AreEqual(24,r.Damage);
            Assert.AreEqual(6,r.Kills);Assert.AreEqual(1,r.Enemies.Count);Assert.IsTrue(r.Enemies[0].Boss);
            Assert.AreEqual(0,r.Arrows.Count);Assert.AreEqual(2,r.X);
        }
        [Test] public void SeedReproducesLayout()
        {
            var a=new RunModel(2026);var b=new RunModel(2026);
            Assert.AreEqual(a.Platforms[0].X,b.Platforms[0].X);Assert.AreEqual(a.Enemies[0].X,b.Enemies[0].X);
        }
        [Test] public void DefeatOpensPortalBeforeChestAndFreezesCombat()
        {
            var r=new RunModel(80);EnterBoss(r);var boss=r.Enemies[0];
            boss.Hp=0;r.Hp=60;r.Step(0,false,false);
            Assert.IsTrue(r.BossDissolving);Assert.IsFalse(r.ExitOpen);Assert.IsFalse(r.ChestAvailable);
            float x=r.X;r.HurtTimer=0;r.Hurt(40);Assert.AreEqual(60,r.Hp);
            for(int i=0;i<240&&!r.ExitOpen;i++)r.Step(1,true,true,true,true,true,true,true,true);
            Assert.IsTrue(r.ExitOpen);Assert.IsFalse(r.ChestAvailable);Assert.AreEqual(x,r.X);
            for(int i=0;i<240&&!r.ChestAvailable;i++)r.Step(0,false,false);
            Assert.IsTrue(r.ChestAvailable);Assert.IsTrue(r.BossDissolving);Assert.IsFalse(r.CanOpenChest);
            FinishBossDefeat(r);r.X=r.ChestX;r.Y=0;r.Step(0,false,false,enter:true);
            Assert.IsTrue(r.ChestOpened);Assert.AreEqual(100,r.Hp);
            r.Hp=50;r.Step(0,false,false,enter:true);Assert.AreEqual(50,r.Hp);
        }
        [TestCase(90f,100f,10f)]
        [TestCase(30f,70f,40f)]
        [TestCase(100f,100f,0f)]
        public void ChestPotionIsImmediateCappedAndCannotBeUsedWhileEnemyLives(float hp,float expected,float healed)
        {
            var r=new RunModel(81);EnterBoss(r);var boss=r.Enemies[0];
            boss.Hp=0;r.Step(0,false,false);FinishBossDefeat(r);
            r.X=r.ChestX;r.Y=0;r.Hp=hp;
            var extra=new Enemy {X=5,Hp=20,MaxHp=20};r.Enemies.Add(extra);
            r.Step(0,false,false,enter:true);Assert.IsFalse(r.ChestOpened);Assert.AreEqual(hp,r.Hp);
            extra.Hp=0;r.Step(0,false,false,enter:true);
            Assert.IsTrue(r.ChestOpened);Assert.AreEqual(expected,r.Hp);Assert.AreEqual(healed,r.LastHealing);
        }
        [Test] public void AshLancePiercesTargetsAndUsesItsOwnCooldown()
        {
            var r=WithBossSpell(SpellKind.AshLance);r.X=10;
            var a=new Enemy {X=13,Hp=200,MaxHp=200,Cooldown=10,MinX=13,MaxX=13};
            var b=new Enemy {X=17,Hp=200,MaxHp=200,Cooldown=10,MinX=17,MaxX=17};
            r.Enemies.Add(a);r.Enemies.Add(b);r.Step(0,false,false,lance:true);
            for(int i=0;i<60;i++)r.Step(0,false,false);
            Assert.AreEqual(200-24*3.2f,a.Hp,.01f);Assert.AreEqual(a.Hp,b.Hp,.01f);
            Assert.Greater(r.LanceCooldown,0);Assert.AreEqual(0,r.MagicCooldown);Assert.AreEqual(0,r.NovaCooldown);
            r.Step(0,false,false,lance:true);Assert.IsFalse(r.Casting);
        }
        [Test] public void CrimsonNovaHitsBothSidesAndAirTargetsOnceAndCanBeCancelled()
        {
            var r=WithBossSpell(SpellKind.CrimsonNova);r.X=10;
            foreach(float x in new[]{8f,12f,20f})r.Enemies.Add(new Enemy {X=x,Y=2.5f,Hp=200,MaxHp=200,MinX=x,MaxX=x,Cooldown=10});
            r.Step(0,false,false,nova:true);
            for(int i=0;i<35;i++)r.Step(0,false,false);
            Assert.AreEqual(152,r.Enemies[0].Hp);Assert.AreEqual(152,r.Enemies[1].Hp);Assert.AreEqual(200,r.Enemies[2].Hp);
            Assert.Greater(r.NovaCooldown,0);Assert.AreEqual(0,r.MagicCooldown);Assert.AreEqual(0,r.LanceCooldown);
            var cancelled=WithBossSpell(SpellKind.CrimsonNova);cancelled.Step(0,false,false,nova:true);cancelled.Step(0,false,false,roll:true);
            for(int i=0;i<35;i++)cancelled.Step(0,false,false);
            Assert.AreEqual(0,cancelled.NovaCooldown);Assert.AreEqual(0,cancelled.NovaTimer);
        }
        [Test] public void OpenDoorNeedsAnExplicitPressNearbyInEveryRoom()
        {
            var r=new RunModel(24);
            for(int room=0;room<=RunModel.CombatRoomCount;room++)
            {
                if(r.BossIntroducing)FinishBossIntro(r);
                foreach(var enemy in r.Enemies)enemy.Hp=0;
                r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false);
                if(r.BossDissolving)FinishBossDefeat(r);
                Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(RunPhase.Combat,r.Phase);
                Assert.IsTrue(r.CanUseExit);
                // Passing through the open gate without pressing up does not transition.
                for(int i=0;i<30;i++)r.Step(1,false,false);
                Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(RunPhase.Combat,r.Phase);
                r.X=RunModel.ExitX-5;r.Step(0,false,false,enter:true);
                Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(RunPhase.Combat,r.Phase);
                r.X=RunModel.ExitX;r.Y=3;r.Grounded=false;r.VelocityY=0;
                r.Step(0,false,false,enter:true);
                Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(RunPhase.Combat,r.Phase);
                r.Y=0;r.VelocityY=0;r.Step(0,false,false,enter:true);
                if(room<RunModel.CombatRoomCount)Assert.AreEqual(room+1,r.RoomIndex);
                else {Assert.AreEqual(2,r.Chapter);Assert.AreEqual(0,r.RoomIndex);Assert.AreEqual(RunPhase.Combat,r.Phase);}
            }
        }
        [Test] public void PressingUpCannotBypassALockedDoorOrQueueAFutureTransition()
        {
            var r=new RunModel(25);r.X=RunModel.ExitX;
            r.Step(0,false,false,enter:true);
            Assert.AreEqual(0,r.RoomIndex);Assert.IsFalse(r.CanUseExit);
            foreach(var enemy in r.Enemies)enemy.Hp=0;
            r.X=RunModel.ExitX;r.Step(0,false,false);
            Assert.AreEqual(0,r.RoomIndex);Assert.IsTrue(r.CanUseExit);
            r.Step(0,false,false,enter:true);Assert.AreEqual(1,r.RoomIndex);
            r.HurtTimer=0;r.Hurt(100);
            foreach(var enemy in r.Enemies)enemy.Hp=0;
            r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);
            Assert.AreEqual(1,r.RoomIndex);Assert.AreEqual(RunPhase.Dead,r.Phase);
        }
        [Test] public void DownJumpPassesCurrentLedgeButLandsOnTheLowerOne()
        {
            var r=new RunModel(21);r.Enemies.Clear();r.Platforms.Clear();
            r.Platforms.Add(new Platform(8,8,3.2f));
            r.Platforms.Add(new Platform(8,8,1.6f));
            r.X=10;r.Y=3.2f;r.Grounded=true;
            r.Step(0,true,false,down:true);
            Assert.Less(r.Y,3.2f);Assert.Less(r.VelocityY,0);Assert.IsFalse(r.Grounded);
            // Holding down without a new jump edge must not skip another floor.
            for(int i=0;i<60;i++)r.Step(0,false,false,down:true);
            Assert.AreEqual(1.6f,r.Y,.001f);Assert.IsTrue(r.Grounded);
            // A later ordinary jump can land on the same upper ledge again.
            r.Step(0,true,false);
            for(int i=0;i<60;i++)r.Step(0,false,false);
            Assert.AreEqual(3.2f,r.Y,.001f);Assert.IsTrue(r.Grounded);
        }
        [Test] public void DownJumpNeverPassesThroughTheSolidGround()
        {
            var r=new RunModel(22);r.Enemies.Clear();
            r.Step(0,true,false,down:true);
            Assert.AreEqual(0,r.Y);Assert.IsTrue(r.Grounded);
            r.X=10;r.Y=1.6f;r.Grounded=true;
            r.Step(0,true,false,down:true);
            for(int i=0;i<60;i++)r.Step(0,false,false);
            Assert.AreEqual(0,r.Y);Assert.IsTrue(r.Grounded);
        }
        [Test] public void DownAloneKeepsThePlayerOnThePlatform()
        {
            var r=new RunModel(23);r.Enemies.Clear();r.X=10;r.Y=1.6f;
            for(int i=0;i<60;i++)r.Step(0,false,false,down:true);
            Assert.AreEqual(1.6f,r.Y,.001f);Assert.IsTrue(r.Grounded);
        }
        [Test] public void EveryCombatRoomSealsItsOwnExitAndHasSupportedArcherPatrols()
        {
            var r=new RunModel(11);int[] counts={4,6,7};
            for(int room=0;room<RunModel.CombatRoomCount;room++)
            {
                Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(counts[room],r.RemainingMonsters);
                Assert.IsFalse(r.Enemies.Exists(e=>e.Boss));
                foreach(var enemy in r.Enemies)
                {
                    Assert.GreaterOrEqual(enemy.MinX,8);Assert.LessOrEqual(enemy.MaxX,56);
                    if(enemy.Y>0)Assert.IsTrue(r.Platforms.Exists(p=>p.Y==enemy.Y&&p.X<=enemy.MinX&&p.X+p.Width>=enemy.MaxX));
                }
                foreach(var enemy in r.Enemies)enemy.Hp=0;
                r.Enemies[0].Hp=1;r.X=RunModel.ExitX;r.Y=6;r.Step(1,false,false);
                Assert.AreEqual(room,r.RoomIndex);Assert.Less(r.X,RunModel.ExitX);
                r.Enemies[0].Hp=0;r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false,enter:true);
            }
            Assert.AreEqual(RunStage.BossArena,r.Stage);Assert.AreEqual(3,r.RoomIndex);
            Assert.AreEqual(1,r.Enemies.Count);Assert.IsTrue(r.Enemies[0].Flying);
        }
        [Test] public void UpperPlatformsHaveAnOverlappingSingleJumpApproachInEveryRoom()
        {
            var r=new RunModel(12);
            for(int room=0;room<=RunModel.CombatRoomCount;room++)
            {
                foreach(var platform in r.Platforms)
                {
                    Assert.GreaterOrEqual(platform.X,0);Assert.LessOrEqual(platform.X+platform.Width,RunModel.WorldWidth);
                    if(platform.Y>2.2f)
                        Assert.IsTrue(r.Platforms.Exists(lower=>lower.Y<platform.Y&&platform.Y-lower.Y<=2.2f&&
                            lower.X<platform.X+platform.Width&&lower.X+lower.Width>platform.X),"Missing reachable step");
                }
                if(room==RunModel.CombatRoomCount)break;
                foreach(var enemy in r.Enemies)enemy.Hp=0;
                r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false,enter:true);
            }
        }
    }
}
