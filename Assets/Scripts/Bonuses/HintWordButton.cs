namespace Bonuses
{
    public class HintWordButton : AbstractBonusButton
    {
        protected override void OnOn()
        {
            GridController.StartHintWord();
        }

        protected override void OnOff()
        {
            GridController.StopHintWord();
        }
    }
}