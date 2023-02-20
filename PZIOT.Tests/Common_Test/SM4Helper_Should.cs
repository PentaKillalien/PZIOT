
using Newtonsoft.Json;
using PZIOT.Common.Helper.SM;
using PZIOT.Model.PZIOTModels;
using System;
using Xunit;

namespace PZIOT.Tests.Common_Test
{
    public class SM4Helper_Should
    {

        [Fact]
        public void Encrypt_ECB_Test()
        {
            var plainText = "暗号";

            var sm4 = new SM4Helper();

            Console.Out.WriteLine("ECB模式");
            var cipherText = sm4.Encrypt_ECB(plainText);
            Console.Out.WriteLine("密文: " + cipherText);

            Assert.NotNull(cipherText);
            Assert.Equal("VhVDC0KzyZjAVMpwz0GyQA==", cipherText);
        }

        [Fact]
        public void Decrypt_ECB_Test()
        {
            var cipherText = "Y9ygWexdpuLQjW/qsnZNQw==";

            var sm4 = new SM4Helper();

            Console.Out.WriteLine("ECB模式");
            var plainText = sm4.Decrypt_ECB(cipherText);
            Console.Out.WriteLine("明文: " + plainText);

            Assert.NotNull(plainText);
            Assert.Equal("狗蛋啊狗蛋", plainText);
        }
        [Fact]
        public void EquipmentDriver_Test()
        {
            TcpClientConnectionModel c = new TcpClientConnectionModel()
            {
                Port = 1922,
                Serverip = "192.168.0.16",
                TimeOut = 2000
            };
            string s = JsonConvert.SerializeObject(c);
            Console.WriteLine(s);
            Assert.NotNull(s);
            //Assert.Equal("192", s);
        }
        [Fact]
        public void EquipmentDriver_Test2()
        {
            ModbusMasterModel c = new ModbusMasterModel()
            {
                Address="192.168.88.160",
                 Port=1988,
                  Timeout=1000,
                   ReadTimeout=0,
                    Retries=0,
                     WriteTimeout=0
            };
            string s = JsonConvert.SerializeObject(c);
            Console.WriteLine(s);
            Assert.NotNull(s);
            //Assert.Equal("192", s);
        }
        [Fact]
        public void EquipmentDriver_Test3()
        {
            S7NetModel c = new S7NetModel()
            {
                Address = "10.0.48.7",
                Rack=1,
                Slot=0,
                PlcType="S7300"
            };
            string s = JsonConvert.SerializeObject(c);
            Console.WriteLine(s);
            Assert.NotNull(s);
            //Assert.Equal("192", s);
        }
    }
}
