// UILayer/Hubs/TradeHub.cs
using Microsoft.AspNetCore.SignalR;
using SharedModels; // <--- וודא שורה זו קיימת!

namespace UILayer.Hubs
{
    // TradeHub חייב להיות יורש של Hub<ITradeClient> כאשר ITradeClient מגיע מ-SharedModels
    public class TradeHub : Hub<ITradeClient> // <--- וודא ש-ITradeClient כאן הוא מ-SharedModels
    {
        // כאן אתה יכול להוסיף לוגיקה ספציפית ל-Hub אם נדרש,
        // למשל, Overrides ל-OnConnectedAsync או OnDisconnectedAsync.
    }
}