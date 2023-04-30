namespace LD53.Gameplay
{
    public interface IReceivable
    {
        bool Receive(Mail mail);
        void Highlight();
        void Unhighlight();
    }
}
