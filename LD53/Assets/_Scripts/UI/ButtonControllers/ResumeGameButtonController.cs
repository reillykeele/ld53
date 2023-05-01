using Util.Systems;
using Util.UI.Controllers.Selectables.Buttons;

namespace LD53.UI.ButtonControllers
{
    public class ResumeGameButtonController : AButtonController
    {
        protected override void OnClick() => GameSystem.Instance.ResumeGame();
    }
}
