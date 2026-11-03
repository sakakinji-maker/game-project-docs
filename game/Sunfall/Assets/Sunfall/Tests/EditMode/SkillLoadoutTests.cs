using System.Collections.Generic;
using NUnit.Framework;
using Sunfall.Core;

namespace Sunfall.Tests
{
    public partial class RunModelTests
    {
        static RunModel WithAllBossSkills()
        {
            var r=new RunModel(71);
            for(int chapter=1;chapter<=7;chapter++)
            {
                EnterBoss(r);r.Enemies[0].Hp=0;r.Step(0,false,false);FinishBossDefeat(r,false);Assert.IsTrue(r.ChooseBossSkill(0));
                if(chapter<7){r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false,enter:true);}
            }
            r.Enemies.Clear();r.X=20;r.Y=0;return r;
        }
        [Test] public void LearnedSkillsRemainAvailableButOnlyFourAreEquipped()
        {
            var r=WithAllBossSkills();Assert.AreEqual(8,new List<SpellKind>(r.LearnedSkills).Count);
            Assert.AreEqual(SpellKind.EmberWave,r.EquippedSkill(0));Assert.AreEqual(SpellKind.AshLance,r.EquippedSkill(1));
            Assert.AreEqual(SpellKind.ChainPull,r.EquippedSkill(2));Assert.AreEqual(SpellKind.SunLance,r.EquippedSkill(3));
            Assert.IsTrue(r.HasSpell(SpellKind.RoyalLance));Assert.IsFalse(r.IsSpellEquipped(SpellKind.RoyalLance));
            Assert.IsFalse(r.EquipSkill(4,SpellKind.RoyalLance));Assert.IsFalse(r.EquipSkill(-1,SpellKind.RoyalLance));
            Assert.IsFalse(r.EquipSkill(0,SpellKind.CrimsonNova));Assert.IsFalse(r.EquipSkill(0,(SpellKind)999));
            Assert.IsTrue(r.EquipSkill(0,SpellKind.RoyalLance));Assert.IsTrue(r.HasSpell(SpellKind.EmberWave));Assert.IsFalse(r.IsSpellEquipped(SpellKind.EmberWave));
            Assert.IsTrue(r.EquipSkill(2,SpellKind.RoyalLance));Assert.AreEqual(SpellKind.ChainPull,r.EquippedSkill(0));Assert.AreEqual(SpellKind.RoyalLance,r.EquippedSkill(2));
            var unique=new HashSet<SpellKind>();for(int i=0;i<4;i++)Assert.IsTrue(unique.Add(r.EquippedSkill(i).Value));
            Assert.IsTrue(r.UnequipSkill(2));Assert.IsNull(r.EquippedSkill(2));Assert.IsTrue(r.HasSpell(SpellKind.RoyalLance));
            Assert.IsTrue(r.EquipSkill(2,SpellKind.EmberWave));Assert.AreEqual(8,new List<SpellKind>(r.LearnedSkills).Count);
        }
        [Test] public void EmptyAndUnequippedSkillsCannotBeCastThroughAnyInputPath()
        {
            var r=WithAllBossSkills();Assert.IsTrue(r.UnequipSkill(0));
            r.Step(0,false,false,skillSlot:0,magic:true,lateSlot:3);Assert.IsFalse(r.Casting);Assert.AreEqual(0,r.LateCooldown(3));
            r.Step(0,false,false,skillSlot:99);Assert.IsFalse(r.Casting);
            Assert.IsTrue(r.EquipSkill(0,SpellKind.RoyalLance));r.Step(0,false,false,skillSlot:0);Assert.IsTrue(r.Casting);Assert.AreEqual(SpellKind.RoyalLance,r.CurrentSpell);
            Assert.IsFalse(r.EquipSkill(0,SpellKind.EmberWave));Assert.IsFalse(r.UnequipSkill(0));
            for(int i=0;i<30;i++)r.Step(0,false,false);
            Assert.Greater(r.LateCooldown(3),0);Assert.AreEqual(0,r.MagicCooldown);
        }
        [Test] public void CooldownFollowsSpellAcrossReplacementAndSlotSwap()
        {
            var r=WithAllBossSkills();r.Step(0,false,false,skillSlot:0);for(int i=0;i<30;i++)r.Step(0,false,false);
            float remaining=r.SkillCooldown(SpellKind.EmberWave);Assert.Greater(remaining,0);
            Assert.IsTrue(r.EquipSkill(0,SpellKind.HeartBreaker));Assert.That(r.SkillCooldown(SpellKind.EmberWave),Is.EqualTo(remaining));
            for(int i=0;i<12;i++)r.Step(0,false,false);
            Assert.Less(r.SkillCooldown(SpellKind.EmberWave),remaining);float after=r.SkillCooldown(SpellKind.EmberWave);
            Assert.IsTrue(r.EquipSkill(2,SpellKind.EmberWave));Assert.AreEqual(after,r.SkillCooldown(SpellKind.EmberWave));
            r.Step(0,false,false,skillSlot:2);Assert.IsFalse(r.Casting);
            r.Step(0,false,false,skillSlot:0);Assert.IsTrue(r.Casting);Assert.AreEqual(SpellKind.HeartBreaker,r.CurrentSpell);
        }
        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)]
        public void AnySlotCanCastAnAssignedSkillAndLoadoutSurvivesPortal(int slot)
        {
            var r=EnterLateChapter(4);var spell=r.SolarSpellReward.Value;Assert.IsTrue(r.EquipSkill(slot,spell));
            r.Enemies.Clear();r.Step(0,false,false,skillSlot:slot);Assert.IsTrue(r.Casting);Assert.AreEqual(spell,r.CurrentSpell);
            for(int i=0;i<30;i++)r.Step(0,false,false);
            var before=new SpellKind?[4];for(int i=0;i<4;i++)before[i]=r.EquippedSkill(i);
            r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false,enter:true);Assert.AreEqual(1,r.RoomIndex);
            for(int i=0;i<4;i++)Assert.AreEqual(before[i],r.EquippedSkill(i));
            Assert.IsTrue(r.HasSpell(spell));
            var fresh=new RunModel(r.Seed,r.Equipment,r.Progression);Assert.AreEqual(SpellKind.EmberWave,fresh.EquippedSkill(0));
            for(int i=1;i<4;i++)Assert.IsNull(fresh.EquippedSkill(i));
        }
        [Test] public void NewRewardFillsAnEmptySlotWithoutReplacingOtherSlots()
        {
            var r=EnterLateChapter(4);Assert.IsTrue(r.UnequipSkill(1));
            var before0=r.EquippedSkill(0);var before2=r.EquippedSkill(2);var before3=r.EquippedSkill(3);
            EnterBoss(r,false);Assert.IsFalse(r.EquipSkill(1,SpellKind.EmberWave));FinishBossIntro(r);
            r.Enemies[0].Hp=0;r.Step(0,false,false);FinishBossDefeat(r,false);
            Assert.IsFalse(r.EquipSkill(1,SpellKind.EmberWave));Assert.IsTrue(r.ChooseBossSkill(1));
            Assert.AreEqual(SpellKind.CrownGear,r.EquippedSkill(1));Assert.AreEqual(before0,r.EquippedSkill(0));
            Assert.AreEqual(before2,r.EquippedSkill(2));Assert.AreEqual(before3,r.EquippedSkill(3));
        }
    }
}
