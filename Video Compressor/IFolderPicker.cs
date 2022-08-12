namespace Video_Compressor
{
    public interface IFolderPicker
    {
        Task<string> PickFolder();
    }
}