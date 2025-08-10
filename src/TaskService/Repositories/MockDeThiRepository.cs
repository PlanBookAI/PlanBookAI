using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskService.Models.Entities;
using System.Linq;

namespace TaskService.Repositories
{
    /// <summary>
    /// Mock implementation của IDeThiRepository để test
    /// </summary>
    public class MockDeThiRepository : IDeThiRepository
    {
        private readonly List<DeThi> _deThi;

        public MockDeThiRepository()
        {
            _deThi = new List<DeThi>
            {
                new DeThi
                {
                    id = "1",
                    tieuDe = "De thi Hoa hoc hoc ky 1",
                    monHoc = "Hoa hoc",
                    thoiGianLam = 45,
                    tongDiem = 10.0f,
                    CauHois = new List<CauHoi>
                    {
                        new CauHoi
                        {
                            id = "1",
                            noiDung = "Nguyen tu cua nguyen to nao co cau hinh electron lop ngoai cung la 2s²2p⁴?",
                            monHoc = "Hoa hoc",
                            doKho = "Trung binh",
                            dapAnDung = "O"
                        },
                        new CauHoi
                        {
                            id = "2",
                            noiDung = "Chat nao sau day la axit manh?",
                            monHoc = "Hoa hoc",
                            doKho = "De",
                            dapAnDung = "HCl"
                        }
                    }
                },
                new DeThi
                {
                    id = "2",
                    tieuDe = "De thi Hoa hoc hoc ky 2",
                    monHoc = "Hoa hoc",
                    thoiGianLam = 60,
                    tongDiem = 10.0f,
                    CauHois = new List<CauHoi>
                    {
                        new CauHoi
                        {
                            id = "3",
                            noiDung = "Phan ung nao sau day la phan ung oxi hoa khu?",
                            monHoc = "Hoa hoc",
                            doKho = "Kho",
                            dapAnDung = "Fe + CuSO₄"
                        },
                        new CauHoi
                        {
                            id = "4",
                            noiDung = "Cong thuc phan tu cua etanol la gi?",
                            monHoc = "Hoa hoc",
                            doKho = "De",
                            dapAnDung = "C₂H₅OH"
                        },
                        new CauHoi
                        {
                            id = "5",
                            noiDung = "Trong bang tuan hoan, nguyen to X co so thu tu 17. X thuoc nhom nao?",
                            monHoc = "Hoa hoc",
                            doKho = "Trung binh",
                            dapAnDung = "VIIA"
                        }
                    }
                }
            };
        }

        public Task<List<DeThi>> GetAllAsync()
        {
            return Task.FromResult(_deThi);
        }

        public Task<DeThi?> GetByIdAsync(string id)
        {
            return Task.FromResult(_deThi.FirstOrDefault(d => d.id == id));
        }

        public Task<DeThi> CreateAsync(DeThi deThi)
        {
            if (string.IsNullOrEmpty(deThi.id))
            {
                deThi.id = Guid.NewGuid().ToString();
            }
            _deThi.Add(deThi);
            return Task.FromResult(deThi);
        }

        public Task<DeThi> UpdateAsync(DeThi deThi)
        {
            var existingDeThi = _deThi.FirstOrDefault(d => d.id == deThi.id);
            if (existingDeThi != null)
            {
                var index = _deThi.IndexOf(existingDeThi);
                _deThi[index] = deThi;
            }
            return Task.FromResult(deThi);
        }

        public Task DeleteAsync(string id)
        {
            var deThi = _deThi.FirstOrDefault(d => d.id == id);
            if (deThi != null)
            {
                _deThi.Remove(deThi);
            }
            return Task.CompletedTask;
        }
    }
}
