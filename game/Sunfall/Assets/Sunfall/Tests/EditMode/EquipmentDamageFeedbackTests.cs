using NUnit.Framework;
using Sunfall.Core;

namespace Sunfall.Tests
{
    public sealed class EquipmentDamageFeedbackTests
    {
        static RunModel Prepare(EquipmentInventory items,out Enemy target)
        {
            var run=new RunModel(42,items);run.Enemies.Clear();run.Platforms.Clear();
            target=new Enemy {X=3,Y=0,Hp=1000,MaxHp=1000,Cooldown=100};
            run.Enemies.Add(target);return run;
        }
        [TestCase(0,false,31.2f)]
        [TestCase(1,false,33.12f)]
        [TestCase(5,false,40.8f)]
        [TestCase(0,true,31.2f)]
        [TestCase(1,true,33.12f)]
        [TestCase(5,true,40.8f)]
        public void EquippedSwordAndFeedbackMatchActualHealthLoss(int upgrade,bool airborne,float expected)
        {
            var items=new EquipmentInventory(31,new[]{upgrade,0,0,0,0});items.Equip(EquipmentId.AshSword);
            var run=Prepare(items,out var target);
            if(airborne){run.Y=1;run.Grounded=false;}
            run.Step(0,false,true);for(int i=0;i<6;i++)run.Step(0,false,false);
            Assert.That(1000-target.Hp,Is.EqualTo(expected).Within(.001f));
            Assert.AreEqual(1,run.DamageNumbers.Count);
            Assert.That(run.DamageNumbers[0].Amount,Is.EqualTo(expected).Within(.001f));
            Assert.IsTrue(run.DamageNumbers[0].Sword);
            for(int i=0;i<60;i++)run.Step(0,false,false);
            Assert.IsEmpty(run.DamageNumbers);
        }
        [Test] public void ComboFinisherUsesEquippedSwordBonus()
        {
            var items=new EquipmentInventory(1);items.Equip(EquipmentId.AshSword);
            var run=Prepare(items,out var target);
            for(int strike=0;strike<3;strike++)
            {
                run.Step(0,false,true);
                while(run.AttackTimer>0)run.Step(0,false,false);
            }
            Assert.That(1000-target.Hp,Is.EqualTo(31.2f*3.6f).Within(.001f));
        }
        [TestCase(false,0,60f)]
        [TestCase(true,0,75f)]
        [TestCase(true,1,79.2f)]
        [TestCase(true,5,96f)]
        public void MagicFeedbackUsesRelicInsteadOfSword(bool equipHeart,int upgrade,float expected)
        {
            var items=new EquipmentInventory(31,new[]{0,0,0,upgrade,0});items.Equip(EquipmentId.AshSword);
            if(equipHeart)items.Equip(EquipmentId.FurnaceHeart);
            var run=Prepare(items,out var target);
            run.Step(0,false,false,magic:true);for(int i=0;i<25;i++)run.Step(0,false,false);
            Assert.That(1000-target.Hp,Is.EqualTo(expected).Within(.001f));
            Assert.AreEqual(1,run.DamageNumbers.Count);
            Assert.That(run.DamageNumbers[0].Amount,Is.EqualTo(expected).Within(.001f));
            Assert.IsFalse(run.DamageNumbers[0].Sword);
        }
        [Test] public void UnequipAndRestartDoNotKeepOrStackSwordBonus()
        {
            var items=new EquipmentInventory(1);items.Equip(EquipmentId.AshSword);
            var run=Prepare(items,out var target);run.Enemies.Clear();
            Assert.IsTrue(run.Unequip(EquipmentSlot.Weapon));
            run.Enemies.Add(target);run.Step(0,false,true);
            for(int i=0;i<6;i++)run.Step(0,false,false);
            Assert.That(1000-target.Hp,Is.EqualTo(24).Within(.001f));
            items.Equip(EquipmentId.AshSword);
            var next=Prepare(items,out var nextTarget);next.Step(0,false,true);
            for(int i=0;i<6;i++)next.Step(0,false,false);
            Assert.That(1000-nextTarget.Hp,Is.EqualTo(31.2f).Within(.001f));
        }
    }
}
