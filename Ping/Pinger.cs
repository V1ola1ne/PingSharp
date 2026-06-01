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
                    Console.WriteLine("DestinationNetworkUnreachable");
                    break;
                case IPStatus.DestinationPortUnreachable:
                    Console.WriteLine("Destination Port Unreachable");
                    break;
                case IPStatus.BadHeader:
                    Console.WriteLine("Bad Header");
                    break;
                case IPStatus.BadDestination:
                    Console.WriteLine("Bad Destination");
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
