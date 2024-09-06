using Microsoft.VisualBasic;
using System;

namespace ObserverPattern
{
    class Product : IEquatable<Product>
    {
        public int Code { get; set; }
        public string Name { get; set; }

        public bool Equals(Product? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Code.Equals(other.Code) && Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            int hashProductName = Name == null ? 0 : Name.GetHashCode();
            int hashProductCode = Code.GetHashCode();
            return hashProductName ^ hashProductCode;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            #region products distinct
            List<Product> products = new List<Product>();
            products.Add(new Product
            {
                Name = "Test",
                Code = 1,
            });
            products.Add(new Product
            {
                Name = "Test",
                Code = 1,
            });
            var data = products.Distinct().ToList();
            var a = 1;
            #endregion

            BaggageHandler provider = new();
            ArrivalsMonitor observer1 = new("BaggageClaimMonitor1");
            ArrivalsMonitor observer2 = new("SecurityExit");

            provider.BaggageStatus(712, "Detroit", 3);
            observer1.Subscribe(provider);

            provider.BaggageStatus(712, "Kalamazoo", 3);
            provider.BaggageStatus(400, "New York-Kennedy", 1);
            provider.BaggageStatus(712, "Detroit", 3);
            observer2.Subscribe(provider);

            provider.BaggageStatus(511, "San Francisco", 2);
            provider.BaggageStatus(712);
            observer2.Unsubscribe();

            provider.BaggageStatus(400);
            provider.LastBaggageClaimed();
        }
    }

    public readonly record struct BaggageInfo(
     int FlightNumber,
     string From,
     int Carousel
     );

    public sealed class BaggageHandler : IObservable<BaggageInfo>
    {
        private readonly HashSet<IObserver<BaggageInfo>> _observers = new ();
        private readonly HashSet<BaggageInfo> _flights = new ();

        public IDisposable Subscribe(IObserver<BaggageInfo> observer)
        {
            if (_observers.Add(observer))
            {
                foreach (var flight in _flights)
                {
                    observer.OnNext(flight);
                }
            }
            return new Unsubscriber<BaggageInfo>(_observers, observer);
        }

        public void BaggageStatus(int flightNumber) =>
       BaggageStatus(flightNumber, string.Empty, 0);

        public void BaggageStatus(int flighNumber, string from, int carousel)
        {
            var info = new BaggageInfo(flighNumber, from, carousel);
            if (carousel > 0 && _flights.Add(info))
            {
                foreach (var observer in _observers)
                {
                    observer.OnNext(info);
                }
            }
            else if (carousel is 0)
            {
                if (_flights.RemoveWhere(flight => flight.FlightNumber == info.FlightNumber) > 0)
                {
                    foreach (var observer in _observers)
                    {
                        observer.OnNext(info);
                    }
                }
            }
        }

        public void LastBaggageClaimed()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
            _observers.Clear();
        }
    }

    internal class Unsubscriber<BaggageInfo> : IDisposable
    {
        private readonly ISet<IObserver<BaggageInfo>> _observers;
        private readonly IObserver<BaggageInfo> _observer;

        internal Unsubscriber(ISet<IObserver<BaggageInfo>> observers,
            IObserver<BaggageInfo> observer) => (_observers, _observer) = (observers, observer);

        public void Dispose() => _observers.Remove(_observer);
    }

    public class ArrivalsMonitor : IObserver<BaggageInfo>
    {
        private readonly string _name;
        private readonly List<string> _flights = new();
        //private readonly string _format = "{0,-20} {1,5}  {2, 3}";
        private readonly string _format = "{0,-20} {1,5}  {2, 3}";

        private IDisposable? _cancellation;

        public ArrivalsMonitor(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            _name = name;
        }

        public virtual void Subscribe(BaggageHandler provider) =>
            _cancellation = provider.Subscribe(this);

        public virtual void Unsubscribe()
        {
            _cancellation?.Dispose();
            _flights.Clear();
        }
        public virtual void OnCompleted() => _flights.Clear();

        public virtual void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public virtual void OnNext(BaggageInfo info)
        {
            bool updated = false;
            if (info.Carousel is 0)
            {
                string flightNumber = string.Format("{0,5}", info.FlightNumber);
                for (int index = _flights.Count - 1; index >= 0; index--)
                {
                    string flightInfo = _flights[index];
                    if (flightInfo.Substring(21, 5).Equals(flightNumber))
                    {
                        updated = true;
                        _flights.RemoveAt(index);
                    }
                }
            }
            else
            {
                string flightInfo = string.Format(_format, info.From, info.FlightNumber, info.Carousel);
                if(_flights.Contains(flightInfo) is false)
                {
                    _flights.Add(flightInfo);
                    updated = true;
                }
            }

            if (updated)
            {
                _flights.Sort();
                Console.WriteLine($"Arrivals information from {_name}");
                foreach (var flight in _flights)
                {
                    Console.WriteLine(flight);
                }
                Console.WriteLine();
            }
        }
    }
}