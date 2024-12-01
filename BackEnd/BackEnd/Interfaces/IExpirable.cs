namespace BackEnd.Interfaces
{
    public interface IExpirable
    {
        DateTime? date { get; set; }
        bool? isActive { get; set; }
    }
}

