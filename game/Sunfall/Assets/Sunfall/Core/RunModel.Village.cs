namespace Sunfall.Core
{
    public sealed partial class RunModel
    {
        public bool IsVillagePractice {get;private set;}
        public const float VillageMinX=20f/34f,VillageMaxX=1080f/34f;
        // Reuse the real jump, casting, cooldown and projectile simulation, with
        // no enemies, rewards or encounter exits in the village.
        public static RunModel CreateVillagePractice(EquipmentInventory equipment,PlayerProgression progression,RunModel previous=null)
        {
            var practice=new RunModel(previous==null?0:previous.Seed,equipment,progression);
            practice.IsVillagePractice=true;
            practice.Enemies.Clear();practice.Platforms.Clear();
            practice.X=280f/34f;practice.HurtTimer=0;
            if(previous!=null)
            {
                practice.BossSpellReward=previous.BossSpellReward;
                practice.ChainSpellReward=previous.ChainSpellReward;
                practice.SolarSpellReward=previous.SolarSpellReward;
                for(int i=0;i<4;i++)
                {
                    practice.lateRewards[i]=previous.lateRewards[i];
                    practice.equippedSkills[i]=previous.equippedSkills[i];
                }
            }
            return practice;
        }
    }
}
