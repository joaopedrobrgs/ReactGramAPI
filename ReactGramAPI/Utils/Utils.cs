using System.Threading;

namespace ReactGramAPI.Utils;

public class Utils
{

    private static SemaphoreSlim semaphore = new(1, 1);
    private static SemaphoreSlim semaphoreLog = new(1, 1);

    public static string GetPublicDir()
    {

        string dir = null,
            dirBase = AppDomain.CurrentDomain.BaseDirectory;

        if (dirBase.EndsWith(Path.DirectorySeparatorChar.ToString()))
            dirBase = dirBase.Substring(0, dirBase.Length - 1);
        else { };
#if DEBUG
        //if (IsGestaoNetCore())
        //    dir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(dirBase))), "ClientApp", "public", "data");
        //else
        try
        {
            dirBase = Path.GetDirectoryName(Path.GetDirectoryName(dirBase));
            if (dirBase.Contains("Utilities"))
                dir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(dirBase))), "data");
            else
                dir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(dirBase)), "data");
        }
        catch
        {
            dir = Path.Combine(dirBase, "data");
        }
#else
            //if (IsGestaoNetCore())
            //    dir = Path.Combine(dirBase, "ClientApp", "build", "data");
            //else
            //{
            if (dirBase.EndsWith("bin"))
                dir = Path.Combine(Path.GetDirectoryName(dirBase), "data");
            else
                dir = Path.Combine(dirBase, "data");
            //}
#endif

        if (!dir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            dir += Path.DirectorySeparatorChar.ToString();

        if (!Directory.Exists(dir))
        {
            semaphore.Wait();
            Directory.CreateDirectory(dir);
            semaphore.Release();
        }
        return dir;
    }
    public static string GetPublicDir(string rest)
    {

        string dir = null,
            dirBase = AppDomain.CurrentDomain.BaseDirectory;

        if (dirBase.EndsWith(Path.DirectorySeparatorChar.ToString()))
            dirBase = dirBase.Substring(0, dirBase.Length - 1);
        else { };
#if DEBUG
        //if (IsGestaoNetCore())
        //    dir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(dirBase))), "ClientApp", "public", "data", rest);
        //else
        try
        {
            dirBase = Path.GetDirectoryName(Path.GetDirectoryName(dirBase));
            if (dirBase.Contains("Utilities"))
                dir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(dirBase))), "data", rest);
            else
                dir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(dirBase)), "data", rest);
        }
        catch
        {
            dir = Path.Combine(dirBase, "data", rest);
        }
#else
            //if (IsGestaoNetCore())
            //    dir = Path.Combine(dirBase, "ClientApp", "build", "data", rest);
            //else
            //{
            if (dirBase.EndsWith("bin"))
                dir = Path.Combine(Path.GetDirectoryName(dirBase), "data", rest);
            else
                dir = Path.Combine(dirBase, "data", rest);
            //}
#endif

        if (!dir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            dir += Path.DirectorySeparatorChar.ToString();

        if (!Directory.Exists(dir))
        {
            semaphore.Wait();
            Directory.CreateDirectory(dir);
            semaphore.Release();
        }
        return dir;
    }
}
