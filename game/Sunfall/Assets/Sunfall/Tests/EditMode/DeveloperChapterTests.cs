using System;
using NUnit.Framework;
using Sunfall.Core;

namespace Sunfall.Tests
{
    public partial class RunModelTests
    {
        [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)]
        [TestCase(5)] [TestCase(6)] [TestCase(7)]
        public void DeveloperJumpStartsRequestedFirstRoomWithCleanCombat(int chapter)
        {
            var items=new EquipmentInventory(31);items.Equip(EquipmentId.PilgrimCoat);
            var growth=new PlayerProgression(250);
            var previous=new RunModel(123,items,growth);
            previous.HurtTimer=0;previous.Hurt(1000);
            previous.Waves.Add(new EmberWave());
            var next=RunModel.CreateDeveloperChapter(chapter,123,items,growth,previous);
            Assert.AreEqual(chapter,next.Chapter);Assert.AreEqual(0,next.RoomIndex);
            Assert.AreEqual(RunStage.Approach,next.Stage);Assert.AreEqual(RunPhase.Combat,next.Phase);
            Assert.AreEqual(2,next.X);Assert.AreEqual(0,next.Y);Assert.IsTrue(next.Grounded);
            Assert.AreEqual(120,next.Hp);Assert.AreSame(items,next.Equipment);Assert.AreSame(growth,next.Progression);
            Assert.AreEqual(250,growth.Gold);Assert.AreEqual(0,growth.Experience);
            Assert.IsNotEmpty(next.Enemies);Assert.IsFalse(next.Enemies.Exists(e=>e.Boss));
            Assert.IsEmpty(next.Waves);Assert.IsFalse(next.Cinematic);Assert.IsFalse(next.SkillChoicePending);
            Assert.IsFalse(next.IsVillagePractice);Assert.IsFalse(next.ExitOpen);
            Assert.AreEqual(0,previous.Hp);Assert.AreEqual(1,previous.Waves.Count);
        }
        [Test] public void DeveloperJumpPreservesLearnedSkillsAndLoadoutWithoutGrantingRewards()
        {
            var previous=WithAllBossSkills();previous.EquipSkill(0,SpellKind.RoyalLance);
            previous.Step(0,false,false,skillSlot:0);
            var next=RunModel.CreateDeveloperChapter(2,previous.Seed,previous.Equipment,previous.Progression,previous);
            CollectionAssert.AreEquivalent(previous.LearnedSkills,next.LearnedSkills);
            for(int slot=0;slot<4;slot++)Assert.AreEqual(previous.EquippedSkill(slot),next.EquippedSkill(slot));
            foreach(var spell in next.LearnedSkills)Assert.AreEqual(0,next.SkillCooldown(spell));
            Assert.IsFalse(next.Casting);Assert.IsFalse(next.BossDefeated);Assert.IsFalse(next.ChestOpened);
            var fresh=RunModel.CreateDeveloperChapter(7,123,new EquipmentInventory(),new PlayerProgression());
            CollectionAssert.AreEqual(new[]{SpellKind.EmberWave},fresh.LearnedSkills);
        }
        [Test] public void DeveloperJumpLeavesVillagePracticeAndRejectsUnimplementedChapters()
        {
            var village=RunModel.CreateVillagePractice(new EquipmentInventory(),new PlayerProgression());
            var next=RunModel.CreateDeveloperChapter(3,village.Seed,village.Equipment,village.Progression,village);
            Assert.IsFalse(next.IsVillagePractice);Assert.IsNotEmpty(next.Enemies);
            Assert.Throws<ArgumentOutOfRangeException>(()=>RunModel.CreateDeveloperChapter(0,0,village.Equipment,village.Progression));
            Assert.Throws<ArgumentOutOfRangeException>(()=>RunModel.CreateDeveloperChapter(8,0,village.Equipment,village.Progression));
            Assert.IsTrue(village.IsVillagePractice);Assert.IsEmpty(village.Enemies);
        }
    }
}
