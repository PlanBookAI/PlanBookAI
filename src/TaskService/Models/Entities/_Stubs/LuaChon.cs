using System;

namespace TaskService.Models.Entities
{
    public class LuaChon
    {
        public String id { get; set; }
        public String noiDung { get; set; }
        public String cauHoiId { get; set; }
        public virtual CauHoi CauHoi { get; set; }
    }
}
