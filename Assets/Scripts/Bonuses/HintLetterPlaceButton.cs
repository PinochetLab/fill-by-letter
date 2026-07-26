namespace Bonuses
{
    public class HintLetterPlaceButton : AbstractBonusButton
    {
        protected override void OnOn()
        {
            GridController.StartHintLetterPlace();
        }

        protected override void OnOff()
        {
            GridController.StopHintLetterPlace();
        }
    }
}