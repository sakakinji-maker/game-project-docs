using System;
using System.Collections.Generic;

namespace Sunfall.Core
{
    public static class LateSpellCatalog
    {
        public static int Slot(SpellKind spell)=>((int)spell-(int)SpellKind.HeartBreaker)/2;
        public static bool Contains(SpellKind spell)=>spell>=SpellKind.HeartBreaker&&spell<=SpellKind.SolarCrown;
        public static SpellKind Option(int chapter,int option)=>(SpellKind)((int)SpellKind.HeartBreaker+(chapter-4)*2+option);
        public static string Name(SpellKind spell)
        {
            switch(spell){case SpellKind.HeartBreaker:return "심장 파쇄";case SpellKind.CrownGear:return "왕관 톱니";case SpellKind.MirrorShards:return "거울 파편";case SpellKind.MirrorWard:return "거울 장막";case SpellKind.MoonBlades:return "초승달 쌍날";case SpellKind.NightMantle:return "밤의 장막";case SpellKind.RoyalLance:return "왕의 광창";case SpellKind.SolarCrown:return "태양 왕관";default:return "";}
        }
        public static float Reuse(SpellKind spell)
        {
            switch(spell){case SpellKind.HeartBreaker:return 8;case SpellKind.CrownGear:return 7;case SpellKind.MirrorShards:return 7;case SpellKind.MirrorWard:return 12;case SpellKind.MoonBlades:return 8;case SpellKind.NightMantle:return 12;case SpellKind.RoyalLance:return 10;default:return 12;}
        }
        public static string Description(SpellKind spell)
        {
            switch(spell)
            {
                case SpellKind.HeartBreaker:return "전방 5칸, 높이 3칸의 충격파.\n마력 ×4 피해 · 재사용 8초";
                case SpellKind.CrownGear:return "13칸을 관통하는 왕관의 톱니.\n마력 ×3.5 피해 · 재사용 7초";
                case SpellKind.MirrorShards:return "세 방향으로 관통 파편을 발사.\n파편당 마력 ×1.6 · 재사용 7초";
                case SpellKind.MirrorWard:return "4초간 피해를 흡수하는 거울 보호막.\n흡수량 35 + 마력 ×0.5 · 재사용 12초";
                case SpellKind.MoonBlades:return "좌우로 초승달 관통 날을 발사.\n각 마력 ×3.2 피해 · 재사용 8초";
                case SpellKind.NightMantle:return "주변 3.5칸에 마력 ×2 피해.\n4초간 받는 피해 40% 감소 · 재사용 12초";
                case SpellKind.RoyalLance:return "전방 18칸을 즉시 관통하는 광창.\n마력 ×5 피해 · 재사용 10초";
                case SpellKind.SolarCrown:return "시전 위치에 반경 5칸 태양 문장.\n마력 ×2.2 피해를 3회 · 재사용 12초";
                default:return "";
            }
        }
    }
    public sealed class LateSpellShot
    {
        public SpellKind Kind;
        public float X,Y,VX,VY,Damage,Life,Radius=.3f;
        internal readonly HashSet<Enemy> Hit=new HashSet<Enemy>();
    }
    public sealed class LateSpellEffect
    {
        public SpellKind Kind;
        public float X,Y,Remaining,Damage,PulseDelay;
        public int Facing,Pulses;
    }
    public sealed partial class RunModel
    {
        readonly SpellKind?[] lateRewards=new SpellKind?[4];
        readonly float[] lateCooldowns=new float[4];
        public int PendingSkillChapter {get;private set;}
        public bool SkillChoicePending=>PendingSkillChapter>=1&&PendingSkillChapter<=FinalChapter;
        public bool CanChooseBossSkill=>SkillChoicePending&&PendingSkillChapter==Chapter&&Stage==RunStage.BossArena&&BossDefeated&&Phase==RunPhase.Combat&&!Cinematic&&Hp>0;
        public SpellKind? LateReward(int slot)=>slot>=0&&slot<4?lateRewards[slot]:null;
        public float LateCooldown(int slot)=>slot>=0&&slot<4?lateCooldowns[slot]:0;
        public float LateReuse(int slot)=>LateReward(slot).HasValue?LateSpellCatalog.Reuse(lateRewards[slot].Value)*Equipment.MagicReuseMultiplier:1;
        SpellKind? LatestLateReward {get {for(int i=3;i>=0;i--)if(lateRewards[i].HasValue)return lateRewards[i];return null;}}
        public float MirrorShield {get;private set;}
        public float MirrorShieldTime {get;private set;}
        public float NightMantleTime {get;private set;}
        public readonly List<LateSpellShot> LateShots=new List<LateSpellShot>();
        public readonly List<LateSpellEffect> LateEffects=new List<LateSpellEffect>();
        public SpellKind? BossReward(int chapter)
        {
            switch(chapter){case 1:return BossSpellReward;case 2:return ChainSpellReward;case 3:return SolarSpellReward;default:return LateReward(chapter-4);}
        }
        public bool ChooseBossSkill(int option)
        {
            if(!CanChooseBossSkill||option<0||option>1)return false;
            if(BossReward(PendingSkillChapter).HasValue)return false;
            var reward=BossSkillCatalog.Option(PendingSkillChapter,option);
            switch(PendingSkillChapter)
            {
                case 1:BossSpellReward=reward;break;
                case 2:ChainSpellReward=reward;break;
                case 3:SolarSpellReward=reward;break;
                default:lateRewards[PendingSkillChapter-4]=reward;break;
            }
            PendingSkillChapter=0;AutoEquipNewSkill(reward);SpellRewardNotice=7;return true;
        }
        public bool ChooseRandomBossSkill()
        {
            if(!CanChooseBossSkill)return false;
            return ChooseBossSkill(Range(2));
        }
        bool HasLateSpell(SpellKind spell)=>LateSpellCatalog.Contains(spell)&&lateRewards[LateSpellCatalog.Slot(spell)]==spell;
        bool ReadyLateSlot(int slot)=>slot>=0&&slot<4&&lateRewards[slot].HasValue&&IsSpellEquipped(lateRewards[slot].Value)&&lateCooldowns[slot]<=0;
        void ResetLateSpellEffects()
        {
            LateShots.Clear();LateEffects.Clear();MirrorShield=MirrorShieldTime=NightMantleTime=0;
        }
        void ResetLateRoomMagic(){ResetLateSpellEffects();Array.Clear(lateCooldowns,0,lateCooldowns.Length);}
        void ReleaseLateSpell()
        {
            var kind=CurrentSpell;int slot=LateSpellCatalog.Slot(kind);lateCooldowns[slot]=LateReuse(slot);
            float cx=X,cy=Y+1;
            if(kind==SpellKind.CrownGear)AddLateShot(kind,cx,cy,ActionFacing*14,0,MagicDamage*3.5f,13f/14);
            else if(kind==SpellKind.MirrorShards)
            {
                for(int i=-1;i<=1;i++)AddLateShot(kind,cx,cy,ActionFacing*(float)Math.Cos(i*.23f)*18,(float)Math.Sin(i*.23f)*18,MagicDamage*1.6f,12f/18);
            }
            else if(kind==SpellKind.MoonBlades)
            {AddLateShot(kind,cx,cy,18,0,MagicDamage*3.2f,12f/18);AddLateShot(kind,cx,cy,-18,0,MagicDamage*3.2f,12f/18);}
            else if(kind==SpellKind.MirrorWard){MirrorShield=35+MagicDamage*.5f;MirrorShieldTime=4;}
            else if(kind==SpellKind.NightMantle){NightMantleTime=4;LateAreaDamage(cx,cy,3.5f,MagicDamage*2);}
            else if(kind==SpellKind.SolarCrown)LateAreaDamage(cx,cy,5,MagicDamage*2.2f);
            else
            {
                float reach=kind==SpellKind.HeartBreaker?5:18,damage=MagicDamage*(kind==SpellKind.HeartBreaker?4:5);
                foreach(var e in Enemies)
                {
                    float forward=(e.X-cx)*ActionFacing;
                    if(e.Hp>0&&forward+e.BodyHalfWidth>=0&&forward-e.BodyHalfWidth<=reach&&e.Y<Y+3&&e.Y+e.BodyHeight>Y)DamageEnemy(e,damage);
                }
            }
            LateEffects.Add(new LateSpellEffect {Kind=kind,X=cx,Y=cy,Facing=ActionFacing,Remaining=kind==SpellKind.SolarCrown?1.8f:.5f,Damage=MagicDamage*2.2f,Pulses=kind==SpellKind.SolarCrown?2:0,PulseDelay=.6f});
        }
        void AddLateShot(SpellKind kind,float x,float y,float vx,float vy,float damage,float life)
        {LateShots.Add(new LateSpellShot {Kind=kind,X=x,Y=y,VX=vx,VY=vy,Damage=damage,Life=life});}
        void LateAreaDamage(float x,float y,float radius,float damage)
        {
            foreach(var e in Enemies)
            {
                float dx=x-Clamp(x,e.X-e.BodyHalfWidth,e.X+e.BodyHalfWidth),dy=y-Clamp(y,e.Y,e.Y+e.BodyHeight);
                if(e.Hp>0&&dx*dx+dy*dy<=radius*radius)DamageEnemy(e,damage);
            }
        }
        void UpdateLateMagic(float dt)
        {
            for(int i=0;i<4;i++)lateCooldowns[i]=Math.Max(0,lateCooldowns[i]-dt);
            MirrorShieldTime=Math.Max(0,MirrorShieldTime-dt);if(MirrorShieldTime<=0)MirrorShield=0;
            NightMantleTime=Math.Max(0,NightMantleTime-dt);
            for(int i=LateShots.Count-1;i>=0;i--)
            {
                var shot=LateShots[i];float oldX=shot.X,oldY=shot.Y,step=Math.Min(dt,shot.Life);
                shot.X+=shot.VX*step;shot.Y+=shot.VY*step;shot.Life-=dt;
                foreach(var e in Enemies)
                {
                    if(e.Hp<=0||shot.Hit.Contains(e))continue;float enter=0,leave=1;
                    if(ClipAxis(oldX,shot.X-oldX,e.X-e.BodyHalfWidth-shot.Radius,e.X+e.BodyHalfWidth+shot.Radius,ref enter,ref leave)&&ClipAxis(oldY,shot.Y-oldY,e.Y-shot.Radius,e.Y+e.BodyHeight+shot.Radius,ref enter,ref leave))
                    {shot.Hit.Add(e);DamageEnemy(e,shot.Damage);}
                }
                if(shot.Life<=0||shot.X<0||shot.X>WorldWidth||shot.Y<0||shot.Y>12)LateShots.RemoveAt(i);
            }
            for(int i=LateEffects.Count-1;i>=0;i--)
            {
                var effect=LateEffects[i];effect.Remaining-=dt;
                if(effect.Pulses>0)
                {
                    effect.PulseDelay-=dt;
                    if(effect.PulseDelay<=0){LateAreaDamage(effect.X,effect.Y,5,effect.Damage);effect.Pulses--;effect.PulseDelay+=.6f;}
                }
                if(effect.Remaining<=0)LateEffects.RemoveAt(i);
            }
        }
    }
}
