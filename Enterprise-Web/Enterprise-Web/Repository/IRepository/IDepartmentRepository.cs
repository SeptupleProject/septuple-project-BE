﻿using Enterprise_Web.DTOs;
using Enterprise_Web.Pagination.Filter;
using EnterpriseWeb.Models;

namespace Enterprise_Web.Repository.IRepository
{
    public interface IDepartmentRepository
    {
        (List<DepartmentDTO>, PaginationFilter, int) GetAll(PaginationFilter filter);
        Task <Department> GetDeptById(int id);
        Task Create(Department department);
        Task Update(Department department);
        Task Delete(int id);
        bool CheckNameExist(Department department);
    }
}
