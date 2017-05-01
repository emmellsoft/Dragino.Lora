using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Dragino.Gps.Messages;
using Dragino.Gps.Messages.Standard;

namespace Dragino.Gps
{
    internal class GpsMessageHandler : IDisposable
    {
        private readonly SerialDevice _serialDevice;
        private bool _isDisposed;

        public GpsMessageHandler(SerialDevice serialDevice)
        {
            _serialDevice = serialDevice;
            Task.Run(PerpetualGpsRead);
        }

        public void Dispose()
        {
            _isDisposed = true;
        }

        public event EventHandler<StandardGpsMessageEventArgs> OnStandardMessage;

        public event EventHandler<GpsMessageEventArgs> OnCustomMessage;

        public async Task SendMessage(GpsMessage message)
        {
            IBuffer txBuffer = GpsMessageSerializer.Serialize(message).AsBuffer();
            await _serialDevice.OutputStream.WriteAsync(txBuffer);
        }

        public async Task SendEmptyMessage()
        {
            IBuffer txBuffer = new[] { (byte)'\r', (byte)'\n' }.AsBuffer();
            await _serialDevice.OutputStream.WriteAsync(txBuffer);
        }

        private async Task PerpetualGpsRead()
        {
            const uint chunkLength = 100;

            var text = new StringBuilder();

            DateTime systemBeginReceiveMessageDateTime = DateTime.UtcNow;

            try
            {
                using (var dataReader = new DataReader(_serialDevice.InputStream))
                {
                    while (!_isDisposed)
                    {
                        uint bytesToRead = await dataReader.LoadAsync(chunkLength);
                        if (bytesToRead == 0)
                        {
                            continue;
                        }

                        var buffer = new byte[bytesToRead];

                        dataReader.ReadBytes(buffer);

                        for (int i = 0; i < bytesToRead; i++)
                        {
                            char c = (char)buffer[i];

                            if (c == '\n')
                            {
                                string packetText = text.ToString();
                                text.Clear();

                                //Debug.WriteLine($"Raw message: \"{packetText.Trim()}\"");

                                GpsMessage message;
                                if (GpsMessageSerializer.TryDeserialize(packetText, out message))
                                {
                                    StandardGpsMessage standardGpsMessage;
                                    if (StandardGpsMessageConverter.TryConvertToStandardGpsMessage(message,
                                        out standardGpsMessage))
                                    {
                                        OnStandardMessage?.Invoke(this, new StandardGpsMessageEventArgs(standardGpsMessage, systemBeginReceiveMessageDateTime));
                                    }
                                    else
                                    {
                                        OnCustomMessage?.Invoke(this, new GpsMessageEventArgs(message, systemBeginReceiveMessageDateTime));
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine($"Invalid GPS message: \"{packetText}\"");
                                }
                            }
                            else
                            {
                                if (text.Length == 0)
                                {
                                    systemBeginReceiveMessageDateTime = DateTime.UtcNow;
                                }

                                text.Append(c);
                            }
                        }
                    }
                }
            }
            catch
            {
                if (_isDisposed)
                {
                    return;
                }

                throw;
            }
            finally
            {
                _serialDevice.Dispose();
            }
        }
    }
}