namespace BackEnd.Interfaces
{
    /// <summary>
    /// This interface is responsibile for all the functions of the logic part of Iban
    /// </summary>
    public interface IIBanService
    {
        /// <summary>
        /// Function that validates the user's IBAN through IbanValidator
        /// </summary>
        /// <param name="IBAN"></param>
        /// <returns>Returns true if the IBAN is valid, false if is not</returns>
        bool ValidateIBAN(string IBAN);
    }
}
