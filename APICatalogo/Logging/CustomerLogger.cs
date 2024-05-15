namespace APICatalogo.Logging;

public class CustomerLogger : ILogger
{
    readonly string loggerName;
    readonly CustomLoggerProviderConfiguration loggerConfig;

    public CustomerLogger(string name, CustomLoggerProviderConfiguration config)
    {
        loggerName = name;
        loggerConfig = config;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == loggerConfig.LogLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string mesage = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state, exception)}";

        EscreverTextoNoArquivo(mesage);
    }

    public void EscreverTextoNoArquivo(string mesage)
    {
        string caminhoArquivoLog = @"c:\temp\.NET_Log.txt";
        using (StreamWriter sw = new StreamWriter(caminhoArquivoLog, true)) 
        { 
            try
            {
                sw.WriteLine(mesage);
                sw.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
