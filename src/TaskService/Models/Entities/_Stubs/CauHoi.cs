using System;
using System.Collections.Generic;

namespace TaskService.Models.Entities
{
    public class CauHoi
    {
        public String id { get; set; }
        public String noiDung { get; set; }
        public String monHoc { get; set; }
        public String doKho { get; set; }
        public String dapAnDung { get; set; }
        public virtual ICollection<LuaChon> LuaChons { get; set; }
        
        public CauHoi()
        {
            LuaChons = new List<LuaChon>();
        }
    }
}
