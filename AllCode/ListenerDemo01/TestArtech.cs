using System.Data;
using System.Diagnostics;
using System;

namespace ListenerDemo01
{
    public class Observer<T> : IObserver<T>
    {
        private readonly Action<T> _onNext;
        public Observer(Action<T> onNext) => _onNext = onNext;

        public void OnCompleted() { }
        public void OnError(Exception error) { }
        public void OnNext(T value) => _onNext(value);
    }

    internal class TestArtech
    {
        public static void Start(string[] args)
        {
            DiagnosticListener.AllListeners.Subscribe(new Observer<DiagnosticListener>(
            listener =>
            {
                if (listener.Name == "Artech-Data-SqlClient")
                {
                    listener.Subscribe(new Observer<KeyValuePair<string, object?>>(eventData =>
                    {
                        Console.WriteLine($"Event Name: {eventData.Key}");
                        if (eventData.Value != null)
                        {
                            dynamic payload = eventData.Value;
                            Console.WriteLine($"CommandType: {payload.CommandType}");
                            Console.WriteLine($"CommandText: {payload.CommandText}");
                        }
                    }));
                }
            }));

            var source = new DiagnosticListener("Artech-Data-SqlClient");
            if (source.IsEnabled("CommandExecution"))
            {
                source.Write("CommandExecution", new { 
                    CommandType = CommandType.Text, 
                    CommandText = "SELECT * FROM T_USER" });
            }
        }
    }
}
