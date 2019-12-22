using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Orbitality.SaveSystem
{
    public class FileSaveSystem : ISaveSystem
    {
        private string filePath;

        public FileSaveSystem(string filePath)
        {
            this.filePath = filePath;
        }
        
        public void Save<T>(T data) where T : class
        {
            using (FileStream writer = File.Create(filePath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(writer, data);
            }
        }

        public bool Load<T>(out T data) where T : class
        {
            data = null;

            if (File.Exists(filePath))
            {
                using (FileStream reader = File.Open(filePath, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    data = (T) bf.Deserialize(reader);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
