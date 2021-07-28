using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System.IO;
using System.Linq;

[assembly: OwinStartup(typeof(Startup))]

public class Startup
{
    protected void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        };
        using (TextReader tr = new StreamReader(e.FullPath))
        {
            var text = tr.ReadLine() + " °C";
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.broadcastMessage("server", text);
            tr.Close();
        }    
    }

    public void Configuration(IAppBuilder app)
    {
        var watcher = new FileSystemWatcher(@"D:\Help")
        {
            NotifyFilter = NotifyFilters.Attributes
                            | NotifyFilters.CreationTime
                            | NotifyFilters.DirectoryName
                            | NotifyFilters.FileName
                            | NotifyFilters.LastAccess
                            | NotifyFilters.LastWrite
                            | NotifyFilters.Security
                            | NotifyFilters.Size
        };

        watcher.Changed += OnChanged;
        watcher.Filter = "H1.txt";

        watcher.EnableRaisingEvents = true;

        // Any connection or hub wire up and configuration should go here
        app.MapSignalR();
    }
}
