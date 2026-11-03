using NUnit.Framework;
using Sunfall.Core;

namespace Sunfall.Tests
{
    public partial class RunModelTests
    {
        [Test] public void VillageUsesRealJumpAndAirborneSpellRelease()
        {
            var village=RunModel.CreateVillagePractice(new EquipmentInventory(),new PlayerProgression());
            village.Step(0,true,false);
            Assert.IsFalse(village.Grounded);Assert.Greater(village.Y,0);
            village.Step(0,false,false,skillSlot:0);
            Assert.IsTrue(village.Casting);
            for(int i=0;i<15;i++)village.Step(0,false,false);
            Assert.Greater(village.Waves.Count,0);Assert.Greater(village.MagicCooldown,0);
            for(int i=0;i<90;i++)village.Step(0,false,false);
            Assert.IsTrue(village.Grounded);Assert.AreEqual(0,village.Y);
        }
        [Test] public void VillageHasNoEnemiesRewardsOrEncounterExit()
        {
            var progression=new PlayerProgression();
            var village=RunModel.CreateVillagePractice(new EquipmentInventory(),progression);
            village.X=RunModel.ExitX;
            for(int i=0;i<120;i++)village.Step(1,false,false,enter:true,skillSlot:0);
            Assert.IsEmpty(village.Enemies);Assert.IsEmpty(village.Platforms);
            Assert.IsFalse(village.CanUseExit);Assert.IsFalse(village.ChestAvailable);
            Assert.AreEqual(0,village.RoomIndex);Assert.AreEqual(1,village.Chapter);
            Assert.AreEqual(0,progression.Gold);Assert.AreEqual(0,progression.Experience);
            Assert.AreEqual(RunModel.VillageMaxX,village.X);
        }
        [Test] public void VillageCopiesEarnedSkillsButNeverUnlocksFreshJourneyRewards()
        {
            var source=WithAllBossSkills();
            var village=RunModel.CreateVillagePractice(source.Equipment,source.Progression,source);
            CollectionAssert.AreEquivalent(source.LearnedSkills,village.LearnedSkills);
            for(int i=0;i<4;i++)Assert.AreEqual(source.EquippedSkill(i),village.EquippedSkill(i));
            var before=source.EquippedSkill(0);
            village.UnequipSkill(0);Assert.AreEqual(before,source.EquippedSkill(0));
            var fresh=new RunModel(42,source.Equipment,source.Progression);
            Assert.IsFalse(fresh.IsVillagePractice);
            CollectionAssert.AreEqual(new[]{SpellKind.EmberWave},fresh.LearnedSkills);
        }
    }
}
