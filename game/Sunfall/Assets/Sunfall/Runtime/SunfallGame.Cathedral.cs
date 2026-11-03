using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        void DrawCathedralLandmarks()
        {
            Color stone=new Color(.115f,.115f,.15f,.96f),edge=new Color(.28f,.25f,.29f),gold=new Color(.55f,.43f,.27f);
            float floor=SY(0),roof=SY(12);
            // Reuse the distant panorama, with a stone nave and pointed windows in front.
            Box(new Rect(0,roof,1280,floor-roof),new Color(.045f,.045f,.075f,run.RoomIndex==0?.5f:.87f));
            for(int bay=0;bay<7;bay++)
            {
                float x=SX(4+bay*9),top=SY(9.8f);
                if(x<-180||x>1460)continue;
                Box(new Rect(x-18,roof,36,floor-roof),stone);
                Box(new Rect(x-22,roof,5,floor-roof),edge);
                Box(new Rect(x+17,roof,5,floor-roof),edge);
                for(float row=roof+28;row<floor;row+=33)Box(new Rect(x-18,row,36,2),edge);
                Box(new Rect(x-30,floor-24,60,24),stone);
                float window=x+150;
                GothicPolygon(gold,new Vector2(window-58,top+40),new Vector2(window,top-25),new Vector2(window+58,top+40),new Vector2(window+58,top+190),new Vector2(window-58,top+190));
                GothicPolygon(new Color(.075f,.065f,.125f),new Vector2(window-52,top+43),new Vector2(window,top-17),new Vector2(window+52,top+43),new Vector2(window+52,top+184),new Vector2(window-52,top+184));
                for(int pane=0;pane<4;pane++)
                {
                    float y=top+58+pane*31;
                    GothicDiamond(window,y,13,pane%2==0?new Color(.44f,.2f,.32f):new Color(.34f,.34f,.48f));
                    GothicLine(new Vector2(window-49,y+15),new Vector2(window+49,y+15),edge,3);
                }
                GothicLine(new Vector2(window,top-14),new Vector2(window,top+184),gold,3);
                // Tattered royal banners alternate with the windows.
                if(bay%2==run.RoomIndex%2)
                {
                    GothicPolygon(new Color(.25f,.055f,.1f),new Vector2(x-21,top+26),new Vector2(x+21,top+26),new Vector2(x+21,top+140),new Vector2(x,top+125),new Vector2(x-21,top+140));
                    GothicDiamond(x,top+64,10,gold);
                }
            }
            if(run.RoomIndex==0){DrawBrokenArch(12,10);DrawBrokenArch(52,10);}
            if(run.RoomIndex==3){DrawBrokenArch(32,11);DrawSolarSeal();}
            if(run.RoomIndex==2)
            {
                float cx=SX(32),cy=SY(7);
                GothicDisc(cx,cy,75,new Color(.43f,.32f,.18f));
                GothicDisc(cx,cy,65,new Color(.045f,.035f,.06f));
                for(int side=-1;side<=1;side+=2)
                    DrawChainLinks(new Vector2(cx+side*115,cy-95),new Vector2(cx-side*60,cy+75),gold,18);
                Box(new Rect(cx-95,floor-40,190,40),stone);
                GothicLine(new Vector2(cx-95,floor-40),new Vector2(cx+95,floor-40),gold,3);
                GothicDiamond(cx,cy,10,new Color(.82f,.58f,.28f));
            }
        }
        void DrawCathedralStone(float x,float top,float width,bool ground)
        {
            float height=ground?70:31;
            Box(new Rect(x,top,width,height),new Color(.17f,.17f,.21f));
            Box(new Rect(x,top+6,width,4),new Color(.32f,.29f,.29f));
            for(int row=0;row<3;row++)
            {
                float y=top+12+row*20;if(y>=top+height)break;
                Box(new Rect(x,y,width,2),new Color(.085f,.085f,.12f));
                for(float joint=x+(row%2)*25;joint<x+width;joint+=50)
                    Box(new Rect(joint,y,2,Mathf.Min(18,top+height-y)),new Color(.09f,.09f,.12f));
            }
        }
    }
}
