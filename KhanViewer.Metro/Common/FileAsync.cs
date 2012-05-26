using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace KhanViewer.Common
{
    /// <summary>
    /// this class found @ http://www.stevesaxon.me/post/2011/windows8-async-file-access/
    /// </summary>
    public static class FileAsync
    {
        public static async void Read(StorageFolder folder, string fileName, Action<uint, DataReader> reader)
        {
            var file = await folder.GetFileAsync(fileName);
            var stream = await file.OpenAsync(FileAccessMode.Read);
            var inputstream = stream.GetInputStreamAt(0);

            uint fileSize = (uint)stream.Size;

            var rdr = new DataReader(inputstream);
            rdr.LoadAsync(fileSize);

            reader(fileSize, rdr);
        }

        public static async void Write(StorageFolder folder, string fileName, CreationCollisionOption collisionOption, Action<DataWriter> writer, Action<bool> complete = null)
        {
            var file = await folder.CreateFileAsync(fileName);
            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            var outStream = stream.GetOutputStreamAt(0);

            DataWriter datawriter = new DataWriter(outStream);

            writer(datawriter);

            await datawriter.StoreAsync();
            bool result = await outStream.FlushAsync();

            if (complete != null) complete(result);
        }
    }
}
