﻿using System;
using System.Net;
using System.Runtime.InteropServices;

namespace ModbusTcp.Protocol.Request
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ModbusRequest16 : ModbusBase
    {
        public const int FixedLength = 7;

        public ModbusRequest16()
        {
            FunctionCode = 0x10;
            UnitIdentifier = 0xFF;
            Console.WriteLine($"Unit Id set to {UnitIdentifier} for Function Code 0x10.");
        }

        public ModbusRequest16(int offset, float[] values)
            : this()
        {

            ReferenceNumber = (short)offset;
            WordCount = (short)(values.Length * 2);
            RegisterValues = values.ToNetworkBytes();
            ByteCount = (byte)RegisterValues.Length;

            Header.Length = (short) (FixedLength + RegisterValues.Length);
        }

        [MarshalAs(UnmanagedType.U1)]
        public byte UnitIdentifier;

        [MarshalAs(UnmanagedType.U1)]
        public byte FunctionCode;

        [MarshalAs(UnmanagedType.U2)]
        public short ReferenceNumber;

        [MarshalAs(UnmanagedType.U2)]
        public short WordCount;

        [MarshalAs(UnmanagedType.U1)]
        public byte ByteCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] RegisterValues;

        public override byte[] ToNetworkBuffer()
        {
            var copy = (ModbusRequest16)MemberwiseClone();
            copy.Header = Header.Clone();

            copy.Header.Length = IPAddress.HostToNetworkOrder(Header.Length);
            copy.Header.ProtocolIdentifier = IPAddress.HostToNetworkOrder(Header.ProtocolIdentifier);
            copy.Header.TransactionIdentifier = IPAddress.HostToNetworkOrder(Header.TransactionIdentifier);

            copy.WordCount = IPAddress.HostToNetworkOrder(copy.WordCount);

            var buffer = copy.ToNetworkBytes();
            var outputBuffer = new byte[buffer.Length - 2 + RegisterValues.Length];
            Array.Copy(buffer, outputBuffer, buffer.Length - 2);
            Array.Copy(RegisterValues, 0, outputBuffer, buffer.Length - 2, RegisterValues.Length);

            return outputBuffer;
        }
    }
}
