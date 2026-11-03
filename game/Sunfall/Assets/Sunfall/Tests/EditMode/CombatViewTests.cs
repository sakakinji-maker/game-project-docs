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
            var run=ArcherRoom(out var archer);archer.Y=3;archer.Windup=.01f;
            run.SetCombatView(0,14,-1,4);
            for(int i=0;i<90;i++)run.Step(0,false,false);
            Assert.AreEqual(0,archer.Windup);Assert.IsEmpty(run.Arrows);
        }
    }
}
