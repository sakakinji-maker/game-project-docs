using NUnit.Framework;
using Sunfall.Core;

namespace Sunfall.Tests
{
    public sealed class AirAttackRecoveryTests
    {
        static RunModel JumpSlash()
        {
            var run=new RunModel(12345);run.Enemies.Clear();run.Platforms.Clear();
            run.Step(0,true,true);
            return run;
        }
        [Test] public void LandingClearsOldSlashBeforeWalkingOffAnotherLedge()
        {
            var run=JumpSlash();Assert.IsTrue(run.AirAttack);
            for(int i=0;i<120&&!run.Grounded;i++)run.Step(0,false,false);
            Assert.IsTrue(run.Grounded);Assert.IsFalse(run.AirAttack);Assert.AreEqual(0,run.AttackStage);
            // Walking off has no jump input, so it does not call CancelSword.
            run.Y=3;run.VelocityY=0;run.Step(1,false,false);
            Assert.IsFalse(run.Grounded);Assert.IsFalse(run.AirSwordRecovery);
        }
        [Test] public void EarlyLandingDoesNotCancelAnActiveStrike()
        {
            var run=JumpSlash();run.Y=.01f;run.VelocityY=-1;
            run.Step(0,false,false);
            Assert.IsTrue(run.Grounded);Assert.IsTrue(run.SwordActive);Assert.IsTrue(run.AirAttack);
            for(int i=0;i<40;i++)run.Step(0,false,false);
            Assert.IsFalse(run.SwordActive);Assert.IsFalse(run.AirAttack);Assert.AreEqual(0,run.AttackStage);
        }
        [Test] public void AirRecoveryKeepsItsActionDirectionWhileMovementTurns()
        {
            var run=JumpSlash();int direction=run.ActionFacing;
            for(int i=0;i<22;i++)run.Step(-1,false,false);
            Assert.IsTrue(run.AirSwordRecovery);Assert.AreEqual(-1,run.Facing);
            Assert.AreEqual(direction,run.ActionFacing);
        }
    }
}
