// DataLayer/Repositories/ICurrencyPairRepository.cs
using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public interface ICurrencyPairRepository
    {
        Task<List<CurrencyPair>> GetAllCurrencyPairsAsync();
        Task<CurrencyPair?> GetCurrencyPairByIdAsync(Guid id);
        // תיקון כאן: שינוי הפרמטר מ-CurrencyPair בודד לרשימה
        Task UpdateCurrencyPairsAsync(List<CurrencyPair> currencyPairs); // <--- תיקון קריטי!
        Task InitializeCurrencyPairsAsync();
    }
}