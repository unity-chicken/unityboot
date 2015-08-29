using UnityEngine;
using System.Collections;

public enum LogLevel {
    Debug,
    Info,
    Warning,
    Error
};

public class Logger {
    static LogLevel logLevelForSendingServer = LogLevel.Debug;
    public static bool globalSendLogSwitch = true;
    public bool forceSendToLogServer = false;
    public delegate void OnSendLog(string msg, LogLevel logLevel);
    public static event OnSendLog onSendLog = null;

    string _prefix = "";
    bool useTime = false;
    string prefix {
        get {
            if (useTime == false) {
                return _prefix;
            }

            return _prefix + string.Format("{0:F2} ", Time.realtimeSinceStartup);
        }
    }


    public Logger() {
    }

    public Logger(string prefix, bool useTime = false) {
        this._prefix = prefix + " ";
        this.useTime = useTime;
    }

    public void SetPrefix(string prefix) {
        _prefix = prefix;
    }

    public void Log(object message) {
        Log(prefix + ObjectToString(message), LogLevel.Debug);
    }

    public void Log(string format, params Object[] args) {
        string message = GetFormatString(format, args);
        Log(prefix + message, LogLevel.Debug);
    }

    public void LogInfo(object message) {
        Log(prefix + ObjectToString(message), LogLevel.Info);
    }

    public void LogInfo(string format, params Object[] args) {
        string message = GetFormatString(format, args);
        Log(prefix + message, LogLevel.Info);
    }

    public void LogWarning(object message) {
        Log(prefix + ObjectToString(message), LogLevel.Warning);
    }

    public void LogWarning(string format, params Object[] args) {
        string message = GetFormatString(format, args);
        Log(prefix + message, LogLevel.Warning);
    }

    public void LogError(object message) {
        Log(prefix + ObjectToString(message), LogLevel.Error);
    }

    public void LogError(string format, params Object[] args) {
        string message = GetFormatString(format, args);
        Log(prefix + message, LogLevel.Error);
    }

    void Log(string msg, LogLevel logLevel) {
        if (logLevel == LogLevel.Debug) {
            Debug.Log(msg);
        } else if (logLevel == LogLevel.Info) {
            Debug.LogWarning(msg);
        } else if (logLevel == LogLevel.Warning) {
            Debug.LogWarning(msg);
        } else if (logLevel == LogLevel.Error) {
            Debug.LogError(msg);
        }

        if (logLevel >= logLevelForSendingServer) {
            if (onSendLog != null) {
                onSendLog(msg, logLevel);
            }
        }
    }

    string ObjectToString(object message) {
        string text = "";
        if (message != null) {
            text = message.ToString();
        }
        return text;
    }

    string GetFormatString(string format, params Object[] args) {
        if (format == null) {
            return "";
        }

        if (args.Length == 0) {
            return format;
        }

        return string.Format(format, args);
    }
}
