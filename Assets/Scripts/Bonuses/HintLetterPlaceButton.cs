namespace Bonuses
{
    public class HintLetterPlaceButton : AbstractHintButton
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