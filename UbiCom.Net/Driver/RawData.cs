namespace UbiCom.Net.Driver
{
    internal class RawData
    {
        internal byte[] Header { get; set; }
        internal byte[] Body { get; set; }
        internal int Length { get; set; }

        public RawData()
        {
            this.Header = null;
            this.Body = null;
            this.Length = 0;
        }
    }
}