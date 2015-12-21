# FO.NET
## Error Handling

During the rendering process FO.NET may throw 3 types of event. The events are listed below in order of severity. 
* Info
* Warning
* Error

By default the Info and Warning events are written to standard output, whereas the Error event will generate a `System.SystemException`.

You may override this default behaviour by subscribing to any of the events via the `FonetDriver` class. Some examples are shown below 

**Code Fragment - Log the Info event to disk**
```
void Example()
{
    FonetDriver driver = FonetDriver.Make();
    driver.OnInfo += new FonetDriver.FonetEventHandler(FonetInfo);
    // ...
}

public void FonetInfo(object Driver, FonetEventArgs e)
{
    // Log message to disk
    logTextWriter.WriteLine("[INFO] " + e.GetMessage());
}
```

**Code Fragment - Log to Windows Event Service**

```
void Example()
{
    FonetDriver driver = FonetDriver.Make();
    driver.OnError += new FonetDriver.FonetEventHandler(FonetError);
    // ...
}

public void FonetError(object Driver, FonetEventArgs e)
{
    // Log to Windows Event Service

    const string LogName = "Fonet";
    if (!EventLog.SourceExists(LogName)) {
        EventLog.CreateEventSource(LogName, LogName);
    }

    // Insert into Event Log
    EventLog Log = new EventLog();
    Log.Source = LogName;
    Log.WriteEntry(e.GetMessage(), EventLogEntryType.Error);
}
```
