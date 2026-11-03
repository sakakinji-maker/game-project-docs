using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        void DrawDawnBurst()
        {
            if(run.DawnTimer<=0)return;
            float alpha=Mathf.Clamp01(run.DawnTimer/.55f),radius=RunModel.DawnRadius*34;
            var center=new Vector2(SX(run.DawnX),SY(run.DawnY));
            DrawSpellEffect(SpellKind.DawnBurst,center,Vector2.one*(radius*2.25f),alpha);
        }
    }
}
