using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskService.Models.Entities;

namespace TaskService.Repositories
{
    /// <summary>
    /// Mock implementation của ICauHoiRepository để test
    /// </summary>
    public class MockCauHoiRepository : ICauHoiRepository
    {
        private readonly List<CauHoi> _cauHois;

        public MockCauHoiRepository()
        {
            _cauHois = new List<CauHoi>
            {
                new CauHoi
                {
                    id = "1",
                    noiDung = "Cau hoi Hoa hoc 1",
                    monHoc = "Hoa hoc",
                    doKho = "De",
                    dapAnDung = "A",
                    LuaChons = new List<LuaChon>
                    {
                        new LuaChon { id = "1", noiDung = "A", cauHoiId = "1" },
                        new LuaChon { id = "2", noiDung = "B", cauHoiId = "1" },
                        new LuaChon { id = "3", noiDung = "C", cauHoiId = "1" },
                        new LuaChon { id = "4", noiDung = "D", cauHoiId = "1" }
                    }
                },
                new CauHoi
                {
                    id = "2",
                    noiDung = "Cau hoi Hoa hoc 2",
                    monHoc = "Hoa hoc",
                    doKho = "Trung binh",
                    dapAnDung = "B",
                    LuaChons = new List<LuaChon>
                    {
                        new LuaChon { id = "5", noiDung = "A", cauHoiId = "2" },
                        new LuaChon { id = "6", noiDung = "B", cauHoiId = "2" },
                        new LuaChon { id = "7", noiDung = "C", cauHoiId = "2" },
                        new LuaChon { id = "8", noiDung = "D", cauHoiId = "2" }
                    }
                },
                new CauHoi
                {
                    id = "3",
                    noiDung = "Cau hoi Hoa hoc 3",
                    monHoc = "Hoa hoc",
                    doKho = "Kho",
                    dapAnDung = "C",
                    LuaChons = new List<LuaChon>
                    {
                        new LuaChon { id = "9", noiDung = "A", cauHoiId = "3" },
                        new LuaChon { id = "10", noiDung = "B", cauHoiId = "3" },
                        new LuaChon { id = "11", noiDung = "C", cauHoiId = "3" },
                        new LuaChon { id = "12", noiDung = "D", cauHoiId = "3" }
                    }
                },
                new CauHoi
                {
                    id = "4",
                    noiDung = "Cau hoi Hoa hoc 4",
                    monHoc = "Hoa hoc",
                    doKho = "Rat kho",
                    dapAnDung = "D",
                    LuaChons = new List<LuaChon>
                    {
                        new LuaChon { id = "13", noiDung = "A", cauHoiId = "4" },
                        new LuaChon { id = "14", noiDung = "B", cauHoiId = "4" },
                        new LuaChon { id = "15", noiDung = "C", cauHoiId = "4" },
                        new LuaChon { id = "16", noiDung = "D", cauHoiId = "4" }
                    }
                },
                new CauHoi
                {
                    id = "5",
                    noiDung = "Cau hoi Hoa hoc 5",
                    monHoc = "Hoa hoc",
                    doKho = "De",
                    dapAnDung = "A",
                    LuaChons = new List<LuaChon>
                    {
                        new LuaChon { id = "17", noiDung = "A", cauHoiId = "5" },
                        new LuaChon { id = "18", noiDung = "B", cauHoiId = "5" },
                        new LuaChon { id = "19", noiDung = "C", cauHoiId = "5" },
                        new LuaChon { id = "20", noiDung = "D", cauHoiId = "5" }
                    }
                },
                new CauHoi
                {
                    id = "6",
                    noiDung = "Cau hoi Test 1",
                    monHoc = "Test",
                    doKho = "Test",
                    dapAnDung = "A",
                    LuaChons = new List<LuaChon>
                    {
                        new LuaChon { id = "21", noiDung = "A", cauHoiId = "6" },
                        new LuaChon { id = "22", noiDung = "B", cauHoiId = "6" },
                        new LuaChon { id = "23", noiDung = "C", cauHoiId = "6" },
                        new LuaChon { id = "24", noiDung = "D", cauHoiId = "6" }
                    }
                },
                new CauHoi
                {
                    id = "7",
                    noiDung = "Cau hoi Test 2",
                    monHoc = "Test",
                    doKho = "Test",
                    dapAnDung = "B",
                    LuaChons = new List<LuaChon>
                    {
                        new LuaChon { id = "25", noiDung = "A", cauHoiId = "7" },
                        new LuaChon { id = "26", noiDung = "B", cauHoiId = "7" },
                        new LuaChon { id = "27", noiDung = "C", cauHoiId = "7" },
                        new LuaChon { id = "28", noiDung = "D", cauHoiId = "7" }
                    }
                },
                // Thêm câu hỏi với tiếng Việt không dấu để test
                new CauHoi
                {
                    id = "8",
                    noiDung = "Cau hoi Hoa hoc khong dau 1",
                    monHoc = "Hoa hoc",
                    doKho = "Trung binh",
                    dapAnDung = "A",
                    LuaChons = new List<LuaChon>
                    {
                        new LuaChon { id = "29", noiDung = "A", cauHoiId = "8" },
                        new LuaChon { id = "30", noiDung = "B", cauHoiId = "8" },
                        new LuaChon { id = "31", noiDung = "C", cauHoiId = "8" },
                        new LuaChon { id = "32", noiDung = "D", cauHoiId = "8" }
                    }
                },
                new CauHoi
                {
                    id = "9",
                    noiDung = "Cau hoi Hoa hoc khong dau 2",
                    monHoc = "Hoa hoc",
                    doKho = "Trung binh",
                    dapAnDung = "B",
                    LuaChons = new List<LuaChon>
                    {
                        new LuaChon { id = "33", noiDung = "A", cauHoiId = "9" },
                        new LuaChon { id = "34", noiDung = "B", cauHoiId = "9" },
                        new LuaChon { id = "35", noiDung = "C", cauHoiId = "9" },
                        new LuaChon { id = "36", noiDung = "D", cauHoiId = "9" }
                    }
                }
            };
        }

        public Task<List<CauHoi>> GetAllAsync()
        {
            return Task.FromResult(_cauHois);
        }

        public Task<CauHoi?> GetByIdAsync(string id)
        {
            return Task.FromResult(_cauHois.FirstOrDefault(c => c.id == id));
        }

        public Task<List<CauHoi>> GetByMonHocAndDoKhoAsync(string monHoc, string doKho)
        {
            return Task.FromResult(_cauHois.Where(c => c.monHoc == monHoc && c.doKho == doKho).ToList());
        }

        public Task<CauHoi> CreateAsync(CauHoi cauHoi)
        {
            cauHoi.id = Guid.NewGuid().ToString();
            _cauHois.Add(cauHoi);
            return Task.FromResult(cauHoi);
        }

        public Task<CauHoi> UpdateAsync(CauHoi cauHoi)
        {
            var existingCauHoi = _cauHois.FirstOrDefault(c => c.id == cauHoi.id);
            if (existingCauHoi != null)
            {
                var index = _cauHois.IndexOf(existingCauHoi);
                _cauHois[index] = cauHoi;
            }
            return Task.FromResult(cauHoi);
        }

        public Task DeleteAsync(string id)
        {
            var cauHoi = _cauHois.FirstOrDefault(c => c.id == id);
            if (cauHoi != null)
            {
                _cauHois.Remove(cauHoi);
            }
            return Task.CompletedTask;
        }
    }
}
