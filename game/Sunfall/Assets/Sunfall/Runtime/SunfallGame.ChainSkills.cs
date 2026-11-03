using UnityEngine;
using Sunfall.Core;

namespace Sunfall
{
    public sealed partial class SunfallGame
    {
        void DrawPlayerChainSpell()
        {
            if(run.ChainEffectTimer<=0)return;
            float elapsed=.45f-run.ChainEffectTimer,alpha=Mathf.Clamp01(run.ChainEffectTimer/.15f);
            if(run.ChainEffectKind==SpellKind.ChainPull)
            {
                var hero=RenderPosition(run,run.X,run.Y);
                Vector2 from=new Vector2(SX(hero.x),SY(hero.y+1));
                Vector2 to=new Vector2(SX(run.ChainEndX),SY(run.ChainEffectY+1));
                if(run.ChainTarget!=null)
                {
                    var p=RenderPosition(run.ChainTarget,run.ChainTarget.X,run.ChainTarget.Y);
                    to=new Vector2(SX(p.x),SY(p.y+run.ChainTarget.BodyHeight*.5f));
                }
                float reach=Mathf.Clamp01(elapsed/.075f);
                to=Vector2.Lerp(from,to,1-(1-reach)*(1-reach));
                float bend=Mathf.Lerp(12,2,Mathf.Clamp01(elapsed/.18f));
                DrawAnimatedChain(from,to,alpha,bend,0,true);
                if(elapsed>=.075f)DrawAnimatedSpark(to,20*(1-Mathf.Clamp01((elapsed-.075f)/.18f)),new Color(1,.8f,.4f,alpha));
                return;
            }
            float ground=SY(run.ChainEffectY-.4f),top=SY(run.ChainEffectY+4);
            for(int i=0;i<7;i++)
            {
                float x=SX(run.ChainEffectX+(i-3)*(RunModel.ChainRainRadius/3));
                // Damage still releases immediately. The impact flash marks that
                // instant; visible links snap down, then recoil with a travelling bend.
                float local=Mathf.Max(0,elapsed-i*.006f),fall=Mathf.Clamp01(local/.065f);
                float recoil=Mathf.Clamp01((local-.14f)/.3f);
                float bottom=Mathf.Lerp(top,ground,1-(1-fall)*(1-fall))-recoil*recoil*45;
                DrawAnimatedChain(new Vector2(x,top),new Vector2(x,bottom),alpha,3+recoil*8,i*.13f,true);
                DrawAnimatedSpark(new Vector2(x,ground),28*(1-Mathf.Clamp01(elapsed/.22f)),new Color(1,.8f,.4f,alpha));
            }
            float left=SX(run.ChainEffectX-RunModel.ChainRainRadius),width=RunModel.ChainRainRadius*68;
            DrawSpellEffect(SpellKind.DawnBurst,new Vector2(left+width*.5f,ground),new Vector2(width,35),alpha*.6f);
        }
    }
}
