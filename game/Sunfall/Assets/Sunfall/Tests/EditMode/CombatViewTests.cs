using NUnit.Framework;
using Sunfall.Core;

namespace Sunfall.Tests
{
    public sealed class CombatViewTests
    {
        static RunModel ArcherRoom(out Enemy archer)
        {
            var run=new RunModel(12345);run.Enemies.Clear();
            archer=new Enemy {Kind=EnemyKind.Archer,X=9,Y=0,Hp=50,MaxHp=50,MinX=8,MaxX=12,Cooldown=0};
            run.Enemies.Add(archer);return run;
        }
        [Test] public void OffscreenArcherCancelsWindupWithoutFiring()
        {
            var run=ArcherRoom(out var archer);archer.Windup=.01f;
            run.SetCombatView(0,8,-1,12);
            for(int i=0;i<90;i++)run.Step(0,false,false);
            Assert.AreEqual(9,archer.X);Assert.AreEqual(0,archer.Windup);Assert.IsEmpty(run.Arrows);
        }
        [Test] public void ArcherCanAttackOnceCameraRevealsIt()
        {
            var run=ArcherRoom(out var archer);run.SetCombatView(0,8,-1,12);run.Step(0,false,false);
            run.SetCombatView(0,14,-1,12);bool fired=false;
            for(int i=0;i<120;i++){run.Step(0,false,false);fired|=run.Arrows.Count>0;}
            Assert.IsTrue(fired);
        }
        [Test] public void EnemyAboveViewportCannotFire()
        {
            var run=ArcherRoom(out var archer);archer.Y=5;archer.Windup=.01f;
            run.SetCombatView(0,14,-1,4);
            for(int i=0;i<90;i++)run.Step(0,false,false);
            Assert.AreEqual(0,archer.Windup);Assert.IsEmpty(run.Arrows);
        }
        [Test] public void VisibleTorsoKeepsWindupWhenFeetAreBelowCamera()
        {
            var run=ArcherRoom(out var archer);archer.Windup=.01f;
            run.SetCombatView(0,14,.5f,12);run.Step(0,false,false);
            Assert.Greater(archer.AttackPose,0);Assert.IsNotEmpty(run.Arrows);
        }
        [Test] public void GroundGuardReacquiresPlayerAcrossPatrolBoundary()
        {
            var run=new RunModel(12345);run.Enemies.Clear();run.X=7;
            var guard=new Enemy {Kind=EnemyKind.Melee,X=10,Y=0,Hp=50,MaxHp=50,MinX=10,MaxX=20,Cooldown=0};
            run.Enemies.Add(guard);run.SetCombatView(0,25,-1,12);
            for(int i=0;i<90;i++)run.Step(0,false,false);
            Assert.Less(guard.X,10);Assert.IsTrue(guard.Windup>0||guard.AttackPose>0||run.Hp<run.MaxHp);
            run.SetCombatView(25,50,-1,12);run.Step(0,false,false);
            run.X=guard.X+1;run.SetCombatView(0,25,-1,12);
            bool attacked=false;
            for(int i=0;i<120;i++){run.Step(0,false,false);attacked|=guard.Windup>0||guard.AttackPose>0;}
            Assert.IsTrue(attacked);Assert.AreEqual(1,guard.Facing);
        }
    }
}
