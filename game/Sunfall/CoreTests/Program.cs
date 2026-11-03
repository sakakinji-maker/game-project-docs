using System;
using System.Linq;
using Sunfall.Core;
static class Program
{
    static int passed;
    static void Check(bool ok,string name){if(!ok)throw new Exception(name);passed++;Console.WriteLine("PASS "+name);}
    static void Clear(RunModel r){foreach(var e in r.Enemies)e.Hp=0;r.Step(0,false,false);for(int i=0;i<240&&r.BossDissolving;i++)r.Step(0,false,false);if(r.CanChooseBossSkill)r.ChooseBossSkill(0);}
    static void EnterBoss(RunModel r)
    {
        for(int room=r.RoomIndex;room<RunModel.CombatRoomCount;room++)
        {Clear(r);r.X=RunModel.ExitX;r.Y=0;r.Step(0,false,false,enter:true);}
        for(int i=0;i<240&&r.BossIntroducing;i++)r.Step(0,false,false);
    }
    static void Main()
    {
        var a=new RunModel(42);var b=new RunModel(42);
        Check(a.Enemies.Select(e=>e.X).SequenceEqual(b.Enemies.Select(e=>e.X)),"same seed spawn");
        Check(a.Platforms.Select(p=>p.X).SequenceEqual(b.Platforms.Select(p=>p.X)),"same seed platforms");
        a.Step(1,true,false);Check(a.Y>0&&!a.Grounded,"jump starts");
        for(int i=0;i<180;i++)a.Step(0,false,false);
        Check(a.Y==0&&a.Grounded,"gravity lands");
        a=new RunModel(1);for(int i=0;i<1000;i++)a.Step(-1,false,false);
        Check(a.X>=.5f,"world left bound");
        a=new RunModel(7);a.X=a.Enemies[0].X-1;a.Y=a.Enemies[0].Y;a.Grounded=false;a.Facing=1;
        float hp=a.Enemies[0].Hp;a.Step(0,false,true);
        // Sword damage now occurs after the startup, not on the input frame.
        for(int frame=0;frame<6;frame++)a.Step(0,false,false);
        Check(a.Enemies[0].Hp==hp-a.Damage,"damage equals displayed stats");
        hp=a.Enemies[0].Hp;a.Step(0,false,true);Check(a.Enemies[0].Hp==hp,"attack cooldown");
        a=new RunModel(5);a.HurtTimer=0;a.Hurt(12);a.Hurt(12);Check(a.Hp==88,"invulnerability blocks repeated damage");
        a=new RunModel(4);Clear(a);
        Check(a.Phase==RunPhase.Combat&&a.ExitOpen,"clearing enemies keeps exploration active");
        Check(a.Damage==24&&a.MaxHp==100,"clearing does not grant stat upgrades");
        a.X=RunModel.ExitX;a.Step(0,false,false,enter:true);Check(a.RoomIndex==1&&a.Stage==RunStage.Approach,"first gate enters ruined garden");
        EnterBoss(a);Check(a.Stage==RunStage.BossArena&&a.Phase==RunPhase.Combat,"three cleared rooms enter boss arena");
        Clear(a);var reward=a.BossSpellReward;a.X=RunModel.ExitX;a.Step(0,false,false,enter:true);Check(a.Chapter==2&&a.Phase==RunPhase.Combat&&a.BossSpellReward==reward,"defeated boss opens chapter two and preserves spell");
        a=new RunModel(8);a.X=RunModel.ExitX;a.Step(0,false,false,enter:true);
        Check(a.Phase==RunPhase.Combat,"living guardian seals exit");
        a.HurtTimer=0;a.Hurt(999);Clear(a);Check(a.Phase==RunPhase.Dead,"death has precedence over exit");
        a=new RunModel(8);Check(a.Hp==100&&a.Damage==24,"new journey resets stats");
        for(int seed=0;seed<1000;seed++)
        {
            a=new RunModel(seed);b=new RunModel(seed);
            if(!a.Platforms.Select(p=>p.X).SequenceEqual(b.Platforms.Select(p=>p.X)))throw new Exception("map nondeterminism");
            if(a.Enemies.Any(e=>e.X<8||e.X>RunModel.WorldWidth-2))throw new Exception("unsafe spawn");
            if(a.Platforms.Any(p=>p.X<0||p.X+p.Width>RunModel.WorldWidth))throw new Exception("platform outside map");
            foreach(var e in a.Enemies.Where(e=>e.Y>0&&!e.Flying))
                if(!a.Platforms.Any(p=>p.Y==e.Y&&e.MinX>=p.X&&e.MaxX<=p.X+p.Width))throw new Exception("unsupported enemy");
            EnterBoss(a);
            if(a.Stage!=RunStage.BossArena)throw new Exception("boss gate did not transition");
            Clear(a);
            a.X=RunModel.ExitX;a.Step(0,false,false,enter:true);
            if(a.Chapter!=2||a.Phase!=RunPhase.Combat)throw new Exception("boss exit did not enter chapter two");
            for(int room=0;room<RunModel.CombatRoomCount;room++)
            {
                if(a.Enemies.Any(e=>e.Y>0&&!a.Platforms.Any(p=>p.Y==e.Y&&p.X<=e.MinX&&p.X+p.Width>=e.MaxX)))throw new Exception("chapter two unsupported enemy");
                Clear(a);a.X=RunModel.ExitX;a.Step(0,false,false,enter:true);
            }
            if(a.Stage!=RunStage.BossArena||!a.Enemies[0].ChainWarden)throw new Exception("chapter two exit did not enter chain warden arena");
            for(int i=0;i<240&&a.BossIntroducing;i++)a.Step(0,false,false);
            reward=a.BossSpellReward;Clear(a);
            if(!a.ChainSpellReward.HasValue||!a.HasSpell(a.ChainSpellReward.Value)||a.HasSpell(SpellKind.ChainPull)==a.HasSpell(SpellKind.ChainRain))throw new Exception("second boss did not grant exactly one chain spell");
            a.X=RunModel.ExitX;a.Step(0,false,false,enter:true);
            if(a.Chapter!=3||a.Phase!=RunPhase.Combat||a.BossSpellReward!=reward)throw new Exception("chain warden exit or reward preservation failed");
            var chainReward=a.ChainSpellReward;
            for(int room=0;room<RunModel.CombatRoomCount;room++)
            {
                if(a.RemainingMonsters!=7+room||a.Enemies.Any(e=>e.Boss))throw new Exception("cathedral room population incorrect");
                if(a.Platforms.Any(p=>p.X<0||p.X+p.Width>RunModel.WorldWidth))throw new Exception("cathedral platform outside map");
                if(a.Enemies.Any(e=>e.Y>0&&!a.Platforms.Any(p=>p.Y==e.Y&&p.X<=e.MinX&&p.X+p.Width>=e.MaxX)))throw new Exception("cathedral unsupported enemy");
                Clear(a);a.X=RunModel.ExitX;a.Step(0,false,false,enter:true);
            }
            if(a.Stage!=RunStage.BossArena||!a.Enemies[0].SolarRegent)throw new Exception("cathedral did not enter solar boss arena");
            for(int i=0;i<240&&a.BossIntroducing;i++)a.Step(0,false,false);
            Clear(a);a.X=RunModel.ExitX;a.Step(0,false,false,enter:true);
            if(!a.SolarSpellReward.HasValue||a.HasSpell(SpellKind.SunLance)==a.HasSpell(SpellKind.DawnBurst))throw new Exception("third boss did not grant exactly one solar spell");
            if(a.Chapter!=4||a.Phase!=RunPhase.Combat||a.BossSpellReward!=reward||a.ChainSpellReward!=chainReward)throw new Exception("solar boss exit or rewards changed");
            var solarReward=a.SolarSpellReward;
            for(int chapter=4;chapter<=7;chapter++)
            {
                for(int room=0;room<RunModel.CombatRoomCount;room++)
                {
                    if(a.Chapter!=chapter||a.RemainingMonsters!=8+room||a.Enemies.Any(e=>e.Boss))throw new Exception("late room population incorrect");
                    if(a.Platforms.Any(p=>p.X<0||p.X+p.Width>RunModel.WorldWidth))throw new Exception("late platform outside map");
                    if(a.Enemies.Any(e=>e.Y>0&&!a.Platforms.Any(p=>p.Y==e.Y&&p.X<=e.MinX&&p.X+p.Width>=e.MaxX)))throw new Exception("late unsupported enemy");
                    Clear(a);a.X=RunModel.ExitX;a.Step(0,false,false,enter:true);
                }
                if(a.Stage!=RunStage.BossArena||a.Enemies[0].LateChapter!=chapter)throw new Exception("wrong late boss");
                for(int i=0;i<240&&a.BossIntroducing;i++)a.Step(0,false,false);
                Clear(a);a.X=RunModel.ExitX;a.Step(0,false,false,enter:true);
                if(a.Phase!=(chapter==7?RunPhase.Won:RunPhase.Combat)||a.BossSpellReward!=reward||a.ChainSpellReward!=chainReward||a.SolarSpellReward!=solarReward)throw new Exception("late exit or rewards changed");
            }
        }
        Check(true,"1000 seeds: bounded map, supported enemies, deterministic layout, exit transition");
        Console.WriteLine(passed+" checks passed");
    }
}


