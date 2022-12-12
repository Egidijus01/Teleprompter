



using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using static System.Math;

//var lines = ReadFrom("text.txt");
//foreach (var line in lines)
//{
//    Console.WriteLine(line);
//    if (!string.IsNullOrWhiteSpace(line))
//    {
//        var pause = Task.Delay(200);
//        pause.Wait();
//    }
//}

await RunTeleprompter();

 static async Task RunTeleprompter()
{
    var config = new TelePrompterConfig();
    var displayTask = ShowTeleprompter(config);

    var speedTask = GetInputed(config);
    await Task.WhenAny(displayTask, speedTask);
}





static IEnumerable<string> 
 ReadFrom(string file)
{
    var linesLength = 0;
    string? line;
    using (var reader = File.OpenText(file))
    {
        while ((line = reader.ReadLine()) != null)
        {
            var words = line.Split(' ');
            foreach (var word in words)
            {
                yield return word + " ";
                linesLength += word.Length + 1;
                if (linesLength > 70)
                {
                    yield return Environment.NewLine;
                    linesLength = 0;
                }
                
            }
            yield return Environment.NewLine;
        }
    }
}

static async Task ShowTeleprompter(TelePrompterConfig config)
{
    var words = ReadFrom("text.txt");
    foreach (var word in words)
    {
        Console.Write(word);
        if(!string.IsNullOrWhiteSpace(word))
        {
            await Task.Delay(config.DelayInMilliseconds);
        }
    }
    config.SetDone();
}

static async Task GetInputed(TelePrompterConfig config)
{
    var delay = 200;
    Action work = () =>
    {
        do
        {
            var key = Console.ReadKey(true);
            if (key.KeyChar == '>')
            {
                config.UpdateDelay(-10);
            }
            else if (key.KeyChar == '<')
            {
                config.UpdateDelay(10);
            }
            else if (key.KeyChar == 'x' || key.KeyChar == 'X')
            {
                config.SetDone();
            }
        } while (!config.Done);
    };
    await Task.Run(work);
}

internal class TelePrompterConfig
{
    public int DelayInMilliseconds { get; private set; } = 200;
    public void UpdateDelay(int increment) // negative to speed up
    {
        var newDelay = Min(DelayInMilliseconds + increment, 1000);
        newDelay = Max(newDelay, 20);
        DelayInMilliseconds = newDelay;
    }
    public bool Done { get; private set; }
    public void SetDone()
    {
        Done = true;
    }
}