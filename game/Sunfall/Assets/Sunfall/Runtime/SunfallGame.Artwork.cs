using System;
using UnityEngine;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        // Compose effect-local coordinates on the CPU. Moving GUI's origin to
        // the effect centre leaves negative sprite rectangles inside GUI clips.
        Matrix4x4 artworkSpace=Matrix4x4.identity;

        readonly struct ArtworkScope : IDisposable
        {
            readonly SunfallGame game;
            readonly Matrix4x4 previous;
            public ArtworkScope(SunfallGame game,Vector2 center,float angle,int facing)
            {
                this.game=game;previous=game.artworkSpace;
                game.artworkSpace=previous*Matrix4x4.TRS(new Vector3(center.x,center.y,0),
                    Quaternion.Euler(0,0,angle),new Vector3(facing<0?-1:1,1,1));
            }
            public void Dispose(){game.artworkSpace=previous;}
        }

        ArtworkScope ArtworkAt(Vector2 center,float angle=0,int facing=1)
        {return new ArtworkScope(this,center,angle,facing);}

        void DrawArtwork(Texture2D texture,Rect rect,Rect uv)
        {
            if(texture==null||rect.width<=0||rect.height<=0)return;
            Vector2 center=artworkSpace.MultiplyPoint3x4(rect.center);
            Vector2 right=artworkSpace.MultiplyVector(Vector3.right);
            Vector2 down=artworkSpace.MultiplyVector(Vector3.up);
            var size=new Vector2(rect.width*right.magnitude,rect.height*down.magnitude);
            float angle=Mathf.Atan2(right.y,right.x)*Mathf.Rad2Deg;
            float handedness=right.x*down.y-right.y*down.x<0?-1:1;
            var pivot=new Vector3(center.x,center.y,0);
            var matrix=GUI.matrix;
            try
            {
                // Keep the rectangle at its actual viewport position. Only its
                // orientation changes around that position; parent clips remain.
                GUI.matrix=matrix*Matrix4x4.Translate(pivot)*
                    Matrix4x4.Rotate(Quaternion.Euler(0,0,angle))*
                    Matrix4x4.Scale(new Vector3(1,handedness,1))*Matrix4x4.Translate(-pivot);
                GUI.DrawTextureWithTexCoords(new Rect(center-size*.5f,size),texture,uv,true);
            }
            finally {GUI.matrix=matrix;}
        }
    }
}
