using System;
using System.Collections.Generic;
using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        readonly Dictionary<SpellKind,Texture2D> skillIcons=new Dictionary<SpellKind,Texture2D>();
        static readonly Color SkillReady=new Color(.56f,.91f,.76f);
        static readonly Color SkillWaiting=new Color(1f,.64f,.44f);
        void LoadSkillIcons()
        {
            skillIcons.Clear();
            foreach(SpellKind spell in Enum.GetValues(typeof(SpellKind)))
                skillIcons[spell]=Resources.Load<Texture2D>("SkillIcons/"+spell+"V1");
        }
        static Color SkillAccent(SpellKind spell)
        {
            switch(spell)
            {
                case SpellKind.EmberWave: case SpellKind.AshLance:return new Color(1,.64f,.3f);
                case SpellKind.CrimsonNova: case SpellKind.HeartBreaker: case SpellKind.CrownGear:return new Color(1,.4f,.45f);
                case SpellKind.ChainPull: case SpellKind.ChainRain: case SpellKind.MirrorShards: case SpellKind.MirrorWard:return new Color(.79f,.65f,1);
                case SpellKind.MoonBlades: case SpellKind.NightMantle:return new Color(.55f,.8f,1);
                default:return HudLight;
            }
        }
        void DrawSkillIcon(Rect rect,SpellKind spell,bool dim=false)
        {
            var previous=GUI.color;
            GUI.color=dim?new Color(.38f,.38f,.43f):Color.white;
            if(skillIcons.TryGetValue(spell,out var texture)&&texture!=null)
                GUI.DrawTexture(rect,texture,ScaleMode.ScaleToFit,true);
            else
            {
                GUI.color=Color.white;
                DrawGothicSkillSymbol(rect.center,SkillSymbol(spell),dim?HudIron:SkillAccent(spell));
            }
            GUI.color=previous;
        }
        void SkillBadge(Rect rect,string label,Color color,int fontSize=14)
        {
            GothicPlaque(rect,5,color);
            GothicPlaque(new Rect(rect.x+1,rect.y+1,rect.width-2,rect.height-2),4,HudInk);
            Text(label,rect.x+3,rect.y,rect.width-6,rect.height,fontSize,color,TextAnchor.MiddleCenter);
        }
        void DrawSkillCooldownBar(Rect rect,SpellKind spell)
        {
            float ready=1-Mathf.Clamp01(run.SkillCooldown(spell)/Mathf.Max(.01f,run.SkillReuse(spell)));
            GothicPlaque(rect,3,HudGold);
            Box(new Rect(rect.x+1,rect.y+1,rect.width-2,rect.height-2),HudInk);
            if(ready>0)Box(new Rect(rect.x+2,rect.y+2,(rect.width-4)*ready,rect.height-4),ready>=1?SkillReady:SkillAccent(spell));
        }
        void DrawEquippedSkillIcon(Rect slot,SpellKind spell,int index)
        {
            float x=slot.center.x;
            float remaining=run.SkillCooldown(spell);
            bool cooling=remaining>0,active=run.Casting&&run.CurrentSpell==spell;
            Color accent=SkillAccent(spell);
            Text(SpellLabel(spell),slot.x-4,578,slot.width+8,20,14,HudLight,TextAnchor.MiddleCenter);
            GothicPlaque(new Rect(x-39,598,78,74),10,active?Color.white:cooling?HudGold:accent);
            GothicPlaque(new Rect(x-37,600,74,70),8,HudInk);
            DrawSkillIcon(new Rect(x-32,602,64,64),spell,cooling&&!active);
            GothicDiamond(x-40,635,4,accent);GothicDiamond(x+40,635,4,accent);
            if(cooling&&!active)
            {
                Box(new Rect(x-30,617,60,28),new Color(.015f,.02f,.035f,.94f));
                // Round up the last tenth so a waiting skill never reads 0.0.
                Text((Mathf.Ceil(remaining*10)/10).ToString("0.0"),x-30,615,60,30,25,Color.white,TextAnchor.MiddleCenter);
            }
            string key=keys.Label(5+index);
            SkillBadge(new Rect(x-31,654,62,22),key,HudLight,key.Length>8?9:key.Length>5?11:16);
            DrawSkillCooldownBar(new Rect(slot.x+12,680,slot.width-24,8),spell);
            Text(active?"시전 중":cooling?"재사용 대기":"사용 준비",slot.x,690,slot.width,20,13,active?Color.white:cooling?SkillWaiting:SkillReady,TextAnchor.MiddleCenter);
        }
        void DrawSkillWindowSlot(int slot,SpellKind? spell,float x)
        {
            var rect=new Rect(x,120,260,60);
            GothicPlaque(rect,9,spell.HasValue&&spell.Value==selectedSkill?HudLight:HudGold);
            GothicPlaque(new Rect(x+2,122,256,56),7,HudInk);
            if(spell.HasValue)
            {
                DrawSkillIcon(new Rect(x+7,126,48,48),spell.Value);
                Text(SpellLabel(spell.Value),x+65,125,185,25,19,HudLight);
                float remaining=run.SkillCooldown(spell.Value);
                Text(remaining>0?$"대기 {Mathf.Ceil(remaining*10)/10:0.0}초":"사용 준비",x+177,153,73,20,12,remaining>0?SkillWaiting:SkillReady,TextAnchor.MiddleRight);
                var selected=spell.Value;hits.Add(new Hit(rect,()=>SelectSkill(selected)));
            }
            else Text("빈 슬롯",x+65,125,185,25,19,new Color(.6f,.65f,.68f));
            string key=keys.Label(5+slot);
            Text((slot+1)+"번 · "+key,x+65,153,103,20,key.Length>8?10:13,HudLight);
        }
        void DrawLearnedSkillRow(SpellKind spell,float y)
        {
            int slot=run.EquippedSlot(spell);bool selected=selectedSkill==spell;
            var rect=new Rect(95,y,444,37);
            GothicPlaque(new Rect(92,y-2,450,41),7,selected?HudLight:slot>=0?HudGold:HudIron);
            GothicPlaque(rect,5,selected?new Color(.13f,.12f,.13f):HudInk);
            DrawSkillIcon(new Rect(99,y+2,33,33),spell);
            Text(SpellLabel(spell),142,y+1,203,35,20,selected?Color.white:HudLight,TextAnchor.MiddleLeft);
            if(slot>=0)
            {
                SkillBadge(new Rect(349,y+6,80,25),"장착 "+(slot+1),HudLight);
                string key=keys.Label(5+slot);
                Text(key,434,y+1,98,35,key.Length>8?9:key.Length>5?11:15,HudLight,TextAnchor.MiddleCenter);
            }
            else Text("미장착",433,y+1,99,35,14,new Color(.62f,.68f,.71f),TextAnchor.MiddleCenter);
            hits.Add(new Hit(rect,()=>SelectSkill(spell)));
        }
    }
}
