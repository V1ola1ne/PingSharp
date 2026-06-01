using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Security.Cryptography;
using System.Net.Sockets;

namespace Ping
{
    internal class Pinger
    {
        private int Delay { get; set; }
        private int Amount { get; set; }
        private bool SendInfinite { get; set; }
        private bool ResolveIP { get; set; }
        private bool DontFragment { get; set; }
        private int TimeOut { get; set; }
        private int Length { get; set; }
        public IPAddress HostAddress { get; set; }
        public IPHostEntry? Host { get; set; }

        public Pinger(string Host, int? delay = 0, int? amount = 5, bool sendInfinite = false, bool resolveIP = false, bool dontFragment = false, int? timeOut = 1000, int? length = 32)
        {
            Delay = delay ?? 0;
            Amount = amount ?? 5;
            SendInfinite = sendInfinite;
            ResolveIP = resolveIP;
            DontFragment = dontFragment;
            TimeOut = timeOut ?? 1000;
            Length = length ?? 32;

            if (!IPAddress.IsValid(Host))
            {
                try
                {
                    this.Host = Dns.GetHostEntry(Host);
                    HostAddress = this.Host.AddressList[RandomNumberGenerator.GetInt32(this.Host.AddressList.Length)];
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                    Environment.Exit(1);
                }
            }
            else
            {
                HostAddress = IPAddress.Parse(Host);
                this.Host = null;
            }

            if (ResolveIP && this.Host is null)
            {
                this.Host = Dns.GetHostEntry(Host);
            }
        }

        public async Task ResolveIPAddress()
        {
            Host = await Dns.GetHostEntryAsync(HostAddress);
        }

        public async Task<PingReply[]> SendPing(bool Verbosity = false)
        {
            PingReply[] results = new PingReply[Amount];
            using System.Net.NetworkInformation.Ping p = new();

            PingOptions opt = new()
            {
                DontFragment = DontFragment,
            };

            byte[] buffer = new byte[Length];

            PopulateBuffer(buffer);
            

            if (!SendInfinite)
            {
                for (int i = 0; i < Amount; ++i)
                {
                    results[i] = await Send(p, buffer, opt, Verbosity);
                }
            }
            else
            {
                while (SendInfinite)
                {
                    await Send(p, buffer, opt,Verbosity);
                }
            }

            return results;
        }

        private async Task<PingReply> Send(System.Net.NetworkInformation.Ping p, byte[] buffer, PingOptions opt, bool Verbosity = false)
        {
            PingReply r = await p.SendPingAsync(HostAddress, TimeOut, buffer, opt);

            if (Verbosity)
            {
                EvaluateReply(r);
            }

            await Task.Delay(Delay);

            return r;
        }

        private static void EvaluateReply(PingReply r)
        {
            switch (r.Status)
            {
                case IPStatus.Success:
                    Console.WriteLine($"Reply from {r.Address}: Bytes={r.Buffer.Length} RTT={r.RoundtripTime} TTL={r.Options?.Ttl}");
                    break;
                case IPStatus.BadRoute:
                    Console.WriteLine("Invalide Route");
                    break;
                case IPStatus.DestinationHostUnreachable:
                    Console.WriteLine("Destination Host Unreachable");
                    break;
                case IPStatus.DestinationNetworkUnreachable:
                    Console.WriteLine("Destination Network Unreachable");
                    break;
                case IPStatus.DestinationPortUnreachable:
                    Console.WriteLine("Destination Port Unreachable");
                    break;
                case IPStatus.BadHeader:
                    Console.WriteLine("Invalid Header");
                    break;
                case IPStatus.BadDestination:
                    Console.WriteLine("Invalid Destination");
                    break;
                case IPStatus.BadOption:
                    Console.WriteLine("Invalid ICMP option");
                    break;
                case IPStatus.TimedOut:
                    Console.WriteLine("Request timed out");
                    break;
                case IPStatus.TtlExpired:
                    Console.WriteLine("TTL Expired");
                    break;
                case IPStatus.Unknown:
                    Console.WriteLine("Unknown Failure Reason");
                    break;
                case IPStatus.DestinationProhibited:
                    Console.WriteLine("Destination Prohibited");
                    break;
                case IPStatus.NoResources:
                    Console.WriteLine("Insufficient Network Resources");
                    break;
                case IPStatus.HardwareError:
                    Console.WriteLine("Hardware encountered an Error");
                    break;
                case IPStatus.PacketTooBig:
                    Console.WriteLine("Packet too big");
                    break;
                case IPStatus.TtlReassemblyTimeExceeded:
                    Console.WriteLine("TTL reassembly time exceeded");
                    break;
                case IPStatus.ParameterProblem:
                    Console.WriteLine("Parameter Problem");
                    break;
                case IPStatus.SourceQuench:
                    Console.WriteLine("Source Quench");
                    break;
                case IPStatus.DestinationUnreachable:
                    Console.WriteLine("Destination Unreachable");
                    break;
                case IPStatus.TimeExceeded:
                    Console.WriteLine("TTL Exceeded");
                    break;
                case IPStatus.UnrecognizedNextHeader:
                    Console.WriteLine("Invalid Next Header field");
                    break;
                case IPStatus.IcmpError:
                    Console.WriteLine("ICMP Protocol Error");
                    break;
                case IPStatus.DestinationScopeMismatch:
                    Console.WriteLine("Destination Scope does not match Source Scope");
                    break;
            }
        }

        private static void PopulateBuffer(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; ++i)
            {
                buffer[i] = 100;
            }
        }
    }
}
