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
            // A visible torso can still engage when its feet or head cross the
            // viewport edge during a jump/camera return. Fully hidden enemies wait.
            return !hasCombatView||(enemy.X+.45f>=combatLeft&&enemy.X-.45f<=combatRight&&
                enemy.Y+enemy.BodyHeight>=combatBottom&&enemy.Y<=combatTop);
        }
    }
}
