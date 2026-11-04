using System;

namespace Sunfall.Core
{
    public sealed partial class Enemy
    {
        public int LateChapter;
        public bool LateColumns=>Pattern==BossPattern.HeartLances||Pattern==BossPattern.MirrorPrison||Pattern==BossPattern.Sunfall;
        public bool LateSafeBand=>Pattern==BossPattern.NightVeil||Pattern==BossPattern.LastEclipse;
        public bool LateVolley=>Pattern==BossPattern.GearVolley||Pattern==BossPattern.MirrorFan||Pattern==BossPattern.MoonVolley;
        public bool LateDash=>Pattern==BossPattern.CrescentDive;
        public float LateReach=>Pattern==BossPattern.MirrorStep?2.5f:Pattern==BossPattern.RoyalCleave?10:7;
        public float LateSafeRadius=>PhaseTwo?2.5f:3.3f;
        public int LateColumnCount=>PhaseTwo?5:3;
        public float LateColumnX(int i)=>Math.Max(1,Math.Min(63,AimX+(i-LateColumnCount/2)*(LateChapter==5?3:4)));
        public float LateWindupDuration=>LateSafeBand?(PhaseTwo?1.65f:1.9f):Pattern==BossPattern.MirrorStep?1.1f:LateDash?1.15f:LateColumns?(PhaseTwo?.95f:1.2f):(PhaseTwo?.8f:1.05f);
    }
    public sealed partial class RunModel
    {
        public const int FinalChapter=7;
        public static string ChapterName(int chapter)
        {
            switch(chapter){case 1:return "태양을 잃은 숲";case 2:return "잿빛 성문";case 3:return "왕도 대성당";case 4:return "왕관의 심장부";case 5:return "거울 성역";case 6:return "일식의 첨탑";case 7:return "꺼진 태양의 왕좌";default:return "불씨 마을";}
        }
        public static string LateBossName(int chapter)
        {
            switch(chapter){case 4:return "왕관의 심장지기 · 바르칸";case 5:return "기억의 거울지기 · 리세아";case 6:return "일식의 날개 · 녹티스";case 7:return "태양을 삼킨 왕 · 솔베인";default:return "";}
        }
        void CreateLateBossArena()
        {
            Platforms.Add(new Platform(8,8,1.6f));Platforms.Add(new Platform(18,8,1.6f));
            Platforms.Add(new Platform(38,8,1.6f));Platforms.Add(new Platform(48,8,1.6f));
            if(Chapter==6){Platforms.Add(new Platform(13,8,3.2f));Platforms.Add(new Platform(43,8,3.2f));}
            float hp=Chapter==4?780:Chapter==5?920:Chapter==6?1080:1280;
            Enemies.Add(new Enemy {X=BossSpawnX,Y=Chapter==6?2.2f:0,Hp=hp,MaxHp=hp,Boss=true,LateChapter=Chapter,Flying=Chapter==6,MinX=8,MaxX=56,Cooldown=1.3f,Pattern=(BossPattern)((int)BossPattern.HammerSweep+(Chapter-4)*3)});
            BossIntroRemaining=BossIntroDuration;
        }
        void CreateLateCombatRoom()
        {
            // Every rise is <=1.6 and stairs overlap: no double-jump requirement.
            if(Chapter==5)
            {
                Platforms.Add(new Platform(8,9,1.6f));Platforms.Add(new Platform(13,9,3.2f));
                Platforms.Add(new Platform(27,10,1.6f));Platforms.Add(new Platform(42,9,3.2f));Platforms.Add(new Platform(47,9,1.6f));
                AddEnemy(18,3.2f,EnemyKind.Archer,14,21);AddEnemy(46,3.2f,EnemyKind.Archer,43,50);
                if(RoomIndex>0)AddEnemy(32,1.6f,EnemyKind.Archer,28,36);
            }
            else if(Chapter==6)
            {
                Platforms.Add(new Platform(7,8,1.6f));Platforms.Add(new Platform(12,8,3.2f));Platforms.Add(new Platform(17,9,4.8f));
                Platforms.Add(new Platform(38,9,4.8f));Platforms.Add(new Platform(43,8,3.2f));Platforms.Add(new Platform(48,9,1.6f));
                AddEnemy(22,4.8f,EnemyKind.Archer,18,25);AddEnemy(42,4.8f,EnemyKind.Archer,39,46);
                if(RoomIndex>0)AddEnemy(53,1.6f,EnemyKind.Archer,49,56);
            }
            else
            {
                Platforms.Add(new Platform(8,10,1.6f));Platforms.Add(new Platform(23,8,1.6f));Platforms.Add(new Platform(28,8,3.2f));
                Platforms.Add(new Platform(33,8,1.6f));Platforms.Add(new Platform(46,10,1.6f));
                AddEnemy(13,1.6f,EnemyKind.Archer,9,17);AddEnemy(51,1.6f,EnemyKind.Archer,47,55);
                if(RoomIndex>0)AddEnemy(32,3.2f,EnemyKind.Archer,29,35);
            }
            // Alternate shield lines and melee packs; arrival/exit remain clear.
            AddEnemy(11,0,EnemyKind.Melee,8,15);AddEnemy(19,0,EnemyKind.Heavy,16,23);
            AddEnemy(28,0,EnemyKind.Melee,25,31);AddEnemy(36,0,EnemyKind.Heavy,33,40);
            AddEnemy(45,0,EnemyKind.Melee,42,49);AddEnemy(54,0,EnemyKind.Heavy,51,56);
            if(RoomIndex==2)AddEnemy(32,0,EnemyKind.Melee,29,35);
            // Different room paths within each chapter, with supported guards.
            if(RoomIndex==1)Platforms.Add(new Platform(26,12,1.6f));
            if(RoomIndex==2)
            {
                Platforms.Add(new Platform(3,7,1.6f));
                // The mirror chapel already has a landing ending at x56.
                // Keep x56..ExitX clear so the player can descend to the portal.
                if(Chapter!=5)Platforms.Add(new Platform(54,7,1.6f));
            }
        }
        void UpdateLateBoss(Enemy boss,float dt)
        {
            if(!boss.PhaseTwo&&boss.Hp<=boss.MaxHp*.5f)
            {
                ClearBossAttack(boss);boss.PhaseTwo=true;boss.PhaseTransition=1.2f;boss.Cooldown=.6f;boss.PatternIndex=2;return;
            }
            if(boss.PhaseTransition>0){boss.PhaseTransition=Math.Max(0,boss.PhaseTransition-dt);return;}
            if(boss.DashRemaining>0)
            {
                float oldX=boss.X,oldY=boss.Y,remaining=boss.DashRemaining;
                float portion=Math.Min(1,dt/remaining);
                boss.X+=(boss.DashEndX-boss.X)*portion;boss.Y+=(boss.DashEndY-boss.Y)*portion;
                boss.DashRemaining=Math.Max(0,remaining-dt);
                float enter=0,leave=1;
                if(!boss.DashContact&&ClipAxis(oldX,boss.X-oldX,X-1.2f,X+1.2f,ref enter,ref leave)&&ClipAxis(oldY,boss.Y-oldY,Y-boss.BodyHeight,Y+1.9f,ref enter,ref leave))
                {boss.DashContact=true;Hurt(24);}
                if(boss.DashRemaining<=0)boss.AttackPose=.4f;
                return;
            }
            if(boss.Windup>0)
            {
                boss.Windup=Math.Max(0,boss.Windup-dt);if(boss.Windup>0)return;
                boss.AttackPose=boss.AttackReleaseDuration;boss.Cooldown=boss.PhaseTwo?1.15f:1.65f;
                if(boss.LateColumns)
                {
                    for(int i=0;i<boss.LateColumnCount;i++)if(Math.Abs(X-boss.LateColumnX(i))<=1.05f&&Y<10){Hurt(20+boss.LateChapter);break;}
                }
                else if(boss.LateSafeBand)
                {if(Math.Abs(X-boss.AimX)>boss.LateSafeRadius-.35f)Hurt(26+boss.LateChapter);}
                else if(boss.LateVolley)FireLateVolley(boss);
                else if(boss.LateDash)
                {boss.DashRemaining=boss.DashDuration=.55f;boss.DashEndX=boss.AimX;boss.DashEndY=boss.AimY;boss.DashContact=false;}
                else
                {
                    if(boss.Pattern==BossPattern.MirrorStep){boss.X=boss.AimX;boss.Y=0;}
                    if(Math.Abs(X-boss.X)<=boss.LateReach+.35f&&Y<(boss.Pattern==BossPattern.MirrorStep?2.5f:1.2f))Hurt(22+boss.LateChapter);
                }
                return;
            }
            boss.Cooldown=Math.Max(0,boss.Cooldown-dt);if(boss.AttackPose>0)return;
            boss.Facing=X>=boss.X?1:-1;
            if(boss.Flying)boss.Y+=(2.2f-boss.Y)*Math.Min(1,dt*2);
            if(Math.Abs(X-boss.X)>6)
            {
                float next=Clamp(boss.X+boss.Facing*(boss.PhaseTwo?3.1f:2.4f)*dt,boss.MinX,boss.MaxX);
                boss.Walking=Math.Abs(next-boss.X)>.001f;boss.X=next;
            }
            if(boss.Cooldown>0)return;
            boss.Pattern=(BossPattern)((int)BossPattern.HammerSweep+(boss.LateChapter-4)*3+boss.PatternIndex%3);boss.PatternIndex++;
            // Commit targets once so all warning shapes remain truthful.
            boss.AimX=Clamp(X,2,62);boss.AimY=Clamp(Y,0,5);
            if(boss.Pattern==BossPattern.MirrorStep)boss.AimX=Clamp(X+boss.Facing*1.5f,8,56);
            if(boss.LateSafeBand)boss.AimX=Clamp(X+boss.Facing*4,4,60);
            if(boss.LateVolley)boss.AimY=Y+1;
            boss.Windup=boss.WindupDuration;boss.Walking=false;
        }
        void FireLateVolley(Enemy boss)
        {
            float ox=boss.X+boss.Facing,oy=boss.Y+2;
            float aim=(float)Math.Atan2(boss.AimY-oy,boss.AimX-ox);
            int spread=boss.PhaseTwo?2:1;
            for(int i=-spread;i<=spread;i++)
            {
                float angle=aim+i*(boss.LateChapter==5?.3f:.2f),speed=boss.LateChapter==6?12:10;
                BloodOrbs.Add(new BloodOrb {Owner=boss,X=ox,Y=oy,VelocityX=(float)Math.Cos(angle)*speed,VelocityY=(float)Math.Sin(angle)*speed});
            }
        }
    }
}
