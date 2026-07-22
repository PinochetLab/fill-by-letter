namespace Bonuses
{
    public class HintWordButton : AbstractHintButton
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