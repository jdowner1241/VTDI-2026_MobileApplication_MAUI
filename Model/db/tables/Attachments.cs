using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Model.db.tables
{
    public class Attachments : BaseDB
    {
        public string AttachmentName { get;set;} = string.Empty;
        public string AttachmentType { get;set;} = string.Empty;
        public string AttachmentPath { get;set;} = string.Empty;

        public Attachments() { }
    }
}
