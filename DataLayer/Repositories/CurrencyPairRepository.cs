// DataLayer/Repositories/CurrencyPairRepository.cs
using DataLayer.Data; // וודא שזה DataLayer.Data, כי שם ה-DbContext שלך
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic; // וודא שזה קיים!
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class CurrencyPairRepository : ICurrencyPairRepository
    {
        private readonly ApplicationDbContext _context; // שם ה-DbContext שלך הוא ApplicationDbContext

        public CurrencyPairRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CurrencyPair>> GetAllCurrencyPairsAsync()
        {
            return await _context.CurrencyPairs.ToListAsync();
        }

        public async Task<CurrencyPair?> GetCurrencyPairByIdAsync(Guid id)
        {
            return await _context.CurrencyPairs.FirstOrDefaultAsync(cp => cp.Id == id);
        }

        // ************ תיקון קריטי כאן ************
        // שינוי חתימת המתודה כדי שתקבל רשימה של CurrencyPair
        public async Task UpdateCurrencyPairsAsync(List<CurrencyPair> currencyPairs) // <--- תיקון חתימה!
        {
            // מכיוון שאתה טוען את ה-currencyPairs ב-SimulationService
            // באמצעות GetAllCurrencyPairsAsync() מאותו ה-context,
            // Entity Framework Core כבר עוקב אחרי האובייקטים הללו.
            // לכן, כל שינוי שנעשה בהם ב-SimulationService יזוהה אוטומטית.
            // כל מה שצריך לעשות כאן הוא לשמור את השינויים ב-context.
            await _context.SaveChangesAsync();
        }
        // *****************************************

        public async Task InitializeCurrencyPairsAsync()
        {
            // לרוב, זה מטופל ע"י SeedData במיגרציות.
            // אם אתה רוצה לאתחל נתונים בצורה אחרת, תממש כאן.
            // כרגע זה פשוט מסיים את המשימה.
            await Task.CompletedTask;
        }
    }
}