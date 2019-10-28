using System;
using System.Runtime.InteropServices;
using System.Text;

namespace testapp
{
    internal class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        private static bool is64BitProcess = (IntPtr.Size == 8);

        private static void Main(string[] args)
        {
            var dir = new StringBuilder(@"S:\Steam\steamapps\common\Arma 3");
            StringBuilder fileList;
            // var fileList = new Arma.FileList(@"S:\Steam\steamapps\common\Arma 3", recursive: true, calcMD5: false);
            fileList = Invoke(dir, Int32.MaxValue);
            Console.WriteLine(fileList.ToString().ToJson(true));
            Console.ReadLine();
        }

#if WIN64

        [DllImport(@"S:\Steam\steamapps\common\Arma 3\Mods\@Bluscream\Bluscream_x64.dll", CharSet = CharSet.Unicode)]
        private static extern StringBuilder RVExtension(StringBuilder input, int size);
#else

        [DllImport(@"S:\Steam\steamapps\common\Arma 3\Mods\@Bluscream\Bluscream.dll", CharSet = CharSet.Unicode)]
        private static extern StringBuilder Invoke(StringBuilder input, int size);
    }

#endif
}