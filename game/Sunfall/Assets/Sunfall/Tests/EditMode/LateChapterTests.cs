using NUnit.Framework;
using Sunfall.Core;

namespace Sunfall.Tests
{
    public partial class RunModelTests
    {
        static RunModel EnterLateChapter(int chapter)
        {
            var r=FinishThreeBosses(19);r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);
            while(r.Chapter<chapter)
            {
                EnterBoss(r);r.Enemies[0].Hp=0;r.Step(0,false,false);FinishBossDefeat(r);r.ChooseBossSkill(0);
                r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false,enter:true);
            }
            return r;
        }
        [Test] public void SevenChapterJourneyPreservesAllRewardsAndEndsOnlyAfterFinalBossPortal()
        {
            var r=EnterLateChapter(4);var first=r.BossSpellReward;var chain=r.ChainSpellReward;var sun=r.SolarSpellReward;
            var equipment=r.Equipment;var growth=r.Progression;
            for(int chapter=4;chapter<=7;chapter++)
            {
                for(int room=0;room<3;room++)
                {
                    Assert.AreEqual(chapter,r.Chapter);Assert.AreEqual(room,r.RoomIndex);Assert.AreEqual(8+room,r.RemainingMonsters);
                    foreach(var p in r.Platforms){Assert.GreaterOrEqual(p.X,0);Assert.LessOrEqual(p.X+p.Width,64);}
                    foreach(var e in r.Enemies)if(e.Y>0)Assert.IsTrue(r.Platforms.Exists(p=>p.Y==e.Y&&p.X<=e.MinX&&p.X+p.Width>=e.MaxX));
                    r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false,enter:true);Assert.AreEqual(room,r.RoomIndex);
                    foreach(var e in r.Enemies)e.Hp=0;
                    r.X=RunModel.ExitX;r.Y=3;r.Step(0,false,false,enter:true);Assert.AreEqual(room,r.RoomIndex);
                    r.Y=0;r.VelocityY=0;r.Step(0,false,false);Assert.AreEqual(room,r.RoomIndex);
                    r.Step(0,false,false,enter:true);Assert.AreEqual(RunPhase.Combat,r.Phase);
                }
                Assert.AreEqual(RunStage.BossArena,r.Stage);var boss=r.Enemies[0];Assert.AreEqual(chapter,boss.LateChapter);
                FinishBossIntro(r);r.X=RunModel.ExitX;r.Step(0,false,false,enter:true);Assert.AreEqual(chapter,r.Chapter);Assert.IsFalse(r.ExitOpen);
                boss.Hp=0;r.Step(0,false,false);Assert.IsTrue(r.BossDissolving);Assert.IsFalse(r.CanUseExit);FinishBossDefeat(r);
                Assert.IsTrue(r.ChestAvailable);Assert.IsFalse(r.ChestOpened);
                Assert.AreEqual(first,r.BossSpellReward);Assert.AreEqual(chain,r.ChainSpellReward);Assert.AreEqual(sun,r.SolarSpellReward);Assert.AreEqual(0,r.SpellRewardNotice);
                Assert.IsFalse(r.CanUseExit);Assert.IsTrue(r.ChooseBossSkill(0));
                r.Hp=57;r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false,enter:true);
                Assert.AreEqual(57,r.Hp);Assert.AreSame(equipment,r.Equipment);Assert.AreSame(growth,r.Progression);
                Assert.IsTrue(r.HasSpell(first.Value));Assert.IsTrue(r.HasSpell(chain.Value));Assert.IsTrue(r.HasSpell(sun.Value));
                Assert.AreEqual(chapter==7?RunPhase.Won:RunPhase.Combat,r.Phase);
                Assert.AreEqual(chapter==7?7:chapter+1,r.Chapter);
            }
            for(int i=0;i<10;i++)r.Step(1,true,true,enter:true);
            Assert.AreEqual(7,r.Chapter);Assert.AreEqual(RunPhase.Won,r.Phase);
        }
        [TestCase(4)] [TestCase(5)] [TestCase(6)] [TestCase(7)]
        public void LateBossPhaseChangeCancelsPendingAttacksAndRetargetsAfterWarning(int chapter)
        {
            var r=EnterLateChapter(chapter);EnterBoss(r);var b=r.Enemies[0];
            b.Windup=.01f;b.DashRemaining=.3f;b.Hp=b.MaxHp*.5f;
            r.BloodOrbs.Add(new BloodOrb {Owner=b,X=r.X,Y=1});float hp=r.Hp;
            r.Step(0,false,false);Assert.IsTrue(b.PhaseTwo);Assert.Greater(b.PhaseTransition,0);
            Assert.AreEqual(0,b.Windup);Assert.AreEqual(0,b.DashRemaining);Assert.AreEqual(0,r.BloodOrbs.Count);Assert.AreEqual(hp,r.Hp);
            b.PhaseTransition=0;b.Cooldown=0;r.Step(0,false,false);Assert.Greater(b.Windup,0);
            float target=b.AimX;r.X+=5;r.Step(0,false,false);Assert.AreEqual(target,b.AimX);
            b.Hp=0;r.Step(0,false,false);Assert.AreEqual(0,b.Windup);Assert.AreEqual(0,b.DashRemaining);Assert.AreEqual(0,r.BloodOrbs.Count);
        }
        [TestCase(6)] [TestCase(7)]
        public void LateEclipseSafeBandProtectsPlayerAndOutsideTakesDamage(int chapter)
        {
            var r=EnterLateChapter(chapter);EnterBoss(r);var b=r.Enemies[0];
            b.Pattern=chapter==6?BossPattern.NightVeil:BossPattern.LastEclipse;b.AimX=20;b.Windup=.01f;
            r.X=20;r.Y=0;r.HurtTimer=0;float hp=r.Hp;r.Step(0,false,false);Assert.AreEqual(hp,r.Hp);
            b.Windup=.01f;r.X=30;r.HurtTimer=0;r.Step(0,false,false);Assert.Less(r.Hp,hp);
        }
        [Test] public void MirrorTeleportStrikesCommittedLocationAndDiveCannotRepeatContactDamage()
        {
            var r=EnterLateChapter(5);EnterBoss(r);var b=r.Enemies[0];
            b.Pattern=BossPattern.MirrorStep;b.AimX=24;b.Windup=.01f;r.X=40;r.HurtTimer=0;float hp=r.Hp;
            r.Step(0,false,false);Assert.AreEqual(24,b.X);Assert.AreEqual(hp,r.Hp);
            r=EnterLateChapter(6);EnterBoss(r);b=r.Enemies[0];
            b.Pattern=BossPattern.CrescentDive;b.X=10;b.Y=0;b.AimX=20;b.AimY=0;b.Windup=.01f;r.X=14;r.Y=0;r.HurtTimer=0;
            r.Step(0,false,false);Assert.Greater(b.DashRemaining,0);
            for(int i=0;i<36;i++)r.Step(0,false,false);
            Assert.IsTrue(b.DashContact);Assert.AreEqual(0,b.DashRemaining);Assert.That(b.X,Is.EqualTo(20).Within(.01f));
        }
    }
}
