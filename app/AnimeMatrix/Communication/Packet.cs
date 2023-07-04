// Source thanks to https://github.com/vddCore/Starlight :)

namespace GHelper.AnimeMatrix.Communication
{
    public abstract class Packet
    {
        private int _currentDataIndex = 1;

        public byte[] Data { get; }

        internal Packet(byte reportId, int packetLength, params byte[] data)
        {
            if (packetLength < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(packetLength),
                    "Packet length must be at least 1."
                );
            }

            Data = new byte[packetLength];
            Data[0] = reportId;

            if (data.Length > 0)
            {
                if (_currentDataIndex >= Data.Length)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(data),
                        "Your packet length does not allow for initial data to be appended."
                    );
                }

                AppendData(data);
            }
        }

        public Packet AppendData(params byte[] data)
            => AppendData(out _, data);

        public Packet AppendData(out int bytesWritten, params byte[] data)
        {
            bytesWritten = 0;

            for (var i = 0;
                 i < data.Length && _currentDataIndex < Data.Length - 1;
                 i++, bytesWritten++, _currentDataIndex++)
            {
                if (_currentDataIndex > Data.Length - 1)
                    break;

                Data[_currentDataIndex] = data[i];
            }

            return this;
        }
    }
}