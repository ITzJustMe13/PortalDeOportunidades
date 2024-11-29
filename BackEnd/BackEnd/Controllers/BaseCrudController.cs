using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    public abstract class BaseCrudController<T> : ResponseController
    {
        public abstract Task<IActionResult> GetEntityById(int id);
        public abstract Task<IActionResult> CreateEntity(T entity);
        public abstract Task<IActionResult> UpdateEntity(int id, T entity);
        public abstract Task<IActionResult> DeleteEntity(int id);

        
    }
}
