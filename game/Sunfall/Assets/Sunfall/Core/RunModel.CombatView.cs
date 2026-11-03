namespace Sunfall.Core
{
    public sealed partial class RunModel
    {
        bool hasCombatView;
        float combatLeft,combatRight,combatBottom,combatTop;
        // The renderer supplies its current visible world bounds, not the camera
        // target. Headless callers keep the original distance-based encounters.
        public void SetCombatView(float left,float right,float bottom,float top)
        {
            if(float.IsNaN(left)||float.IsNaN(right)||float.IsNaN(bottom)||float.IsNaN(top)||
               float.IsInfinity(left)||float.IsInfinity(right)||float.IsInfinity(bottom)||float.IsInfinity(top)||
               right<=left||top<=bottom)return;
            hasCombatView=true;combatLeft=left;combatRight=right;combatBottom=bottom;combatTop=top;
        }
        bool InCombatView(Enemy enemy)
        {
            return !hasCombatView||(enemy.X>=combatLeft&&enemy.X<=combatRight&&
                enemy.Y>=combatBottom&&enemy.Y+enemy.BodyHeight<=combatTop);
        }
    }
}
