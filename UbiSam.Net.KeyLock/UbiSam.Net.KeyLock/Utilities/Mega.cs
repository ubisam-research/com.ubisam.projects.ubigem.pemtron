using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace UbiSam.Net.KeyLock.Utilities
{
    public class Mega
    {
        #region [Properties]
        public bool Initialized { get; private set; }
        public bool Connected { get; protected set; }
        public int ID { get; private set; }
        public string IDAsString { get { return this.ID.ToString("X4"); } }
        public int SerialNumber { get; protected set; }
        public string SerialNumberAsString { get { return this.SerialNumber.ToString("X8"); } }
        public string ProductCode { get; private set; }
        #endregion
        #region [Member Variables]
        private readonly Mutex _mutex;
        private bool _mutexCatched;
        #endregion
        #region [Constructors]
        protected Mega()

        {
            this._mutexCatched = false;
            this._mutex = new Mutex(false, "UbiSam_Mega_C83C2824-F825-4380-BBF4-6C14FA94BAF3");
            this.Initialized = false;
            this.Connected = false;
            this.ProductCode = string.Empty;
        }
        ~Mega()
        {
            if (this._mutexCatched == true)
            {
                try
                {
                    this._mutex?.ReleaseMutex();
                }
                catch { }
            }
        }
        #endregion
        #region [Public Methods]

        public void Initialize()

        {
            this.Initialized = true;
        }

        public bool Connect()

        {
            bool result;

            result = false;

            try
            {
                this._mutexCatched = this._mutex.WaitOne();
            }
            catch { }

            if (this._mutexCatched == true)
            {
                this.ID = this.InitLock();

                if (this.ID == 0x86ec)
                {
                    this.Connected = true;

                    this.ReadSerialNumber();
                    this.ProductCode = this.ReadProduct();
                    result = true;
                }
                else
                {
                    this.ProductCode = string.Empty;
                }
            }

            if (this._mutexCatched == true)
            {
                this._mutex.ReleaseMutex();
            }

            return result;
        }

        public void Check()
        {
            try
            {
                this._mutexCatched = this._mutex.WaitOne();
            }
            catch { }

            if (this._mutexCatched == true)
            {
                int result = this.CheckLock();

                if (result != 0)
                {
                    this.Connected = false;
                    this.ProductCode = string.Empty;
                }
            }

            if (this._mutexCatched == true)
            {
                this._mutex.ReleaseMutex();
            }
        }

        public bool WriteProduct(string data)

        {
            System.Text.ASCIIEncoding ascii;
            byte[] bytes;
            byte[] buffer;
            int writed;
            bool result;

            result = false;

            if (data != null && data.Length < 58 && this.Connected == true)
            {
                buffer = new byte[58];
                Array.Clear(buffer, 0, buffer.Length);
                ascii = new System.Text.ASCIIEncoding();
                bytes = ascii.GetBytes(data);
                Array.Copy(bytes, 0, buffer, 0, bytes.Length);

                try
                {
                    this._mutexCatched = this._mutex.WaitOne();
                }
                catch { }

                if (this._mutexCatched == true)
                {
                    for (int i = 0; i < buffer.Length; ++i)
                    {
                        writed = WriteLock(i, buffer[i]);
                        result = buffer[i] == writed;

                        if (result == false)
                        {
                            break;
                        }
                    }
                }

                if (this._mutexCatched == true)
                {
                    this._mutex.ReleaseMutex();
                }
            }

            return result;
        }
        protected string ReadProduct()
        {
            System.Text.ASCIIEncoding ascii;
            byte[] buffer;
            int readed;
            string result;

            result = string.Empty;

            if (this.Connected == true)
            {
                buffer = new byte[58];
                Array.Clear(buffer, 0, buffer.Length);
                ascii = new System.Text.ASCIIEncoding();

                for (int i = 0; i < buffer.Length; ++i)
                {
                    readed = ReadLock(i);

                    if (readed < 1)
                    {
                        buffer[i] = 0x20;
                    }
                    else
                    {
                        buffer[i] = (byte)readed;
                    }
                }
                result = ascii.GetString(buffer).Trim();
            }

            return result;
        }
        #endregion
        #region [Protected Abstract Methods]

        protected virtual int InitLock()
        {
            return 0;
        }


        protected virtual int CheckLock()
        {
            return -1;
        }


        protected virtual void ReadSerialNumber()
        {
        }


        protected virtual int WriteLock(int address, int data)
        {
            return data - 1;
        }


        protected virtual int WriteLockEx(int address, int data)
        {
            return data - 1;
        }


        protected virtual int ReadLock(int address)
        {
            return -1;
        }


        protected virtual int ReadLockEx(int address)
        {
            return -1;
        }

        #endregion
        #region [Private Methods]
        private static Mega Construct(string dirPath, string fileName)
        {
            Mega result;
            Assembly assembly;
            Module[] modules;
            ConstructorInfo constructorInfo;
            Type searchedType;
            Type baseType;

            searchedType = null;
            result = null;

            if (System.IO.File.Exists($@"{dirPath}\{fileName}") == true)
            {
                assembly = Assembly.LoadFrom($@"{dirPath}\{fileName}");

                if (assembly != null)
                {
                    modules = assembly.GetModules();

                    foreach (Module module in modules)
                    {
                        if (searchedType == null)
                        {
                            foreach (Type type in module.GetTypes())
                            {
                                baseType = type.BaseType;

                                if (baseType != null && baseType == typeof(Mega))
                                {
                                    searchedType = type;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (searchedType != null)
            {
                constructorInfo = searchedType.GetConstructor(Type.EmptyTypes);

                if (constructorInfo != null)
                {
                    result = constructorInfo.Invoke(new object[] { }) as Mega;
                }
            }

            return result;
        }
        public static Mega Construct()
        {
            Mega result;
            string processType;
            string dirPath;
            string fileName;

            bool is64BitProcess = Environment.Is64BitProcess;

            processType = "86";

            if (is64BitProcess == true)
            {
                processType = "64";
            }

            dirPath = $@"{AppDomain.CurrentDomain.BaseDirectory}";
            fileName = $@"UbiSam.Net.KeyLock.Mega{processType}.dll";

            result = Construct(dirPath, fileName);

            if (result == null)
            {
                dirPath = $@"{Path.GetDirectoryName(typeof(Mega).Assembly.Location)}";
                result = Construct(dirPath, fileName);
            }

            if (result == null)
            {
                dirPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\UbiSam\UbiGEM\BIN";
                result = Construct(dirPath, fileName);
            }

            if (result == null)
            {
                dirPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}\UbiSam\UbiGEM\BIN";
                result = Construct(dirPath, fileName);
            }

            return result;
        }
        #endregion
    }
}
