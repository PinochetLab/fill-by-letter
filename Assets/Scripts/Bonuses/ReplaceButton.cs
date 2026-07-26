namespace Bonuses
{
    public class ReplaceButton : AbstractBonusButton
    {
        protected override void OnOn()
        {
            GridController.StartReplace();
        }

        protected override void OnOff()
        {
            GridController.StopReplace();
        }
    }
}