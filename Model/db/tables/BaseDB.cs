using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public abstract class BaseDB
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
    }
}
