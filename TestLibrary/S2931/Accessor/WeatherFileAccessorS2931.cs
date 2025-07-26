public class ResourceHolder : IDisposable
{
    private FileStream fs;
    public void OpenResource(string path)
    {
        this.fs = new FileStream(path, FileMode.Open);
    }

    public void CloseResource()
    {
        this.fs.Dispose();
    }

    public void Dispose()
    {
        CloseResource();
    }
}