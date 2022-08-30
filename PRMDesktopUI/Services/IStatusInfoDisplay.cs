namespace PRMDesktopUI.Services
{
    public interface IStatusInfoDisplay
    {
        void ShowMessage(string message, string? header = "", string? title = "");
    }
}