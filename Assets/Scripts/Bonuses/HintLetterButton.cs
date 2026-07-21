namespace Bonuses
{
    public class HintLetterButton : AbstractHintButton
    {
        protected override void OnOn()
        {
            GridController.StartHintLetter();
        }

        protected override void OnOff()
        {
            GridController.StopHintLetter();
        }
    }
}