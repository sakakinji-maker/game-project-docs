using NUnit.Framework;
using Sunfall.Core;

namespace Sunfall.Tests
{
    public partial class RunModelTests
    {
        [TestCase(1,0)] [TestCase(1,1)] [TestCase(2,0)] [TestCase(2,1)] [TestCase(3,0)] [TestCase(3,1)]
        public void EarlyBossWaitsForExplicitChoiceAndCannotReplacePreviousRewards(int chapter,int option)
        {
            var r=new RunModel(25);
            for(int c=1;c<chapter;c++)
            {
                EnterBoss(r);r.Enemies[0].Hp=0;r.Step(0,false,false);FinishBossDefeat(r,false);
                Assert.IsTrue(r.ChooseBossSkill(1));r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false,enter:true);
            }
            var earlier=new[]{r.BossSpellReward,r.ChainSpellReward,r.SolarSpellReward};
            Assert.IsFalse(r.ChooseBossSkill(option));Assert.IsFalse(r.SkillChoicePending);
            EnterBoss(r);r.Enemies[0].Hp=0;r.Step(0,false,false);
            Assert.AreEqual(chapter,r.PendingSkillChapter);Assert.IsNull(r.BossReward(chapter));Assert.AreEqual(0,r.SpellRewardNotice);
            Assert.IsFalse(r.ChooseBossSkill(option));Assert.IsFalse(r.ChooseRandomBossSkill());
            FinishBossDefeat(r,false);Assert.IsTrue(r.CanChooseBossSkill);
            Assert.IsFalse(r.HasSpell(BossSkillCatalog.Option(chapter,0)));Assert.IsFalse(r.HasSpell(BossSkillCatalog.Option(chapter,1)));
            r.X=RunModel.ExitX;r.Y=0;float hp=r.Hp;
            for(int i=0;i<40;i++)r.Step(1,true,true,magic:true,enter:true,lance:true,nova:true,chain:true,solar:true);
            Assert.AreEqual(chapter,r.Chapter);Assert.AreEqual(RunModel.ExitX,r.X);Assert.AreEqual(hp,r.Hp);Assert.IsFalse(r.Casting);
            Assert.IsFalse(r.CanUseExit);Assert.IsFalse(r.CanOpenChest);Assert.IsFalse(r.ChooseBossSkill(-1));Assert.IsFalse(r.ChooseBossSkill(2));
            Assert.IsTrue(r.ChooseBossSkill(option));var selected=BossSkillCatalog.Option(chapter,option);
            Assert.AreEqual(selected,r.BossReward(chapter));Assert.AreEqual(selected,r.LatestSpellReward);Assert.Greater(r.SpellRewardNotice,0);
            Assert.IsFalse(r.SkillChoicePending);Assert.IsTrue(r.HasSpell(selected));Assert.IsFalse(r.HasSpell(BossSkillCatalog.Option(chapter,1-option)));
            Assert.IsFalse(r.ChooseBossSkill(1-option));Assert.IsFalse(r.ChooseRandomBossSkill());
            for(int c=1;c<chapter;c++)Assert.AreEqual(earlier[c-1],r.BossReward(c));
            r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);Assert.AreEqual(chapter+1,r.Chapter);Assert.IsTrue(r.HasSpell(selected));
            r.Enemies.Clear();r.Step(0,false,false,lance:selected==SpellKind.AshLance,nova:selected==SpellKind.CrimsonNova,chain:chapter==2,solar:chapter==3);
            Assert.IsTrue(r.Casting);Assert.AreEqual(selected,r.CurrentSpell);
            var restart=new RunModel(r.Seed,r.Equipment,r.Progression);Assert.IsFalse(restart.SkillChoicePending);Assert.IsNull(restart.BossReward(chapter));
        }
        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)] [TestCase(5)] [TestCase(6)] [TestCase(7)]
        public void EveryEarlyChoiceCombinationSurvivesThroughChapterFour(int choices)
        {
            var r=new RunModel(41);var equipment=r.Equipment;var growth=r.Progression;
            for(int chapter=1;chapter<=3;chapter++)
            {
                EnterBoss(r);r.Enemies[0].Hp=0;r.Step(0,false,false);FinishBossDefeat(r,false);
                Assert.IsTrue(r.ChooseBossSkill((choices>>(chapter-1))&1));
                r.Hp=59;r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false,enter:true);
                Assert.AreEqual(59,r.Hp);Assert.AreSame(equipment,r.Equipment);Assert.AreSame(growth,r.Progression);
                for(int c=1;c<=chapter;c++)Assert.IsTrue(r.HasSpell(BossSkillCatalog.Option(c,(choices>>(c-1))&1)));
            }
            Assert.AreEqual(4,r.Chapter);Assert.IsTrue(r.HasSpell(SpellKind.EmberWave));
            EnterBoss(r);r.Enemies[0].Hp=0;r.Step(0,false,false);FinishBossDefeat(r,false);Assert.IsTrue(r.ChooseBossSkill(1));
            Assert.IsTrue(r.HasSpell(SpellKind.CrownGear));
            for(int c=1;c<=3;c++)Assert.IsTrue(r.HasSpell(BossSkillCatalog.Option(c,(choices>>(c-1))&1)));
        }
    }
}
