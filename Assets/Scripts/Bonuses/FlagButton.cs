namespace Bonuses
{
    public class FlagButton : AbstractBonusButton
    {
        protected override void OnOn()
        {
            GridController.StartFlag();
        }

        protected override void OnOff()
        {
            GridController.StopFlag();
        }
    }
}