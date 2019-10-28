using Maca134.Arma.Serializer;
using System;

namespace testapp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var fileList = new Arma.FileList(@"S:\Steam\steamapps\common\Arma 3", recursive: true, calcMD5: false);
            Console.WriteLine(fileList.ToJson(true));
            Console.WriteLine(Converter.SerializeObject(fileList));
            Console.ReadLine();
        }
    }
}