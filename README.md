# SJP.ProcessRedux

> Simple event-driven process handling for .NET

[![License (MIT)](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT) [![Build status](https://ci.appveyor.com/api/projects/status/rcc4nink97a0ya9d?svg=true)](https://ci.appveyor.com/project/sjp/processredux)

Avoid many of the complexities of working with `System.Diagnostics.Process` and make it easy to stream standard input, output and error from processes.

## Highlights

* Supports .NET Framework 4.5+, .NET Core 1.1+, .NET Standard 1.4+.
* Enables easy writing of *data* (i.e. `byte[]`) to and from standard input, output and error.
* Avoid some of the common mistakes of `System.Diagnostics.Process`, e.g. forgetting `RedirectStandardOutput = true` when trying to observe standard output.
* Tasks that are asynchronous in nature (e.g. waiting for a process to exit) are now async/await compatible.
* [Reactively](http://reactivex.io/) observe asynchronous streams of data or lines of text using `ObservableDataStreamingProcess` and `ObservableTextStreamingProcess`.

## Installation

**NOTE:** This project is still a work in progress. However, once ready, it will be available by the following methods.

```powershell
Install-Package SJP.ProcessRedux
```

or

```console
dotnet add package SJP.ProcessRedux
```

## Usage

A demonstration of `DataStreamingProcess` is to transcode a [FLAC](https://xiph.org/flac/) audio file to MP3 using [FFmpeg](https://www.ffmpeg.org/).

```csharp
const string flacInputPath = @"C:\tmp\test.flac";
const string mp3OutputPath = @"C:\tmp\test.mp3";
const string ffmpegPath = @"C:tmp\ffmpeg.exe";

var processConfig = new ProcessConfiguration(ffmpegPath)
{
    Arguments = $"-i \"{ flacInputPath }\" -b:a 192k -f mp3 -"
};

using (var writer = new BinaryWriter(File.OpenWrite(mp3OutputPath)))
using (var process = new DataStreamingProcess(processConfig))
{
    process.OutputDataReceived += (s, data) => writer.Write(data);
    process.Start();
    var exitCode = process.WaitForExit();
}
```

How is this better/easier than `System.Diagnostics.Process`? Firstly it assumes that all of the output is purely binary in nature, so events are processed when data is received and not when a line is received. Secondly, because `System.Diagnostics.Process` provides the output stream directly, and the events to read from a line, you are not provided with event-driven standard output/error processing. The built-in events assume textual output which would produce a malformed MP3 file if we were to use it in the example shown previously.

Another example is more straightforward. Consider a simple application that just prints `Hello, world!`, reading this is largely similar to `System.Diagnostics.Process`, but with less boilerplate.

```csharp
var helloWorldPath = @"C:\tmp\helloworld.exe"
var processConfig = new ProcessConfiguration(helloWorldPath);

var builder = new StringBuilder(); // to store results
using (var process = new TextStreamingProcess(processConfig))
{
    process.OutputLineReceived += (s, data) => builder.Append(data);
    process.Start();
    var exitCode = process.WaitForExit();
}

builder.ToString(); // "Hello, world!"
```

This shows how streaming textual output is simpler using `TextStreamingProcess`. We did not need to set `RedirectStandardOutput = true`, it is already knows to handle standard output. The one catch is that this event should be bound before the process starts in order to capture all of the output.

While it is true that `System.Diagnostics.Process` can inform you when the process has closed, it is easy to forget that `EnableRaisingEvents = true` must be set for its `Exited` event to be raised. Instead, ProcessRedux provides two ways in which you can be informed of a process exiting in an asynchronous manner. Lets run the same example as before but ignoring its output.

```csharp
var helloWorldPath = @"C:\tmp\helloworld.exe"
var processConfig = new ProcessConfiguration(helloWorldPath);

using (var process = new TextStreamingProcess(processConfig))
{
    process.Exited += (sender, exitCode)
        => Console.WriteLine($"Event: { helloWorldPath } exited with exit code { exitCode }.");
    process.Start();
    var exitCode = await process.WaitForExitAsync();
    Console.WriteLine($"Task: { helloWorldPath } exited with exit code { exitCode }.")
}
```

We can see how there are now two methods by which we can determine when a process has completed. This enables us to use whichever method is most convenient, with minimal boilerplate.

There are also [reactive](http://reactivex.io/) process handlers that provide subscriptions to streams of output from a process.

```csharp
TODO
```

## API

*TODO*