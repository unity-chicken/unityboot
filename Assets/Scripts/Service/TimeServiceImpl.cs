using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TimeServiceImpl : Singleton<TimeServiceImpl>, TimeService {
    public long GetUnixTime() {
        return UnixTimestampFromDateTime(DateTime.Now);
    }

    long UnixTimestampFromDateTime(DateTime date) {
        date = date.ToUniversalTime();
        long unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
        unixTimestamp /= TimeSpan.TicksPerSecond;
        return unixTimestamp;
    }
}

