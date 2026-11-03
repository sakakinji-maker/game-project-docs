using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        const string StoryProgressKey="sunfall.story.seen.v1";
        const int KnownStoryMask=(1<<20)-1;
        int seenStoryMask;
        bool OpeningStorySeen=>(seenStoryMask&1)!=0;

        // Stable save IDs: opening 0, boss arrivals 1..7, defeats 8..14,
        // chapter arrivals 3..7 at 15..19. Do not key saves by translated text.
        static int StoryBit(StoryLine[] lines)
        {
            if(lines==null)return 0;
            if(ReferenceEquals(lines,OpeningStory))return 1;
            if(ReferenceEquals(lines,BossArrivalStory))return 1<<1;
            if(ReferenceEquals(lines,ChainBossArrivalStory))return 1<<2;
            if(ReferenceEquals(lines,SolarBossArrivalStory))return 1<<3;
            if(ReferenceEquals(lines,BossDefeatStory))return 1<<8;
            if(ReferenceEquals(lines,ChainBossDefeatStory))return 1<<9;
            if(ReferenceEquals(lines,SolarBossDefeatStory))return 1<<10;
            if(ReferenceEquals(lines,CathedralArrivalStory))return 1<<15;
            if(ReferenceEquals(lines,CrownHeartArrivalStory))return 1<<16;
            for(int i=0;i<4;i++)
            {
                if(ReferenceEquals(lines,LateBossArrivalStories[i]))return 1<<(4+i);
                if(ReferenceEquals(lines,LateBossDefeatStories[i]))return 1<<(11+i);
            }
            for(int i=0;i<3;i++)if(ReferenceEquals(lines,LateArrivalStories[i]))return 1<<(17+i);
            return 0;
        }
        void LoadStoryProgress()
        {
            if(PlayerPrefs.HasKey(StoryProgressKey))
            {seenStoryMask=PlayerPrefs.GetInt(StoryProgressKey,0)&KnownStoryMask;return;}
            // Older versions did not record scenes. Existing adventure progress
            // is enough to bypass the opening, but cannot identify later scenes.
            bool legacyAdventure=PlayerPrefs.GetInt(ProgressionKey+"gold",0)>0||
                PlayerPrefs.GetInt(ProgressionKey+"level",1)>1||
                PlayerPrefs.GetInt(ProgressionKey+"xp",0)>0||
                PlayerPrefs.GetInt(EquipmentSaveKey+"owned",0)!=0;
            seenStoryMask=legacyAdventure?1:0;
            PlayerPrefs.SetInt(StoryProgressKey,seenStoryMask);PlayerPrefs.Save();
        }
        bool HasSeenStory(StoryLine[] lines)=>(seenStoryMask&StoryBit(lines))!=0;
        void RememberStory(StoryLine[] lines)
        {
            int bit=StoryBit(lines);
            if(bit==0||(seenStoryMask&bit)!=0)return;
            seenStoryMask|=bit;
            PlayerPrefs.SetInt(StoryProgressKey,seenStoryMask);
            PlayerPrefs.Save(); // Persist at completion/skip, even before the next kill or reward.
        }
        void ResetStoryProgress()
        {
            seenStoryMask=0;
            // Keep an explicit zero so legacy migration cannot restore seen flags.
            // ResetDeveloperProgress flushes this together with equipment/growth.
            PlayerPrefs.SetInt(StoryProgressKey,0);
        }
    }
}
