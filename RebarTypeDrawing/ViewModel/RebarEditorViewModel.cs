using Mvvm;

namespace RebarTypeDrawing.ViewModel
{
    public class RebarEditorViewModel : BindableBase
    {
        public string SampleProp { get; set; }

        public RebarEditorViewModel()
        {
            SampleProp = "Test";
        }
    }
}
