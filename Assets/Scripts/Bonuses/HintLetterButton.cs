namespace Bonuses
{
    public class HintLetterButton : AbstractBonusButton
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