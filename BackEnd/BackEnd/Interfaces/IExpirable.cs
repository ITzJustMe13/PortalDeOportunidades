namespace BackEnd.Interfaces
{
    public interface IExpirable
    {
        DateTime Date { get; set; }
        bool IsActive { get; set; }
    }
}

