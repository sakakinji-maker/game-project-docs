using System;

namespace Sunfall.Core
{
    public sealed partial class RunModel
    {
        // Runtime entry is gated to Editor / Development Build. Create a clean
        // encounter instead of carrying projectiles, death or pending rewards.
        public static RunModel CreateDeveloperChapter(int chapter,int seed,
            EquipmentInventory equipment,PlayerProgression progression,RunModel previous=null)
        {
            if(chapter<1||chapter>FinalChapter)throw new ArgumentOutOfRangeException(nameof(chapter));
            var next=new RunModel(seed,equipment,progression);
            if(previous!=null)
            {
                next.BossSpellReward=previous.BossSpellReward;
                next.ChainSpellReward=previous.ChainSpellReward;
                next.SolarSpellReward=previous.SolarSpellReward;
                for(int i=0;i<4;i++)
                {
                    next.lateRewards[i]=previous.lateRewards[i];
                    next.equippedSkills[i]=previous.equippedSkills[i];
                }
                next.Damage=previous.Damage;next.AttackPeriod=previous.AttackPeriod;
            }
            next.Chapter=chapter;next.RoomIndex=0;next.Stage=RunStage.Approach;
            next.CreateMap();next.Hp=next.MaxHp;
            return next;
        }
    }
}
