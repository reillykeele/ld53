using Util.UI.Controllers.Selectables.Buttons;

namespace LD53.UI.ButtonControllers
{
    public class ReturnToPreviousButtonController : AButtonController
    {
        protected override void OnClick()
        {
            _canvasController.ReturnToPrevious();
        }
    }
}
