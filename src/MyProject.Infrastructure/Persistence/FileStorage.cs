namespace MyProject.Infrastructure.Persistence;

public class FileStorage : IDisposable
{
    private StreamWriter? _writer;

    public void Open(string path)
    {
        _writer = new StreamWriter(path);
    }

    public void Write(string data)
    {
        _writer?.WriteLine(data);
    }

    public void Dispose()
    {
        _writer?.Dispose();
    }
}