using System;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using TicketResolver.Models;

namespace TicketResolver.DAL
{
    public class MasterDAL
    {
        private readonly Database db = DatabaseFactory.CreateDatabase();
    }
}
