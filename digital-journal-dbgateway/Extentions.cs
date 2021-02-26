using DigitalJournal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DigitalJournal
{
    static class Extentions
    {
        public static IQueryable<IEntity> Set(this DbContext _context, Type t)
        {
            return (IQueryable<IEntity>)_context.GetType()
                                                .GetMethods().Where(x => x.Name == "Set" && x.GetParameters().Length == 0).First()
                                                .MakeGenericMethod(t)
                                                .Invoke(_context, null);
        }
    }
}