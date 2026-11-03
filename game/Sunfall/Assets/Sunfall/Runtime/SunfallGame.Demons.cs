using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        Texture2D[] demonSheets;
        float nextDemonArtRetry;
        bool demonArtWarning;
        static readonly string[] DemonArtNames={"Melee","Archer","Heavy"};
        static readonly string[] DemonLabels={"근접병","궁수","중갑병"};
        // Original PNG coordinates, with padding around EVERY nontransparent pixel.
        // Root follows the hips horizontally and the grounded heel vertically.
        // The archer's first row extends below the mathematical half of the sheet.
        static readonly HeroFrame[][] DemonFrames={
            new[]{
                new HeroFrame(101,70,530,546,404,609),
                new HeroFrame(715,91,455,524,1030,609),
                new HeroFrame(67,656,501,552,405,1201),
                new HeroFrame(623,692,620,516,875,1201)
            },
            new[]{
                new HeroFrame(96,83,503,557,409,633),
                new HeroFrame(727,108,506,532,1030,633),
                new HeroFrame(49,654,601,569,349,1216),
                new HeroFrame(676,653,549,573,970,1219)
            },
            new[]{
                new HeroFrame(107,149,461,411,354,553),
                new HeroFrame(724,162,376,398,979,553),
                new HeroFrame(107,704,424,431,380,1128),
                new HeroFrame(695,747,512,401,910,1133)
            }
        };

        void LoadDemonArt()
        {
            foreach(var clip in demonAttackClips)clip.Load();
            demonSheets=new Texture2D[3];bool ready=true;
            for(int i=0;i<3;i++)
            {
                var sheet=Resources.Load<Texture2D>("Demons/"+DemonArtNames[i]);
                demonSheets[i]=sheet;
                if(sheet==null){ready=false;continue;}
                sheet.filterMode=FilterMode.Point;sheet.wrapMode=TextureWrapMode.Clamp;
            }
            nextDemonArtRetry=Time.realtimeSinceStartup+1;
            if(ready)demonArtWarning=false;
        }

        void DrawDemon(Enemy enemy)
        {
            if((enemy.Hp<=0&&enemy.DeathTimer<=0)||SX(enemy.X)<-180||SX(enemy.X)>1460)return;
            int kind=(int)enemy.Kind;
            if((demonSheets==null||demonSheets[kind]==null||demonAttackClips[kind].Texture==null)&&Time.realtimeSinceStartup>=nextDemonArtRetry)LoadDemonArt();
            var position=RenderPosition(enemy,enemy.X,enemy.Y);float x=SX(position.x),feetY=SY(position.y);
            float height=kind==2?88:74;
            Color accent=kind==1?new Color(.77f,.54f,1):kind==2?new Color(1,.75f,.35f):new Color(1,.48f,.43f);
            if(enemy.Hp>0&&enemy.Windup>0)
            {
                if(kind==1)
                {
                    // Aim locks with the drawn bow; the shot itself is not homing.
                    float end=Mathf.Clamp(enemy.X+enemy.Facing*9,.5f,RunModel.WorldWidth-.5f);
                    Box(new Rect(SX(Mathf.Min(enemy.X,end)),SY(enemy.Y+1.4f),Mathf.Abs(end-enemy.X)*34,2),new Color(.7f,.4f,1,.45f));
                }
                else
                {
                    float start=enemy.Facing<0?enemy.X-enemy.AttackReach:enemy.X;
                    Box(new Rect(SX(start),feetY-11,enemy.AttackReach*34,11),new Color(accent.r,accent.g,accent.b,.35f));
                }
                float progress=1-Mathf.Clamp01(enemy.Windup/enemy.WindupDuration);
                Box(new Rect(x-20,feetY-height-16,40*progress,3),accent);
                Text("!",x-12,feetY-height-44,24,26,22,accent,TextAnchor.MiddleCenter);
            }
            int pose=enemy.Windup>0?2:enemy.AttackPose>0?3:enemy.Walking?((int)(enemy.AnimationTime*5)%2):0;
            var texture=demonSheets==null?null:demonSheets[kind];
            if(texture==null)
            {
                if(!demonArtWarning){demonArtWarning=true;Debug.LogWarning("Sunfall: Demon textures are importing. Using a visible fallback and retrying.",this);}
                Box(new Rect(x-16,feetY-height,32,height),accent);
            }
            else
            {
                var frame=DemonFrames[kind][pose];float scale=kind==2?.22f:.14f;
                var size=new Vector2(1254,1254);
                bool attacking=SelectDemonAttack(enemy,out var attackClip,out var attackPose);
                if(attacking){texture=attackClip.Texture;frame=attackClip.Frames[attackPose];scale=attackClip.Scale;size=attackClip.Size;}
                var savedMatrix=GUI.matrix;var savedColor=GUI.color;
                var pivot=new Vector3(x,feetY,0);
                GUI.matrix=savedMatrix*Matrix4x4.Translate(pivot)*Matrix4x4.Scale(new Vector3(enemy.Facing,1,1))*Matrix4x4.Translate(-pivot);
                float alpha=enemy.Hp<=0?Mathf.Clamp01(enemy.DeathTimer/.3f):1;
                GUI.color=enemy.Flash>0?new Color(1,.5f,.5f,alpha):new Color(1,1,1,alpha);
                DrawCharacterPose(texture,frame,size,scale,x,feetY);
                GUI.matrix=savedMatrix;GUI.color=savedColor;
            }
            if(enemy.Hp<=0)return;
            Box(new Rect(x-22,feetY-height-9,44,4),new Color(.15f,.17f,.2f));
            Box(new Rect(x-22,feetY-height-9,44*Mathf.Clamp01(enemy.Hp/enemy.MaxHp),4),accent);
            Text(DemonLabels[kind],x-42,feetY+14,84,24,13,accent,TextAnchor.MiddleCenter);
        }

        void DrawDemonArrows()
        {
            foreach(var arrow in run.Arrows)
            {
                var position=RenderPosition(arrow,arrow.X,arrow.Y);float x=SX(position.x),y=SY(position.y);
                Box(new Rect(x-11,y-1,22,2),new Color(.7f,.5f,.9f));
                Box(new Rect(x+arrow.Facing*10-3,y-3,6,6),new Color(.94f,.8f,1));
                Box(new Rect(x-arrow.Facing*9-2,y-4,4,8),new Color(.55f,.3f,.75f));
            }
        }
    }
}


