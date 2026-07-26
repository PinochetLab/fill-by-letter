namespace Bonuses
{
    public class EraseButton : AbstractBonusButton
    {
        protected override void OnOn()
        {
            GridController.StartErase();
        }

        protected override void OnOff()
        {
            GridController.StopErase();
        }
    }
}