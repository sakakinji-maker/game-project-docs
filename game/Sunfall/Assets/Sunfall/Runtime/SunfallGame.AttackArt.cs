using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        // Original PNG coordinates, manually aligned at hips/heels (virtual base in air).
        // Sheets contain eight independently drawn poses: 0..2 prepare, 3 contact,
        // 4..7 follow-through/recovery. Attack animation never drives damage.
        sealed class AttackClip
        {
            public readonly string Resource;
            public readonly float Scale;
            public readonly HeroFrame[] Frames;
            public Texture2D Texture;
            public readonly Vector2 Size=new Vector2(1024,1536);
            public AttackClip(string resource,float scale,HeroFrame[] frames)
            {Resource=resource;Scale=scale;Frames=frames;}
            public void Load()
            {
                Texture=Resources.Load<Texture2D>(Resource);
                if(Texture==null)return;
                Texture.filterMode=FilterMode.Point;Texture.wrapMode=TextureWrapMode.Clamp;
            }
        }
        readonly AttackClip[] heroAttackClips={
            new AttackClip("Hero/Combat/Combo1V3",0.3f,new[]{
                new HeroFrame(149,128,319,249,270,375),
                new HeroFrame(661,50,207,327,780,375),
                new HeroFrame(172,430,215,315,295,744),
                new HeroFrame(635,457,294,288,770,744),
                new HeroFrame(119,811,334,293,265,1102),
                new HeroFrame(623,880,339,224,755,1102),
                new HeroFrame(114,1202,344,260,260,1461),
                new HeroFrame(619,1202,345,260,765,1461)
            }),
            new AttackClip("Hero/Combat/Combo2V3",0.28f,new[]{
                new HeroFrame(130,72,331,272,260,342),
                new HeroFrame(618,79,315,265,747,342),
                new HeroFrame(128,413,331,312,263,726),
                new HeroFrame(591,426,364,302,728,726),
                new HeroFrame(119,784,277,352,220,1135),
                new HeroFrame(589,790,306,346,720,1135),
                new HeroFrame(129,1179,323,327,260,1505),
                new HeroFrame(678,1186,248,320,825,1505)
            }),
            new AttackClip("Hero/Combat/Combo3V3",0.28f,new[]{
                new HeroFrame(109,51,294,335,280,384),
                new HeroFrame(603,66,331,320,780,384),
                new HeroFrame(136,388,285,409,287,795),
                new HeroFrame(579,459,407,338,727,795),
                new HeroFrame(74,887,409,237,240,1123),
                new HeroFrame(572,891,422,233,740,1123),
                new HeroFrame(93,1234,470,241,250,1474),
                new HeroFrame(614,1151,312,325,720,1474)
            }),
            new AttackClip("Hero/Combat/AirSlashV3",0.3f,new[]{
                new HeroFrame(116,87,217,266,260,381.667f),
                new HeroFrame(621,54,203,313,750,396.667f),
                new HeroFrame(160,421,213,318,300,766.667f),
                new HeroFrame(646,469,340,271,710,766.667f),
                new HeroFrame(120,786,320,323,200,1116.667f),
                new HeroFrame(639,847,334,248,710,1126.667f),
                new HeroFrame(160,1203,296,262,245,1486.667f),
                new HeroFrame(605,1206,322,263,735,1496.667f)
            }),
            new AttackClip("Hero/Combat/EmberCastV3",0.255f,new[]{
                new HeroFrame(136,62,312,296,258,355),
                new HeroFrame(625,64,312,295,745,358),
                new HeroFrame(130,431,321,305,255,735),
                new HeroFrame(614,437,372,298,745,733),
                new HeroFrame(109,809,397,303,225,1110),
                new HeroFrame(615,813,299,298,742,1110),
                new HeroFrame(128,1180,325,306,255,1485),
                new HeroFrame(616,1178,320,308,742,1485)
            })
        };
        readonly AttackClip[] demonAttackClips={
            new AttackClip("Demons/MeleeAttackV2",0.24f,new[]{
                new HeroFrame(112,66,335,314,285,377),
                new HeroFrame(578,75,318,304,790,377),
                new HeroFrame(146,416,294,355,345,769),
                new HeroFrame(583,484,407,287,775,769),
                new HeroFrame(89,831,415,303,280,1132),
                new HeroFrame(573,817,365,317,775,1132),
                new HeroFrame(114,1174,356,322,290,1495),
                new HeroFrame(574,1171,354,326,785,1495)
            }),
            new AttackClip("Demons/ArcherAttackV2",0.22f,new[]{
                new HeroFrame(117,48,302,338,285,385),
                new HeroFrame(581,15,335,374,740,387),
                new HeroFrame(93,388,363,382,278,768),
                new HeroFrame(555,390,435,381,755,769),
                new HeroFrame(85,780,355,376,285,1155),
                new HeroFrame(586,821,328,335,747,1155),
                new HeroFrame(114,1183,322,335,290,1517),
                new HeroFrame(612,1176,310,342,755,1517)
            }),
            new AttackClip("Demons/HeavyAttackV2",0.285f,new[]{
                new HeroFrame(75,57,391,326,275,378),
                new HeroFrame(591,20,346,370,805,387),
                new HeroFrame(102,393,367,383,335,774),
                new HeroFrame(569,441,439,338,755,774),
                new HeroFrame(60,849,461,293,255,1140),
                new HeroFrame(559,786,345,356,780,1140),
                new HeroFrame(68,1177,387,314,270,1490),
                new HeroFrame(568,1168,399,324,765,1490)
            })
        };
        // Dash, blood bolts, ground wave.
        readonly AttackClip[] bossAttackClips={
            new AttackClip("Demons/BossDashV2",0.36f,new[]{
                new HeroFrame(85,50,310,330,295,362.222f),
                new HeroFrame(596,67,280,319,783,387.222f),
                new HeroFrame(63,447,350,290,315,722.222f),
                new HeroFrame(602,468,358,247,810,742.222f),
                new HeroFrame(73,847,392,177,300,1112.222f),
                new HeroFrame(592,788,350,334,785,1112.222f),
                new HeroFrame(80,1142,336,336,290,1472.222f),
                new HeroFrame(594,1145,346,335,790,1472.222f)
            }),
            new AttackClip("Demons/BossBoltsV2",0.36f,new[]{
                new HeroFrame(109,49,288,320,305,367.222f),
                new HeroFrame(613,49,291,320,805,367.222f),
                new HeroFrame(109,414,332,319,305,742.222f),
                new HeroFrame(601,412,363,325,805,742.222f),
                new HeroFrame(98,784,351,326,300,1112.222f),
                new HeroFrame(607,785,329,325,805,1112.222f),
                new HeroFrame(107,1160,324,330,300,1482.222f),
                new HeroFrame(609,1164,301,327,805,1482.222f)
            }),
            new AttackClip("Demons/BossWaveV2",0.36f,new[]{
                new HeroFrame(89,59,329,337,310,382.222f),
                new HeroFrame(545,60,356,335,785,382.222f),
                new HeroFrame(99,413,335,340,310,737.222f),
                new HeroFrame(578,419,349,343,785,742.222f),
                new HeroFrame(101,783,340,343,310,1112.222f),
                new HeroFrame(557,781,385,345,785,1112.222f),
                new HeroFrame(90,1152,342,334,310,1482.222f),
                new HeroFrame(577,1151,348,335,795,1482.222f)
            })
        };
        static int TimedPose(float remaining,float duration,int start,int count)
        {
            float progress=duration>0?1-Mathf.Clamp01(remaining/duration):1;
            return start+Mathf.Min(count-1,Mathf.FloorToInt(progress*count));
        }
        static int EnemyAttackPose(Enemy e)
        {
            return e.Windup>0?TimedPose(e.Windup,e.WindupDuration,0,3):
                TimedPose(e.AttackPose,e.AttackReleaseDuration,3,5);
        }
        bool SelectDemonAttack(Enemy e,out AttackClip clip,out int pose)
        {
            clip=demonAttackClips[(int)e.Kind];pose=EnemyAttackPose(e);
            return clip.Texture!=null&&e.Hp>0&&(e.Windup>0||e.AttackPose>0);
        }
        bool SelectBossAttack(Enemy e,out AttackClip clip,out int pose)
        {
            int kind=e.Pattern==BossPattern.Dash?0:e.Pattern==BossPattern.BloodBolts?1:2;
            clip=bossAttackClips[e.PhaseTransition>0?2:kind];pose=0;
            if(clip.Texture==null||e.Hp<=0)return false;
            if(e.PhaseTransition>0)
            {
                // Awakening uses anticipation, without showing a false damage release.
                pose=e.PhaseTransition>.25f?TimedPose(e.PhaseTransition-.25f,.75f,0,3):7;
                return true;
            }
            if(e.Windup>0){pose=TimedPose(e.Windup,e.WindupDuration,0,3);return true;}
            if(e.DashRemaining>0){pose=TimedPose(e.DashRemaining,e.DashDuration,3,2);return true;}
            if(e.AttackPose<=0)return false;
            pose=e.Pattern==BossPattern.Dash?TimedPose(e.AttackPose,e.AttackReleaseDuration,5,3):EnemyAttackPose(e);
            return true;
        }
    }
}
