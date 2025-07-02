using KlaarDesk.Plugins;
using Newtonsoft.Json;

namespace TestPlugin;

public class FakePlugin : IKlaarDeskChannelPlugin
{
    public string Name { get; } = "Fake Slack Plugin";
    private PluginChannelContext _context;

    public class TESTSTS
    {
        public string Name { get; set; } = "Test";
    }
    public void Initialise(PluginChannelContext context)
    {
        _context = context;
    }

    public Dictionary<string, string> ConfigurationFields()
    {
        return new();
    }

    public bool ValidateConfiguration()
    {
        return true;
    }

    public void TicketUpdated()
    {
        var x = new TESTSTS();
        Console.WriteLine("Serialising object to JSON:");
        var p = JsonConvert.SerializeObject(x);
        Console.WriteLine(p);
        File.WriteAllText("./test.json", p);
        var result = _context.TicketRepository.AddReply();
        Console.WriteLine(result);
    }

    public void LookForTicketsAndReplies()
    {
    }
}