namespace ExiaServer.Model
{
    public interface IModel
    {
        void AddObserver(IObserver obs);
       
        void RemoveObserver(IObserver obs);

        void Notify(string text);

    }
}