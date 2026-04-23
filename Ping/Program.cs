using Ping;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;


StringBuilder sb = new();

if (args.Length == 0)
{
    sb.Append(Console.ReadLine());
}
else
{
    sb.Append(args[0]);
}

string HostString = sb.ToString();

Pinger p = new(HostString, GetDelay(args), GetAmount(args), GetSI(args), GetResolveIP(args));

if (p.Host is null)
{
    Console.WriteLine($"Pinging {p.HostAddress}:");
}
else
{
    Console.WriteLine($"Pinging {p.HostAddress}[{p.Host?.HostName}]");
}

List<PingReply> replies = await p.SendPing(true);

Console.WriteLine($"\nSend = {replies.Count}\tReceived = {replies.Where(x => x.Status == IPStatus.Success).Count()}\tLost = {replies.Where(x => x.Status != IPStatus.Success).Count()}");
Console.WriteLine($"Max RTT = {replies.Max(x => x.RoundtripTime)}\tMin RTT = {replies.Min(x => x.RoundtripTime)}\tAverage RTT = {GetAverageRTT(replies)}");

static long GetAverageRTT(List<PingReply> replies)
{
    long avg = 0;

    for (int i = 0; i < replies.Count; i++)
    {
        avg += replies[i].RoundtripTime;
    }

    return avg / replies.Count;
}


static bool GetSI(string[] args)
{
    return args.Contains("t");
}

static bool GetResolveIP(string[] args)
{
    return args.Contains("a");
}

static int? GetDelay(string[] args)
{
    if (args.Contains("d"))
    {
        return int.Parse(args[args.IndexOf("d") + 1]);
    }
    return null;
}

static int? GetAmount(string[] args)
{
    if (args.Contains("n"))
    {
        return int.Parse(args[args.IndexOf("n") + 1]);
    }
    return null;
}