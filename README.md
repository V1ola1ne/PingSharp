# Ping
This is a C# implementation of ping.exe from Windows. <br>
functionality is not quite the same yet, but execution is already "faster".<br>
In the sense, that you do not have to wait a second after each ping.<br>
## Implemented Fuctionality
- `t` => Equivalent to `/t`. Allowing infinite execution
- `l <size>` => Equivalent to `/l <size>`. Specify the Buffer Length
- `n <amount>` => Equivalent to `/n <amount>`. Specify the Amount of pings to an address.
- `w <timeout>` => Equivalent to `/w <timeout>`. Specify the ping time out.
- `a` => Equivalent to `/a`. Resolve a specified IP-Address to its DNS Name. Output will be displayed
### Additional Fuctionality
#### Delay
Set the delay between pings by specifying `d <delay>`. If not Specified. Delay will be 0 and pings will complete as fast as possible.
