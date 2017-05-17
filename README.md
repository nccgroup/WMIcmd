# Purpose
A small utility which only uses WMI to
* execute command shell commands
* capture stdout from these commands write to the registry
* read and then delete from the registry

# Design
The tool us comprised of:
- a very small subset of the NCC Group internal core library (WMICore)
- command execution (WMIcmd)

# Usage

```
C:\Data\NCC\!Code\Git.Public\WMIcmd\WMIcmd\bin\Debug>WMIcmd.exe --help
NCC Group WMIcmd 1.0.0.0
Released under AGPL

  -h, --host            Host (IP address or hostname - default: localhost)

  -u, --username        Username to authenticate with

  -p, --password        Password to authenticate with

  -d, --domain          Domain to authenticate with

  -v, --Verbose         (Default: False) Prints all messages to standard
                        output.

  -c, --Command         (Default: ) Command to run e.g. "nestat-ano"

  -s, --CommandSleep    (Default: 10000) Command sleep in milliseconds -
                        increase if getting truncated output

  --help                Display this help screen.
```

## Example - a non domain joined machine
Note: use administrative credentials

```
CDOHostGet.exe -h 192.168.1.165 -d hostname -u localadmin -p theirpassword
```

## Example - domain joined machine
Note: use administrative credentials

```
CDOHostGet.exe -h 192.168.1.165 -d domain -u domainadmin -p theirpassword
```

## Example expected output
Note: use administrative credentials

```
C:\Data\NCC\!Code\Git.Public\WMIcmd\WMIcmd\bin\Debug>WMIcmd.exe -d win10host -h win10host -u superuser -p password -c "netstat -an"
[!] Connecting with superuser
[i] Connecting to win10host
[i] Connected
[i] Command: netstat -an
[i] Running command...
[i] Getting stdout from registry from SOFTWARE\
[i] Full command output received
Active Connections
Proto  Local Address          Foreign Address        State
TCP    0.0.0.0:135            0.0.0.0:0              LISTENING
TCP    0.0.0.0:445            0.0.0.0:0              LISTENING
TCP    0.0.0.0:5357           0.0.0.0:0              LISTENING
TCP    0.0.0.0:5985           0.0.0.0:0              LISTENING
TCP    0.0.0.0:7680           0.0.0.0:0              LISTENING
TCP    0.0.0.0:18800          0.0.0.0:0              LISTENING
TCP    0.0.0.0:47001          0.0.0.0:0              LISTENING
TCP    0.0.0.0:49152          0.0.0.0:0              LISTENING
TCP    0.0.0.0:49153          0.0.0.0:0              LISTENING
TCP    0.0.0.0:49154          0.0.0.0:0              LISTENING
TCP    0.0.0.0:49664          0.0.0.0:0              LISTENING
TCP    0.0.0.0:49665          0.0.0.0:0              LISTENING
TCP    0.0.0.0:49666          0.0.0.0:0              LISTENING
TCP    0.0.0.0:49667          0.0.0.0:0              LISTENING
TCP    0.0.0.0:49668          0.0.0.0:0              LISTENING
TCP    0.0.0.0:49669          0.0.0.0:0              LISTENING
TCP    0.0.0.0:49671          0.0.0.0:0              LISTENING
TCP    0.0.0.0:49713          0.0.0.0:0              LISTENING
.. snip ..
```

