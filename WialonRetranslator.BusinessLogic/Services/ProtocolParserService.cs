using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WialonRetranslator.Core.Interfaces;
using WialonRetranslator.Core.Models;
using WialonRetranslator.DataAccess.Repository;

namespace WialonRetranslator.BusinessLogic.Services
{
    public class ProtocolParserService : IProtocolParserService
    {
        private readonly IPointRepository _pointRepository;
        private int _port;
        private string _message;
        
        public ProtocolParserService(IPointRepository pointRepository)
        {
            _pointRepository = pointRepository;
            Console.WriteLine("Введите номер порта для запуска службы TCP слушателя сообщений под протоколом Wialon Retranslator v1.0.");
            while (!int.TryParse(Console.ReadLine(), out _port) || _port > 65536 || _port <= 0)
            {
                Console.WriteLine("Номер порта должен быть числом от 1 до 65536, а также порт должен быть свободен.");
            }
        }

        public void StartTCPServer()
        {
            TcpListener server=null;
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, _port);

                server.Start();

                Byte[] bytes = new Byte[256];

                while(true)
                {
                    Console.Write("Waiting for a connection... ");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    NetworkStream stream = client.GetStream();

                    int i;

                    while((i = stream.Read(bytes, 0, bytes.Length))!=0)
                    {
                        _message = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        SaveMessage();
                        
                        
                        var ok = "0x11";
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(ok);

                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", ok);
                    }

                    client.Close();
                }
            }
            catch(SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
        }

        private void SaveMessage()
        {
            var len = ConvertText(8, ref _message, "int");
            var id = ConvertText(32, ref _message, "string");
            var timeStamp = Int64.Parse(ConvertText(8, ref _message, "int", true));
            var time = UnixTimeStampToDateTime(timeStamp).ToUniversalTime();

            var bitMask = ConvertText(8, ref _message, "int", true);
            
            var typeBlock1 = ConvertText(4, ref _message, "block");
            var lenBlock1 = ConvertText(8, ref _message, "int", true);
            var attrHide1 = ConvertText(2, ref _message, "block");
            var typeData1 = ConvertText(2, ref _message, "block");
            
            var nameBlock = ConvertText(16, ref _message, "string");
            Dictionary<string,string> blockData = GetBlockData(nameBlock);

            var newPoint = new Point()
            {
                PointTime = time,
                DeviceId = id,
                Latitude = blockData["latitude"],
                Longtitude = blockData["longitude"]
            };

            if (newPoint.Latitude == null || newPoint.Longtitude == null || newPoint.DeviceId == null )
            {
                throw new ArgumentNullException("newpoint object is incorrect");
            }
            
            _pointRepository.Create(newPoint);
        }
        private Dictionary<string,string> GetPosInfo()
        {
            var longitude  = ConvertText(16, ref _message, "double");
            var latitude  = ConvertText(16, ref _message, "double");
            var altitude = ConvertText(16, ref _message, "double");
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("longitude", longitude);
            result.Add("latitude", latitude);
            result.Add("altitude ", altitude);
            return result;
        }

        private Dictionary<string,string> GetBlockData(string nameBlock)
        {
            Dictionary<string, string> result = null;
            switch (nameBlock)
            {
                case "posinfo ":
                    result = GetPosInfo();
                    break;
                case "pwr_ext":
                    break;
                case "avl_inputs":
                    break;
                case "imag":
                    break;
                case "avl_outputs":
                    break;
                case "avl_driver":
                    break;
                case "pwr_int":
                    break;
                default:
                    break;
            }

            return result;
        }
        private string ConvertText(int len, ref string text, string type, bool isLittleEndian = false)
        {
            string buffer = "";
            string result = null;
            for (int i = 0; i < len; i++)
            {
                buffer = buffer + text[i];    
            }
            text = text.Substring(len);

            switch (type)
            {
                case "int":
                    result = FromHexStringToInt(buffer, isLittleEndian).ToString();
                    break;
                case "string":
                    result = FromHexStringToString(buffer);
                    break;
                case "block":
                    result = Convert.ToInt32(buffer, 16).ToString();
                    break;
                case "double":
                    result = FromHexStringToDouble(buffer, isLittleEndian).ToString();
                    break;
                
            }

            return result;
        }
        
        private string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        private string FromHexStringToString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            //return BitConverter.ToString(bytes);
            return Encoding.ASCII.GetString(bytes);

        }
        private int FromHexStringToInt(string hexString, bool isLittleEndian)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            if (isLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt32(bytes, 0);
        }
        private double FromHexStringToDouble(string hexString, bool isLittleEndian)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            if (isLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToDouble(bytes, 0);
        }
        
        private DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dtDateTime;
        }
    }
}