using OmronFinsTCP.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PZIOT.Common.Omron
{
    public class OmronFinsTcpPlus
    {
        private TcpClient client;

        private NetworkStream stream;
        /// <summary>
        /// plc链接状态
        /// </summary>
        public bool IsConnected { get => client.Connected; }
        //
        // 摘要:
        //     PLC节点号
        public byte PlcNode { get; private set; }

        //
        // 摘要:
        //     PC节点号
        public byte PcNode { get; private set; }

        private short SendData(byte[] sd)
        {
            if (stream == null)
            {
                return -1;
            }

            try
            {
                stream.Write(sd, 0, sd.Length);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        private short ReceiveData(byte[] rd)
        {
            if (stream == null)
            {
                return -1;
            }

            try
            {
                int num = 0;
                do
                {
                    int num2 = stream.Read(rd, num, rd.Length - num);
                    if (num2 == 0)
                    {
                        return -1;
                    }

                    num += num2;
                }
                while (num < rd.Length);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        //
        // 摘要:
        //     Fins读写指令生成
        //
        // 参数:
        //   rw:
        //     读写类型
        //
        //   mr:
        //     寄存器类型
        //
        //   mt:
        //     地址类型
        //
        //   ch:
        //     起始地址
        //
        //   offset:
        //     位地址：00-15,字地址则为00
        //
        //   cnt:
        //     地址个数,按位读写只能是1
        private byte[] FinsCmd(RorW rw, PlcMemory mr, MemoryType mt, short ch, short offset, short cnt)
        {
            byte[] array = new byte[34]
            {
                70, 73, 78, 83, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0
            };
            if (rw == RorW.Read)
            {
                array[6] = 0;
                array[7] = 26;
            }
            else if (mt == MemoryType.Word)
            {
                array[6] = (byte)((cnt * 2 + 26) / 256);
                array[7] = (byte)((cnt * 2 + 26) % 256);
            }
            else
            {
                array[6] = 0;
                array[7] = 27;
            }

            array[8] = 0;
            array[9] = 0;
            array[10] = 0;
            array[11] = 2;
            array[12] = 0;
            array[13] = 0;
            array[14] = 0;
            array[15] = 0;
            array[16] = 128;
            array[17] = 0;
            array[18] = 2;
            array[19] = 0;
            array[20] = PlcNode;
            array[21] = 0;
            array[22] = 0;
            array[23] = PcNode;
            array[24] = 0;
            array[25] = byte.MaxValue;
            if (rw == RorW.Read)
            {
                array[26] = 1;
                array[27] = 1;
            }
            else
            {
                array[26] = 1;
                array[27] = 2;
            }

            array[28] = FinsClass.GetMemoryCode(mr, mt);
            array[29] = (byte)(ch / 256);
            array[30] = (byte)(ch % 256);
            array[31] = (byte)offset;
            array[32] = (byte)(cnt / 256);
            array[33] = (byte)(cnt % 256);
            return array;
        }

        //
        // 摘要:
        //     实例化PLC操作对象
        public OmronFinsTcpPlus()
        {
            client = new TcpClient();
        }

        //
        // 摘要:
        //     与PLC建立TCP连接
        //
        // 参数:
        //   rIP:
        //     PLC的IP地址
        //
        //   rPort:
        //     端口号，一般为9600
        //
        //   timeOut:
        //     超时时间，默认3000毫秒
        //
        // 返回结果:
        //     0为成功
        public short Link(string rIP, int rPort, short timeOut = 3000)
        {
            if (BasicClass.PingCheck(rIP, timeOut))
            {
                if (client != null)
                {

                    client.Dispose();
                }
                client = new TcpClient();
                client.Connect(rIP, rPort);
                stream = client.GetStream();
                Thread.Sleep(10);
                if (SendData(FinsClass.HandShake()) != 0)
                {
                    return -1;
                }

                byte[] array = new byte[24];
                if (ReceiveData(array) != 0)
                {
                    return -1;
                }

                if (array[15] != 0)
                {
                    return -1;
                }

                PcNode = array[19];
                PlcNode = array[23];
                return 0;
            }

            return -1;
        }

        //
        // 摘要:
        //     关闭PLC操作对象的TCP连接
        //
        // 返回结果:
        //     0为成功
        public short Close()
        {
            try
            {
                stream.Close();
                client.Close();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        //
        // 摘要:
        //     得到一个数据
        //
        // 参数:
        //   mrch:
        //     起始地址（地址：D100；位：W100.1）
        //
        // 类型参数:
        //   T:
        //     支持：int16,int32,bool,float
        //
        // 返回结果:
        //     结果值
        //
        // 异常:
        //   T:System.Exception:
        //     暂不支持此类型
        //
        //   T:System.Exception:
        //     读取数据失败
        public T GetData<T>(string mrch) where T : new()
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return GetData<T>(plcMemory, txtq);
        }

        //
        // 摘要:
        //     得到一个数据
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     起始地址（地址：100；位：100.01）
        //
        // 类型参数:
        //   T:
        //     支持：int16,int32,bool,float
        //
        // 返回结果:
        //     结果值
        //
        // 异常:
        //   T:System.Exception:
        //     暂不支持此类型
        //
        //   T:System.Exception:
        //     读取数据失败
        public T GetData<T>(PlcMemory mr, object ch) where T : new()
        {
            T val = new T();
            if (val is short)
            {
                if (ReadWord(mr, short.Parse(ch.ToString()), out var reData) == 0)
                {
                    return (T)(object)reData;
                }
            }
            else if (val is bool)
            {
                if (GetBitState(mr, ch.ToString(), out var bs) == 0)
                {
                    return (T)(object)(bs == 1);
                }
            }
            else if (val is int)
            {
                if (ReadInt32(mr, short.Parse(ch.ToString()), out var reData2) == 0)
                {
                    return (T)(object)reData2;
                }
            }
            else
            {
                if (!(val is float))
                {
                    throw new Exception("暂不支持此类型");
                }

                if (ReadReal(mr, short.Parse(ch.ToString()), out var reData3) == 0)
                {
                    return (T)(object)reData3;
                }
            }

            throw new Exception("读取数据失败");
        }

        //
        // 摘要:
        //     设置一个数据
        //
        // 参数:
        //   mrch:
        //     起始地址（地址：D100；位：W100.1）
        //
        //   inData:
        //     写入的数据
        //
        // 类型参数:
        //   T:
        //     支持：int16,int32,bool,float
        //
        // 返回结果:
        //     是否成功
        //
        // 异常:
        //   T:System.Exception:
        //     暂不支持此类型
        //
        //   T:System.Exception:
        //     写入数据失败
        public void SetData<T>(string mrch, T inData) where T : new()
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            SetData(plcMemory, txtq, inData);
        }

        //
        // 摘要:
        //     设置一个数据
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     起始地址（地址：100；位：100.01）
        //
        //   inData:
        //     写入的数据
        //
        // 类型参数:
        //   T:
        //     支持：int16,int32,bool,float
        //
        // 返回结果:
        //     是否成功
        //
        // 异常:
        //   T:System.Exception:
        //     暂不支持此类型
        //
        //   T:System.Exception:
        //     写入数据失败
        public void SetData<T>(PlcMemory mr, object ch, T inData) where T : new()
        {
            short num = -1;
            short inData2 = default(short);
            int num2;
            if (inData is short)
            {
                object obj = inData;
                inData2 = (short)((obj is short) ? obj : null);
                num2 = 1;
            }
            else
            {
                num2 = 0;
            }

            if (num2 != 0)
            {
                num = WriteWord(mr, short.Parse(ch.ToString()), inData2);
            }
            else
            {
                bool flag = default(bool);
                int num3;
                if (inData is bool)
                {
                    object obj2 = inData;
                    flag = (bool)((obj2 is bool) ? obj2 : null);
                    num3 = 1;
                }
                else
                {
                    num3 = 0;
                }

                if (num3 != 0)
                {
                    num = SetBitState(mr, ch.ToString(), flag ? BitState.ON : BitState.OFF);
                }
                else
                {
                    int reData = default(int);
                    int num4;
                    if (inData is int)
                    {
                        object obj3 = inData;
                        reData = (int)((obj3 is int) ? obj3 : null);
                        num4 = 1;
                    }
                    else
                    {
                        num4 = 0;
                    }

                    if (num4 != 0)
                    {
                        num = WriteInt32(mr, short.Parse(ch.ToString()), reData);
                    }
                    else
                    {
                        float reData2 = default(float);
                        int num5;
                        if (inData is float)
                        {
                            object obj4 = inData;
                            reData2 = (float)((obj4 is float) ? obj4 : null);
                            num5 = 1;
                        }
                        else
                        {
                            num5 = 0;
                        }

                        if (num5 == 0)
                        {
                            throw new Exception("暂不支持此类型");
                        }

                        num = WriteReal(mr, short.Parse(ch.ToString()), reData2);
                    }
                }
            }

            if (num != 0)
            {
                throw new Exception("写入数据失败");
            }
        }

        //
        // 摘要:
        //     得到多个数据
        //
        // 参数:
        //   mrch:
        //     起始地址（地址：D100；位：W100.1）
        //
        //   count:
        //     读取个数
        //
        // 类型参数:
        //   T:
        //     支持：int16,int32,bool,float
        //
        // 返回结果:
        //     结果值
        //
        // 异常:
        //   T:System.Exception:
        //     暂不支持此类型
        //
        //   T:System.Exception:
        //     读取数据失败
        public T[] GetDatas<T>(string mrch, int count) where T : new()
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return GetDatas<T>(plcMemory, txtq, count);
        }

        //
        // 摘要:
        //     得到多个数据
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     起始地址（地址：100；位：100.01）
        //
        //   count:
        //     读取个数
        //
        // 类型参数:
        //   T:
        //     支持：int16,int32,bool,float
        //
        // 返回结果:
        //     结果值
        //
        // 异常:
        //   T:System.Exception:
        //     暂不支持此类型
        //
        //   T:System.Exception:
        //     读取数据失败
        public T[] GetDatas<T>(PlcMemory mr, object ch, int count) where T : new()
        {
            T val = new T();
            if (val is short)
            {
                if (ReadWords(mr, short.Parse(ch.ToString()), Convert.ToInt16(count), out var reData) == 0)
                {
                    return (T[])(object)reData;
                }

                throw new Exception("读取数据失败");
            }

            if (val is bool)
            {
                T[] array = new T[count];
                short num = short.Parse(ch.ToString()!.Split('.')[0]);
                short num2 = short.Parse(ch.ToString()!.Split('.')[1]);
                for (int i = 0; i < array.Length; i++)
                {
                    if (GetBitState(mr, $"{num}.{num2}", out var bs) == 0)
                    {
                        array[i] = (T)(object)(bs == 1);
                        num2 = (short)(num2 + 1);
                        if (num2 > 15)
                        {
                            num = (short)(num + 1);
                            num2 = 0;
                        }

                        continue;
                    }

                    throw new Exception("读取数据失败");
                }

                return array;
            }

            if (val is int)
            {
                T[] array2 = new T[count];
                short num3 = short.Parse(ch.ToString());
                for (int j = 0; j < array2.Length; j++)
                {
                    if (ReadInt32(mr, Convert.ToInt16(num3 + j), out var reData2) == 0)
                    {
                        array2[j] = (T)(object)reData2;
                        continue;
                    }

                    throw new Exception("读取数据失败");
                }

                return array2;
            }

            if (val is float)
            {
                T[] array3 = new T[count];
                short num4 = short.Parse(ch.ToString());
                for (int k = 0; k < array3.Length; k++)
                {
                    if (ReadReal(mr, Convert.ToInt16(num4 + k), out var reData3) == 0)
                    {
                        array3[k] = (T)(object)reData3;
                        continue;
                    }

                    throw new Exception("读取数据失败");
                }

                return array3;
            }

            throw new Exception("暂不支持此类型");
        }

        //
        // 摘要:
        //     设置多个数据
        //
        // 参数:
        //   mrch:
        //     起始地址（地址：D100；位：W100.1）
        //
        //   inDatas:
        //     写入的数据
        //
        // 类型参数:
        //   T:
        //     支持：int16,
        //
        // 返回结果:
        //     是否成功
        //
        // 异常:
        //   T:System.Exception:
        //     暂不支持此类型
        //
        //   T:System.Exception:
        //     写入数据失败
        public void SetDatas<T>(string mrch, params T[] inDatas) where T : new()
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            SetDatas(plcMemory, txtq, inDatas);
        }

        //
        // 摘要:
        //     设置多个数据
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     起始地址（地址：100；位：100.01）
        //
        //   inDatas:
        //     写入的数据
        //
        // 类型参数:
        //   T:
        //     支持：int16,
        //
        // 返回结果:
        //     是否成功
        //
        // 异常:
        //   T:System.Exception:
        //     暂不支持此类型
        //
        //   T:System.Exception:
        //     写入数据失败
        public void SetDatas<T>(PlcMemory mr, object ch, params T[] inDatas) where T : new()
        {
            short num = -1;
            short[] array = inDatas as short[];
            if (array == null)
            {
                throw new Exception("暂不支持此类型");
            }

            if (WriteWords(mr, short.Parse(ch.ToString()), Convert.ToInt16(array.Length), array) != 0)
            {
                throw new Exception("写入数据失败");
            }
        }

        //
        // 摘要:
        //     读值方法（多个连续值）
        //
        // 参数:
        //   mrch:
        //     起始地址。如：D100,W100.1
        //
        //   cnt:
        //     地址个数
        //
        //   reData:
        //     返回值
        //
        // 返回结果:
        //     0为成功
        public short ReadWords(string mrch, short cnt, out short[] reData)
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return ReadWords(plcMemory, short.Parse(txtq), cnt, out reData);
        }

        //
        // 摘要:
        //     读值方法（多个连续值）
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     起始地址
        //
        //   cnt:
        //     地址个数
        //
        //   reData:
        //     返回值
        //
        // 返回结果:
        //     0为成功
        public short ReadWords(PlcMemory mr, short ch, short cnt, out short[] reData)
        {
            reData = new short[cnt];
            int num = 30 + cnt * 2;
            byte[] array = new byte[num];
            byte[] sd = FinsCmd(RorW.Read, mr, MemoryType.Word, ch, 0, cnt);
            if (SendData(sd) == 0)
            {
                if (ReceiveData(array) == 0)
                {
                    bool flag = true;
                    if (array[11] == 3)
                    {
                        flag = ErrorCode.CheckHeadError(array[15]);
                    }

                    if (flag)
                    {
                        if (ErrorCode.CheckEndCode(array[28], array[29]))
                        {
                            for (int i = 0; i < cnt; i++)
                            {
                                byte[] value = new byte[2]
                                {
                                    array[30 + i * 2 + 1],
                                    array[30 + i * 2]
                                };
                                reData[i] = BitConverter.ToInt16(value, 0);
                            }

                            return 0;
                        }

                        return -1;
                    }

                    return -1;
                }

                return -1;
            }

            return -1;
        }

        //
        // 摘要:
        //     读单个字方法
        //
        // 参数:
        //   mrch:
        //     起始地址。如：D100,W100.1
        //
        //   reData:
        //     返回值
        //
        // 返回结果:
        //     0为成功
        public short ReadWord(string mrch, out short reData)
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return ReadWord(plcMemory, short.Parse(txtq), out reData);
        }

        //
        // 摘要:
        //     读单个字方法
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     起始地址
        //
        //   reData:
        //     返回值
        //
        // 返回结果:
        //     0为成功
        public short ReadWord(PlcMemory mr, short ch, out short reData)
        {
            reData = 0;
            if (ReadWords(mr, ch, 1, out var reData2) != 0)
            {
                return -1;
            }

            reData = reData2[0];
            return 0;
        }

        //
        // 摘要:
        //     写值方法（多个连续值）
        //
        // 参数:
        //   mrch:
        //     起始地址。如：D100,W100.1
        //
        //   cnt:
        //     地址个数
        //
        //   inData:
        //     写入值
        //
        // 返回结果:
        //     0为成功
        public short WriteWords(string mrch, short cnt, short[] inData)
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return WriteWords(plcMemory, short.Parse(txtq), cnt, inData);
        }

        //
        // 摘要:
        //     写值方法（多个连续值）
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     起始地址
        //
        //   cnt:
        //     地址个数
        //
        //   inData:
        //     写入值
        //
        // 返回结果:
        //     0为成功
        public short WriteWords(PlcMemory mr, short ch, short cnt, short[] inData)
        {
            byte[] array = new byte[30];
            byte[] array2 = FinsCmd(RorW.Write, mr, MemoryType.Word, ch, 0, cnt);
            byte[] array3 = new byte[cnt * 2];
            for (int i = 0; i < cnt; i++)
            {
                byte[] bytes = BitConverter.GetBytes(inData[i]);
                array3[i * 2] = bytes[1];
                array3[i * 2 + 1] = bytes[0];
            }

            byte[] array4 = new byte[cnt * 2 + 34];
            array2.CopyTo(array4, 0);
            array3.CopyTo(array4, 34);
            if (SendData(array4) == 0)
            {
                if (ReceiveData(array) == 0)
                {
                    bool flag = true;
                    if (array[11] == 3)
                    {
                        flag = ErrorCode.CheckHeadError(array[15]);
                    }

                    if (flag)
                    {
                        if (ErrorCode.CheckEndCode(array[28], array[29]))
                        {
                            return 0;
                        }

                        return -1;
                    }

                    return -1;
                }

                return -1;
            }

            return -1;
        }

        //
        // 摘要:
        //     写单个字方法
        //
        // 参数:
        //   mrch:
        //     起始地址。如：D100,W100.1
        //
        //   inData:
        //     写入数据
        //
        // 返回结果:
        //     0为成功
        public short WriteWord(string mrch, short inData)
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return WriteWord(plcMemory, short.Parse(txtq), inData);
        }

        //
        // 摘要:
        //     写单个字方法
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     地址
        //
        //   inData:
        //     写入数据
        //
        // 返回结果:
        //     0为成功
        public short WriteWord(PlcMemory mr, short ch, short inData)
        {
            short[] inData2 = new short[1] { inData };
            if (WriteWords(mr, ch, 1, inData2) != 0)
            {
                return -1;
            }

            return 0;
        }

        //
        // 摘要:
        //     读值方法-按位bit（单个）
        //
        // 参数:
        //   mrch:
        //     起始地址。如：W100.1
        //
        //   bs:
        //     返回开关状态枚举EtherNetPLC.BitState，0/1
        //
        // 返回结果:
        //     0为成功
        public short GetBitState(string mrch, out short bs)
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return GetBitState(plcMemory, txtq, out bs);
        }

        //
        // 摘要:
        //     读值方法-按位bit（单个）
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     地址000.00
        //
        //   bs:
        //     返回开关状态枚举EtherNetPLC.BitState，0/1
        //
        // 返回结果:
        //     0为成功
        public short GetBitState(PlcMemory mr, string ch, out short bs)
        {
            bs = 0;
            byte[] array = new byte[31];
            short ch2 = short.Parse(ch.Split('.')[0]);
            short offset = short.Parse(ch.Split('.')[1]);
            byte[] sd = FinsCmd(RorW.Read, mr, MemoryType.Bit, ch2, offset, 1);
            if (SendData(sd) == 0)
            {
                if (ReceiveData(array) == 0)
                {
                    bool flag = true;
                    if (array[11] == 3)
                    {
                        flag = ErrorCode.CheckHeadError(array[15]);
                    }

                    if (flag)
                    {
                        if (ErrorCode.CheckEndCode(array[28], array[29]))
                        {
                            bs = array[30];
                            return 0;
                        }

                        return -1;
                    }

                    return -1;
                }

                return -1;
            }

            return -1;
        }

        //
        // 摘要:
        //     写值方法-按位bit（单个）
        //
        // 参数:
        //   mrch:
        //     起始地址。如：W100.1
        //
        //   bs:
        //     开关状态枚举EtherNetPLC.BitState，0/1
        //
        // 返回结果:
        //     0为成功
        public short SetBitState(string mrch, BitState bs)
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return SetBitState(plcMemory, txtq, bs);
        }

        //
        // 摘要:
        //     写值方法-按位bit（单个）
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     地址000.00
        //
        //   bs:
        //     开关状态枚举EtherNetPLC.BitState，0/1
        //
        // 返回结果:
        //     0为成功
        public short SetBitState(PlcMemory mr, string ch, BitState bs)
        {
            byte[] array = new byte[30];
            short ch2 = short.Parse(ch.Split('.')[0]);
            short offset = short.Parse(ch.Split('.')[1]);
            byte[] array2 = FinsCmd(RorW.Write, mr, MemoryType.Bit, ch2, offset, 1);
            byte[] array3 = new byte[35];
            array2.CopyTo(array3, 0);
            array3[34] = (byte)bs;
            if (SendData(array3) == 0)
            {
                if (ReceiveData(array) == 0)
                {
                    bool flag = true;
                    if (array[11] == 3)
                    {
                        flag = ErrorCode.CheckHeadError(array[15]);
                    }

                    if (flag)
                    {
                        if (ErrorCode.CheckEndCode(array[28], array[29]))
                        {
                            return 0;
                        }

                        return -1;
                    }

                    return -1;
                }

                return -1;
            }

            return -1;
        }

        //
        // 摘要:
        //     读一个浮点数的方法，单精度，在PLC中占两个字
        //
        // 参数:
        //   mrch:
        //     起始地址，会读取两个连续的地址，因为单精度在PLC中占两个字
        //
        //   reData:
        //     返回一个float型
        //
        // 返回结果:
        //     0为成功
        public short ReadReal(string mrch, out float reData)
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return ReadReal(plcMemory, short.Parse(txtq), out reData);
        }

        //
        // 摘要:
        //     读一个浮点数的方法，单精度，在PLC中占两个字
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     起始地址，会读取两个连续的地址，因为单精度在PLC中占两个字
        //
        //   reData:
        //     返回一个float型
        //
        // 返回结果:
        //     0为成功
        public short ReadReal(PlcMemory mr, short ch, out float reData)
        {
            reData = 0f;
            int num = 34;
            byte[] array = new byte[num];
            byte[] sd = FinsCmd(RorW.Read, mr, MemoryType.Word, ch, 0, 2);
            if (SendData(sd) == 0)
            {
                if (ReceiveData(array) == 0)
                {
                    bool flag = true;
                    if (array[11] == 3)
                    {
                        flag = ErrorCode.CheckHeadError(array[15]);
                    }

                    if (flag)
                    {
                        if (ErrorCode.CheckEndCode(array[28], array[29]))
                        {
                            byte[] value = new byte[4]
                            {
                                array[31],
                                array[30],
                                array[33],
                                array[32]
                            };
                            reData = BitConverter.ToSingle(value, 0);
                            return 0;
                        }

                        return -1;
                    }

                    return -1;
                }

                return -1;
            }

            return -1;
        }

        //
        // 摘要:
        //     写一个浮点数的方法，单精度，在PLC中占两个字
        //
        // 参数:
        //   mrch:
        //     起始地址，会读取两个连续的地址，因为单精度在PLC中占两个字
        //
        //   reData:
        //     返回一个float型
        //
        // 返回结果:
        //     0为成功
        public short WriteReal(string mrch, float reData)
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return WriteReal(plcMemory, short.Parse(txtq), reData);
        }

        //
        // 摘要:
        //     写一个浮点数的方法，单精度，在PLC中占两个字
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     起始地址，会读取两个连续的地址，因为单精度在PLC中占两个字
        //
        //   reData:
        //     返回一个float型
        //
        // 返回结果:
        //     0为成功
        public short WriteReal(PlcMemory mr, short ch, float reData)
        {
            byte[] bytes = BitConverter.GetBytes(reData);
            short[] array = new short[2];
            if (bytes != null)
            {
                array[0] = BitConverter.ToInt16(bytes, 0);
            }

            if (bytes.Length > 2)
            {
                array[1] = BitConverter.ToInt16(bytes, 2);
            }

            return WriteWords(mr, ch, 2, array);
        }

        //
        // 摘要:
        //     读一个int32的方法，在PLC中占两个字
        //
        // 参数:
        //   mrch:
        //     起始地址，会读取两个连续的地址，因为int32在PLC中占两个字
        //
        //   reData:
        //     返回一个int型
        //
        // 返回结果:
        //     0为成功
        public short ReadInt32(string mrch, out int reData)
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return ReadInt32(plcMemory, short.Parse(txtq), out reData);
        }

        //
        // 摘要:
        //     读一个int32的方法，在PLC中占两个字
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     起始地址，会读取两个连续的地址，因为int32在PLC中占两个字
        //
        //   reData:
        //     返回一个int型
        //
        // 返回结果:
        //     0为成功
        public short ReadInt32(PlcMemory mr, short ch, out int reData)
        {
            reData = 0;
            int num = 34;
            byte[] array = new byte[num];
            byte[] sd = FinsCmd(RorW.Read, mr, MemoryType.Word, ch, 0, 2);
            if (SendData(sd) == 0)
            {
                if (ReceiveData(array) == 0)
                {
                    bool flag = true;
                    if (array[11] == 3)
                    {
                        flag = ErrorCode.CheckHeadError(array[15]);
                    }

                    if (flag)
                    {
                        if (ErrorCode.CheckEndCode(array[28], array[29]))
                        {
                            byte[] value = new byte[4]
                            {
                                array[31],
                                array[30],
                                array[33],
                                array[32]
                            };
                            reData = BitConverter.ToInt32(value, 0);
                            return 0;
                        }

                        return -1;
                    }

                    return -1;
                }

                return -1;
            }

            return -1;
        }

        //
        // 摘要:
        //     写一个int32的方法，在PLC中占两个字
        //
        // 参数:
        //   mrch:
        //     起始地址，会读取两个连续的地址，因为int32在PLC中占两个字
        //
        //   reData:
        //     返回一个int型
        //
        // 返回结果:
        //     0为成功
        public short WriteInt32(string mrch, int reData)
        {
            string txtq;
            PlcMemory plcMemory = ConvertClass.GetPlcMemory(mrch, out txtq);
            return WriteInt32(plcMemory, short.Parse(txtq), reData);
        }

        //
        // 摘要:
        //     写一个int32的方法，在PLC中占两个字
        //
        // 参数:
        //   mr:
        //     地址类型枚举
        //
        //   ch:
        //     起始地址，会读取两个连续的地址，因为int32在PLC中占两个字
        //
        //   reData:
        //     返回一个int型
        //
        // 返回结果:
        //     0为成功
        public short WriteInt32(PlcMemory mr, short ch, int reData)
        {
            byte[] bytes = BitConverter.GetBytes(reData);
            short[] array = new short[2];
            if (bytes != null)
            {
                array[0] = BitConverter.ToInt16(bytes, 0);
            }

            if (bytes.Length > 2)
            {
                array[1] = BitConverter.ToInt16(bytes, 2);
            }

            return WriteWords(mr, ch, 2, array);
        }
    }
    internal class ConvertClass
    {
        //
        // 摘要:
        //     得到枚举值
        //
        // 参数:
        //   txt:
        //     如：D100,W100.1
        //
        //   txtq:
        public static PlcMemory GetPlcMemory(string txt, out string txtq)
        {
            PlcMemory plcMemory = PlcMemory.DM;
            char c = txt.Trim().ToUpper().FirstOrDefault();
            switch (c)
            {
                case 'D':
                    plcMemory = PlcMemory.DM;
                    break;
                case 'W':
                    plcMemory = PlcMemory.WR;
                    break;
                case 'H':
                    plcMemory = PlcMemory.HR;
                    break;
                case 'A':
                    plcMemory = PlcMemory.AR;
                    break;
                case 'C':
                case 'I':
                    plcMemory = PlcMemory.CIO;
                    break;
                default:
                    throw new Exception($"寄存器【{txt}】无效的前缀[{c}]");
            }

            txtq = Regex.Replace(txt, "[^0-9.]", "");
            return plcMemory;
        }


    }

    internal class ErrorCode
    {
        //
        // 摘要:
        //     （若返回的头指令为3）检查命令头中的错误代码
        //
        // 参数:
        //   Code:
        //     错误代码
        //
        // 返回结果:
        //     指示程序是否可以继续进行
        internal static bool CheckHeadError(byte Code)
        {
            return Code switch
            {
                0 => true,
                1 => false,
                2 => false,
                3 => false,
                _ => false,
            };
        }

        //
        // 摘要:
        //     检查命令帧中的EndCode
        //
        // 参数:
        //   Main:
        //     主码
        //
        //   Sub:
        //     副码
        //
        // 返回结果:
        //     指示程序是否可以继续进行
        internal static bool CheckEndCode(byte Main, byte Sub)
        {
            switch (Main)
            {
                case 0:
                    switch (Sub)
                    {
                        case 0:
                            return true;
                        case 64:
                            return true;
                        case 1:
                            return false;
                    }

                    break;
                case 1:
                    switch (Sub)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                        case 3:
                            return false;
                        case 4:
                            return false;
                        case 5:
                            return false;
                        case 6:
                            return false;
                    }

                    break;
                case 2:
                    switch (Sub)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                        case 3:
                            return false;
                        case 4:
                            return false;
                        case 5:
                            return false;
                    }

                    break;
                case 3:
                    switch (Sub)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                        case 3:
                            return false;
                        case 4:
                            return false;
                    }

                    break;
                case 4:
                    switch (Sub)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                    }

                    break;
                case 5:
                    switch (Sub)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                        case 3:
                            return false;
                        case 4:
                            return false;
                    }

                    break;
                case 16:
                    switch (Sub)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                        case 3:
                            return false;
                        case 4:
                            return false;
                        case 5:
                            return false;
                    }

                    break;
                case 17:
                    switch (Sub)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                        case 3:
                            return false;
                        case 4:
                            return false;
                        case 6:
                            return false;
                        case 9:
                            return false;
                        case 10:
                            return false;
                        case 11:
                            return false;
                        case 12:
                            return false;
                    }

                    break;
                case 32:
                    switch (Sub)
                    {
                        case 2:
                            return false;
                        case 3:
                            return false;
                        case 4:
                            return false;
                        case 5:
                            return false;
                        case 6:
                            return false;
                        case 7:
                            return false;
                    }

                    break;
                case 33:
                    switch (Sub)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                        case 3:
                            return false;
                        case 5:
                            return false;
                        case 6:
                            return false;
                        case 7:
                            return false;
                        case 8:
                            return false;
                    }

                    break;
                case 34:
                    switch (Sub)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                        case 3:
                            return false;
                        case 4:
                            return false;
                        case 5:
                            return false;
                        case 6:
                            return false;
                        case 7:
                            return false;
                        case 8:
                            return false;
                    }

                    break;
                case 35:
                    switch (Sub)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                        case 3:
                            return false;
                    }

                    break;
                case 36:
                    {
                        byte b5 = Sub;
                        byte b6 = b5;
                        if (b6 != 1)
                        {
                            break;
                        }

                        return false;
                    }
                case 37:
                    switch (Sub)
                    {
                        case 2:
                            return false;
                        case 3:
                            return false;
                        case 4:
                            return false;
                        case 5:
                            return false;
                        case 6:
                            return false;
                        case 7:
                            return false;
                        case 9:
                            return false;
                        case 10:
                            return false;
                        case 13:
                            return false;
                        case 15:
                            return false;
                        case 16:
                            return false;
                    }

                    break;
                case 38:
                    switch (Sub)
                    {
                        case 1:
                            return false;
                        case 2:
                            return false;
                        case 4:
                            return false;
                        case 5:
                            return false;
                        case 6:
                            return false;
                        case 7:
                            return false;
                        case 8:
                            return false;
                        case 9:
                            return false;
                        case 10:
                            return false;
                        case 11:
                            return false;
                    }

                    break;
                case 48:
                    {
                        byte b3 = Sub;
                        byte b4 = b3;
                        if (b4 != 1)
                        {
                            break;
                        }

                        return false;
                    }
                case 64:
                    {
                        byte b = Sub;
                        byte b2 = b;
                        if (b2 != 1)
                        {
                            break;
                        }

                        return false;
                    }
            }

            return false;
        }
    }
    internal class FinsClass
    {
        //
        // 摘要:
        //     获取内存区码
        //
        // 参数:
        //   mr:
        //     寄存器类型
        //
        //   mt:
        //     地址类型
        internal static byte GetMemoryCode(PlcMemory mr, MemoryType mt)
        {
            if (mt == MemoryType.Bit)
            {
                return mr switch
                {
                    PlcMemory.CIO => 48,
                    PlcMemory.WR => 49,
                    PlcMemory.HR => 50,
                    PlcMemory.AR => 51,
                    PlcMemory.DM => 2,
                    _ => 0,
                };
            }

            return mr switch
            {
                PlcMemory.CIO => 176,
                PlcMemory.WR => 177,
                PlcMemory.HR => 178,
                PlcMemory.AR => 179,
                PlcMemory.DM => 130,
                _ => 0,
            };
        }

        //
        // 摘要:
        //     PC请求连接的握手信号，第一次连接会分配PC节点号
        internal static byte[] HandShake()
        {
            return new byte[20]
            {
                70, 73, 78, 83, 0, 0, 0, 12, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };
        }
    }
    internal class BasicClass
    {
        internal static bool PingCheck(string ip, int timeOut)
        {
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(ip, timeOut);
            if (pingReply.Status == IPStatus.Success)
            {
                return true;
            }

            return false;
        }
    }
}
