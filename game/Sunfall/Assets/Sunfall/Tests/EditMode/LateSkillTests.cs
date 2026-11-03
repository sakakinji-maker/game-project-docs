using NUnit.Framework;
using Sunfall.Core;

namespace Sunfall.Tests
{
    public partial class RunModelTests
    {
        static RunModel WithLateSkill(int chapter,int option)
        {
            var r=EnterLateChapter(chapter);EnterBoss(r);r.Enemies[0].Hp=0;r.Step(0,false,false);FinishBossDefeat(r);
            Assert.IsTrue(r.ChooseBossSkill(option));Assert.IsTrue(r.EquipSkill(3,LateSpellCatalog.Option(chapter,option)));r.Enemies.Clear();r.X=20;r.Y=0;r.VelocityY=0;return r;
        }
        [TestCase(4,0)] [TestCase(4,1)] [TestCase(5,0)] [TestCase(5,1)]
        [TestCase(6,0)] [TestCase(6,1)] [TestCase(7,0)] [TestCase(7,1)]
        public void LateRewardRequiresChoiceAndGrantsExactlyOneWithoutReplacingEarlierSpells(int chapter,int option)
        {
            var r=EnterLateChapter(chapter);var first=r.BossSpellReward;var chain=r.ChainSpellReward;var solar=r.SolarSpellReward;
            Assert.IsFalse(r.ChooseBossSkill(option));Assert.IsFalse(r.ChooseRandomBossSkill());
            for(int i=chapter-4;i<4;i++)Assert.IsNull(r.LateReward(i));
            EnterBoss(r);r.Enemies[0].Hp=0;r.Step(0,false,false);
            Assert.AreEqual(chapter,r.PendingSkillChapter);Assert.IsFalse(r.ChooseBossSkill(option));FinishBossDefeat(r);
            Assert.IsTrue(r.CanChooseBossSkill);r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);Assert.AreEqual(chapter,r.Chapter);Assert.AreEqual(RunPhase.Combat,r.Phase);
            Assert.IsFalse(r.ChooseBossSkill(-1));Assert.IsFalse(r.ChooseBossSkill(2));
            Assert.IsTrue(r.ChooseBossSkill(option));Assert.IsFalse(r.SkillChoicePending);Assert.IsFalse(r.ChooseBossSkill(1-option));Assert.IsFalse(r.ChooseRandomBossSkill());
            var selected=LateSpellCatalog.Option(chapter,option);Assert.IsTrue(r.HasSpell(selected));Assert.IsFalse(r.HasSpell(LateSpellCatalog.Option(chapter,1-option)));
            Assert.AreEqual(selected,r.LatestSpellReward);Assert.AreEqual(first,r.BossSpellReward);Assert.AreEqual(chain,r.ChainSpellReward);Assert.AreEqual(solar,r.SolarSpellReward);
            for(int i=0;i<chapter-4;i++)Assert.AreEqual(LateSpellCatalog.Option(i+4,0),r.LateReward(i));
            r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false,enter:true);
            Assert.IsTrue(r.HasSpell(selected));Assert.AreEqual(chapter==7?RunPhase.Won:RunPhase.Combat,r.Phase);
            var fresh=new RunModel(r.Seed,r.Equipment,r.Progression);for(int i=0;i<4;i++)Assert.IsNull(fresh.LateReward(i));
        }
        [TestCase(4,0)] [TestCase(4,1)] [TestCase(5,0)] [TestCase(5,1)]
        [TestCase(6,0)] [TestCase(6,1)] [TestCase(7,0)] [TestCase(7,1)]
        public void SelectedLateSpellCastsWithIndependentCooldownAndLockedSlotsDoNothing(int chapter,int option)
        {
            var r=WithLateSkill(chapter,option);var spell=LateSpellCatalog.Option(chapter,option);int slot=chapter-4;
            r.Step(0,false,false,lateSlot:99);Assert.IsFalse(r.Casting);
            r.Step(0,false,false,lateSlot:slot);Assert.IsTrue(r.Casting);Assert.AreEqual(spell,r.CurrentSpell);
            for(int i=0;i<16;i++)r.Step(0,false,false);
            Assert.Greater(r.LateCooldown(slot),0);Assert.AreEqual(0,r.MagicCooldown);Assert.AreEqual(0,r.ChainCooldown);Assert.AreEqual(0,r.SolarCooldown);
            Assert.IsTrue(r.LateEffects.Exists(e=>e.Kind==spell));
            for(int i=0;i<20;i++)r.Step(0,false,false);
            r.Step(0,false,false,lateSlot:slot);Assert.IsFalse(r.Casting);
            r.Step(0,false,false,magic:true);Assert.IsTrue(r.Casting);Assert.AreEqual(SpellKind.EmberWave,r.CurrentSpell);
            var fresh=new RunModel(2);fresh.Step(0,false,false,lateSlot:slot);Assert.IsFalse(fresh.Casting);
        }
        [Test] public void OptionalRandomChoiceIsSeededAndCannotReroll()
        {
            var a=EnterLateChapter(4);var b=EnterLateChapter(4);
            foreach(var r in new[]{a,b}){EnterBoss(r);r.Enemies[0].Hp=0;r.Step(0,false,false);FinishBossDefeat(r);Assert.IsTrue(r.ChooseRandomBossSkill());}
            Assert.AreEqual(a.LateReward(0),b.LateReward(0));var reward=a.LateReward(0);
            Assert.IsFalse(a.ChooseRandomBossSkill());Assert.AreEqual(reward,a.LateReward(0));
        }
        [Test] public void GearPiercesEachEnemyOnceAndHeartBreakerOnlyHitsItsForwardArea()
        {
            var r=WithLateSkill(4,1);var target=new Enemy {X=22,Y=0,Hp=999,MaxHp=999,MinX=22,MaxX=22,Cooldown=100};r.Enemies.Add(target);
            r.Step(0,false,false,lateSlot:0);for(int i=0;i<65;i++)r.Step(0,false,false);
            Assert.That(target.Hp,Is.EqualTo(999-r.MagicDamage*3.5f).Within(.01f));
            r=WithLateSkill(4,0);var ahead=new Enemy {X=23,Hp=999,MaxHp=999,Cooldown=100,MinX=23,MaxX=23};var behind=new Enemy {X=17,Hp=999,MaxHp=999,Cooldown=100,MinX=17,MaxX=17};
            r.Enemies.Add(ahead);r.Enemies.Add(behind);r.Step(0,false,false,lateSlot:0);for(int i=0;i<16;i++)r.Step(0,false,false);
            Assert.That(ahead.Hp,Is.EqualTo(999-r.MagicDamage*4).Within(.01f));Assert.AreEqual(999,behind.Hp);
        }
        [Test] public void MirrorShieldAbsorbsAndExpiresWhileNightMantleReducesDamage()
        {
            var r=WithLateSkill(5,1);r.Step(0,false,false,lateSlot:1);for(int i=0;i<16;i++)r.Step(0,false,false);
            float hp=r.Hp,shield=r.MirrorShield;r.HurtTimer=0;r.Hurt(10);Assert.AreEqual(hp,r.Hp);Assert.Less(r.MirrorShield,shield);
            for(int i=0;i<250;i++)r.Step(0,false,false);Assert.AreEqual(0,r.MirrorShield);Assert.AreEqual(0,r.MirrorShieldTime);
            r=WithLateSkill(6,1);r.Step(0,false,false,lateSlot:2);for(int i=0;i<16;i++)r.Step(0,false,false);
            hp=r.Hp;r.HurtTimer=0;r.Hurt(20);Assert.That(hp-r.Hp,Is.EqualTo((20-r.Progression.Defense)*.6f).Within(.01f));
        }
        [Test] public void SolarCrownPulsesThreeTimesAtCastPositionAndRoomChangeClearsEffectsOnly()
        {
            var r=WithLateSkill(7,1);var target=new Enemy {X=23,Hp=999,MaxHp=999,MinX=23,MaxX=23,Cooldown=100};r.Enemies.Add(target);
            r.Step(0,false,false,lateSlot:3);for(int i=0;i<130;i++)r.Step(0,false,false);
            Assert.That(target.Hp,Is.EqualTo(999-r.MagicDamage*2.2f*3).Within(.02f));Assert.AreEqual(0,r.LateEffects.Count);
            r=WithLateSkill(5,1);r.Step(0,false,false,lateSlot:1);for(int i=0;i<30;i++)r.Step(0,false,false);
            Assert.Greater(r.MirrorShield,0);r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);
            Assert.AreEqual(6,r.Chapter);Assert.AreEqual(0,r.MirrorShield);Assert.AreEqual(0,r.LateEffects.Count);Assert.AreEqual(0,r.LateCooldown(1));Assert.IsTrue(r.HasSpell(SpellKind.MirrorWard));
        }
    }
}
