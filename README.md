# Sherlock
Sherlock is a small library that provodes an asynchronous pipe for simple
cross-thread communication.  The pipe is uni-directional and intended for
communication between a single producer and consumer.  You can think of the
pipe as a one-to-one communication channel.

Sherlock is useful in decoupling a producer from a consumer, and it can be used
as a communication mechanism between subsystems in your application.

## Getting Started
We are still working on our initial version and so are not yet on NuGet - we'll
get to it soon! In the meantime, you must clone this repository and build
Sherlock manually.

Using Sherlock is easy!  The following examples demonstrate the basics of
Sherlock and should provide you with enough information to use it in your own
projects.

### Hello Pipe
This example is the "hello world" of Sherlock.  It simply creates a producer
thread and a consumer thread and demonstrates communication between them.

```csharp
class Program
{
    static void Produce(Sherlock.IPipeWriter<int> writer)
    {
        for (var i = 0; i < 5; i++)
            writer.Write(i);

        writer.Close();
    }

    static void Consume(Sherlock.IPipeReader<int> reader)
    {
        int item;
        while (reader.Read(out item))
            Console.WriteLine(item);
    }

    static void Main(string[] args)
    {
        var pipe = Sherlock.Pipe.Open<int>();

        var producer = new Thread(() => Produce(pipe.Writer));
        producer.Start();

        var consumer = new Thread(() => Consume(pipe.Reader));
        consumer.Start();

        producer.Join();
        consumer.Join();

        // Output:
        // 0
        // 1
        // 2
        // 3
        // 4
    }
}
```

### Introduction to Buffers
The previous example never blocks the producer thread, so if you produce many
values much faster than you consume them, you could run out of memory! In this
case, you need additional control over when your producer is blocked.  Sherlock
provides _buffers_ that describe the behavior of the data put in the pipe and 
when the producer is blocked.

The following example shows a sliding buffer, which drops old data when another
write would cause the pipe to contain too many values.

```csharp
class Program
{
    static void Produce(Sherlock.IPipeWriter<int> writer)
    {
        int item = 0;
        while (writer.Write(item))
            item++;

        writer.Close();
    }

    static void Consume(Sherlock.IPipeReader<int> reader)
    {
        int item;

        for (int i = 0; i < 5; i++)
        {
            reader.Read(out item);
            Console.WriteLine(item);
            Thread.Sleep(5);
        }

        reader.Close();
    }

    static void Main(string[] args)
    {
        var buffer = new Sherlock.SlidingBuffer<int>(10);
        var pipe = Sherlock.Pipe.Open<int>(buffer);

        var producer = new Thread(() => Produce(pipe.Writer));
        producer.Start();

        var consumer = new Thread(() => Consume(pipe.Reader));
        consumer.Start();

        producer.Join();
        consumer.Join();

        // Output similar to:
        // 0
        // 8310
        // 37212
        // 54573
        // 79371

    }
}
```

### Linking up with LINQ
So you use LINQ a lot, huh? We do too!  Sherlock provides a _PipedEnumerable_
to get you back to writing beautiful code super-quick.

```csharp
class Program
{
    static void Produce(Sherlock.IPipeWriter<int> writer)
    {
        for (int i = 0; i < 100; i++)
            writer.Write(i);

        writer.Close();
    }

    static void Main(string[] args)
    {
        var buffer = new Sherlock.BoundedBuffer<int>(10);
        var pipe = Sherlock.Pipe.Open<int>(buffer);

        var producer = new Thread(() => Produce(pipe.Writer));
        producer.Start();

        var sum = new Sherlock.PipedEnumerable<int>(pipe.Reader)
            .Take(10)
            .Aggregate((x, y) => x + y);

        Console.WriteLine(sum);

        // Output:
        // 45
    }
}
```

## Contributing
We welcome pull requests, feature proposals, and issue discussions.

* Fork the project and create a topic branch.
* Write tests that demonstrate the bug or new feature.
* Implement the bug fix or feature.
* Add a line to CHANGELOG.md describing your changes.
* Commit, push, and make a pull request.

Please always use topic branches.  We will not accept pull requests from the master branch.

## License
Sherlock is released under the [MIT License](http://opensource.org/licenses/MIT).