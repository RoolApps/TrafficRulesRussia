using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AppData
{
    internal static class ResourceRetriever
    {
        private const string ResourcesFileName = "AppResources.bin";
        private static Dictionary<string, object> resourcesDictionary = null;
        private static Dictionary<string, object> ResourcesDictionary
        {
            get
            {
                if(resourcesDictionary == null)
                {
                    var resourcesFileContent = ReadFile(ResourcesFileName);
                    resourcesDictionary = DeserializeDictionary(resourcesFileContent);
                }
                return resourcesDictionary;
            }
        }

        private static byte[] ReadFile(String fileName)
        {
            try
            {
                StorageFile file = ApplicationData.Current.LocalFolder.GetFileAsync(fileName).AsTask().Result;
                List<byte> bytes = new List<byte>();
                using(var stream = file.OpenStreamForReadAsync().Result)
                {
                    int readBytesCount = 0;
                    int bufferLength = 4096;
                    do
                    {
                        var buffer = new byte[bufferLength];
                        readBytesCount = stream.Read(buffer, 0, bufferLength);
                        bytes.AddRange(buffer.Take(readBytesCount));
                    } while (readBytesCount == bufferLength);
                }
                return bytes.ToArray();
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        private static void SaveFile(byte[] bytes, String fileName)
        {
            using(var stream = ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting).Result)
            {
                var bytesLength = bytes.Length;
                int bufferLength = 4096;
                int offset = 0;
                do
                {
                    var buffer = bytes.Skip(offset).Take(Math.Min(bytesLength, bufferLength + offset)).ToArray();
                    stream.Write(buffer, 0, buffer.Length);
                    offset += buffer.Length;
                } while (offset % bufferLength == 0);
            }
        }

        private static Dictionary<string, object> DeserializeDictionary(byte[] bytes)
        {
            if(bytes == null)
            {
                return new Dictionary<string, object>();
            }
            DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>));
            var dictionary = new Dictionary<string, object>();
            using(var memoryStream = new MemoryStream(bytes))
            {
                dictionary = (Dictionary<string, object>)serializer.ReadObject(memoryStream);
            }
            return dictionary;
        }

        private static byte[] SerializeDictionary(Dictionary<string, object> dictionary)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>));
            byte[] bytes;
            using(var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, dictionary);
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }

        internal static T GetResourceByName<T>(String name)
        {
            if (!ResourcesDictionary.ContainsKey(name))
            {
                return default(T);
            }
            else
            {
                return (T)ResourcesDictionary[name];
            }
        }

        internal static void SetResourceByName<T>(String name, T value)
        {
            if (ResourcesDictionary.ContainsKey(name))
            {
                ResourcesDictionary.Remove(name);
            }
            ResourcesDictionary.Add(name, value);
            var bytes = SerializeDictionary(ResourcesDictionary);
            SaveFile(bytes, ResourcesFileName);
        }
    }
}
