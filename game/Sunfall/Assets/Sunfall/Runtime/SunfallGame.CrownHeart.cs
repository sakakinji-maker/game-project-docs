using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        void DrawCrownHeartLandmarks()
        {
            float floor=SY(0),roof=SY(12);
            Color iron=new Color(.09f,.07f,.105f),rim=new Color(.34f,.23f,.25f),gold=new Color(.61f,.4f,.22f);
            Box(new Rect(0,roof,1280,floor-roof),new Color(.025f,.015f,.04f,.86f));
            for(int bay=0;bay<8;bay++)
            {
                float x=SX(2+bay*9);
                if(x<-170||x>1450)continue;
                GothicPolygon(iron,new Vector2(x-28,floor),new Vector2(x-17,roof+45),new Vector2(x,roof),new Vector2(x+17,roof+45),new Vector2(x+28,floor));
                GothicLine(new Vector2(x-21,floor),new Vector2(x-12,roof+50),rim,3);
                GothicLine(new Vector2(x+21,floor),new Vector2(x+12,roof+50),rim,3);
                for(int band=0;band<5;band++)
                {
                    float y=SY(1.3f+band*2);
                    Box(new Rect(x-25,y,50,7),rim);GothicDiamond(x,y+4,5,gold);
                }
                if(run.RoomIndex==0)
                {
                    GothicPolygon(new Color(.25f,.04f,.09f),new Vector2(x+46,roof+90),new Vector2(x+97,roof+90),new Vector2(x+97,roof+210),new Vector2(x+71,roof+193),new Vector2(x+46,roof+210));
                    GothicDiamond(x+71,roof+135,16,gold);
                }
            }
            if(run.RoomIndex==0){DrawBrokenArch(17,10);DrawBrokenArch(48,10);}
            else if(run.RoomIndex==1)
            {
                for(int row=0;row<3;row++)
                {
                    float y=SY(3.2f+row*2.5f),start=SX(0),end=SX(64);
                    GothicLine(new Vector2(start,y),new Vector2(end,y),rim,9);
                    GothicLine(new Vector2(start,y),new Vector2(end,y),new Color(.57f,.3f,.17f,.65f),2);
                    for(int pulse=0;pulse<8;pulse++)
                    {
                        float worldX=Mathf.Repeat(pulse*8-visualClock*(1.2f+row*.35f),64);
                        GothicDiamond(SX(worldX),y,3,new Color(.92f,.65f,.3f,.7f));
                    }
                }
                DrawCrownMechanism(20,7.7f,54);DrawCrownMechanism(45,7.7f,65);
            }
            else
            {
                float x=SX(32),y=SY(7.1f),pulse=1+Mathf.Sin(visualClock*1.6f)*.035f;
                DrawCrownMechanism(32,7.1f,112);
                for(int side=-1;side<=1;side+=2)
                {
                    DrawChainLinks(new Vector2(SX(32+side*9),SY(11)),new Vector2(x+side*38,y+48),gold,24);
                    DrawChainLinks(new Vector2(SX(32+side*8),SY(2.8f)),new Vector2(x+side*30,y-38),gold,20);
                }
                GothicPolygon(new Color(.56f,.13f,.22f),new Vector2(x,y-72*pulse),new Vector2(x+43*pulse,y),new Vector2(x,y+72*pulse),new Vector2(x-43*pulse,y));
                GothicDiamond(x,y,20,new Color(.95f,.63f,.35f));
                if(run.BossDefeated)DrawLateSeam(x,y,HudLight);
                // The mother's thread remains visible inside the royal mechanism.
                GothicLine(new Vector2(x-14,y-50),new Vector2(x+10,y+50),HudLight,2);
                for(int i=0;i<6;i++)GothicLine(new Vector2(x-22,y-36+i*14),new Vector2(x+21,y-43+i*14),HudLight,2);
            }
        }
        void DrawCrownMechanism(float worldX,float worldY,float radius)
        {
            var c=new Vector2(SX(worldX),SY(worldY));
            if(c.x+radius<0||c.x-radius>1280)return;
            Color metal=new Color(.37f,.25f,.23f),gold=new Color(.6f,.41f,.23f);
            GothicDisc(c.x,c.y,radius,metal);GothicDisc(c.x,c.y,radius-9,new Color(.035f,.025f,.05f));
            for(int i=0;i<16;i++)
            {
                float a=i*Mathf.PI/8+(run.BossDefeated?0:visualClock*.08f);var d=new Vector2(Mathf.Cos(a),Mathf.Sin(a));
                GothicLine(c+d*(radius-9),c+d*(radius+9),gold,5);
            }
            GothicDisc(c.x,c.y,radius*.45f,metal);GothicDisc(c.x,c.y,radius*.36f,new Color(.08f,.035f,.07f));
        }
        void DrawCrownStone(float x,float top,float width,bool ground)
        {
            DrawCathedralStone(x,top,width,ground);
            Box(new Rect(x,top+4,width,4),new Color(.46f,.29f,.22f));
            Box(new Rect(x,top+11,width,2),new Color(.66f,.4f,.23f));
            for(float rivet=x+14;rivet<x+width;rivet+=45)GothicDiamond(rivet,top+21,3,new Color(.6f,.43f,.29f));
        }
    }
}
