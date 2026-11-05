using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        static readonly Color HudIron=new Color(.13f,.16f,.19f);
        static readonly Color HudGold=new Color(.64f,.49f,.29f);
        static readonly Color HudLight=new Color(.93f,.78f,.5f);
        static readonly Color HudInk=new Color(.025f,.035f,.055f);

        void DrawGothicHudBackdrop()
        {
            // One continuous surface avoids seams between translucent strips
            // at fractional window scales. No full-width decorative rules.
            Box(new Rect(0,0,1280,130),new Color(.025f,.035f,.055f,.91f));
        }

        void DrawPlayerVitals()
        {
            float fraction=Mathf.Clamp01(run.Hp/run.MaxHp);
            Color life=fraction<=.3f?new Color(.94f,.31f,.28f):new Color(.32f,.72f,.64f);
            var bar=new Rect(318,43,282,22);
            DrawGothicMeter(bar,fraction,life);
            // The extinguished sun and its surviving ember are the player's crest.
            GothicDisc(288,54,26,HudGold);GothicDisc(288,54,23,HudInk);
            GothicDisc(288,54,19,HudIron);
            for(int i=0;i<8;i++)
            {
                float angle=i*Mathf.PI/4;
                var direction=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
                GothicLine(new Vector2(288,54)+direction*26,new Vector2(288,54)+direction*30,HudGold,2);
            }
            DrawGothicFlame(new Vector2(288,54),.75f,HudLight);
            Text($"{run.Hp:0} / {run.MaxHp:0}",318,73,190,28,21,new Color(.9f,.94f,.88f));
            Text(fraction<=.3f?"생명 · 위험":"생명 · 남은 불씨",318,16,230,23,15,fraction<=.3f?life:HudLight);
            Text($"공격 {run.SwordDamage:0.0}",502,77,100,24,15,new Color(.64f,.7f,.71f),TextAnchor.MiddleRight);
        }

        void DrawGothicMeter(Rect r,float fraction,Color color)
        {
            fraction=Mathf.Clamp01(fraction);
            GothicPlaque(new Rect(r.x-9,r.y-7,r.width+18,r.height+14),8,HudIron);
            GothicPlaque(new Rect(r.x-5,r.y-4,r.width+10,r.height+8),6,HudGold);
            GothicPlaque(new Rect(r.x-3,r.y-2,r.width+6,r.height+4),5,HudInk);
            // Clip only the liquid. The complete ornamental frame never shrinks with HP.
            if(fraction>0)
            {
                GUI.BeginGroup(new Rect(r.x,r.y,r.width*fraction,r.height));
                GothicPlaque(new Rect(0,0,r.width,r.height),4,color);
                Box(new Rect(4,2,Mathf.Max(0,r.width-8),3),new Color(1,1,.85f,.24f));
                Box(new Rect(4,r.height-5,Mathf.Max(0,r.width-8),4),new Color(0,0,0,.22f));
                GUI.EndGroup();
            }
            for(int side=-1;side<=1;side+=2)
            {
                float x=side<0?r.x-8:r.xMax+8;
                GothicDiamond(x,r.center.y,7,HudGold);
                GothicLine(new Vector2(x,r.center.y),new Vector2(x+side*16,r.center.y-12),HudGold,2);
                GothicLine(new Vector2(x,r.center.y),new Vector2(x+side*16,r.center.y+12),HudGold,2);
            }
        }

        void DrawGothicSkillTray()
        {
            // Retain the existing touch hit rectangles, and use the same seals on desktop.
            Box(new Rect(0,580,1280,140),new Color(.018f,.026f,.043f,.84f));
            DrawGothicSkill(roll,"구르기",keys.Label(4),0,run.RollCooldown,run.EffectiveRollReuse,run.Rolling);
            DrawEquippedSkillSlots();
            DrawGothicSkill(attack,"검 3연타",keys.Label(2),2,0,1,run.SwordActive);
            DrawGothicSkill(jump,"점프",keys.Label(3),3,0,1,!run.Grounded);
            if(TouchControls)
            {
                DrawGothicDirection(left,"←");DrawGothicDirection(right,"→");
                if((run.CanUseExit||run.CanOpenChest)&&!paused&&!settings&&run.Phase==RunPhase.Combat)
                {
                    // End before the first skill slot so tapping a door cannot cast.
                    var door=new Rect(250,682,190,30);
                    GothicPlaque(door,12,HudGold);
                    GothicPlaque(new Rect(door.x+2,door.y+2,door.width-4,door.height-4),10,HudInk);
                    Text(run.CanOpenChest?"↑ 상자 열기":"↑ 문 이동",door.x,door.y,door.width,door.height,21,HudLight,TextAnchor.MiddleCenter);
                    hits.Add(new Hit(door,()=>{if(run.CanUseExit||run.CanOpenChest)queuedEnter=true;}));
                }
            }
            else
            {
                Text($"{keys.Label(0)} / {keys.Label(1)} 이동 · ↑ 문/상자 · Esc 정지",20,588,180,48,12,new Color(.76f,.78f,.77f));
                Text($"↓ + {keys.Label(3)} 내려가기 · {keys.Label(2)} 유지: 연속 베기",20,640,180,42,12,new Color(.55f,.64f,.66f));
            }
        }

        void DrawGothicSkill(Rect slot,string name,string key,int symbol,float remaining,float reuse,bool active,bool locked=false)
        {
            float x=slot.center.x,y=625;
            Color accent=symbol==5?new Color(1,.3f,.43f):symbol==1||symbol==4?new Color(1,.57f,.23f):symbol==0?new Color(.5f,.81f,.79f):HudLight;
            bool cooling=remaining>0;
            Text(name,slot.x-4,578,slot.width+8,20,name.Length>=6?12:15,new Color(.83f,.81f,.74f),TextAnchor.MiddleCenter);
            GothicDisc(x,y,33,HudGold);GothicDisc(x,y,31,HudIron);GothicDisc(x,y,28,HudInk);
            GothicDiamond(x,y-30,4,active?accent:HudLight);
            GothicDiamond(x-32,y,4,HudGold);GothicDiamond(x+32,y,4,HudGold);
            if(locked)
            {
                DrawGothicSkillSymbol(new Vector2(x,y),symbol,new Color(.2f,.23f,.26f));
                Text("잠김",x-32,y-14,64,28,19,new Color(.68f,.69f,.7f),TextAnchor.MiddleCenter);
                Text(symbol>=8?(symbol-4)+"층 보상":symbol==7?"3탄 보스 보상":symbol==6?"2탄 보스 보상":"1탄 보스 보상",slot.x-6,659,slot.width+12,19,12,new Color(.57f,.62f,.63f),TextAnchor.MiddleCenter);
                return;
            }
            // A clockwise replenishing ring and numeric seconds communicate cooldown together.
            float ready=cooling?1-Mathf.Clamp01(remaining/reuse):1;
            for(int i=0;i<40;i++)
            {
                float a=-Mathf.PI*.5f+i*Mathf.PI/20;
                float b=a+Mathf.PI/24;
                Color ring=(i+1)/40f<=ready?accent:new Color(.22f,.24f,.26f);
                GothicLine(new Vector2(x+Mathf.Cos(a)*26,y+Mathf.Sin(a)*26),new Vector2(x+Mathf.Cos(b)*26,y+Mathf.Sin(b)*26),ring,2);
            }
            Color icon=cooling?new Color(.22f,.26f,.29f):accent;
            DrawGothicSkillSymbol(new Vector2(x,y),symbol,icon);
            if(cooling)Text($"{remaining:0.0}",x-32,y-14,64,28,22,Color.white,TextAnchor.MiddleCenter);
            else if(active)GothicDiamond(x,y+21,3,accent);
            // Fit user-rebound keys without wrapping over neighboring skills.
            Text(TouchControls?(cooling?"재사용 대기":active?"사용 중":"준비"):key,slot.x,659,slot.width,19,key.Length>10?10:key.Length>6?12:14,HudLight,TextAnchor.MiddleCenter);
        }

        void DrawGothicSkillSymbol(Vector2 c,int symbol,Color color)
        {
            if(symbol>=8){DrawLateSpellSymbol(c,symbol-8,color);return;}
            if(symbol==7)
            {
                GothicDisc(c.x,c.y,10,color);GothicDisc(c.x,c.y,6,HudInk);
                for(int i=0;i<8;i++)
                {
                    float a=i*Mathf.PI/4;var dir=new Vector2(Mathf.Cos(a),Mathf.Sin(a));
                    GothicLine(c+dir*13,c+dir*20,color,2);
                }
                return;
            }
            if(symbol==6){DrawChainLinks(c+new Vector2(-16,15),c+new Vector2(15,-15),color,4);return;}
            if(symbol==1){DrawGothicFlame(c,1,color);return;}
            if(symbol==4)
            {
                GothicLine(c+new Vector2(-17,13),c+new Vector2(14,-14),color,3);
                GothicPolygon(color,c+new Vector2(18,-19),c+new Vector2(14,-3),c+new Vector2(3,-14));return;
            }
            if(symbol==5)
            {
                for(int i=0;i<16;i++)
                {
                    float a=i*Mathf.PI/8,b=(i+1)*Mathf.PI/8;
                    GothicLine(c+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*17,c+new Vector2(Mathf.Cos(b),Mathf.Sin(b))*17,color,2);
                }
                DrawGothicFlame(c,.65f,color);return;
            }
            if(symbol==0)
            {
                for(int i=0;i<17;i++)
                {
                    float a=(i*15+35)*Mathf.Deg2Rad,b=a+12*Mathf.Deg2Rad;
                    GothicLine(c+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*15,c+new Vector2(Mathf.Cos(b),Mathf.Sin(b))*15,color,3);
                }
                GothicPolygon(color,c+new Vector2(8,-15),c+new Vector2(19,-15),c+new Vector2(17,-4));
                GothicDisc(c.x,c.y-5,4,color);
                GothicLine(c+new Vector2(-2,0),c+new Vector2(6,6),color,4);
                GothicLine(c+new Vector2(6,6),c+new Vector2(-4,10),color,3);
            }
            else if(symbol==2)
            {
                GothicPolygon(color,c+new Vector2(13,-19),c+new Vector2(11,-5),c+new Vector2(-5,11),c+new Vector2(-10,6),c+new Vector2(6,-10));
                GothicLine(c+new Vector2(-15,3),c+new Vector2(-2,16),HudGold,3);
                GothicLine(c+new Vector2(-8,10),c+new Vector2(-15,17),color,3);
            }
            else
            {
                GothicLine(c+new Vector2(0,12),c+new Vector2(0,-13),color,4);
                GothicLine(c+new Vector2(0,-13),c+new Vector2(-10,-3),color,3);
                GothicLine(c+new Vector2(0,-13),c+new Vector2(10,-3),color,3);
                GothicLine(c+new Vector2(-13,17),c+new Vector2(13,17),HudGold,2);
            }
        }

        void DrawGothicFlame(Vector2 c,float scale,Color color)
        {
            GothicPolygon(color,c+new Vector2(-3,-20)*scale,c+new Vector2(8,-6)*scale,c+new Vector2(12,-12)*scale,
                c+new Vector2(16,6)*scale,c+new Vector2(7,17)*scale,c+new Vector2(-7,17)*scale,
                c+new Vector2(-15,7)*scale,c+new Vector2(-12,-6)*scale,c+new Vector2(-7,0)*scale);
            GothicPolygon(HudInk,c+new Vector2(1,-3)*scale,c+new Vector2(7,9)*scale,c+new Vector2(0,14)*scale,c+new Vector2(-5,9)*scale);
        }
        void DrawGothicDirection(Rect r,string arrow)
        {
            GothicDisc(r.center.x,625,33,HudGold);GothicDisc(r.center.x,625,30,HudInk);
            Text(arrow,r.x,596,r.width,56,32,HudLight,TextAnchor.MiddleCenter);
        }
        void GothicPlaque(Rect r,float cut,Color color)
        {
            if(r.width<=0||r.height<=0)return;
            cut=Mathf.Clamp(cut,0,Mathf.Min(r.width,r.height)*.5f);
            // Only beveled corners need scanlines. The solid centre is one quad,
            // with disjoint edges so translucent panels retain the same opacity.
            float first=Mathf.Floor(r.y),last=Mathf.Ceil(r.yMax);
            float middleTop=Mathf.Min(last,Mathf.Ceil(r.y+cut));
            float middleBottom=Mathf.Max(middleTop,Mathf.Floor(r.yMax-cut));
            DrawPlaqueRows(r,cut,color,first,middleTop);
            if(middleBottom>middleTop)Box(new Rect(r.x,middleTop,r.width,middleBottom-middleTop),color);
            DrawPlaqueRows(r,cut,color,middleBottom,last);
        }
        void DrawPlaqueRows(Rect r,float cut,Color color,float first,float last)
        {
            for(float y=first;y<last;y++)
            {
                float scan=y+.5f;if(scan<r.y||scan>=r.yMax)continue;
                float inset=Mathf.Max(0,Mathf.Max(r.y+cut-scan,scan-(r.yMax-cut)));
                Box(new Rect(r.x+inset,y,Mathf.Max(0,r.width-inset*2),1),color);
            }
        }
        void GothicDiamond(float x,float y,float radius,Color color)
        {GothicPolygon(color,new Vector2(x,y-radius),new Vector2(x+radius,y),new Vector2(x,y+radius),new Vector2(x-radius,y));}
        void GothicDisc(float x,float y,float radius,Color color)
        {
            for(float offset=-radius;offset<radius;offset+=2)
            {
                float half=Mathf.Sqrt(Mathf.Max(0,radius*radius-(offset+1)*(offset+1)));
                Box(new Rect(Mathf.Round(x-half),y+offset,Mathf.Round(half*2),2),color);
            }
        }
        void GothicLine(Vector2 from,Vector2 to,Color color,float thickness)
        {
            var delta=to-from;var previous=GUI.color;
            using(ArtworkAt((from+to)*.5f,Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg))
            {
                try
                {
                    GUI.color=color;
                    DrawArtwork(white,new Rect(-delta.magnitude*.5f,-thickness*.5f,delta.magnitude,thickness),new Rect(0,0,1,1));
                }
                finally {GUI.color=previous;}
            }
        }
        void GothicPolygon(Color color,params Vector2[] points)
        {
            float min=points[0].y,max=min;
            foreach(var p in points){min=Mathf.Min(min,p.y);max=Mathf.Max(max,p.y);}
            var intersections=new float[points.Length];
            for(float y=Mathf.Floor(min);y<max;y+=1)
            {
                int count=0;float scan=y+.5f;
                for(int i=0;i<points.Length;i++)
                {
                    Vector2 a=points[i],b=points[(i+1)%points.Length];
                    if((a.y<=scan&&b.y>scan)||(b.y<=scan&&a.y>scan))
                        intersections[count++]=a.x+(scan-a.y)*(b.x-a.x)/(b.y-a.y);
                }
                System.Array.Sort(intersections,0,count);
                for(int i=0;i+1<count;i+=2)Box(new Rect(intersections[i],y,intersections[i+1]-intersections[i],1),color);
            }
        }
    }
}
